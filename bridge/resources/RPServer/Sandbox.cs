using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.Controllers.Util;
using RPServer.Game;
using RPServer.InternalAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;
using Shared.Enums;
using Entity = GTANetworkAPI.Entity;
using Vehicle = GTANetworkAPI.Vehicle;

namespace RPServer
{
    internal class Sandbox : Script
    {
        public Sandbox()
        {
        }


        [Command("cmds")]
        public void Cmds(Client player)
        {
            player.SendChatMessage("/logout /toggletwofactorga /toggletwofactoremail");
            player.SendChatMessage("/veh /ecc /heal /hmc /time /weather /getping /onlineppl /givegun");
            player.SendChatMessage("/setskin /setnick /togflymode /getcamcords /spawnme /playanimation /stopani");
            player.SendChatMessage("/loadipl /removeipl /resetipls /gotopos /getpos /fd");
            player.SendChatMessage("/changechar /addx /addy /addz /getfowardpos /testclothes");
            player.SendChatMessage("/createmarker /createtextlabel, /createblip /gotowp");
        }

        [Command("setforumname")]
        public void setforumname(Client client, string forumName)
        {
            client.GetAccount().ForumName = forumName;
        }

        [Command("test")]
        public void test(Client client)
        {
            client.TriggerEvent("test");
        }


        [Command("gotowp")]
        public void cmd_gotowaypoint(Client client)
        {
            client.TriggerEvent("gotowaypoint");
        }


        [Command("createmarker")]
        public void cmd_createmarker(Client client, uint type)
        {
            NAPI.Marker.CreateMarker(type, client.Position, new Vector3(), new Vector3(), 1.0f, new Color(255, 0, 125), true, 0);
        }

        [Command("createtextlabel")]
        public void cmd_createtextlabel(Client client, string text, float range, int fontInt)
        {
            if (!Enum.IsDefined(typeof(Font), fontInt))
            {
                client.SendChatMessage("Invalid font, valid values are 0, 1, 2, 4, 7");
                return;
            }

            Enum.TryParse<Font>(fontInt.ToString(), out var font);

            NAPI.TextLabel.CreateTextLabel(text, client.Position, range, font, new Color(255, 0, 25));
        }

        [Command("createblip")]
        public void cmd_createtextlabel(Client client, string text, uint sprite)
        {
            NAPI.Blip.CreateBlip(sprite, client.Position, 1.0f, 0, text, 255, 0F, false, 0, 0);
        }



        [Command("testclothes")]
        public void cmdtestclothes(Client client)
        {
            client.TriggerEvent("testclothes");

        }

        [Command("addx")]
        public void addx(Client client)
        {
            Vector3 pos = client.Position;
            pos.X += 2;
            client.Position = pos;
        }
        [Command("addy")]
        public void addy(Client client)
        {
            Vector3 pos = client.Position;
            pos.Y += 2;
            client.Position = pos;
        }
        [Command("addz")]
        public void addz(Client client)
        {
            Vector3 pos = client.Position;
            pos.Z += 2;
            client.Position = pos;
        }

        [Command("getforwardpos")]
        public void getforwardpos(Client client, float x, float y, float z, float heading)
        {
            client.TriggerEvent("testpos", x, y, z, heading);
        }

        [Command("gethere", GreedyArg = true)]
        public void CmdGetHere(Client client, string trg)
        {
            var target = ClientMethods.FindClient(trg);

            if (target == null)
                return;


            if (client == target)
            {
                client.SendChatMessage("Can't /gethere to urself.");
                return;
            }


            target.Position = client.Position.Around(2);
        }


        [Command("goto", GreedyArg = true)]
        public void CmdGoto(Client client, string trg)
        {
            Client target = ClientMethods.FindClient(trg);

            if (target == null)
                return;

            if (client == target)
            {
                client.SendChatMessage("Can't /goto to urself.");
                return;
            }

            client.Position = target.Position.Around(2);
        }


        [Command("fd")]
        public void CmdFD(Client player)
        {
            player.TriggerEvent("tpinfront");
        }

        [Command("spawnme")]
        public void SpawnMe(Client player)
        {
            NAPI.Player.SpawnPlayer(player, Initialization.DefaultSpawnPos);
        }

        [Command("playanimation")]
        public void PlayAnimation(Client player, string animDict, string animName, int flag)
        {
            player.PlayAnimation(animDict, animName, flag);
        }

        [Command("stopani")]
        public void StopAni(Client player)
        {
            player.StopAnimation();
        }

        [Command("getpos")]
        public void GetPos(Client player)
        {
            player.SendChatMessage(player.Position + "Heading: " + player.Heading);
        }

        [Command("loadipl")]
        public void LoadIPL(Client player, string IPLName)
        {
            NAPI.World.RequestIpl(IPLName);
        }

        [Command("removeipl")]
        public void RemoveIPL(Client player, string IPLName)
        {
            NAPI.World.RemoveIpl(IPLName);
        }

        [Command("resetipls")]
        public void ResetIPLs(Client player)
        {
            NAPI.World.ResetIplList();
        }

        [Command("gotopos", GreedyArg = true)]
        public void GotoPOS(Client player, string pos)
        {
            var matches = Regex.Matches(pos, @"([-]?[0-9]+\.[0-9]*)+");
            if (matches.Count < 3) return;

            var newPos = new Vector3();
            

            newPos.X = float.Parse(matches[0].Value, CultureInfo.InvariantCulture.NumberFormat);
            newPos.Y = float.Parse(matches[1].Value, CultureInfo.InvariantCulture.NumberFormat);
            newPos.Z = float.Parse(matches[2].Value, CultureInfo.InvariantCulture.NumberFormat);

            player.Position = newPos;

        }


