using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Shared;
using RAGE;
using RAGE.Elements;
using RAGE.NUI;
using RPServerClient.Globals;
using RPServerClient.Util;
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

            // Temp testing events
            Events.Add(ServerToClient.EndCharSelection, EndCharSelection);
            Events.Add("selectchar", SelectChar);
            Events.Add("playchar", PlayChar);
            Events.Add("createchar", CreateChar);
        }

        private void CreateChar(object[] args)
        {
            RAGE.Chat.Output("!{#6E6E6E}Char: creation not implemented yet. Appearance will be randomized.");
            if(args == null || args.Length < 2) return;
            string firstName = args[0].ToString();
            string lastName = args[1].ToString();

            if (firstName.Length < 3 || lastName.Length < 3)
            {
                RAGE.Chat.Output("Firstname and/or lastname too short.");
                return;
            }

            var r = new Random();
            var player = Player.LocalPlayer;

            player.SetHeadBlendData(0, 0, 0, 0, 0, 0, 0, 0, 0, false);
            for (var i = 0; i <= 12; i++) player.SetHeadOverlay(i, 0, 0);
            for (var i = 0; i <= 19; i++) player.SetFaceFeature(i, 0);
            player.SetComponentVariation(2, 0, 0, 0);
            player.SetHairColor(0, 0);

            player.SetHeadBlendData(r.Next(0, 10), r.Next(0, 10), 0, r.Next(0, 10), r.Next(0, 10), 0, 0.5f, 0.5f, 0, false);
            for (var i = 0; i <= 12; i++)
            {
                player.SetHeadOverlay(i, r.Next(0, 1), r.Next(150, 255));
                for(var j=1;j<=2;j++) player.SetHeadOverlayColor(i, j, r.Next(0,60), r.Next(0,60));
            }
            for (var i = 0; i <= 19; i++) player.SetFaceFeature(i, (float)r.NextDouble() * 2 - 1);
            player.SetComponentVariation(2, r.Next(0, 30), 0, 0);
            player.SetHairColor(r.Next(0, 60), r.Next(0, 10));
            player.ResetAlpha();

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

            if(args.Length < 2) return;

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