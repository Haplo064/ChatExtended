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
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text;
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
using System.Text.RegularExpressions;
using Dalamud.Logging;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatExtended
{
    public partial class ChatExtended : IDalamudPlugin
    {
        private void OnChat(XivChatType type, uint id, ref SeString sender, ref SeString message, ref bool handled)
        {
            _messages.Add(new ChatMessage(type, sender, message));

            //Notice Local Date Converter
            if (localDate && type == XivChatType.Notice)
            {
                message = localiseDate(message);
            }

            //Log Chats
            if (chatLogger)
            {
                ChatLogg(type, sender, message);
            }
        }
    }
}
