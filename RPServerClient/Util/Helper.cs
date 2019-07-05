using System;
using RAGE.Elements;
using Vector3 = RAGE.Vector3;

namespace RPServerClient.Util
{
    internal class Helper
    {
        public static Vector3 GetPosInFrontOfPlayer(Player myPlayer, float range)
        {
            var forwardX = myPlayer.Position.X + myPlayer.GetForwardX() * range;
            var forwardY = myPlayer.Position.Y + myPlayer.GetForwardY() * range;
            var pos = new Vector3(forwardX, forwardY, myPlayer.Position.Z + 0.5f);
            return pos;
        }

        public static Vector3 GetPosInFrontOfVector3(Vector3 pos, float heading, float range)
        {
            pos.X += range * (float)Math.Sin(-heading * Math.PI / 180.0);
            pos.Y += range * (float)Math.Cos(-heading * Math.PI / 180.0);
            return pos;
        }
    }
}
