using System.Collections.Generic;
using System.Drawing;
using RAGE;
using RAGE.Game;
using RPServerClient.Chat.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Font = RAGE.Game.Font;
using Player = RAGE.Elements.Player;

namespace RPServerClient.HUD
{
    internal class MainHud : Events.Script
    {
        private string _zoneName = "";
        private string _crossingRoad = "";
        private string _streetName = "";
        private string _headingStr = "";
        private int _lastUpdated = 0;

        public MainHud()
        {
            Events.Tick += Tick;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (Globals.ServerVersion != null)
            {
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw(Globals.ServerVersion, new Point(ScreenRes.UIStandardResX / 2, ScreenRes.UIStandardResY - (int)(ScreenRes.UIStandardResY * 0.03)), 0.35f, Color.White, RAGE.Game.Font.ChaletLondon, true);
            }

            if (Globals.IsAccountLoggedIn && Globals.HasActiveChar)
            {
                // Chatmode
                var mode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                RAGE.Game.Ui.SetTextOutline();
                UIText.Draw(mode.ToString(), new Point((int) (0.03f * ScreenRes.UIStandardResX), (int)(0.26f * ScreenRes.UIStandardResY)), 0.4f, Color.White, Font.Monospace, true);

                var pos = Player.LocalPlayer.Position;

                if (_lastUpdated < RAGE.Game.Misc.GetGameTimer())
                {
                    _lastUpdated = RAGE.Game.Misc.GetGameTimer() + 750;
                    // Get Zone
                    var zone = Zone.GetNameOfZone(pos.X, pos.Y, pos.Z);
                    _zoneName = Ui.GetLabelText(zone);

                    // Get StreetNames
                    int streetNameId = 0;
                    int crossingRoadId = 0;
                    Pathfind.GetStreetNameAtCoord(pos.X, pos.Y, pos.Z, ref streetNameId, ref crossingRoadId);
                    _streetName = Ui.GetStreetNameFromHashKey((uint)streetNameId);
                    _crossingRoad = Ui.GetStreetNameFromHashKey((uint)crossingRoadId);

                    // Get Direction
                    var heading = Player.LocalPlayer.GetHeading();
                    var directionNumba = (int)(heading / 45.0f);
                    switch (directionNumba)
                    {
                        case 0: _headingStr = "N"; break;
                        case 1: _headingStr = "NW"; break;
                        case 2: _headingStr = "W"; break;
                        case 3: _headingStr = "SW"; break;
                        case 4: _headingStr = "S"; break;
                        case 5: _headingStr = "SE"; break;
                        case 6: _headingStr = "E"; break;
                        case 7: _headingStr = "NE"; break;
                    }
                }
                // Draw Zone and StreetNames and Heading (direction)
                var point1 = new Point((int)(ScreenRes.UIStandardResX * 0.18f), (int)(ScreenRes.UIStandardResY * 0.85f));
                var point2 = new Point((int)(ScreenRes.UIStandardResX * 0.18f), (int)(ScreenRes.UIStandardResY * 0.87f));
                var point3 = new Point((int)(ScreenRes.UIStandardResX * 0.18f), (int)(ScreenRes.UIStandardResY * 0.89f));
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw("~c~Zone:~s~ " + _zoneName, point1, 0.4f, Color.White, Font.ChaletComprimeCologne, false);
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw(string.IsNullOrEmpty(_crossingRoad) ? $"~c~Street:~w~ {_streetName}" : $"~c~INTXN of ~w~{_streetName} ~c~and~s~ {_crossingRoad}",
                    point2, 0.4f, Color.BurlyWood, Font.ChaletComprimeCologne, false);
                RAGE.Game.Ui.SetTextOutline();
                RAGE.Game.UIText.Draw($"~c~Direction: ~w~{_headingStr}~s~ ", point3, 0.4f, Color.White, Font.ChaletComprimeCologne, false);


            }
        }
    }
}
