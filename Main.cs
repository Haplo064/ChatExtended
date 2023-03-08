using System;
using System.Diagnostics;
using Dalamud.Configuration;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Plugin;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using Dalamud.Logging;
using ImGuiNET;
using Num = System.Numerics;
using System.Collections.Generic;
using System.Security;
using System.Numerics;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Game.ClientState.JobGauge.Enums;
using Lumina.Excel.GeneratedSheets;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Condition = Dalamud.Game.ClientState.Conditions.Condition;
using System.Text;

namespace ChatExtended
{
    public partial class ChatExtended : IDalamudPlugin
    {
        public string Name => "Chat Extended";
        private readonly DalamudPluginInterface _pi;
        private readonly CommandManager _cm;
        private readonly ClientState _cs;
        private readonly Framework _fw;
        private readonly GameGui _gui;
        private readonly Condition _cond;
        private readonly Config _config;
        private readonly ChatGui _chat;
        private bool _configDraw;
        private int dirtyHack = 0;
        private int _version = 1;
        private List<ChatMessage> _messages = new List<ChatMessage>();
        private List<LogConfig> _logConfigs = new List<LogConfig>();
        
        private bool localDate = true;
        private bool chatLogger = true;

        static string pathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\FFXIVLogs\";
        
        public ChatExtended(
            DalamudPluginInterface pluginInterface,
            CommandManager commandManager,
            ClientState clientState,
            Framework framework,
            GameGui gameGui,
            Condition condition,
            ChatGui chat
        )
        {
            _pi = pluginInterface;
            _cm = commandManager;
            _cs = clientState;
            _fw = framework;
            _gui = gameGui;
            _cond = condition;
            _chat = chat;

            _config = pluginInterface.GetPluginConfig() as Config ?? new Config();

            _pi.UiBuilder.Draw += DrawConfig;
            _pi.UiBuilder.OpenConfigUi += ConfigWindow;
            _chat.ChatMessage += OnChat;
            _cm.AddHandler("/chat", new CommandInfo(Command)
            {
                HelpMessage = "Opens Chat Extended config."
            });
        }

        private void ConfigWindow()
        {
            _configDraw = true;
        }


        public void Dispose()
        {
            _pi.UiBuilder.Draw -= DrawConfig;
            _pi.UiBuilder.OpenConfigUi -= ConfigWindow;
            _cm.RemoveHandler("/chat");
            _chat.ChatMessage -= OnChat;
        }

        private void Command(string command, string arguments)
        {
            _configDraw = !_configDraw;
            SaveConfig();
        }

        private void SaveConfig()
        {
            _pi.SavePluginConfig(_config);
        }
    }


    public class Config : IPluginConfiguration
    {
        public int Version { get; set; } = 1;
    }

    public class ChatMessage
    {
        public XivChatType type;
        public SeString message;
        public SeString sender;
        public DateTime time;

        public SeString Message { get => message; set => message = value; }
        public XivChatType Type { get => type; set => type = value; }
        public SeString Sender { get => sender; set => sender = value; }
        public DateTime Time { get => time; set => time = value; }

        public ChatMessage(XivChatType type, SeString sender, SeString message)
        {
            this.message = message;
            this.type = type;
            this.sender = sender;
            this.time = DateTime.Now;
        }
    }
}