using System;
using RAGE;
using RAGE.Game;
using RPServerClient.Character.Util;

namespace RPServerClient.Character
{
    internal class ChatModeHandler : Events.Script
    {
        public ChatModeHandler()
        {
            CharSelector.CharacterSpawn += OnCharacterSpawn;
            CharSelector.CharacterDespawn += OnCharacterDespawn;
        }

        private void OnCharacterSpawn(object source, EventArgs e)
        {
            var player = source as RAGE.Elements.Player;

            player?.SetData(RPServerClient.Util.LocalDataKeys.CurrentChatMode, ChatMode.NormalChat);
        }

        private void OnCharacterDespawn(object source, EventArgs e)
        {
            var player = source as RAGE.Elements.Player;
            player?.SetData(RPServerClient.Util.LocalDataKeys.CurrentChatMode, ChatMode.NormalChat);
        }
    }
}
