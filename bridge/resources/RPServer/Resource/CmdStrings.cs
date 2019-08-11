// ReSharper disable InconsistentNaming
namespace RPServer.Resource
{
    public static class CmdStrings
    {
        // AuthenticationManager
        public const string CMD_ToggleTwoFactorEmail = "toggletwofactoremail";
        public const string CMD_ToggleTwoFactorGA = "toggletwofactorga";
        public const string CMD_Logout = "logout";

        // Character
        public const string CMD_ChangeChar = "changechar";
        public const string CMD_Alias = "alias";
        public const string CMD_Stats = "stats";

        public const string CMD_Inventory = "inventory";
        public const string CMD_Inventory_Alias = "inv";
        public const string SUBCMD_Inventory_Use = "use";
        public const string SUBCMD_Inventory_Drop = "drop";
        public const string CMD_Inventory_HelpText = "/inv(entory) [use/drop]";

        // Vehicle
        public const string CMD_Vehicle = "vehicle";
        public const string CMD_Vehicle_Alias = "v";
        public const string SUBCMD_Vehicle_Create = "create";
        public const string SUBCMD_Vehicle_List = "list";
        public const string SUBCMD_Vehicle_Stats = "stats";
        public const string SUBCMD_Vehicle_Spawn = "spawn";
        public const string SUBCMD_Vehicle_Despawn = "despawn";
        public const string SUBCMD_Vehicle_Delete = "delete";
        public const string CMD_Vehicle_HelpText = "/v(ehicle) [create/list/stats/spawn/despawn/delete]";

        // Chat
        public const string CMD_B = "b";
        public const string CMD_Me = "me";
        public const string CMD_Do = "do";
        public const string CMD_OOC = "ooc";
        public const string CMD_OOC_Alias = "o";

        // Server
        public const string CMD_Shutdown = "shutdown";
    }
}
