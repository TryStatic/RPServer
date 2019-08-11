﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using RPServer.Models;
using System.Threading.Tasks;
using GTANetworkMethods;
using RPServer.Controllers.Util;
using RPServer.InternalAPI.Extensions;
using RPServer.Models.Inventory;
using RPServer.Resource;
using RPServer.Util;
using static Shared.Data.Colors;
using Task = System.Threading.Tasks.Task;
using Vehicle = GTANetworkAPI.Vehicle;

namespace RPServer.Controllers
{
    internal class VehicleHandler : Script
    {
        public VehicleHandler()
        {
            CharacterHandler.CharacterSpawn += OnCharacterSpawn;
        }

        [Command(CmdStrings.CMD_Vehicle, Alias = CmdStrings.CMD_Vehicle_Alias, GreedyArg = true)]
        public async void CMD_Vehicle(Client client, string args = "")
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            var cmdParser = new CommandParser(args);
            if (!cmdParser.HasNextToken())
            {
                ChatHandler.SendCommandUsageText(client, CmdStrings.CMD_Vehicle_HelpText);
                return;
            }
            switch (cmdParser.GetNextToken())
            {
                case CmdStrings.SUBCMD_Vehicle_Create:
                    if (!cmdParser.HasNextToken())
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) create [model]");
                        return;
                    }

                    var vehicleModel = cmdParser.GetNextToken();

                    uint modelID;
                    if (DataValidator.IsDigitsOnly(vehicleModel))
                    {
                        modelID = uint.Parse(vehicleModel);
                    }
                    else
                    {
                        if (vehicleModel.StartsWith("0x"))
                        {
                            try
                            {
                                modelID = Convert.ToUInt32(vehicleModel, 16);
                            }
                            catch (Exception)
                            {
                                ChatHandler.SendCommandErrorText(client, "This is not a valid vehicle model.");
                                return;
                            }
                        }
                        else
                        {
                            modelID = (uint)NAPI.Util.VehicleNameToModel(vehicleModel);
                        }
                    }

                    if (!DataValidator.ValidatePositiveNumber(DataValidator.ValidationNumbers.VehicleModelID, modelID))
                    {
                        ChatHandler.SendCommandErrorText(client, "This is not a valid vehicle model.");
                        return;
                    }

