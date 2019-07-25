using RPServerClient.Client.Util;
using RPServerClient.Util;
using Player = RAGE.Elements.Player;
using Vector3 = RAGE.Vector3;

namespace RPServerClient.Client
{
    internal class Cam : RAGE.Events.Script
    {
        public readonly int CameraID;

        public Cam()
        {
            CameraID = RAGE.Game.Cam.CreateCam(CameraType.DefaultScriptedCamera, false);
        }

        public void SetPos(Vector3 pos, Vector3 lookAt, bool setActive = false)
        {
            RAGE.Game.Cam.SetCamCoord(CameraID, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.PointCamAtCoord(CameraID, lookAt.X, lookAt.Y, lookAt.Z);
            if(setActive) SetActive(true);
        }

        public static void SetPosWithInterp(Cam destCamId, Cam origCamId, int durationInMs, int easeLocation, int easeRotation)
        {
            RAGE.Game.Cam.SetCamActiveWithInterp(destCamId.CameraID, origCamId.CameraID, durationInMs, easeLocation, easeRotation);
            RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, true, 0);
        }

        /// <summary>
        /// Sets the Camera to point at some Player's Pedbone
        /// </summary>
        /// <param name="player">De playa whom bone you are interested</param>
        /// <param name="bone">Teh boneID</param>
        /// <param name="heading">Teh Direction around the bone to place the camera towards</param>
        /// <param name="distance">Some distance</param>
        /// <param name="setActive">Whether or not to set the camera active</param>
        public void PointAtBone(Player player, Shared.Enums.Bone bone, float heading, float distance, bool setActive = false)
        {
            var boneCoords = player.GetBoneCoords((int)bone, 0, 0, 0);
            var cameraPos = Helper.GetPosInFrontOfVector3(boneCoords, heading, distance);
            SetPos(cameraPos, boneCoords);
            if (setActive) SetActive(true);
        }

        public void SetActive(bool state)
        {
            RAGE.Game.Cam.SetCamActive(CameraID, state);
            RAGE.Game.Cam.RenderScriptCams(state, false, 0, true, true, 0);
        }

        public Vector3 GetPosition(int camera)
        {
            return RAGE.Game.Cam.GetCamCoord(camera);
        }

        public Vector3 GetPointingAt(int camera)
        {
            return RAGE.Game.Cam.GetCamRot(camera, 2);
        }
    }
}