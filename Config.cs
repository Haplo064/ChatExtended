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
using Dalamud.Game.Text;

namespace ChatExtended
{
    public partial class ChatExtended : IDalamudPlugin
    {
        private void DrawConfig()
        {
            if (_configDraw)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(600, 300));
                ImGui.SetNextWindowSize(new Vector2(600, 300), ImGuiCond.FirstUseEver);
                ImGui.Begin("Chat Extender Config", ref _configDraw);
                ImGui.PopStyleVar();

                ImGui.Checkbox("Localise Announcement Date/Time", ref localDate);
                if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Replaces the GMT time in announcements with your local time."); }
                ImGui.Checkbox("Enable Chat Logging", ref chatLogger);
                if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Log chat to 'Documents/FFXIVLogs' text files."); }

                if (chatLogger)
                {
                    var deleteLog = -1;
                    ImGui.BeginTabBar("Logs");
                    if (ImGui.BeginTabItem("Logging Config"))
                    {
                        if (ImGui.Button("Add Log"))
                        {
                            _logConfigs.Add(new LogConfig());
                        }

                        
                        
                        for (int i = 0; i < _logConfigs.Count; i++)
                        {
                            ImGui.InputText($"Log Name##{i}", ref _logConfigs[i].Name,99);
                            ImGui.SameLine();
                            if (ImGui.Button($"Delete##{i}"))
                            {
                                deleteLog = i;
                            }
                        }
                        
                        ImGui.EndTabItem();
                    }

                    var TabNumber = 0;
                    foreach (var log in _logConfigs)
                    {
                        if (ImGui.BeginTabItem($"{log.Name}##{TabNumber}"))
                        {
                            ImGui.Checkbox($"DateStamp File##{TabNumber}", ref log.TimeStampFile);
                            ImGui.SameLine();
                            if (log.TimeStampFile)
                            {
                                ImGui.Text($"  (Saving as {GetDate()+log.Name}.txt)");
                            }
                            else
                            {
                                ImGui.Text($"  (Saving as {log.Name}.txt)");
                            }
                            ImGui.Checkbox($"TimeStamp Chat##{TabNumber}", ref log.TimeStampMessages);
                            if (log.TimeStampMessages)
                            {
                                ImGui.SameLine();
                                ImGui.Checkbox($"24 Hour Time##{TabNumber}", ref log.HourTime);
                            }
                            ImGui.Checkbox($"Add Chat Type##{TabNumber}", ref log.ChatType);
                            ImGui.Checkbox($"Plain Username##{TabNumber}", ref log.SimpleUsername);
                            if (log.SimpleUsername)
                            {
                                ImGui.SameLine();
                                ImGui.Checkbox($"Add World Name##{TabNumber}", ref log.ServerName);
                            }
                            ImGui.Checkbox($"Plaintext##{TabNumber}", ref log.PlainText);

                            
                            ImGui.Text("\n==Example Text Below==");
                            ImGui.Text(ChatExample(log));

                            ImGui.Separator();
                            ImGui.Text("Chat Channels to save");
                            ImGui.Columns(4);
                            for (int i = 1; i < Enum.GetNames((typeof(XivChatType))).Length; i++)
                            {
                                ImGui.Checkbox(Enum.GetNames(typeof(XivChatType))[i], ref log.Channels[i]);
                                if (i % 10 == 0)
                                {
                                    ImGui.NextColumn();
                                }
                            }
                            ImGui.Columns(1);
                            ImGui.EndTabItem();
                            
                        }
                        TabNumber++;
                        
                    }

                    ImGui.EndTabBar();

                    if (deleteLog != -1)
                    {
                        _logConfigs.RemoveAt(deleteLog);
                    }
                }
                ImGui.End();

                if (dirtyHack > 100)
                {
                    SaveConfig();
                    dirtyHack = 0;
                }
                dirtyHack++;
            }
        }
    }
}