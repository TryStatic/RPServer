using Shared;
using RAGE;
using RAGE.Elements;
using RPServerClient.Globals;
using RPServerClient.Util;
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
            var player = Player.LocalPlayer;

            // Stage the model
            player.Position = new Vector3(-169.3321f, 482.2647f, 133.8789f);
            player.FreezePosition(false);
            player.SetHeading(282.6658f);
            //myPlayer.SetAlpha(0, false);
            //Events.CallRemote("ApplyCharSelectionAnimation");


            // Camera
            var cameraPos = Helper.GetPosInFrontOfPlayer(player, 1.5f);
            _characterDisplayCamera = new CustomCamera(cameraPos, player.Position);
            _characterDisplayCamera.SetActive(true);

            // Display the Browser UI
            //CustomBrowser.CreateBrowser("package://CEF/char/index.html");
        }

        /*
         * setHeadBlendData(shapeFirstID, shapeSecondID, shapeThirdID, skinFirstID, skinSecondID, skinThirdID, shapeMix, skinMix, thirdMix, isParent);
         * UpdateHeadBlendData(float shapeMix, float skinMix, float thirdMix)
         * GetPedHeadBlendData(ref int headBlendData)
         * public static int GetPedHeadOverlayValue(int ped, int overlayID)
         * public static int GetNumHeadOverlayValues(int overlayID)
         * public static void SetPedHeadOverlay(int ped, int overlayID, int index, float opacity)
         * public static void SetPedHeadOverlayColor(int ped, int overlayID, int colorType, int colorID, int secondColorID)
         */
    }
}