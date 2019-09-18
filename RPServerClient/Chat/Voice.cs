using System;
using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using RPServerClient.Chat.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Shared.Enums;
using Shared.Events.ClientToServer;
using Font = RAGE.Game.Font;
using Player = RAGE.Elements.Player;

namespace RPServerClient.Chat
{
    internal class Voice : Events.Script
    {
        private const long VoiceRefreshRateInMS = 250;

        private static readonly HashSet<Player> Listeners = new HashSet<Player>();

        private static long _latestProcess;

        public Voice()
        {
            RAGE.Voice.Muted = true;
            Player.LocalPlayer.Voice3d = true;

            Events.Tick += Tick;
            Events.OnPlayerQuit += OnPlayerQuit;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (!Globals.IsAccountLoggedIn || !Globals.HasActiveChar) return;

            if (Chat.IsChatInputActive)
            {
                if (!RAGE.Voice.Muted) RAGE.Voice.Muted = true;
                return;
            }

            if (Input.IsDown((int) KeyCodes.VK_N))
            {
                Ui.SetTextOutline();
                UIText.Draw("Chatting",
                    new Point((int) (0.75f * ScreenRes.UIStandardResX), (int) (0.95f * ScreenRes.UIStandardResY)), 0.4f,
                    Color.Green, Font.Monospace, true);
            }

            if (Input.IsDown((int) KeyCodes.VK_N))
            {
                if (RAGE.Voice.Muted) RAGE.Voice.Muted = false;
            }
            else
            {
                if (!RAGE.Voice.Muted) RAGE.Voice.Muted = true;
            }

            var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (currentTime - _latestProcess > VoiceRefreshRateInMS)
            {
                _latestProcess = currentTime;
                var maxDistance = Shared.Data.Chat.NormalChatMaxDistance;


                var chatmode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                switch (chatmode)
                {
                    case ChatMode.Low:
                        maxDistance = Shared.Data.Chat.LowChatMaxDistance;
                        break;
                    case ChatMode.Normal:
                        maxDistance = Shared.Data.Chat.NormalChatMaxDistance;
                        break;
                    case ChatMode.Shout:
                        maxDistance = Shared.Data.Chat.ShoutChatMaxDistance;
                        break;
                    default:
                        RAGE.Chat.Output("There was an error while selecting chatmode.");
                        break;
                }

                // Add new people
                foreach (var p in Entities.Players.All)
                {
                    if (!p.Exists) continue;
                    if (p == Player.LocalPlayer) continue;

                    var dist = Player.LocalPlayer.Position.DistanceTo(p.Position);
                    if (dist < maxDistance)
                    {
                        var added = Listeners.Add(p);
                        if (added)
                        {
                            Events.CallRemote(VoiceChat.SumbitAddVoiceListener, p.RemoteId);
                            RAGE.Chat.Output($"[DEBUG-CLIENT]: Setting {p.Name} as your voice listener.");
                        }
                    }
                }

                // Remove streamed out ppl
                foreach (var p in Listeners)
                    if (p.Handle != 0)
                    {
                        var dist = Player.LocalPlayer.Position.DistanceTo(p.Position);
                        if (dist > maxDistance)
                        {
                            var removed = Listeners.Remove(p);
                            if (removed)
                            {
                                RAGE.Chat.Output($"[DEBUG-CLIENT]: removing {p.Name} from your voice listeners.");
                                Events.CallRemote(VoiceChat.SumbitRemoveVoiceListener, p.RemoteId);
                            }
                        }
                        else
                        {
                            p.VoiceVolume = 1.0f - dist / maxDistance;
                        }
                    }
                    else
                    {
                        Listeners.Remove(p);
                    }
            }
        }

        private void OnPlayerQuit(Player player)
        {
            Listeners.Remove(player);
        }
    }
}