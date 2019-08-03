using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GTANetworkAPI;
using RPServer.Models;
using System.Threading.Tasks;
using GTANetworkMethods;
using RPServer.Controllers.Util;
using RPServer.InternalAPI.Extensions;
using static Shared.Data.Colors;
using Task = System.Threading.Tasks.Task;
using Vehicle = GTANetworkAPI.Vehicle;

namespace RPServer.Controllers
{
    internal class VehicleHandler : Script
    {
        [Command("vehicle", Alias = "v", GreedyArg = true)]
        public async void CMD_Vehicle(Client client, string args = "")
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            if (args.Trim().Length <= 0)
            {
                ChatHandler.SendCommandUsageText(client, "/v(ehicle) [create/list/stats/spawn/despawn/delete]");
                return;
            }
            string[] arguments = args.Split(' ');
            switch (arguments[0].ToLower())
            {
                case "create":
                    if (arguments.Length < 2)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) create [model]");
                        return;
                    }
                    var vehicleModel = arguments[1].ToLower();
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
                case "list":
                    DisplayVehicles(client, client.GetActiveChar().Vehicles);
                    break;
                case "stats":
                    var pv = client.Vehicle;
                    if (pv == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "You are not in any vehicle.");
                        return;
                    }
                    var vehData = (VehicleModel)pv.GetData("SERVER_VEHICLE_DATA");

                    var serverData = vehData == null ? "That vehicle has no server-side data." : $"\tSqlID: {vehData.ID} | OwnerSQLID: {vehData.OwnerID}";
                    ChatHandler.SendCommandErrorText(client, serverData);
                    ChatHandler.SendCommandErrorText(client, $"{pv.DisplayName} | HP: {pv.Health} | Locked: {pv.Locked} | Plate: {pv.NumberPlate} | PlateStyle: {pv.NumberPlateStyle}");

                    break;
                case "spawn":
                    if (arguments.Length < 2)
                    {
                        DisplayVehicles(client, client.GetActiveChar().Vehicles);
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }
                    int vehSpawnID;
                    try
                    {
                        vehSpawnID = int.Parse(arguments[1]);
                    }
                    catch (Exception)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }

                    var vehToSpawn = client.GetActiveChar().Vehicles.FirstOrDefault(v => v.ID == vehSpawnID);

                    if (vehToSpawn == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "Couldn't find that vehicle");
                        return;
                    }

                    if (vehToSpawn.VehEntity != null)
                    {
                        ChatHandler.SendCommandErrorText(client, "That vehicle is already spawned.");
                        return;
                    }
                    vehToSpawn.VehEntity = NAPI.Vehicle.CreateVehicle(vehToSpawn.Model, client.Position.Around(3), 0, vehToSpawn.PrimaryColor, vehToSpawn.SecondaryColor);
                    vehToSpawn.VehEntity.NumberPlate = vehToSpawn.PlateText;
                    vehToSpawn.VehEntity.NumberPlateStyle = vehToSpawn.PlateStyle;
                    vehToSpawn.VehEntity.SetData("SERVER_VEHICLE_DATA", vehToSpawn);
                    ChatHandler.SendCommandSuccessText(client, "Vehicle spawned.");
                    break;
                case "despawn":
                    if (arguments.Length < 2)
                    {
                        DisplayVehicles(client, client.GetActiveChar().Vehicles);
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }
                    int vehDespawnID;
                    try
                    {
                        vehDespawnID = int.Parse(arguments[1]);
                    }
                    catch (Exception)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) despawn [vehicleID]");
                        return;
                    }
                    var vehToDespawn = client.GetActiveChar().Vehicles.FirstOrDefault(v => v.ID == vehDespawnID);

                    if (vehToDespawn == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "Couldn't find that vehicle");
                        return;
                    }

                    if (vehToDespawn.VehEntity == null)
                    {
                        ChatHandler.SendCommandErrorText(client, "That vehicle is not spawned.");
                        return;
                    }

                    vehToDespawn.VehEntity.Delete();
                    vehToDespawn.VehEntity = null;
                    ChatHandler.SendCommandSuccessText(client, "Vehicle spawned.");
                    break;
                case "delete":
                    if (arguments.Length < 2)
                    {
                        DisplayVehicles(client, client.GetActiveChar().Vehicles);
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) spawn [vehicleID]");
                        return;
                    }
                    int vehDelID;
                    try
                    {
                        vehDelID = int.Parse(arguments[1]);
                    }
                    catch (Exception)
                    {
                        ChatHandler.SendCommandUsageText(client, "/v(ehicle) delete [vehicleID]");
                        return;
                    }
                    var vehToDelete = client.GetActiveChar().Vehicles.FirstOrDefault(v => v.ID == vehDelID);

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
                    break;
                default:
                    ChatHandler.SendCommandUsageText(client, "/v(ehicle) [create/list/stats/spawn/despawn/delete]");
                    break;
            }
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
            var newVeh = new VehicleModel(owner.ID);
            newVeh.Model = model;
            var vehID = await newVeh.CreateAsync();
            if (vehID < 0) throw new Exception("There was an error while creating a new vehicle.");
            owner.Vehicles.Add(newVeh);
            return vehID;
        }
    }
}
