using System;
using System.IO.Pipes;
using System.Text.RegularExpressions;
using RAGE;
using RAGE.Ui;
using RPServerClient.Character;
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

            RAGE.Events.Add("PushToChatUnfiltered", OnPushToChatUnfiltered);
            RAGE.Events.Add("PushToChat", OnPushToChat);
            RAGE.Events.Add("setChatState", OnSetChatState);
            RAGE.Events.OnPlayerChat += OnPlayerChat;
        }

        private void OnPushToChatUnfiltered(object[] args)
        {
            var message = args[0].ToString();
            message = message.Replace("\"", "\\\"");
            RAGE.Chat.Output($"{message}");
        }

        private void OnPlayerChat(string text, Events.CancelEventArgs cancel)
        {
            var mode = Player.LocalPlayer.GetData<ChatMode>(Util.LocalDataKeys.CurrentChatMode);

            switch (mode)
            {
                case ChatMode.NormalChat:
                    Events.CallRemote(Shared.Events.ClientToServer.Chat.SubmitLocalNormalChatMessage, text);
                    // Log Chat
                    break;
                case ChatMode.ShoutChat:
                    Events.CallRemote(Shared.Events.ClientToServer.Chat.SubmitLocalShoutChatMessage, text);
                    // Log Chat
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            cancel.Cancel = true;
        }

        public void OnSetChatState(object[] args)
        {
            if (args[0] == null) return;
            var state = (bool) args[0];
            ChatBrowser.ExecuteJs(state ? "setEnabled(true);" : "setEnabled(false);");
        }

        /// <summary>
        /// Used from the server to push messages to the ChatBox
        /// </summary>
        private void OnPushToChat(object[] args)
        {
            if (args == null || args.Length < 2) return;

            var message = args[0].ToString();
            var senderID = int.Parse(args[1].ToString());

            var senderName = GetSenderName(senderID);
            message = ParseColors(message);

            var chatmode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);


            switch (chatmode)
            {
                case ChatMode.NormalChat:
                    RAGE.Chat.Output($"{senderName} says: {message}");
                    break;
                case ChatMode.ShoutChat:
                    RAGE.Chat.Output($"{senderName} shouts: {message}");
                    break;
            }

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
            message = message.Replace("\"", "\\\"");
            return message;
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
                    return $"Stranger";
                }
                else
                {
                    return $"{alias.AliasText}";
                }
            }
        }
    }
}
