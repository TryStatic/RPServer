using System;
using System.IO;

namespace RPServer.Game
{
    internal class Globals
    {
        public const string SERVER_NAME = "AlphaRP";
#if DEBUG
        public const string VERSION = ThisAssembly.Git.Tag + " on " + ThisAssembly.Git.Branch;
#else
        public const string VERSION = ThisAssembly.Git.BaseTag;
#endif

        internal static string ServerFolder => Path.GetFullPath(
            AppDomain.CurrentDomain.BaseDirectory + ".." + Path.DirectorySeparatorChar + ".." +
            Path.DirectorySeparatorChar);

        internal static string BridgeFolder => ServerFolder + "bridge" + Path.DirectorySeparatorChar;
        internal static string VehicleDataJsonFile => BridgeFolder + "vehicleData.json";
    }
}