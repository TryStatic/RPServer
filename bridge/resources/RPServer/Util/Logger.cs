using System;
using System.Collections.Generic;
using System.IO;
using GTANetworkAPI;
using GTANetworkMethods;

namespace RPServer.Util
{
    internal class Logger : Script
    {
        private const string LogsFolder = "bridge\\logs\\";

        private const string MySqlLogFn = "mysql.log";
        private const string CommandLogFn = "command.log";
        private const string AuthLogFn = "auth.log";

        public Logger()
        {
            AppDomain.CurrentDomain.ProcessExit += OnServerShutdown;

            FileWrite(MySqlLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
            FileWrite(CommandLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
            FileWrite(AuthLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
        }

        public static void MySqlError(string errorMsg, uint errorCode)
        {
            NAPI.Util.ConsoleOutput($"[MySQL Error]: {errorMsg} [Code={errorCode}]");
            FileWriteAsync(LogsFolder + MySqlLogFn, $"[{DateTime.Now}][MySQL Error]: {errorMsg} [Code={errorCode}]");
        }

        public static void MySqlInfo(string msg)
        {
            NAPI.Util.ConsoleOutput($"[MySQL Info]: {msg}");
            FileWriteAsync(MySqlLogFn, $"[{DateTime.Now}][MySQL Info]: {msg}");
        }

        public static void CommandLog(string username, string cmd)
        {
            FileWriteAsync(CommandLogFn, $"[{DateTime.Now}][{username}]: /{cmd}");
        }

        public static void AuthLog(string logStr)
        {
            NAPI.Util.ConsoleOutput($"[Auth]: {logStr}");
            FileWriteAsync(AuthLogFn, $"[{DateTime.Now}]: {logStr}");
        }

        public static async void FileWriteAsync(string file, string message)
        {
            var filePath = LogsFolder + file;
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
            {
                using (StreamWriter sw = new StreamWriter((Stream)stream))
                    await sw.WriteLineAsync(message).ConfigureAwait(false);
            }
        }
        public static void FileWrite(string file, string message)
        {
            var filePath = LogsFolder + file;
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
            {
                using (StreamWriter sw = new StreamWriter((Stream)stream))
                    sw.WriteLine(message);
            }
        }


        private static void OnServerShutdown(object sender, EventArgs e)
        {
            FileWrite(MySqlLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
            FileWrite(CommandLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
            FileWrite(AuthLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
        }
    }
}