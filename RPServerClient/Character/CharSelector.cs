using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;
using RAGE;
using RAGE.Game;
using RAGE.NUI;
using RPServerClient.Client;
using RPServerClient.Util;
using Shared.Data;
using Player = RAGE.Elements.Player;

namespace RPServerClient.Character
{
    public delegate void OnCharacterSpawnDelegate(object source, EventArgs e);

    public delegate void OnCharacterDespawnDelegate(object source, EventArgs e);

    internal class CharSelector : Events.Script
    {
        public static CamHandler _camera;

        private readonly float _displayHeading = 347.3495f;

        //  
        private readonly Vector3 _displayPosition = new Vector3(-438.8815f, 1074.186f, 352.3494f);
        private readonly Vector3 _hiddenPosition = new Vector3(-439.2427f, 1086.262f, 350.5516f);
        private List<CharDisplay> _charList = new List<CharDisplay>();
        private MenuPool _charMenu;

        private bool _disableControls;

        private int _selectedCharId = -1;


        public CharSelector()
        {
            Events.Tick += Render;

            Events.Add(Shared.Events.ServerToClient.Character.InitCharSelector, OnInitCharSelector);
            Events.Add(Shared.Events.ServerToClient.Character.EndCharSelector, OnEndCharSelector);
            Events.Add(Shared.Events.ServerToClient.Character.RenderCharacterList, OnRenderCharacterList);

            // Temp testing events
            Events.Add("selectchar", SelectChar);
            Events.Add("playchar", SpawnChar);
        }

        public static event OnCharacterSpawnDelegate CharacterSpawn;
        public static event OnCharacterDespawnDelegate CharacterDespawn;

        private void Render(List<Events.TickNametagData> nametags)
        {
            if (_disableControls) Pad.DisableAllControlActions(0);
            _charMenu?.ProcessMenus();
        }

        private void OnInitCharSelector(object[] args)
        {
            var player = Player.LocalPlayer;
            player.FreezePosition(true);
            UnStageModel(player);
            _disableControls = true;
            _camera = new CamHandler();
            _camera.SetPos(Helper.GetPosInFrontOfVector3(_displayPosition, _displayHeading, 1.5f), _displayPosition);
            _camera.SetActive(true, true, 3000);
            CharacterDespawn?.Invoke(Player.LocalPlayer, EventArgs.Empty);
        }

        private void OnEndCharSelector(object[] args)
        {
            _charList = null;
            Player.LocalPlayer.FreezePosition(false);
            Ui.DisplayHud(true);
            Ui.DisplayRadar(true);
            _disableControls = false;
            _camera.SetActive(false);
            _camera.Destroy();
            _camera = null;
            RAGE.Chat.Show(true);
        }

        private void SpawnChar(object[] args)
        {
            if (_selectedCharId < 0) return;
            if (!IsOwnChar(_selectedCharId)) return;
            Events.CallRemote(Shared.Events.ClientToServer.Character.SubmitSpawnCharacter, _selectedCharId);
            CharacterSpawn?.Invoke(Player.LocalPlayer, EventArgs.Empty);
        }

        private void SelectChar(object[] args)
        {
            if (args == null || args.Length < 1) return;

            var selectedID = (int) args[0];
            if (selectedID < 0) return;
            if (!IsOwnChar(selectedID)) return;

            StageModel(Player.LocalPlayer);
            _selectedCharId = selectedID;
            Events.CallRemote(Shared.Events.ClientToServer.Character.SubmitCharacterSelection, _selectedCharId);
        }

        private void OnRenderCharacterList(object[] args)
        {
            if (args.Length < 2) return;

            _charList = JsonConvert.DeserializeObject<List<CharDisplay>>(args[0] as string);
            _selectedCharId = (int) args[1];

            _charMenu = new MenuPool();

            var p = new Point(1350, 200);

            var menu = new UIMenu("Char Select", "Select a character", p);
            _charMenu.Add(menu);

            foreach (var c in _charList)
            {
                var ch = new UIMenuItem($"{c.CharName}");
                menu.AddItem(ch);
            }

            var spawnCharItem = new UIMenuColoredItem("Spawn Character", Color.DarkCyan, Color.LightBlue);
            menu.AddItem(spawnCharItem);

            var createCharItem = new UIMenuColoredItem("Create New Character", Color.CadetBlue, Color.LightBlue);
            menu.AddItem(createCharItem);


            menu.OnItemSelect += (sender, item, index) =>
            {
                if (item == createCharItem)
                {
                    _camera.SetActive(false);
                    _camera.Destroy();
                    _camera = null;
                    Events.CallLocal("createchar");
                    _charMenu.CloseAllMenus();
                    return;
                }

                if (item == spawnCharItem)
                {
                    if (_selectedCharId < 0) return;
                    SpawnChar(null);
                    _charMenu.CloseAllMenus();
                    return;
                }

                var selectedChar = _charList.Find(c => c.CharName == item.Text);
                SelectChar(new object[] {selectedChar.CharID});
            };

            menu.Visible = true;

            if (_selectedCharId >= 0) SelectChar(new object[] {_selectedCharId});
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

        private bool IsOwnChar(int selectedCharID)
        {
            return _charList.Any(c => c.CharID == selectedCharID);
        }
    }
}