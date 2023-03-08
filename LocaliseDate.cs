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
using ImGuiNET;
using System.Numerics;
using System.Collections.Generic;
using ChatExtended;
using Dalamud.Game.Text.SeStringHandling;
using System.Globalization;
using System.Text.RegularExpressions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text;

namespace ChatExtended
{
    public partial class ChatExtended : IDalamudPlugin
    {
        private SeString localiseDate(SeString message)
        {
            string pattern = "(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)(. \\d+, \\d+ \\d+:\\d+ \\()[A-Z]{3,4}(\\))";
            Regex rg = new Regex(pattern);

            if (Regex.Match(message.TextValue, pattern).Success)
            {
                for (int i = 0; i < message.Payloads.Count; i++)
                {
                    if (message.Payloads[i].Type == PayloadType.RawText)
                    {
                        var text = (TextPayload)message.Payloads[i];
                        if (Regex.Match(text.Text, pattern).Success)
                        {
                            MatchCollection matches = rg.Matches(text.Text);
                            for (int j = 0; j < matches.Count; j++)
                            {
                                try
                                {
                                    DateTime date = DateTime.ParseExact(matches[j].ToString().Replace("(GMT)", "+0"), "MMM. dd, yyyy H:mm z", CultureInfo.CurrentCulture);
                                    var newText = text.Text.Substring(0, matches[j].Index) + date.ToString() + " (Local Time)" + text.Text.Substring(matches[j].Index + matches[j].Length);
                                    message.Payloads[i] = new TextPayload(newText);
                                }
                                catch (Exception e)
                                {
                                    //PluginLog.Log("Not GMT");
                                }
                            }
                        }
                    }
                }
            }
            return message;
        }
    }
}