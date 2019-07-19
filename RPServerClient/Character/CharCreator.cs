using Multiplayer;
using RAGE.Elements;
using RPServerClient.Globals;
using RPServerClient.Util;
using Shared;
using Events = RAGE.Events;

namespace RPServerClient.Character
{
    internal class CharCreator : Events.Script
    {
        private readonly Quaternion _displayPos = new Quaternion(-169.3321f, 482.2647f, 133.8789f, 282.6658f);
        private readonly Quaternion _hiddenPos = new Quaternion(-163.4660f, 483.5910f, 134.5571f, 282.6658f);
        private CustomCamera _characterDisplayCamera;

        public CharCreator()
        {
            // TEMP commands
            Events.Add("createchar", OnInitCharCreation);

            // Server Events
            Events.Add(ServerToClient.StartCustomization, OnStartCustomization);
            Events.Add(ServerToClient.ResetCharCreation, ResetCharCreation);
            Events.Add(ServerToClient.DisplayCharError, DisplayError);
            Events.Add(ServerToClient.SuccessCharCreation, OnSuccessCharCreation);

            // CEF
            Events.Add("SubmitInitialCharData", SubmitInitialCharData); // Step 1
            Events.Add("UpdateHeadOverlay", OnUpdateHeadOverlay); // Step 2
            Events.Add("UpdateFaceFeature", OnUpdateFaceFeature); // Step 3
            Events.Add("UpdateExtras", OnUpdateExtras); // Step 4
            Events.Add("UpdateHeadBlend", OnUpdateHeadBlend); // Step 5
            Events.Add("SubmitNewCharacter", OnSubmitNewCharacter); // Step 5

            Events.Add("SubmitCancel", OnQuitCharCreation);

        }

        private void OnSuccessCharCreation(object[] args)
        {
            CustomBrowser.ExecuteFunction(new object[] { "ShowStep", "7" });
        }

        private void ResetCharCreation(object[] args)
        {
            ResetAppearance(Player.LocalPlayer);
            CustomBrowser.ExecuteFunction(new object[] { "ShowStep", "1" });
            if (args != null && args.Length > 0) DisplayError(new object[] { args[0].ToString() });
        }

        #region InitilationDestruction
        private void OnInitCharCreation(object[] args)
        {
            UnStageModel(Player.LocalPlayer);
            ResetAppearance(Player.LocalPlayer);
            _characterDisplayCamera = new CustomCamera(Helper.GetPosInFrontOfVector3(_displayPos.GetVector3Part(), _displayPos.W, 1.5f), _displayPos.GetVector3Part(), true);
            CustomBrowser.CreateBrowser("package://CEF/char/charcreator.html");
        }

        private void OnQuitCharCreation(object[] args)
        {
            CustomBrowser.DestroyBrowser(null);
            _characterDisplayCamera.SetCameraState(false);
            _characterDisplayCamera = null;
            Events.CallLocal(ServerToClient.InitCharSelector);
        }

        private void OnSubmitNewCharacter(object[] args)
        {
            var dataAsJson = args[0].ToString();
            Events.CallRemote(ClientToServer.SubmitNewCharacter, dataAsJson);
        }
        #endregion

        #region LocalDisplayCustomization
        private void SubmitInitialCharData(object[] args)
        {
            if (args == null || args.Length < 3)
                return;

            var firstname = args[0].ToString();
            var lastname = args[1].ToString();
            var isMale = (bool)args[2];

            Player.LocalPlayer.Model = isMale ? (uint)1885233650 : 2627665880;

            Events.CallRemote(ClientToServer.SubmitInitialCharData, firstname, lastname);
        }

        private void OnStartCustomization(object[] args)
        {
            StageModel(Player.LocalPlayer);
            Events.CallRemote(ClientToServer.ApplyCharacterEditAnimation);
            ZoomToFace();
            CustomBrowser.ExecuteFunction("ShowNextStep");
        }

        private void OnUpdateHeadBlend(object[] args)
        {
            if (args == null || args.Length < 6) return;

            var shapeFirst = (int)args[0];
            var shapeSecond = (int)args[1];
            var skinFirst = (int)args[2];
            var skinSecond = (int)args[3];
            var shapeMix = (float)args[4];
            var skinMix = (float)args[5];

            Player.LocalPlayer.SetHeadBlendData(shapeFirst, shapeSecond, skinFirst, skinSecond, 0, 0, shapeMix, skinMix, 0, false);
        }

        private void OnUpdateFaceFeature(object[] args)
        {
            if (args == null || args.Length < 2) return;
            var indx = int.Parse(args[0].ToString());
            var value = float.Parse(args[1].ToString());

            Player.LocalPlayer.SetFaceFeature(indx, value);
        }

        private void OnUpdateExtras(object[] args)
        {
            var Hairstyle = (int)args[0];
            var HairColor = (int)args[1];
            var HairHighlightColor = (int)args[2];
            var HairStyleTexture = (int)args[3];
            var EyeColor = (int)args[4];
            Player.LocalPlayer.SetComponentVariation(2, Hairstyle, HairStyleTexture, 0);
            Player.LocalPlayer.SetHairColor(HairColor, HairHighlightColor);
            Player.LocalPlayer.SetEyeColor(EyeColor);
        }

        private void OnUpdateHeadOverlay(object[] args)
        {
            var indx = int.Parse(args[0].ToString());
            var variation = int.Parse(args[1].ToString());
            var opacity = float.Parse(args[2].ToString());
            var color = int.Parse(args[3].ToString());
            var secColor = int.Parse(args[4].ToString());

            Player.LocalPlayer.SetHeadOverlay(indx, variation, opacity);
            if (indx == 1 || indx == 2 || indx == 10)
                Player.LocalPlayer.SetHeadOverlayColor(indx, 1, color, secColor);
            else if (indx == 5 || indx == 8) Player.LocalPlayer.SetHeadOverlayColor(indx, 2, color, secColor);
        }
        #endregion

        #region HelperMethods
        private void ResetAppearance(Player player)
        {
            player.SetHeadBlendData(0, 0, 0, 0, 0, 0, 0, 0, 0, false);
            for (var i = 0; i <= 12; i++) player.SetHeadOverlay(i, 0, 0);
            for (var i = 0; i <= 19; i++) player.SetFaceFeature(i, 0);

            player.SetDefaultComponentVariation();
            player.SetHairColor(0, 0);
        }

        private void StageModel(Player p)
        {
            p.Position = _displayPos.GetVector3Part();
            p.SetHeading(_displayPos.W);
        }

        private void UnStageModel(Player p)
        {
            p.Position = _hiddenPos.GetVector3Part();
        }
        
        private void ZoomToFace()
        {
            var headCoords = Player.LocalPlayer.GetBoneCoords(12844, 0, 0, 0);
            var campos = Helper.GetPosInFrontOfVector3(headCoords, Player.LocalPlayer.GetHeading(), 0.35f);
            _characterDisplayCamera.SetCameraPos(campos, headCoords);
        }

        private void DisplayError(object[] args)
        {
            if (args?[0] == null) return;

            var msg = args[0] as string;

            CustomBrowser.ExecuteFunction(new object[] { "showError", msg.Replace("'", @"\'") });
        }
        #endregion
    }

}
