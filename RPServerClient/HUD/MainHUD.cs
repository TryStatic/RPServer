using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using RAGE;
using RAGE.Game;
using RAGE.NUI;
using RPServerClient.Chat.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Player = RAGE.Elements.Player;

namespace RPServerClient.HUD
{
    internal class MainHud : Events.Script
    {
        public MainHud()
        {
            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (Globals.ServerVersion != null) DrawServerVersion();

            if (Globals.IsAccountLoggedIn && Globals.HasActiveChar)
            {
                DrawChatMode();
                DrawCompass();
                DrawStreets();

            }
        }

        private static int _lastUpdated;
        private static string _zoneName = "";
        private static string _streetName = "";
        private static string _crossingRoad = "";

        private static void DrawStreets()
        {
            var pos = Player.LocalPlayer.Position;

            if (_lastUpdated < Misc.GetGameTimer())
            {
                _lastUpdated = Misc.GetGameTimer() + 750;

                // Get Zone
                var zone = Zone.GetNameOfZone(pos.X, pos.Y, pos.Z);
                _zoneName = Ui.GetLabelText(zone);

                // Get StreetNames
                var streetNameId = 0;
                var crossingRoadId = 0;
                Pathfind.GetStreetNameAtCoord(pos.X, pos.Y, pos.Z, ref streetNameId, ref crossingRoadId);
                _streetName = Ui.GetStreetNameFromHashKey((uint)streetNameId);
                _crossingRoad = Ui.GetStreetNameFromHashKey((uint)crossingRoadId);
            }

            Ui.SetTextOutline();
            UIText.Draw($"{_zoneName}", new Point((int)(0.5f * ScreenRes.UIStandardResX), (int)(0.01f * ScreenRes.UIStandardResY)), 0.32f, Color.White, Font.ChaletLondon, true);
            Ui.SetTextOutline();

            var streets = "";
            streets = _crossingRoad == "" ? $"{_streetName}" : $"{_streetName} & {_crossingRoad}";
            UIText.Draw(streets, new Point((int)(0.5f * ScreenRes.UIStandardResX), (int)(0.033f * ScreenRes.UIStandardResY)), 0.29f, Color.White, Font.ChaletLondon, true);
        }

        private static void DrawCompass()
        {
            const float width = 0.25f;
            const float fov = 180.0f;
            const float ticksBetweenCardinals = 9.0f;

            var PosX = 0.375f;
            var posY = 0.07f;

            var pxDegree = width / fov;

            var camRot = Cam.GetGameplayCamRot(0);
            var playerHeadingDegrees = 360.0f - (camRot.Z + 360.0f) % 360.0f;

            var tickDegree = playerHeadingDegrees - fov / 2.0f;
            var tickDegreeRem = ticksBetweenCardinals - tickDegree % ticksBetweenCardinals;
            var tickPos = PosX + tickDegreeRem * pxDegree;

            tickDegree += tickDegreeRem;


            while (tickPos < PosX + width)
            {
                if (Math.Abs(tickDegree % 90.0f) < Math.E)
                {
                    // Cardinal
                    RAGE.Game.Graphics.DrawRect(tickPos, posY, 0.0015f, 0.01f, 255,255,255,255, 0);
                    Ui.SetTextOutline();
                    UIText.Draw(GetDirectionText(tickDegree), new Point((int) (tickPos * ScreenRes.UIStandardResX), (int) ((posY + 0.015f) * ScreenRes.UIStandardResY)), 0.25f, Color.White, Font.ChaletLondon, true);
                }
                else if (Math.Abs(tickDegree % 45.0f) < Math.E)
                {
                    // Intercardinal
                    Graphics.DrawRect(tickPos, posY, 0.0015f, 0.005f, 255,255,255, 255, 0);
                    Ui.SetTextOutline();
                    UIText.Draw(GetDirectionText(tickDegree), new Point((int)(tickPos * ScreenRes.UIStandardResX), (int)((posY + 0.015f) * ScreenRes.UIStandardResY)), 0.2f, Color.White, Font.ChaletLondon, true);
                }
                else
                {
                    // Tick
                    RAGE.Game.Graphics.DrawRect(tickPos, posY, 0.0015f, 0.002f, 255, 255, 255, 255, 0);
                }

                tickDegree += ticksBetweenCardinals;
                tickPos += pxDegree * ticksBetweenCardinals;
            }


        }

        private static void DrawChatMode()
        {
            var mode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
            Ui.SetTextOutline();
            UIText.Draw(mode.ToString(), new Point((int)(0.03f * ScreenRes.UIStandardResX), (int)(0.26f * ScreenRes.UIStandardResY)), 0.4f, Color.White, Font.Monospace, true);
        }

        private static void DrawServerVersion()
        {
            Ui.SetTextOutline();
            UIText.Draw(Globals.ServerVersion, new Point(ScreenRes.UIStandardResX / 2, ScreenRes.UIStandardResY - (int)(ScreenRes.UIStandardResY * 0.03)), 0.35f, Color.White, Font.ChaletLondon, true);
        }

        private static string GetDirectionText(float degrees)
        {
            degrees %= 360.0f;

            if (degrees >= 0.0f && degrees < 22.5f || degrees >= 337.5f)
            {
                return "N";
            }

            if (degrees >= 22.5 && degrees < 67.5f)
            {
                return "NE";
            }

            if (degrees >= 67.5f && degrees < 112.5f)
            {
                return "E";
            }

            if (degrees >= 112.5f && degrees < 157.5f)
            {
                return "SE";
            }

            if (degrees >= 157.5f && degrees < 202.5f)
            {
                return "S";
            }

            if (degrees >= 202.5f && degrees < 247.5f)
            {
                return "SW";
            }

            if (degrees >= 247.5f && degrees < 292.5f)
            {
                return "W";
            }

            if (degrees >= 292.5f && degrees < 337.5f)
                return "NW";

            return "NW";

        }

    }
}