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
        private void OnSendToChat(object[] args) => RAGE.Chat.Output(args[0].ToString());
    }
}
