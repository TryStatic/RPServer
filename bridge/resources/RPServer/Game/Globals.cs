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

    }
}