                    await CreateVehicleAsync(client.GetActiveChar(), modelID);
                    ChatHandler.SendCommandSuccessText(client, "Vehicle Created!");
                    break;
                case CmdStrings.SUBCMD_Vehicle_List:
                    DisplayVehicles(client, client.GetActiveChar().Vehicles);
                    break;
                case CmdStrings.SUBCMD_Vehicle_Stats:
                    var pv = client.Vehicle;
                    if (pv == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "You are not in any vehicle.");
                        return;
                    }
                    DisplayVehicleStats(client, pv);
                    break;
                case CmdStrings.SUBCMD_Vehicle_Spawn:
                    if (!cmdParser.HasNextToken())
                    {
                        DisplayVehicles(client, client.GetActiveChar().Vehicles);
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }
                    int vehSpawnID;
                    try
                    {
                        vehSpawnID = int.Parse(cmdParser.GetNextToken());
                    }
                    catch (Exception)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }

                    SpawnVehicle(client, vehSpawnID);
                    break;
                case CmdStrings.SUBCMD_Vehicle_Despawn:
                    if (!cmdParser.HasNextToken())
                    {
                        DisplayVehicles(client, client.GetActiveChar().Vehicles);
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }
                    int vehDespawnID;
                    try
                    {
                        vehDespawnID = int.Parse(cmdParser.GetNextToken());
                    }
                    catch (Exception)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) despawn [vehicleID]");
                        return;
                    }
                    DespawnVehicle(client, vehDespawnID);
                    break;
                case CmdStrings.SUBCMD_Vehicle_Delete:
                    if (!cmdParser.HasNextToken())
                    {
                        DisplayVehicles(client, client.GetActiveChar().Vehicles);
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }
                    int vehDelID;
                    try
                    {
                        vehDelID = int.Parse(cmdParser.GetNextToken());
                    }
                    catch (Exception)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) delete [vehicleID]");
                        return;
                    }
                    await DeleteVehicle(client, vehDelID);
                    break;
                default:
                    ChatHandler.SendCommandUsageText(client, CmdStrings.CMD_Vehicle_HelpText);
                    break;
            }
        }

        private void OnCharacterSpawn(object source, EventArgs e)
        {
            var client = source as Client;

            foreach (var veh in NAPI.Pools.GetAllVehicles())
            {
                if (!veh.HasData(DataKey.ServerVehicleData)) continue;
                var spawnedVehData = (VehicleModel)veh.GetData(DataKey.ServerVehicleData);
                foreach (var vehData in client.GetActiveChar().Vehicles)
                {
                    if (vehData != spawnedVehData) continue;
                    vehData.VehEntity = veh;
                    break;
                }
            }
        }

        private async Task DeleteVehicle(Client client, int vehicleSqlID)
        {
            var vehToDelete = client.GetActiveChar().Vehicles.FirstOrDefault(v => v.ID == vehicleSqlID);

            if (vehToDelete == null)
            {
                ChatHandler.SendCommandErrorText(client, "Couldn't find that vehicle.");
                return;
            }

            if (vehToDelete.VehEntity != null)
            {
                vehToDelete.VehEntity.Delete();
                vehToDelete.VehEntity = null;
            }

            client.GetActiveChar().Vehicles.Remove(vehToDelete);
            await vehToDelete.DeleteAsync();
            await DeleteVehicleAsync(client.GetActiveChar(), vehToDelete);

            ChatHandler.SendCommandSuccessText(client, "Vehicle Deleted");
        }

        private void DespawnVehicle(Client client, int vehicleSqlID)
        {
            // Find teh vehicle
            var vehToDespawn = client.GetActiveChar().Vehicles.FirstOrDefault(v => v.ID == vehicleSqlID);

            // Exists?
            if (vehToDespawn == null)
            {
                ChatHandler.SendCommandErrorText(client, "Couldn't find that vehicle");
                return;
            }

            // Is it spawned? (ie. has an entity attached on it?)
            if (vehToDespawn.VehEntity == null)
            {
                ChatHandler.SendCommandErrorText(client, "That vehicle is not spawned.");
                return;
            }

            // Delete the entity
            vehToDespawn.VehEntity.Delete();

            // Set the ref to null
            vehToDespawn.VehEntity = null;

            ChatHandler.SendCommandSuccessText(client, "Vehicle depawned.");
        }

        private void SpawnVehicle(Client client, int vehicleSqlID)
        {
            // Find the vehicle in the database
            var vehToSpawn = client.GetActiveChar().Vehicles.FirstOrDefault(v => v.ID == vehicleSqlID);

            // Exists?
            if (vehToSpawn == null)
            {
                ChatHandler.SendCommandErrorText(client, "That vehicle id doesn't exist or you don't own it.");
                return;
            }

            // Is it spawned already?
            if (vehToSpawn.VehEntity != null)
            {
                ChatHandler.SendCommandErrorText(client, "That vehicle is already spawned.");
                return;
            }

            // Load inventory stuff
            vehToSpawn.Trunk = InventoryModel.LoadInventoryAsync(vehToSpawn, ItemModel.VehicleContainer.Trunk).GetAwaiter().GetResult();
            vehToSpawn.Glovebox = InventoryModel.LoadInventoryAsync(vehToSpawn, ItemModel.VehicleContainer.Glovebox).GetAwaiter().GetResult();

            // Create the entity
            vehToSpawn.VehEntity = NAPI.Vehicle.CreateVehicle(vehToSpawn.Model, client.Position.Around(3), 0, vehToSpawn.PrimaryColor, vehToSpawn.SecondaryColor);

            // Set entity related stuff
            vehToSpawn.VehEntity.NumberPlate = vehToSpawn.PlateText;
            vehToSpawn.VehEntity.NumberPlateStyle = vehToSpawn.PlateStyle;

            // Attach our vehicle instance on the entity
            vehToSpawn.VehEntity.SetData(DataKey.ServerVehicleData, vehToSpawn);

            ChatHandler.SendCommandSuccessText(client, "Vehicle spawned.");
        }

        private void DisplayVehicleStats(Client client, Vehicle pv)
        {
            var vehData = (VehicleModel)pv.GetData(DataKey.ServerVehicleData);

            var serverData = vehData == null ? "That vehicle has no server-side data." : $"\tSqlID: {vehData.ID} | OwnerSQLID: {vehData.OwnerID}";
            ChatHandler.SendClientMessage(client, serverData);
            ChatHandler.SendClientMessage(client, $"VehEntityID: {pv.Handle} | {pv.DisplayName} | HP: {pv.Health} | Locked: {pv.Locked} | Plate: {pv.NumberPlate} | PlateStyle: {pv.NumberPlateStyle}");
            ChatHandler.SendClientMessage(client, "------------------------");
        }

        private void DisplayVehicles(Client client, HashSet<VehicleModel> vehicles)
        {
            if (client.GetActiveChar().Vehicles.Count == 0)
            {
                ChatHandler.SendClientMessage(client, "!{#D4D4D4}You don't own any vehicles.");
            }
            else
            {
                ChatHandler.SendClientMessage(client, "!{#D4D4D4}Your owned vehicles:");
                foreach (var v in client.GetActiveChar().Vehicles)
                {
                    var spawned = v.VehEntity != null;
                    if (spawned) ChatHandler.SendClientMessage(client, $"\t{COLOR_GRAD3}SqlID: {COLOR_WHITE}{v.ID}{COLOR_GRAD3} | Model: {COLOR_WHITE}{v.Model}{COLOR_GRAD3} | Spawned: {COLOR_GREEN}Yes");
                    else ChatHandler.SendClientMessage(client, $"\t{COLOR_GRAD3}SqlID: {COLOR_WHITE}{v.ID}{COLOR_GRAD3} | Model: {COLOR_WHITE}{v.Model}{COLOR_GRAD3} | Spawned: {COLOR_RED}No");
                }
            }
        }

        private async Task DeleteVehicleAsync(CharacterModel owner, VehicleModel vehToDelete)
        {
            owner.Vehicles.Remove(vehToDelete);
            await vehToDelete.DeleteAsync();
        }

        public static async Task<int> CreateVehicleAsync(CharacterModel owner, uint model)
        {
            if(owner == null) throw new Exception("CreateVehicleAsync in VehicleHandler was passed a null ChatacterModel owner.");
            var newVeh = new VehicleModel(owner.ID)
            {
                Model = model,
                PlateText = await GenerateLicensePlateText()
            };
            var vehID = await newVeh.CreateAsync();
            if (vehID < 0) throw new Exception("There was an error while creating a new vehicle.");
            owner.Vehicles.Add(newVeh);
            return vehID;
        }

        private static async Task<string> GenerateLicensePlateText()
        {
            var newPlate = "UNDEF";
            var iterations = 0;
            while (iterations < 10)
            {
                iterations++;
                int index1 = RandomGenerator.GetInstance().Next(7, 10);
                int index2 = RandomGenerator.GetInstance().Next('A', 'Z'+1);
                if (index2 == 'O') index2 += 1;
                int index3 = RandomGenerator.GetInstance().Next('A', 'Z'+1);
                if (index3 == 'O') index3 -= 1;
                int index4 = RandomGenerator.GetInstance().Next('A', 'Z'+1);
                if (index4 == 'O')
                {
                    if (RandomGenerator.GetInstance().Next(0, 2) % 2 == 0) index4 += 1;
                    else index4 -= 1;
                }
                int index5 = RandomGenerator.GetInstance().Next(0, 10);
                int index6 = RandomGenerator.GetInstance().Next(0, 10);
                int index7 = RandomGenerator.GetInstance().Next(0, 10);
                newPlate = $"{index1}{(char)index2}{(char)index3}{(char)index4}{index5}{index6}{index7}";

                var exist = await VehicleModel.ReadByKeyAsync(() => VehicleModel.Mock.PlateText, newPlate);
                if (!exist.Any() || iterations == 10)
                {
                    if (iterations == 10) Logger.GetInstance().ServerError($"GenerateLicensePlate ran too many times. Forcing duplicate. (Plate: {newPlate})"); // After doomsday maybe this will run
                    break;
                }
            }

            return newPlate;
        }
    }
}
