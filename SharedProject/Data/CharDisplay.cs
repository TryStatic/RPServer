namespace Shared.Data
{
    public struct CharDisplay
    {
        public int CharID;
        public string CharName;

        public CharDisplay(int charID, string charName)
        {
            CharID = charID;
            CharName = charName;
        }
    }
}