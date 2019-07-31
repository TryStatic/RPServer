using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipes;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RPServerClient.Client;
using RPServerClient.Util;
using Font = RAGE.Game.Font;
using Player = RAGE.Elements.Player;

namespace RPServerClient.Chat
{
    internal class Voice : Events.Script
    {
        private const float MaxDistance = 50.0f;
        private const long VoiceRefreshRateInMS = 250;

        private static readonly HashSet<Player> Listeners = new HashSet<Player>();

        private static long _latestProcess;
        private static bool _justPressedN;

        public Voice()
        {
            RAGE.Voice.Muted = true;
            Player.LocalPlayer.Voice3d = true;

            Events.Tick += Tick;
            Events.OnPlayerQuit += OnPlayerQuit;
        }
        
        private void Tick(List<Events.TickNametagData> nametags)
        {
            if(!Globals.IsAccountLoggedIn || !Globals.HasActiveChar) return;

            if (RAGE.Input.IsDown((int)Shared.Enums.KeyCodes.VK_N))
            {
                RAGE.Game.Ui.SetTextOutline();
                UIText.Draw("Chatting", new Point((int)(0.75f * ScreenRes.UIStandardResX), (int)(0.95f * ScreenRes.UIStandardResY)), 0.4f, Color.Green, Font.Monospace, true);
            }

            if (RAGE.Input.IsDown((int) Shared.Enums.KeyCodes.VK_N))
            {
                if (!_justPressedN)
                {
                    RAGE.Voice.Muted = false;
                    _justPressedN = true;
                }
            }
            else
            {
                if (_justPressedN)
                {
                    RAGE.Voice.Muted = true;
                    _justPressedN = false;
                }
            }

            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (currentTime - _latestProcess > VoiceRefreshRateInMS)
            {
                _latestProcess = currentTime;

                // Add new people
                foreach (var p in Entities.Players.All)
                {
                    if(!p.Exists) continue;

                    var dist = Player.LocalPlayer.Position.DistanceToSquared(p.Position);
                    if (dist < MaxDistance)
                    {
                        var added = Listeners.Add(p);
                        if (added)
                        {
                            Events.CallRemote(Shared.Events.ClientToServer.VoiceChat.SumbitAddVoiceListener, p.RemoteId);
                            RAGE.Chat.Output($"[DEBUG-CLIENT]: Setting {p.Name} as your voice listener.");
                        }
                    }
                }

                // Remove streamed out ppl
                foreach (var p in Listeners)
                {
                    if (p.Handle != 0)
                    {
                        var dist = Player.LocalPlayer.Position.DistanceToSquared(p.Position);
                        if (dist > MaxDistance)
                        {
                            var removed = Listeners.Remove(p);
                            if (removed)
                            {
                                RAGE.Chat.Output($"[DEBUG-CLIENT]: removing {p.Name} from your voice listeners.");
                                Events.CallRemote(Shared.Events.ClientToServer.VoiceChat.SumbitRemoveVoiceListener, p.RemoteId);
                            }
                        }
                        else
                        {
                            p.VoiceVolume = 1.0f - (dist / MaxDistance);
                        }
                    }
                    else
                    {
                        Listeners.Remove(p);
                    }
                }
            }
        }

        private void OnPlayerQuit(Player player) => Listeners.Remove(player);
    }
}