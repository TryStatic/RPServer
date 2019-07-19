﻿using System.Globalization;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.Game;
using RPServer.Util;

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
            player.SendChatMessage("/setheadblend /setheadoverlay, /setheadoverlaycolor, /setfacefeature, /setcompvar /seteyecolor");
            player.SendChatMessage("/removecam /changechar, /selectchar, /play /createchar /addx /addy /addz /getfowardpos");
            player.SendChatMessage("/createmarker, /createtextlabel, /createblip /gotowaypoint");
        }

        [Command("gotowaypoint")]
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
        public void cmd_createtextlabel(Client client, string text)
        {
            NAPI.TextLabel.CreateTextLabel(text, client.Position, 1.0f, 1.0f, 0, new Color(255, 0, 0), false, 0);
        }

        [Command("createblip")]
        public void cmd_createtextlabel(Client client, string text, uint sprite)
        {
            NAPI.Blip.CreateBlip(sprite, client.Position, 1.0f, 0, text, 255, 0F, false, 0, 0);
        }



        [Command("test")]
        public void test(Client client)
        {
            client.TriggerEvent("test");

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

        [Command("gethere")]
        public void CmdGetHere(Client client, string trg)
        {
            var target = NAPI.Player.GetPlayerFromName(trg);

            if (target == null)
                return;

            target.Position = client.Position.Around(5);
        }


        [Command("goto")]
        public void CmdGoto(Client client, string trg)
        {
            var target = NAPI.Player.GetPlayerFromName(trg);

            if (target == null)
                return;

            client.Position = target.Position.Around(5);
        }

        [Command("fd")]
        public void CmdFD(Client player)
        {
            player.TriggerEvent("tpinfront");
        }

        [Command("setheadblend")]
        public void SetHeadBlend(Client player, int i1, int i2, int i3, int i4, int i5, int i6, float f1, float f2, float f3, int b = 0)
        {
            bool b1 = b != 0;
            player.TriggerEvent("headdata", i1, i2, i3, i4, i5, i6, f1, f2, f3, b1);
        }

        [Command("setheadoverlay")]
        public void SetHeadOverlay(Client player, int i1, int i2, float f1)
        {
            player.TriggerEvent("headoverlay", i1, i2, f1);
        }

        [Command("setheadoverlaycolor")]
        public void SetHeadOverlayColor(Client player, int i1, int i2, int i3, int i4)
        {
            player.TriggerEvent("headoverlaycolor", i1, i2, i3, i4);
        }

        [Command("setfacefeature")]
        public void SetHeadBlend(Client player, int i1, float f1)
        {
            player.TriggerEvent("facefeautre", i1, f1);
        }

        [Command("setcompvar")]
        public void SetCompVar(Client player, int i1, int i2, int i3, int i4)
        {
            player.TriggerEvent("compvar", i1, i2, i3, i4);
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
                if (!p.IsLoggedIn()) player.SendChatMessage($"[UNAUTHED]: Social: {p.SocialClubName}, ClientName: {p.Name}, Ping: {p.Ping}");
                else player.SendChatMessage($"[{p.GetAccount().Username}]: Social: {p.SocialClubName}, ClientName: {p.Name}, Ping: {p.Ping}");
            }
        }

        [Command("veh")]
        public void Veh(Client player, string vehicleName = "")
        {
            if (player.HasData("PERSONAL_VEHICLE"))
            {
                Entity veh = player.GetData("PERSONAL_VEHICLE");
                veh.Delete();
                player.ResetData("PERSONAL_VEHICLE");
            }

            VehicleHash vehHash = NAPI.Util.VehicleNameToModel(vehicleName);
            if (vehHash.ToString().Equals("0"))
                return;
            Vehicle v = NAPI.Vehicle.CreateVehicle(vehHash, player.Position.Around(5), 0f, 0, 0);
            v.NumberPlate = "STATIQUE";
            v.WindowTint = 5;
            v.NumberPlateStyle = 2;
            player.SetData("PERSONAL_VEHICLE", v);
            NAPI.Chat.SendChatMessageToPlayer(player, "Spawned a " + vehicleName + ".");
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
            NAPI.World.SetTime(time, 0, 0);

        }

        [Command("weather")]
        public void cmdWeather(Client player, string weather)
        {
            NAPI.World.SetWeather(weather);

        }
    }
}
