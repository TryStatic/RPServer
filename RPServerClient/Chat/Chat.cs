using System;
using System.Text.RegularExpressions;
using RAGE;
using RAGE.Ui;
using RPServerClient.Character;
using RPServerClient.Chat.Util;
using RPServerClient.Util;
using Player = RAGE.Elements.Player;

namespace RPServerClient.Chat
{
    internal class Chat : RAGE.Events.Script
    {
        public static HtmlWindow ChatBrowser;

        public Chat()
        {
            // Disable Default Chat
            RAGE.Chat.Show(false);
            ChatBrowser = new HtmlWindow("package://CEF/chat/index.html");
            ChatBrowser.MarkAsChat();

            RAGE.Events.OnPlayerChat += OnPlayerChat;

            RAGE.Events.Add(Shared.Events.ServerToClient.Chat.PushChatMessage, OnPushChatMessage);
            RAGE.Events.Add(Shared.Events.ServerToClient.Chat.PushChatMessageUnfiltered, OnPushChatMessageUnfiltered);
        }

        private void OnPlayerChat(string text, Events.CancelEventArgs cancel)
        { // Chat

            var chatmode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);

            Events.CallRemote(Shared.Events.ClientToServer.Chat.SubmitChatMessage, text, chatmode);

            cancel.Cancel = true;
        }

        private void OnPushChatMessage(object[] args) // (message, senderID, colorString)
        {
            if (args == null || args.Length < 3) return;

            var message = args[0].ToString(); // The actual message (colors removed/html escaped)
            var senderID = int.Parse(args[1].ToString()); // The ID of the sender
            var color = args[2].ToString(); // The color of the message

            var senderName = GetSenderName(senderID); // The sender name for THIS client
            var finalMessage = $"{color}{senderName} {message}";

            finalMessage = ParseColors(finalMessage);
            PushToChatBox(finalMessage);
        }

        private void OnPushChatMessageUnfiltered(object[] args) // (message)
        {
            if (args == null || args.Length < 1) return;
            var message = args[0].ToString();
            message = ParseColors(message);
            PushToChatBox(message);
        }

        public void PushToChatBox(string message)
        {
            message = EscapeDoubleQuotes(message);
            RAGE.Chat.Output(message);
        }

        private string ParseColors(string message)
        {
            var matches = new Regex(@"(!{#[0-9A-F]{6}})+").Matches(message);
            var counter = 0;

            foreach (Match m in matches)
            {
                if (m.Length != 10)
                {
                    return "";
                }

                var hexColor = m.Value.Remove(9).Remove(0, 2);

                if (counter == 0) message = message.Replace(m.Value, $"<span style='color: {hexColor}'>");
                else message = message.Replace(m.Value, $"</span><span style='color: {hexColor}'>");

                counter++;
            }
            if (counter != 0) message = message.Insert(message.Length, $"</span>");
            return message;
        }

        private string EscapeDoubleQuotes(string message)
        {
            return message.Replace("\"", "\\\"");
        }

        private string GetSenderName(int senderID)
        {
            if (Player.LocalPlayer.RemoteId == senderID)
            { // this was send by this client
                return $"{Player.LocalPlayer.Name}";
            }
            else
            {
                var alias = AliasManager.ClientAlises.Find(al => al.Player.RemoteId == senderID);
                if (alias == null)
                {
                    return $"Stranger ({senderID})";
                }
                else
                {
                    return $"{alias.AliasText}";
                }
            }
        }
    }
}
