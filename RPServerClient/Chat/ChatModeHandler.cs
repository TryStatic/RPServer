using System;
using System.Collections.Generic;
using RAGE;
using RPServerClient.Character;
using RPServerClient.Chat.Util;
using RPServerClient.Util;

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
            var player = source as RAGE.Elements.Player;

            player?.SetData(LocalDataKeys.CurrentChatMode, ChatMode.Normal);
        }

        private void OnCharacterDespawn(object source, EventArgs e)
        {
            var player = source as RAGE.Elements.Player;
            player?.SetData(LocalDataKeys.CurrentChatMode, ChatMode.Normal);
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            KeyManager.KeyBind(Shared.Enums.KeyCodes.VK_CONTROL, Shared.Enums.KeyCodes.VK_B, () =>
            {
                    var chatmode = RAGE.Elements.Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                    chatmode = chatmode.Next();
                    RAGE.Elements.Player.LocalPlayer.SetData(LocalDataKeys.CurrentChatMode, chatmode);
            });
        }
    }
}
