using RAGE.Ui;

namespace RPServerClient.Chat
{
    internal class CustomChat : RAGE.Events.Script
    {
        public static HtmlWindow ChatBrowser;

        public CustomChat()
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

        private void OnSendToChat(object[] args)
        {
            if (!Globals.Globals.IsAccountLoggedIn) return;
            // TODO: To send the chatmsg they need to have selected a character too
            RAGE.Chat.Output(args[0].ToString());
        }
    }
}
