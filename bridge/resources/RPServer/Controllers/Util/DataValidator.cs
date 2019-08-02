﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RPServer.Game;
using RPServer.Models;
using RPServer.Util;

namespace RPServer.Controllers.Util
{
    internal static class DataValidator
    {
        public static HashSet<int> ValidVehicleIDs;

        public enum ValidationStrings
        {
            Username,
            Password,
            EmailAddress,
            EmailVerificationCode,
            GoogleAuthenticatorCode,
            CharFirstName,
            CharLastName
        }

        public enum ValidationNumbers
        {
            VehicleModelID
        }

        public static bool ValidateString(ValidationStrings strings, string data)
        {
            switch (strings)
            {
                case ValidationStrings.Username: // Username must be at least 4 chars (maybe add settings to tweak these later on)
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 4) return false;
                    break;
                case ValidationStrings.Password: // pass must be at least 4 chars
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 4) return false;
                    break;
                case ValidationStrings.EmailAddress: // Is actually an email address
                    if (string.IsNullOrWhiteSpace(data) || !data.IsValidEmail()) return false;
                    break;
                case ValidationStrings.EmailVerificationCode: // Provided token's length must much whichever the length is on our side
                    if (string.IsNullOrWhiteSpace(data) || data.Length < EmailToken.Length) return false;
                    break;
                case ValidationStrings.GoogleAuthenticatorCode:
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 6 || !IsDigitsOnly(data)) return false;
                    break;
                case ValidationStrings.CharFirstName:
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 2 || data.Length > 15 || !Regex.Match(data, @"[a-zA-Z]{1,15}").Success) return false;
                    break;
                case ValidationStrings.CharLastName:
                    if (string.IsNullOrWhiteSpace(data) || data.Length < 2 || data.Length > 15 || !Regex.Match(data, @"[a-zA-Z]{1,15}").Success) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strings), strings, null);
            }
            return true;
        }

        public static bool ValidateNumber(ValidationNumbers numbers, int data)
        {
            switch (numbers)
            {
                case ValidationNumbers.VehicleModelID:
                    if (ValidVehicleIDs == null)
                    {
                        Logger.GetInstance().ServerError("Tried to access ValidVehicleIDs HashSet before initialized.");
                        return false;
                    }
                    if (!ValidVehicleIDs.Contains(data)) return false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(numbers), numbers, null);
            }
            return true;
        }

        public static void InitializeValidVehicleModelIDs()
        {
            if (ValidVehicleIDs != null)
            {
                Logger.GetInstance().ServerError("OrganizeValidVehicleIDs should only be called once when the server is initialized.");
                return;
            }

            Logger.GetInstance().ServerInfo("Initializing Valid Vehicle Model IDs from VehicleData.json");
            ValidVehicleIDs = new HashSet<int>();
            var vehData = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(File.ReadAllText(Globals.VehicleDataJsonFile));
            foreach (var veh in vehData) ValidVehicleIDs.Add(veh.Key);
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
