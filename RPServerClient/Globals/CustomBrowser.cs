using System;
using System.Collections.Generic;
using System.Linq;
using RAGE.Ui;

namespace RPServerClient.Globals
{
    internal class CustomBrowser : RAGE.Events.Script
    {
        private static object[] _parameters;
        public static HtmlWindow MainBrowser;

        public CustomBrowser()
        {
            RAGE.Events.Add("createBrowser", CreateBrowser);
            RAGE.Events.Add("executeFunction", ExecuteFunction);
            RAGE.Events.Add("destroyBrowser", DestroyBrowser);
            RAGE.Events.OnBrowserCreated += OnBrowserCreated;
        }

        public static void CreateBrowser(object[] args)
        {
            if (MainBrowser != null) return;

            // Get the URL from the parameters
            var url = args[0].ToString();
            // Save the rest of the parameters
            _parameters = args.Skip(1).ToArray();
            // Create the browser
            MainBrowser = new HtmlWindow(url);
        }

        public static void CreateBrowser(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
            {
                throw new Exception("CustomBrowser.CreateBrowser(string link) => Link is null or empty");
            }
            CreateBrowser(new object[]{link});
        }

        public static void ExecuteFunction(string funcName)
        {

            if (string.IsNullOrWhiteSpace(funcName))
            {
                RAGE.Chat.Output("[ExecuteFunction(string fn)]: fn null or empty or whitespace");
                return;
            }

            MainBrowser.ExecuteJs($"{funcName}();");

        }

        public static void ExecuteFunction(object[] args)
        {
            // Check for the parameters
            var input = string.Empty;

            // Split the function and arguments
            var function = args[0].ToString();
            object[] arguments = args.Skip(1).ToArray();

            foreach (object arg in arguments)
            {
                // Append all the arguments
                input += input.Length > 0 ? (", '" + arg + "'") : ("'" + arg + "'");
            }
            // Call the function with the parameters
            MainBrowser.ExecuteJs($"{function}({input});");
        }

        public static void DestroyBrowser(object[] args)
        {
            // Disable the cursor
            Cursor.Visible = false;
            // Destroy the browser
            MainBrowser.Destroy();
            MainBrowser = null;
        }

        public static void OnBrowserCreated(HtmlWindow window)
        {
            if (MainBrowser != null && window.Id != MainBrowser.Id) return;

            // Enable the cursor
            Cursor.Visible = true;

            if (_parameters != null && _parameters.Length > 0)
            {
                // Call the function passed as parameter
                ExecuteFunction(_parameters);
            }
        }
    }
}