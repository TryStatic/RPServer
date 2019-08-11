using RAGE.Elements;

namespace RPServerClient.Character.Util
{
    internal class Alias
    {
        public Alias(Player player, string aliasText)
        {
            Player = player;
            AliasText = aliasText;
        }

        public Player Player { set; get; }
        public string AliasText { set; get; }
    }
}