using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using RAGE;
using RAGE.NUI;
using RAGE.Ui;
using RPServerClient.Util;
using Shared.Enums;
using Player = RAGE.Elements.Player;

namespace RPServerClient
{
    class compvars
    {
        public int[] drawableid = new int[300];
        public int[] textureid = new int[300];
        public int[] paletteid = new int[300];

    }

    internal class Sandbox : Events.Script
    {

        private readonly MenuPool _clothespool = new MenuPool();
        UIMenu menu = new UIMenu("Clothes", "Clothes", new Point(1350, 200));
        private List<compvars> compvars = new List<compvars>();

        public Sandbox()
        {
            for (int i = 0; i < 12; i++)
            {
                compvars.Add(new compvars());
            }

            Events.Tick += Tick;
            //Events.Tick += ProcessWaypointTeleport;

            // FlyScript
            Events.Add("ToggleFlyMode", OnToggleFlyMode);

            // Chars
            Events.Add("tpinfront", TeleportInFront);
            Events.Add("testpos", TestPos);
            Events.Add("testclothes", TestClothes);
            Events.Add("test", test);
            Events.Add("NotifyClient", NotifyClient);
            //Events.Add("gotowaypoint", GotoWaypoint);


            // Boost
            uint stamina = RAGE.Game.Misc.GetHashKey("SP0_STAMINA");
            RAGE.Game.Stats.StatSetInt(stamina, 100, true);
            uint flying = RAGE.Game.Misc.GetHashKey("SP0_FLYING");
            RAGE.Game.Stats.StatSetInt(flying, 100, true);
            uint driving = RAGE.Game.Misc.GetHashKey("SP0_DRIVING");
            RAGE.Game.Stats.StatSetInt(driving, 100, true);
            uint shooting = RAGE.Game.Misc.GetHashKey("SP0_SHOOTING");
            RAGE.Game.Stats.StatSetInt(shooting, 100, true);
            uint strength = RAGE.Game.Misc.GetHashKey("SP0_STRENGTH");
            RAGE.Game.Stats.StatSetInt(strength, 100, true);
            uint stealth = RAGE.Game.Misc.GetHashKey("SP0_STEALTH");
            RAGE.Game.Stats.StatSetInt(stealth, 100, true);
            uint lungCapacity = RAGE.Game.Misc.GetHashKey("SP0_LUNGCAPACITY");
            RAGE.Game.Stats.StatSetInt(lungCapacity, 100, true);

            _clothespool.Add(menu);

            var comps = new List<dynamic>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            var drawables = new List<dynamic>();
            var textures = new List<dynamic>();
            var palletes = new List<dynamic>();

            for (int i = 0; i < 300; i++)
            {
                drawables.Add(i);
                textures.Add(i);
                palletes.Add(i);
            }

            var compsMenu = new UIMenuListItem("CompID", comps, 0);
            menu.AddItem(compsMenu);

            var drawablesMenu = new UIMenuListItem("DrawableID", drawables, 0);
            menu.AddItem(drawablesMenu);

            var texturesMenu = new UIMenuListItem("TextureID", textures, 0);
            menu.AddItem(texturesMenu);

            var palletesMenu = new UIMenuListItem("PalleteID", palletes, 0);
            menu.AddItem(palletesMenu);


            menu.OnListChange += (sender, item, index) =>
            {
                if (item == compsMenu)
                {
                    drawablesMenu.Index = compvars[compsMenu.Index].drawableid[compsMenu.Index];
                    texturesMenu.Index = compvars[compsMenu.Index].textureid[compsMenu.Index];
                    palletesMenu.Index = compvars[compsMenu.Index].paletteid[compsMenu.Index];
                    return;
                }

                compvars[compsMenu.Index].drawableid[compsMenu.Index] = drawablesMenu.Index;
                compvars[compsMenu.Index].textureid[compsMenu.Index] = texturesMenu.Index;
                compvars[compsMenu.Index].paletteid[compsMenu.Index] = palletesMenu.Index;
                RAGE.Chat.Output($"{comps[compsMenu.Index]}, {drawables[drawablesMenu.Index]}, {textures[texturesMenu.Index]}, {palletes[palletesMenu.Index]}");
                Player.LocalPlayer.SetComponentVariation(comps[compsMenu.Index], drawables[drawablesMenu.Index], textures[texturesMenu.Index], palletes[palletesMenu.Index]);
            };

        }

        private void GotoWaypoint(object[] args)
        {
            var waypointCoords = Helper.GetWaypointCoords();
            if (waypointCoords == null)
            {
                RAGE.Chat.Output("No waypoint set");
                return;
            }

            var oldPos = Player.LocalPlayer.Position;
            var groundZ = 0.0f;
            var worked = RAGE.Game.Misc.GetGroundZFor3dCoord(waypointCoords.X, waypointCoords.Y, 900.0f, ref groundZ, false);
            if (worked)
            {
                Player.LocalPlayer.Position = new Vector3(waypointCoords.X, waypointCoords.Y, groundZ);
                RAGE.Chat.Output("Teleported.");
            }
            else
            {
                Player.LocalPlayer.Position = waypointCoords;
                Player.LocalPlayer.FreezePosition(true);

                Task.Delay(1500).ContinueWith(task =>
                {

                    for (var i = 1; i <= 60; i++)
                    {
                        RAGE.Chat.Output(i.ToString());
                        worked = RAGE.Game.Misc.GetGroundZFor3dCoord(waypointCoords.X, waypointCoords.Y, 25 * i, ref groundZ, false);
                        if (!worked) continue;
                        RAGE.Chat.Output("Found groundZ=" + groundZ);
                        Vector3 newPos = new Vector3(waypointCoords.X, waypointCoords.Y, groundZ + 1);
                        Player.LocalPlayer.Position = newPos;
                        break;
                    }
                    if (!worked)
                    {
                        Player.LocalPlayer.Position = oldPos;
                    }
                    Player.LocalPlayer.FreezePosition(false);
                });
            }
        }

        private void NotifyClient(object[] args)
        {
            if(args == null || args.Length < 1) return;

            var msg = args[0] as string;

            RAGE.Game.Ui.SetNotificationTextEntry("STRING");
            RAGE.Game.Ui.AddTextComponentSubstringPlayerName(msg);
            RAGE.Game.Ui.DrawNotification(false, false);

        }

        private void test(object[] args)
        {

        }

        private void TestClothes(object[] args)
        {
            _clothespool.RefreshIndex();
            menu.Visible = true;
        }

        private void TestPos(object[] args)
        {
            var vect = Helper.GetPosInFrontOfVector3(new Vector3((float)args[0], (float)args[1], (float)args[2]), (float)args[3], 1);
            RAGE.Chat.Output(vect.ToString());
        }

        private bool toggle = false;
        private void Tick(List<Events.TickNametagData> nametags)
        {
            KeyManager.KeyBind(KeyCodes.VK_M, () =>
            {
                RAGE.Chat.Show(toggle);

                toggle = !toggle;
            });

            _clothespool?.ProcessMenus();
        }

        private void TeleportInFront(object[] args)
        {
            var pos = Helper.GetPosInFrontOfPlayer(Player.LocalPlayer, 2.0f);
            Player.LocalPlayer.Position = pos;
        }

        private void OnToggleFlyMode(object[] args) => Events.CallLocal("flyModeStart");

    }
}
