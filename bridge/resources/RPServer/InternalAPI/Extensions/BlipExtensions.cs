using GTANetworkAPI;
using Shared.Enums;

namespace RPServer.InternalAPI.Extensions
{
    internal static class BlipExtensions
    {
        internal static Blip CreateBlip(this GTANetworkMethods.Blip blip, BlipSprite sprite, byte color,
            Vector3 position, float scale, string name = "", byte alpha = 255, float drawDistance = 0.0f,
            bool shortRange = false, short rotation = 0, uint dimension = 0)
        {
            return NAPI.Blip.CreateBlip((uint) sprite, position, scale, color, name, alpha, drawDistance,
                shortRange, rotation, dimension);
        }
    }
}