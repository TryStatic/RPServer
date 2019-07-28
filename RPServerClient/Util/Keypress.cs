using System;
using System.Collections.Generic;
using RAGE;
using RPServerClient.Character.Util;

namespace RPServerClient.Util
{
    internal class Keypress : RAGE.Events.Script
    { // Todo: Add a proper key handler
        public static bool IsKeypressReady = true;
        public static double ticks = 0;
        public static double around_one_sec = 80;

        public Keypress()
        {
            Events.Tick += OnRender;
        }

        private void OnRender(List<Events.TickNametagData> nametags)
        {
            ticks++;

            if (Math.Abs(ticks - around_one_sec) < 0.01)
            {
                ticks = 0;
                IsKeypressReady = true;
            }

            if (Input.IsDown((int)Shared.Enums.KeyCodes.VK_F2) && IsKeypressReady)
            { // TODO: Move me out of here and after a proper key handler is added
                RAGE.Ui.Cursor.Visible = !RAGE.Ui.Cursor.Visible;
                IsKeypressReady = false;
            }

            if (Input.IsDown((int)Shared.Enums.KeyCodes.VK_B) && Input.IsDown((int)Shared.Enums.KeyCodes.VK_CONTROL) && IsKeypressReady)
            { // TODO: Move me out of here and after a proper key handler is added
                IsKeypressReady = false;
                var chatmode = RAGE.Elements.Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                chatmode = chatmode.Next();
                RAGE.Chat.Output($"Chatmode set to: {chatmode}");
                RAGE.Elements.Player.LocalPlayer.SetData(LocalDataKeys.CurrentChatMode, chatmode);
            }
        }
    }

    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