        [Command("togflymode")]
        public void ToggleFlyMode(Client player)
        {
            player.TriggerEvent("flyModeStart");
        }

        [Command("getcamcords")]
        public void GetCamCords(Client player)
        {
            player.TriggerEvent("getCamCoords", player.Name);
        }

        [RemoteEvent("saveCamCoords")]
        public void ClientEvent_OnSaveCamCoords(Client client, string coords, string pointAt)
        {
            Logger.GetInstance().ChatLog(coords);
            Logger.GetInstance().ChatLog(pointAt);
        }

        [Command("setnick")]
        public void SetNickName(Client player, string nick)
        {
            if (!player.IsLoggedIn()) return;
            if (string.IsNullOrWhiteSpace(nick))
            {
                player.SendChatMessage("Can't be empty");
                return;
            }
            player.GetAccount().NickName = nick;
            player.SendChatMessage("You set your nick to: " + nick);
        }

        [Command("setskin")]
        public void SetSkin(Client player, string skinName)
        {
            if (DataValidator.IsDigitsOnly(skinName))
            {
                var skinId = uint.Parse(skinName);
                NAPI.Entity.SetEntityModel(player.Handle, skinId);
            }
            else
            {
                NAPI.Entity.SetEntityModel(player.Handle, (uint) NAPI.Util.PedNameToModel(skinName));
            }

            player.SendChatMessage("You set your skin to: " + skinName);
        }

        [Command("givegun")]
        public void GiveGun(Client player, string weaponName, int ammo)
        {
            if (ammo <= 0) ammo = 1000;
            WeaponHash wepHash = NAPI.Util.WeaponNameToModel(weaponName);
            player.GiveWeapon(wepHash, ammo);
            player.SendChatMessage($"Gave you gun {weaponName} with {ammo} ammo.");

        }


        [Command("getping")]
        public void GetPing(Client player)
        {
            player.SendChatMessage("Your ping: " + player.Ping);
        }

        [Command("onlineppl")]
        public void OnlinePpl(Client player)
        {
            player.SendChatMessage("---[Online]---");
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (!ClientExtensions.IsLoggedIn(p)) player.SendChatMessage($"[UNAUTHED]: Social: {p.SocialClubName}, ClientName: {p.Name}, Ping: {p.Ping}");
                else player.SendChatMessage($"[{ClientExtensions.GetAccount(p).Username}]: Social: {p.SocialClubName}, ClientName: {p.Name}, Ping: {p.Ping}");
            }
        }

        [Command("veh")]
        public void Veh(Client player, string vehicleName, int color1 = 0, int color2 = 0)
        {
            if (player.HasData("PERSONAL_VEHICLE"))
            {
                Entity veh = player.GetData("PERSONAL_VEHICLE");
                veh.Delete();
                player.ResetData("PERSONAL_VEHICLE");
            }

            Vehicle v = null;

            if (!vehicleName.StartsWith("0x"))
            {
                VehicleHash vehHash = NAPI.Util.VehicleNameToModel(vehicleName);
                if (vehHash.ToString().Equals("0"))
                    return;

                v = NAPI.Vehicle.CreateVehicle(vehHash, player.Position.Around(5), 0f, 0, 0);
            }
            else
            {
                try
                {
                    var vehID = Convert.ToUInt32(vehicleName, 16);
                    v = NAPI.Vehicle.CreateVehicle(vehID, player.Position.Around(5), 0f, 0, 0);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (v == null)
            {
                NAPI.Chat.SendChatMessageToPlayer(player, "Error.");
                return;
            }

            v.NumberPlate = "STATIQUE";
            v.WindowTint = 5;
            v.NumberPlateStyle = 2;
            v.PrimaryColor = color1;
            v.SecondaryColor = color2;
            player.SetData("PERSONAL_VEHICLE", v);
            player.TriggerEvent("NotifyClient", $"Spawned a ~r~{v.DisplayName}");
        }

        [Command("ecc")]
        public void ecc(Client player)
        {
            var vehicles = NAPI.Pools.GetAllVehicles();
            Vector3 playerpos = player.Position;
            Vehicle closest = null;
            float distance = 999999f;


            foreach (var v in vehicles)
            {
                float cardist = v.Position.DistanceTo(playerpos);
                if (cardist < distance)
                {
                    distance = cardist;
                    closest = v;
                }
            }

            if (closest != null)
            {
                var driver = NAPI.Vehicle.GetVehicleDriver(closest);
                if (driver != null)
                {
                    player.SendChatMessage("Someone else is driving the closest vehicle.");
                    return;
                }
                NAPI.Player.SetPlayerIntoVehicle(player, closest, -1);
            }
            else
            {
                NAPI.Chat.SendChatMessageToPlayer(player, "No car");
            }
        }

        [Command("heal")]
        public void CmdHeal(Client player)
        {
            player.Health = 100;
            player.Armor = 1000;
        }

        [Command("hmc")]
        public void CmdFixMyCar(Client player)
        {
            if (player.IsInVehicle)
            {
                Vehicle veh = player.Vehicle;
                veh.Health = 100.0f;
                veh.Repair();
                player.SendChatMessage("Your vehicle has been fixed.");

            }
        }

        [Command("time")]
        public void CmdExplodeMyCar(Client player, int time)
        {
            var currTime = DateTime.Now;
            var newtime = new DateTime(currTime.Year, currTime.Month, currTime.Day, time, 0, 0);
            WorldHandler.CurrentTime = newtime;
        }

        [Command("weather")]
        public void cmdWeather(Client player, string weather)
        {
            NAPI.World.SetWeather(weather);

        }
    }
}
