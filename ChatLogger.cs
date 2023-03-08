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
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace ChatExtended
{
    public partial class ChatExtended : IDalamudPlugin
    {
        public void ChatLogg(XivChatType type, SeString sender, SeString message)
        {
            foreach (var log in _logConfigs)
            {

                if (log.Channels[Array.IndexOf(log.ChannelEnums, type)])
                {
                    var file = @$"{log.Name}.txt";

                    if (log.TimeStampFile)
                    {
                        file = @$"{GetDate() + log.Name}.txt";
                    }

                    var _sender = sender.TextValue;
                    var _message = message.TextValue;

                    if (log.SimpleUsername)
                    {
                        _sender = PlayerName(sender);
                        if (log.ServerName)
                        {
                            var world = ServerName(sender);
                            if (world.Length > 0)
                            {
                                _sender += $" ({world})";
                            }
                        }
                    }

                    if (log.PlainText)
                    {
                        _message = PlainText(message);
                    }

                    if (!Directory.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);
                    }

                    if (!File.Exists(pathString + file))
                    {
                        using var sw = File.CreateText(pathString + file);
                        var text = String.Empty;
                        if (log.TimeStampMessages)
                        {
                            text += GetTime(log.HourTime);
                        }

                        if (log.ChatType)
                        {
                            text += $"[{type}]";
                        }

                        if (_sender.Length != 0)
                        {
                            text += _sender + ": ";
                        }

                        sw.WriteLine(text + _message);

                    }
                    else
                    {
                        using var sw = File.AppendText(pathString + file);
                        var text = String.Empty;
                        if (log.TimeStampMessages)
                        {
                            text += GetTime(log.HourTime);
                        }

                        if (log.ChatType)
                        {
                            text += $"[{type}]";
                        }

                        if (_sender.Length != 0)
                        {
                            text += _sender + ": ";
                        }

                        sw.WriteLine(text + _message);
                    }
                }
            }
        }

        public string GetTime(bool hourTime)
        {
            string temp = "[";
            if (hourTime)
            {
                if (DateTime.Now.ToString("%H").Length == 1) { temp += "0"; }
                temp += DateTime.Now.ToString("%H" + ":");
            }    
            else
            {
                if (DateTime.Now.ToString("%h").Length == 1) { temp += "0"; }
                temp += DateTime.Now.ToString("%h" + ":");
            }
            if (DateTime.Now.ToString("%m").Length == 1) { temp += "0"; }
            temp += DateTime.Now.ToString("%m" + "]");
            return temp;
        }

        public string ChatExample(LogConfig log)
        {
            var sampleText = "";
            if (log.TimeStampMessages)
            {
                if (log.HourTime)
                {
                    sampleText += "[16:01]";
                }
                else
                {
                    sampleText += "[04:01]";
                }
            }
            if (log.ChatType)
            {
                sampleText += "[Say]";
            }
            if (log.SimpleUsername)
            {
                sampleText += "Hugh Man";
                if (log.ServerName)
                {
                    sampleText += "(Balmung)";
                }
            }
            else
            {
                sampleText += "☐Hugh Man";
            }
            if (log.PlainText)
            {
                sampleText += ": Hey there, good looking.";
            }
            else
            {
                sampleText += ": Hey there, ☐good looking☐.";
            }

            return sampleText;
        }
        public string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-");
        }
        
        public string ServerName(SeString username)
        {
            var server = string.Empty;
            if (username.Payloads.Count == 0 || username.Payloads.Count == 1)
            {
                return server;
            }
            
            foreach (var payload in username.Payloads)
            {
                if (payload.Type == PayloadType.Player)
                {
                    var x = (PlayerPayload)payload;
                    server = x.World.Name.RawString;
                }
            }
            
            return Regex.Replace(server,@"[^\u0000-\u007F]+", string.Empty);
        }
        
        public string PlainText(SeString message)
        {
            return Regex.Replace(message.TextValue,@"[^\u0000-\u007F]+", string.Empty);
        }

        public string PlayerName(SeString username)
        {
            var name = String.Empty;
            if (username.Payloads.Count == 0)
            {
                return name;
            }

            if (username.Payloads.Count == 1)
            {
                name = username.TextValue;
            }

            else
            {
                foreach (var payload in username.Payloads)
                {
                    if (payload.Type == PayloadType.Player)
                    {
                        var x = (PlayerPayload)payload;
                        name = x.PlayerName;
                    }
                }
            }

            
            return Regex.Replace(name,@"[^\u0000-\u007F]+", string.Empty);
        }

        public class LogConfig
        {
            public string Name = "Log";
            public bool TimeStampFile = true;
            public bool TimeStampMessages = true;
            public bool HourTime = true;
            public bool SimpleUsername = true;
            public bool ServerName = true;
            public bool PlainText = true;
            public bool ChatType = true;
            public bool[] Channels = new bool[Enum.GetNames((typeof(XivChatType))).Length];
            public Array ChannelEnums = Enum.GetValues(typeof(XivChatType));

        }
    }
}
