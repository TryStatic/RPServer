using System.Collections.Generic;
using Newtonsoft.Json;
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
        private int _selectedCharId = -1;
        private List<CharDisplay> _charList = new List<CharDisplay>();
        private CustomCamera _characterDisplayCamera;

        public Character()
        {
            Events.Add(ServerToClient.InitCharSelection, OnInitCharSelection);
            Events.Add(ServerToClient.RenderCharacterList, OnRenderCharacterList);

            // Temp testing events
            Events.Add("debugdestroycam", EndCharSelection);
            Events.Add("selectchar", SelectChar);
        }

        private void SelectChar(object[] args)
        {
            if(args == null || args.Length < 1) return;
            
            var selectedID = (int)args[0];
            _selectedCharId = selectedID;
            Events.CallRemote(ClientToServer.SubmitCharacterSelection, _selectedCharId);
        }

        private void OnInitCharSelection(object[] args)
        {
            Events.CallLocal("setChatState", true); // Enabled for testing TODO: needs to be removed
            var player = Player.LocalPlayer;

            // Stage the model
            player.Position = new Vector3(-169.3321f, 482.2647f, 133.8789f);
            player.FreezePosition(false);
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
            //Events.CallRemote("ApplyCharSelectionAnimation");

            _charList = JsonConvert.DeserializeObject<List<CharDisplay>>(args[0] as string);

            // TODO: Add autoselect if there's at least 1 char

            RAGE.Chat.Output("[CLIENT]: Your chars: ");
            foreach (var c in _charList)
            {
                RAGE.Chat.Output($"[CLIENT]: {c.CharID}, {c.CharName}");
            }
            RAGE.Chat.Output("[CLIENT]: -------------");
        }


        private void EndCharSelection(object[] args)
        {
            _characterDisplayCamera?.SetActive(false);
            _charList = null;
            Player.LocalPlayer.FreezePosition(false);
            Events.CallLocal("setChatState", true);
            RAGE.Game.Ui.DisplayHud(true);
            RAGE.Game.Ui.DisplayRadar(true);
            // TODO: Teleport player into the world
        }
    }
}