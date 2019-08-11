using System;
using System.Collections.Generic;
using System.Linq;
using RAGE;
using RAGE.Ui;
using RPServerClient.Util;
using Shared.Enums;

namespace RPServerClient.Client
{
    internal class BrowserHandler : Events.Script
    {
        private static object[] _parameters;
        public static HtmlWindow BrowserHtmlWindow;

        public BrowserHandler()
        {
            Events.Add("createBrowser", CreateBrowser);
            Events.Add("executeFunction", ExecuteFunction);
            Events.Add("destroyBrowser", DestroyBrowser);
            Events.OnBrowserCreated += OnBrowserCreated;
            Events.Tick += Tick;
        }

        public static void CreateBrowser(object[] args)
        {
            if (BrowserHtmlWindow != null) return;

            // Get the URL from the parameters
            var url = args[0].ToString();
            // Save the rest of the parameters
            _parameters = args.Skip(1).ToArray();
            // Create the browser
            BrowserHtmlWindow = new HtmlWindow(url);
        }

        public static void CreateBrowser(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                throw new Exception("CustomBrowser.CreateBrowser(string link) => Link is null or empty");
            CreateBrowser(new object[] {link});
        }

        /// <summary>
        ///     Example: DisplayHi calls DisplayHi();
        /// </summary>
        public static void ExecuteFunction(string funcName)
        {
            if (string.IsNullOrWhiteSpace(funcName))
            {
                RAGE.Chat.Output("[ExecuteFunction(string fn)]: fn null or empty or whitespace");
                return;
            }

            BrowserHtmlWindow.ExecuteJs($"{funcName}();");
        }

        public static void ExecuteFunction(object[] args)
        {
            // Check for the parameters
            var input = string.Empty;

            // Split the function and arguments
            var function = args[0].ToString();
            var arguments = args.Skip(1).ToArray();

            foreach (var arg in arguments)
                // Append all the arguments
                input += input.Length > 0 ? ", '" + arg + "'" : "'" + arg + "'";
            // Call the function with the parameters
            BrowserHtmlWindow.ExecuteJs($"{function}({input});");
        }

        public static void DestroyBrowser(object[] args)
        {
            // Disable the cursor
            Cursor.Visible = false;
            // Destroy the browser
            BrowserHtmlWindow.Destroy();
            BrowserHtmlWindow = null;
        }

        public static void OnBrowserCreated(HtmlWindow window)
        {
            if (BrowserHtmlWindow != null && window.Id != BrowserHtmlWindow.Id) return;

            // Enable the cursor
            Cursor.Visible = true;

            if (_parameters != null && _parameters.Length > 0)
                // Call the function passed as parameter
                ExecuteFunction(_parameters);
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (Chat.Chat.IsChatInputActive) return;

            KeyManager.KeyBind(KeyCodes.VK_F2, () => Cursor.Visible = !Cursor.Visible);
        }
    }
}