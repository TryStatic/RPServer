using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;
// ReSharper disable InconsistentNaming

namespace RPServer.Controllers
{
    public enum CommandGroup
    {
        Account,
        Character,
        Chat,
        Vehicle,
        Server
    }

    public class CommandHandler
    {
        private static readonly List<CommandHandler> CommandList = new List<CommandHandler>();

        public string CommandText { get; }
        public string CommandTextAlias { get; }
        public uint AdminLevel { get; }
        public CommandGroup CommandGroup { get; }

        public bool MustBeSpawned { get; set; } = true;

        public CommandHandler(string commandText, uint adminLevel, CommandGroup group)
        {
            CommandText = commandText;
            AdminLevel = adminLevel;
            CommandGroup = group;
            CommandTextAlias = "";
            CommandList.Add(this);
        }

        public CommandHandler(string commandText, string commandTextAlias, uint adminLevel, CommandGroup group)
        {
            CommandText = commandText;
            CommandTextAlias = commandTextAlias;
            AdminLevel = adminLevel;
            CommandGroup = group;
            CommandList.Add(this);
        }

        public bool IsAuthorized(Client client)
        {
            if (!client.IsLoggedIn())
            {
                ChatHandler.SendCommandErrorText(client, "You are not logged in.");
                return false;
            }

            if (client.GetAccount().AdminLevel < AdminLevel)
            {
                ChatHandler.SendCommandErrorText(client, "You are not authorized to use that command.");
                return false;
            }

            if (MustBeSpawned && !client.HasActiveChar())
            {
                ChatHandler.SendCommandErrorText(client, "You must spawn a character to use that command.");
                return false;
            }

            return true;
        }

        public static List<CommandHandler> GetAllCommands() => CommandList.ToList();

        // Account
        public const string ToggleTwoFactorEmailText = "toggletwofactoremail";
        public static CommandHandler ToggleTwoFactorEmail = new CommandHandler(ToggleTwoFactorEmailText, adminLevel: 0, CommandGroup.Account) { MustBeSpawned = false };
        public const string ToggleTwoFactorGAText = "toggletwofactorga";
        public static CommandHandler ToggleTwoFactorGA = new CommandHandler(ToggleTwoFactorGAText, adminLevel: 0, CommandGroup.Account) { MustBeSpawned = false };
        public const string LogoutText = "logout";
        public static CommandHandler Logout = new CommandHandler(LogoutText, adminLevel: 0, CommandGroup.Account) { MustBeSpawned = false };

        // Character
        public const string StatsText = "stats";
        public static CommandHandler Stats = new CommandHandler(StatsText, adminLevel: 0, CommandGroup.Character);
        public const string ChangeCharText = "changechar";
        public static CommandHandler ChangeChar = new CommandHandler(ChangeCharText, adminLevel: 0, CommandGroup.Character);
        public const string AliasText = "alias";
        public static CommandHandler Alias = new CommandHandler(AliasText, adminLevel: 0, CommandGroup.Character);

        // Chat
        public const string Chat_OOC_Text = "ooc";
        public const string Chat_OOC_Text_Alias = "o";
        public static CommandHandler Chat_OOC = new CommandHandler(Chat_OOC_Text, Chat_OOC_Text_Alias, adminLevel: 0, CommandGroup.Chat);
        public const string Chat_B_Text = "b";
        public static CommandHandler Chat_B = new CommandHandler(Chat_B_Text, adminLevel: 0, CommandGroup.Chat);
        public const string Chat_Me_Text = "me";
        public static CommandHandler Chat_Me = new CommandHandler(Chat_Me_Text, adminLevel: 0, CommandGroup.Chat);
        public const string Chat_Do_Text = "do";
        public static CommandHandler Chat_Do = new CommandHandler(Chat_Do_Text, adminLevel: 0, CommandGroup.Chat);

        // Vehicle
        public const string VehicleText = "vehicle";
        public const string VehicleTextAlias = "v";
        public static CommandHandler Vehicle = new CommandHandler(VehicleText, VehicleTextAlias, adminLevel: 0, CommandGroup.Vehicle);

        // Server
        public const string ShutdownText = "shutdown";
        public static CommandHandler Shutdown = new CommandHandler(ShutdownText, adminLevel: 1, CommandGroup.Server);

    }
}