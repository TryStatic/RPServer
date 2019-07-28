using System;
using System.Collections.Generic;
using RAGE;
using RAGE.Game;
using RPServerClient.Character.Util;
using RPServerClient.Util;

namespace RPServerClient.Character
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

            player?.SetData(LocalDataKeys.CurrentChatMode, ChatMode.NormalChat);
        }

        private void OnCharacterDespawn(object source, EventArgs e)
        {
            var player = source as RAGE.Elements.Player;
            player?.SetData(LocalDataKeys.CurrentChatMode, ChatMode.NormalChat);
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            KeyManager.KeyBind(Shared.Enums.KeyCodes.VK_CONTROL, Shared.Enums.KeyCodes.VK_B, () =>
            {
                    var chatmode = RAGE.Elements.Player.LocalPlayer.GetData<ChatMode>(LocalDataKeys.CurrentChatMode);
                    chatmode = chatmode.Next();
                    RAGE.Chat.Output($"Chatmode set to: {chatmode}");
                    RAGE.Elements.Player.LocalPlayer.SetData(LocalDataKeys.CurrentChatMode, chatmode);
            });
        }
    }
}
