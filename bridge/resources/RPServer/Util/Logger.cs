using System;
using System.IO;
using GTANetworkAPI;

namespace RPServer.Util
{
    internal sealed class Logger
    {
        private static Logger _logger;

        private const string LogsFolder = "bridge\\logs\\";
        private const string MySqlLogFn = "mysql.log";
        private const string CommandLogFn = "command.log";
        private const string AuthLogFn = "auth.log";
        private const string ChatLogFn = "chat.log";

        private Logger()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            Directory.CreateDirectory(LogsFolder);
            FileWrite(MySqlLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
            FileWrite(CommandLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
            FileWrite(AuthLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
            FileWrite(ChatLogFn, $"------------------------ [SERVER STARTED AT: {DateTime.Now}] ------------------------");
        }
        internal void ServerInfo(string sendMsg)
        {
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][SRV INFO]: {sendMsg}");
        }
        public void SqlInfo(string msg)
        {
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][SQL INFO]: {msg}");
            FileWriteAsync(MySqlLogFn, $"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][SQL INFO]: {msg}");
        }
        public void MySqlError(string errorMsg, uint errorCode)
        {
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][MySQL ERROR]: {errorMsg} [Code={errorCode}]");
            FileWriteAsync(MySqlLogFn, $"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][MySQL ERROR]: {errorMsg} [Code={errorCode}]");
        }
        public void SqlError(string errorMsq)
        {
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][SQL ERROR]: {errorMsq}");
            FileWriteAsync(MySqlLogFn, $"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][SQL ERROR]: {errorMsq}");
        }
        public void CommandLog(string username, string cmd)
        {
            FileWriteAsync(CommandLogFn, $"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][{username}]: /{cmd}");
#if DEBUG
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][DEBUG-CMD]: [{username}]: /{cmd}");
#endif
        }
        public void AuthLog(string logStr)
        {
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][AUTH]: {logStr}");
            FileWriteAsync(AuthLogFn, $"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}]: {logStr}");
        }
        internal void ChatLog(string sendMsg)
        {
            FileWriteAsync(ChatLogFn, $"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}]: {sendMsg}");
#if DEBUG
            NAPI.Util.ConsoleOutput($"[{DateTime.Now:MM/dd/yyyy HH:mm:ss}][DEBUG-CHAT]: {sendMsg}");
#endif
        }

        private static async void FileWriteAsync(string file, string message)
        {
            var filePath = LogsFolder + file;
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
            {
                using (StreamWriter sw = new StreamWriter((Stream)stream))
                    await sw.WriteLineAsync(message).ConfigureAwait(false);
            }
        }
        private static void FileWrite(string file, string message)
        {
            var filePath = LogsFolder + file;
            using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
            {
                using (StreamWriter sw = new StreamWriter((Stream)stream))
                    sw.WriteLine(message);
            }
        }
        private static void OnProcessExit(object sender, EventArgs e)
        {
            FileWrite(MySqlLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
            FileWrite(CommandLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
            FileWrite(AuthLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
            FileWrite(ChatLogFn, $"------------------------ [SERVER STOPPED AT: {DateTime.Now}] ------------------------");
        }

        public static Logger GetInstance() => _logger ?? (_logger = new Logger());
    }
}