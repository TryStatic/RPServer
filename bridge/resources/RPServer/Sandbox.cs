using System.Globalization;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer._init;
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
            player.SendChatMessage("/loadipl /removeipl /resetipls /gotopos /getpos");

        }

        [Command("spawnme")]
        public void SpawnMe(Client player)
        {
            NAPI.Player.SpawnPlayer(player, Globals.DefaultSpawnPos);
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
            player.GetAccountData().NickName = nick;
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
                else player.SendChatMessage($"[{p.GetAccountData().Username}]: Social: {p.SocialClubName}, ClientName: {p.Name}, Ping: {p.Ping}");
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
