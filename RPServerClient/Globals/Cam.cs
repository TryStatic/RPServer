using System.Numerics;
using RAGE.Elements;
using Vector3 = RAGE.Vector3;

namespace RPServerClient.Globals
{
    internal class Cam : RAGE.Events.Script
    {
        private static readonly int _cameraID = RAGE.Game.Cam.CreateCamera(CameraHash.DefaultScriptedCamera, true);

        public Cam()
        {
        }

        public static void SetPos(Vector3 pos, Vector3 lookAt, bool setActive = false)
        {
            RAGE.Game.Cam.SetCamCoord(_cameraID, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.PointCamAtCoord(_cameraID, lookAt.X, lookAt.Y, lookAt.Z);

            if(setActive) SetActive(true);
        }

        public static void SetActive(bool state)
        {
            RAGE.Game.Cam.SetCamActive(_cameraID, state);
            RAGE.Game.Cam.RenderScriptCams(state, false, 0, true, true, 0);
        }

        public Vector3 GetPosition(int camera)
        {
            return RAGE.Game.Cam.GetCamCoord(camera);
        }

        private static class CameraHash
        {
            public static readonly uint DefaultScriptedCamera = RAGE.Game.Misc.GetHashKey("DEFAULT_SCRIPTED_CAMERA");
            public static readonly uint DefaultAnimatedCamera = RAGE.Game.Misc.GetHashKey("DEFAULT_ANIMATED_CAMERA");
            public static readonly uint DefaultSplineCamera = RAGE.Game.Misc.GetHashKey("DEFAULT_SPLINE_CAMERA");
            public static readonly uint DefaultScriptedFlyCamera = RAGE.Game.Misc.GetHashKey("DEFAULT_SCRIPTED_FLY_CAMERA");
            public static readonly uint TimedSplineCamera = RAGE.Game.Misc.GetHashKey("TIMED_SPLINE_CAMERA");
        }
    }
}