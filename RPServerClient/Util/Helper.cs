using System;
using Multiplayer;
using RAGE.Elements;
using Vector3 = RAGE.Vector3;

namespace RPServerClient.Util
{
    internal static class Helper
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
            var newPos = new Vector3(pos.X, pos.Y, pos.Z);
            newPos.X += range * (float)Math.Sin(-heading * Math.PI / 180.0);
            newPos.Y += range * (float)Math.Cos(-heading * Math.PI / 180.0);
            return newPos;
        }

        internal static Vector3 GetVector3Part(this Quaternion q)
        {
            return new Vector3(q.X, q.Y, q.Z);
        }

    }
}
