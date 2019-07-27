using System.Text.RegularExpressions;
using RAGE.Ui;

namespace RPServerClient.Chat
{
    internal class Chat : RAGE.Events.Script
    {
        public static HtmlWindow ChatBrowser;

        public Chat()
        {
            RAGE.Events.Add("SendToChat", OnSendToChat);
            RAGE.Events.Add("setChatState", OnSetChatState);
            RAGE.Chat.Show(false);
            ChatBrowser = new HtmlWindow("package://CEF/chat/index.html");
            ChatBrowser.MarkAsChat();
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
        private void OnSendToChat(object[] args)
        {
            if (args == null || args.Length < 1) return;

            var msg = args[0].ToString();

            var matches = new Regex(@"(!{#[0-9A-F]{6}})+").Matches(msg);
            var counter = 0;
            foreach (Match m in matches)
            {
                if (m.Length != 10)
                {
                    return;
                }

                var hexColor = m.Value.Remove(9).Remove(0, 2);

                if (counter == 0) msg = msg.Replace(m.Value, $"<span style='color: {hexColor}'>");
                else msg = msg.Replace(m.Value, $"</span><span style='color: {hexColor}'>");

                counter++;
            }
            if (counter != 0) msg = msg.Insert(msg.Length, $"</span>");

            RAGE.Chat.Output(msg);
        }
    }
}
