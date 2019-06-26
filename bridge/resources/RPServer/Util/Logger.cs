using System.IO;
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

        private static async void FileWriteAsync(string filePath, string messaage)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
            {
                using (StreamWriter sw = new StreamWriter((Stream)stream))
                    await sw.WriteLineAsync(messaage).ConfigureAwait(false);
            }
        }
    }
}
