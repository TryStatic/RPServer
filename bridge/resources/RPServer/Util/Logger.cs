using GTANetworkAPI;

namespace RPServer.Util
{
    internal class Logger
    {
        public static void MySqlError(string errorMsg, uint errorCode)
        {
            NAPI.Util.ConsoleOutput($"[MySQL Error]: {errorMsg} [Code={errorCode}]");
        }

        public static void MySqlInfo(string msg)
        {
            NAPI.Util.ConsoleOutput($"[MySQL Info]: {msg}");
        }
    }
}
