using RPServerClient.Util;
using Player = RAGE.Elements.Player;
using Vector3 = RAGE.Vector3;

namespace RPServerClient.Client
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

        /// <summary>
        /// Sets the Camera to point at some Player's Pedbone
        /// </summary>
        /// <param name="player">De playa whom bone you are interested</param>
        /// <param name="bone">Teh boneID</param>
        /// <param name="heading">Teh Direction around the bone to place the camera towards</param>
        /// <param name="distance">Some distance</param>
        /// <param name="setActive">Whether or not to set the camera active</param>
        public static void PointAtBone(Player player, Shared.Enums.Bone bone, float heading, float distance, bool setActive = false)
        {
            var boneCoords = player.GetBoneCoords((int)bone, 0, 0, 0);
            var cameraPos = Helper.GetPosInFrontOfVector3(boneCoords, heading, distance);
            SetPos(cameraPos, boneCoords);
            if (setActive) SetActive(true);
        }

        public static void SetActive(bool state)
        {
            RAGE.Game.Cam.SetCamActive(_cameraID, state);
            RAGE.Game.Cam.RenderScriptCams(state, false, 0, true, true, 0);
        }

        public static Vector3 GetPosition(int camera)
        {
            return RAGE.Game.Cam.GetCamCoord(camera);
        }

        public static Vector3 GetPointingAt(int camera)
        {
            return RAGE.Game.Cam.GetCamRot(camera, 2);
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