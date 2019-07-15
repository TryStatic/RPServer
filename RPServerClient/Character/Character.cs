using System.Collections.Generic;
using Newtonsoft.Json;
using RAGE;
using RAGE.Elements;
using RPServerClient.Globals;
using RPServerClient.Util;
using Shared;
using Events = RAGE.Events;

namespace RPServerClient.Character
{
    internal class Character : Events.Script
    {
        private int _selectedCharId = -1;
        private List<CharDisplay> _charList = new List<CharDisplay>();
        private CustomCamera _characterDisplayCamera;

        public Character()
        {
            Events.Add(ServerToClient.InitCharSelection, OnInitCharSelection);
            Events.Add(ServerToClient.RenderCharacterList, OnRenderCharacterList);

            Events.Add(ServerToClient.DisplayCharError, DisplayError);
            Events.Add(ServerToClient.MoveCharCreationToNextStep, ShowNextPage);

            // Temp testing events
            Events.Add(ServerToClient.EndCharSelection, EndCharSelection);
            Events.Add("selectchar", SelectChar);
            Events.Add("playchar", PlayChar);
            Events.Add("createchar", CreateChar);

            // CEF Events
            Events.Add("SubmitCharData", SubmitCharData);
            Events.Add("UpdateHeadBlend", OnUpdateHeadBlend);
            Events.Add("UpdateFaceFeature", OnUpdateFaceFeature);
            Events.Add("UpdateHeadOverlay", OnUpdateHeadOverlay);

            Events.Add("ZoomToFace", OnZoomToFace);
        }

        private void ShowNextPage(object[] args)
        {
            CustomBrowser.ExecuteFunction("ShowNextStep");
        }

        private void DisplayError(object[] args)
        {
            if (args[0] == null) return;

            var msg = args[0] as string;

            CustomBrowser.ExecuteFunction(new object[] { "showError", msg.Replace("'", @"\'") });
        }

        private void SubmitCharData(object[] args)
        {
            RAGE.Chat.Output("SubmitCharData on client");
            if (args == null || args.Length < 3)
                return;

            var firstname = args[0].ToString();
            var lastname = args[1].ToString();
            var isMale = (bool) args[2];


            Events.CallRemote(ClientToServer.SubmitInitialCharData, firstname, lastname, isMale);
            RAGE.Chat.Output("ok");


        }

        private void OnUpdateHeadBlend(object[] args)
        {
            if (args == null || args.Length < 6) return;

            var shapeFirst = (int)args[0];
            var shapeSecond = (int)args[1];
            var skinFirst = (int)args[2];
            var skinSecond = (int)args[3];
            var ShapeMix = (float) args[4];
            var skinMix = (float) args[5];

            RAGE.Chat.Output("shapeFirst.ToString():" + shapeFirst.ToString());
            RAGE.Chat.Output("shapeSecond.ToString():" + shapeSecond.ToString());
            RAGE.Chat.Output("skinFirst.ToString():" + skinFirst.ToString());
            RAGE.Chat.Output("skinSecond.ToString():" + skinSecond.ToString());
            RAGE.Chat.Output("ShapeMix.ToString():" + ShapeMix.ToString());
            RAGE.Chat.Output("skinMix.ToString():" + skinMix.ToString());
            RAGE.Chat.Output("-------------------------");

            Player.LocalPlayer.SetHeadBlendData(shapeFirst, shapeSecond, skinFirst, skinSecond, 0, 0, ShapeMix, skinMix, 0, false);
        }


        private void OnUpdateFaceFeature(object[] args)
        {
            if (args == null || args.Length < 2) return;
            var indx = int.Parse(args[0].ToString());
            var value = float.Parse(args[1].ToString());

            RAGE.Chat.Output("Index:" + indx);
            RAGE.Chat.Output("Value:" + value);
            RAGE.Chat.Output("-------------------------");


            Player.LocalPlayer.SetFaceFeature(indx, value);
        }

        private void OnUpdateHeadOverlay(object[] args)
        {
            throw new System.NotImplementedException();
        }

        private void OnZoomToFace(object[] args)
        {
            RAGE.Chat.Output("TODO: POINT CAMERA TO FACE");
        }

        private void CreateChar(object[] args)
        {
            var player = Player.LocalPlayer;

            ResetAppearance(player);
            CustomBrowser.CreateBrowser("package://CEF/char/charcreator.html");

            Events.CallRemote(ClientToServer.ApplyCharacterEditAnimation);
            player.ResetAlpha();
        }

        private void ResetAppearance(Player player)
        {
            player.SetHeadBlendData(0, 0, 0, 0, 0, 0, 0, 0, 0, false);
            for (var i = 0; i <= 12; i++) player.SetHeadOverlay(i, 0, 0);
            for (var i = 0; i <= 19; i++) player.SetFaceFeature(i, 0);
            player.SetComponentVariation(2, 0, 0, 0);
            player.SetHairColor(0, 0);
        }

        private void PlayChar(object[] args)
        {
            if(_selectedCharId < 0) return;
            Events.CallRemote(ClientToServer.SubmitSpawnCharacter, _selectedCharId);
        }

        private void SelectChar(object[] args)
        {
            if(args == null || args.Length < 1) return;
            
            var selectedID = (int)args[0];
            if(selectedID < 0) return;

            _selectedCharId = selectedID;
            Events.CallRemote(ClientToServer.SubmitCharacterSelection, _selectedCharId);
        }

        private void OnInitCharSelection(object[] args)
        {
            Events.CallLocal("setChatState", true); // Enabled for testing TODO: needs to be removed
            var player = Player.LocalPlayer;

            // Stage the model
            player.Position = new Vector3(-169.3321f, 482.2647f, 133.8789f);
            player.FreezePosition(true);
            player.SetHeading(282.6658f);

            // Camera
            var cameraPos = Helper.GetPosInFrontOfPlayer(player, 1.5f);
            _characterDisplayCamera = new CustomCamera(cameraPos, player.Position);
            _characterDisplayCamera.SetActive(true);

        }

        private void OnRenderCharacterList(object[] args)
        {
            // Display the Browser UI
            //CustomBrowser.CreateBrowser("package://CEF/char/index.html");
            //Events.CallRemote(ClientToServer.ApplyCharacterEditAnimation);

            if (args.Length < 2) return;

            _charList = JsonConvert.DeserializeObject<List<CharDisplay>>(args[0] as string);
            _selectedCharId = (int) args[1];

            RAGE.Chat.Output("[CLIENT]: Your chars: ");
            foreach (var c in _charList)
            {
                RAGE.Chat.Output($"[CLIENT]: {c.CharID}, {c.CharName}");
            }
            RAGE.Chat.Output("[CLIENT]: -------------");

            if(_selectedCharId >= 0) SelectChar(new object[]{ _selectedCharId });
        }


        private void EndCharSelection(object[] args)
        {
            _characterDisplayCamera?.SetActive(false);
            _charList = null;
            Player.LocalPlayer.FreezePosition(false);
            Events.CallLocal("setChatState", true);
            RAGE.Game.Ui.DisplayHud(true);
            RAGE.Game.Ui.DisplayRadar(true);
        }
    }
}