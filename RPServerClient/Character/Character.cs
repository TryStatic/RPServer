using Shared;
using RAGE;
using RAGE.Elements;
using RPServerClient.Globals;
using Events = RAGE.Events;

namespace RPServerClient.Character
{
    internal class Character : Events.Script
    {
        private CustomCamera _characterDisplayCamera;

        public Character()
        {
            Events.Add(ServerToClient.InitCharSelection, OnInitCharSelection);
            Events.Add("debugdestroycam", EndCharSelection);
        }

        private void EndCharSelection(object[] args)
        {
            _characterDisplayCamera?.SetActive(false);
            Player.LocalPlayer.FreezePosition(false);
            Events.CallLocal("setChatState", true);
            RAGE.Game.Ui.DisplayHud(true);
            RAGE.Game.Ui.DisplayRadar(true);

            // TODO: Teleport player into the world
        }

        private void OnInitCharSelection(object[] args)
        {
            Events.CallLocal("setChatState", true); // Enabled for testing TODO: needs to be removed
            var myPlayer = Player.LocalPlayer;
            var myPosition = myPlayer.Position;

            // Stage the model
            myPlayer.Position = new Vector3(-169.3321f, 482.2647f, 133.8789f);
            myPlayer.FreezePosition(true);
            myPlayer.SetHeading(282.6658f);
            myPlayer.SetAlpha(0, false);
            Events.CallRemote("ApplyCharSelectionAnimation");

            // Camera
            var cameraPos = GetCameraPosInFrontOfPlayer(myPlayer, 1.5f);
            _characterDisplayCamera = new CustomCamera(cameraPos, myPosition);
            _characterDisplayCamera.SetActive(true);

            // Display the Browser UI
            CustomBrowser.CreateBrowser("package://CEF/char/index.html");
        }

        private static Vector3 GetCameraPosInFrontOfPlayer(Player myPlayer, float range)
        {
            var forwardX = myPlayer.Position.X + myPlayer.GetForwardX() * range;
            var forwardY = myPlayer.Position.Y + myPlayer.GetForwardY() * range;
            var cameraPos = new Vector3(forwardX, forwardY, myPlayer.Position.Z + 0.5f);
            return cameraPos;
        }
    }
}