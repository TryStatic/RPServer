using GTANetworkAPI;
using Shared.Enums;

namespace RPServer.InternalAPI.Extensions
{
    public static class TextLabelExtensions
    {
        internal static TextLabel CreateTextLabel(this GTANetworkMethods.TextLabel textlabel, string text,
            Vector3 position, float range, Font font, Color color, uint dimension = 0)
        {
            if (text == "" || range <= 0) return null;
            return NAPI.TextLabel.CreateTextLabel(text, position, range, 0, (int) font, color, false, dimension);
        }
    }
}