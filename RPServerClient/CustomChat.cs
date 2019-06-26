using System.Data;
using RAGE;
using RAGE.Ui;

namespace RPServerClient
{
    internal class CustomChat : Events.Script
    {
        public static HtmlWindow ChatBrowser;

        public CustomChat()
        {
            Events.Add("SendToChat", OnSendToChat);
            Events.Add("setEnabled", OnSetEnabled);
            Chat.Show(false);
            ChatBrowser = new HtmlWindow("package://CEF/chat/index.html");
            ChatBrowser.MarkAsChat();
        }

        public void OnSetEnabled(object[] args)
        {
            if (args[0] == null) return;
            var state = (bool) args[0];
            ChatBrowser.ExecuteJs(state ? "setEnabled(true);" : "setEnabled(false);");
        }

        private void OnSendToChat(object[] args)
        {
            Chat.Output(args[0].ToString());
        }
    }
}
