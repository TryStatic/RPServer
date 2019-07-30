using System;
using System.Collections.Generic;
using System.Drawing;
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

        private static readonly List<Player> Listeners = new List<Player>();

        private static long _latestProcess;
        private static bool _justPressedN;

        public Voice()
        {
            RAGE.Voice.Muted = true;
            Player.LocalPlayer.Voice3d = true;

            Events.Tick += Tick;
            Events.OnPlayerQuit += OnPlayerQuit;
        }

        private static void AddListener(Player p)
        {
            p.SetData<bool>(LocalDataKeys.IsListener, true);
            Events.CallRemote(Shared.Events.ClientToServer.VoiceChat.SumbitAddVoiceListener, p.RemoteId);

            Listeners.Add(p);
            p.Voice3d = true;
        }

        private static void RemoveListener(Player p, bool notifyServer = true)
        {
            Listeners.Remove(p);
            p.SetData<bool>(LocalDataKeys.IsListener, false);
            if (notifyServer) Events.CallRemote(Shared.Events.ClientToServer.VoiceChat.SumbitRemoveVoiceListener, p.RemoteId);
        }

        public static void OnPlayerQuit(Player player)
        {
            RemoveListener(player, false);
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

                var localPosition = Player.LocalPlayer.Position;

                
                /*#region TEST
                if (!Player.LocalPlayer.GetData<bool>(LocalDataKeys.IsListener))
                {
                    AddListener(Player.LocalPlayer);
                }
                #endregion
                */

                foreach (var p in Entities.Players.Streamed)
                {
                    if (p.GetData<bool>(LocalDataKeys.IsListener)) continue;

                    var dist = localPosition.DistanceToSquared(p.Position);
                    if (dist < MaxDistance) AddListener(p);
                }

                foreach (var p in Listeners)
                {
                    if (p.Handle != 0)
                    {
                        var dist = localPosition.DistanceToSquared(p.Position);

                        if (dist > MaxDistance) RemoveListener(p);
                        else p.VoiceVolume = (1.0f - (dist / MaxDistance));
                    }
                    else
                    {
                        RemoveListener(p);
                    }
                }
            }
        }
    }
}