using System;
using System.Collections.Generic;
using RAGE;
using RAGE.Elements;
using RPServerClient.Character;
using RPServerClient.Chat.Util;
using RPServerClient.Client;
using RPServerClient.Util;
using Shared.Enums;

namespace RPServerClient.Chat
{
    internal class ChatModeHandler : Events.Script
    {
        public ChatModeHandler()
        {
            CharSelector.CharacterSpawn += OnCharacterSpawn;
            CharSelector.CharacterDespawn += OnCharacterDespawn;

            Events.Tick += Tick;
        }

        private void OnCharacterSpawn(object source, EventArgs e)
        {
            var player = source as Player;

            player?.SetData(LocalDataKeys.CurrentChatMode, ChatMode.Normal);
        }

        private void OnCharacterDespawn(object source, EventArgs e)
        {
            var player = source as Player;
            player?.SetData(LocalDataKeys.CurrentChatMode, ChatMode.Normal);
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            if (!Globals.IsAccountLoggedIn || !Globals.HasActiveChar || Chat.IsChatInputActive) return;

            KeyManager.KeyBind(KeyCodes.VK_CONTROL, KeyCodes.VK_B, () =>
            {
                var chatmode = Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                chatmode = chatmode.Next();
                Player.LocalPlayer.SetData(LocalDataKeys.CurrentChatMode, chatmode);
            });
        }
    }
}