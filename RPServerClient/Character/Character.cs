using System;
using RAGE;
using RAGE.Game;

namespace RPServerClient.Character
{
    internal enum ChatMode
    {
        NormalChat,
        ShoutChat
    }

    internal class Character : Events.Script
    {
        public Character()
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
            var player = source as Player;

        }
    }
}
