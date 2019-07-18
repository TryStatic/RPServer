using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RAGE;
using RAGE.Elements;
using RPServerClient.Globals;
using RPServerClient.Util;
using Shared;
using Events = RAGE.Events;

namespace RPServerClient.Character
{
    internal class CharSelector : Events.Script
    {
        private readonly Vector3 _displayPosition = new Vector3(-169.3321f, 482.2647f, 133.8789f);
        private readonly float _displayHeading = 282.6658f;
        private readonly Vector3 _hiddenPosition = new Vector3(-163.4660f, 483.5910f, 134.5571f);

        private int _selectedCharId = -1;
        private List<CharDisplay> _charList = new List<CharDisplay>();
        private CustomCamera _characterDisplayCamera;

        private bool _disableControls = false;


        public CharSelector()
        {
            Events.Tick += Render;

            Events.Add(ServerToClient.InitCharSelector, OnInitCharSelector);
            Events.Add(ServerToClient.EndCharSelector, OnEndCharSelector);
            Events.Add(ServerToClient.RenderCharacterList, OnRenderCharacterList);

            // Temp testing events
            Events.Add("selectchar", SelectChar);
            Events.Add("playchar", SpawnChar);
        }

        private void Render(List<Events.TickNametagData> nametags)
        {
            if (_disableControls) RAGE.Game.Pad.DisableAllControlActions(0);
        }

        private void OnInitCharSelector(object[] args)
        {
            Events.CallLocal("setChatState", true); // Enabled for testing TODO: needs to be removed
            var player = Player.LocalPlayer;

            player.FreezePosition(true);
            UnStageModel(player);

            // Camera
            var cameraPos = Helper.GetPosInFrontOfVector3(_displayPosition, _displayHeading, 1.5f);
            RAGE.Chat.Output(cameraPos.ToString());
            RAGE.Chat.Output(_displayPosition.ToString());
            _characterDisplayCamera = new CustomCamera(cameraPos, _displayPosition);
            _characterDisplayCamera.SetActive(true);
            _disableControls = true;

        }

        private void OnEndCharSelector(object[] args)
        {
            _characterDisplayCamera?.SetActive(false);
            _charList = null;
            Player.LocalPlayer.FreezePosition(false);
            Events.CallLocal("setChatState", true);
            RAGE.Game.Ui.DisplayHud(true);
            RAGE.Game.Ui.DisplayRadar(true);
            _disableControls = false;
        }

        private void SpawnChar(object[] args)
        {
            if(_selectedCharId < 0) return;
            if(!IsOwnChar(_selectedCharId)) return;
            Events.CallRemote(ClientToServer.SubmitSpawnCharacter, _selectedCharId);
        }
        
        private void SelectChar(object[] args)
        {
            if(args == null || args.Length < 1) return;
            
            var selectedID = (int)args[0];
            if(selectedID < 0) return;
            if(!IsOwnChar(selectedID)) return;

            StageModel(Player.LocalPlayer);
            _selectedCharId = selectedID;
            Events.CallRemote(ClientToServer.SubmitCharacterSelection, _selectedCharId);
        }

        private void OnRenderCharacterList(object[] args)
        {
            // Display the Browser UI
            //CustomBrowser.CreateBrowser("package://CEF/char/index.html");

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

        private void StageModel(Player p)
        {
            p.Position = _displayPosition;
            p.SetHeading(_displayHeading);
        }

        private void UnStageModel(Player p)
        {
            p.Position = _hiddenPosition;
        }

        private bool IsOwnChar(int selectedCharID) => _charList.Any(c => c.CharID == selectedCharID);
    }
}