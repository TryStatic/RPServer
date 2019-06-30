using RAGE;

namespace RPServerClient.Globals
{
    internal class CustomCamera
    {
        private readonly int _cameraId;

        public CustomCamera(Vector3 camCord, Vector3 pointAt)
        {
            if (camCord == null || pointAt == null) return;

            _cameraId = RAGE.Game.Cam.CreateCamera(RAGE.Game.Misc.GetHashKey("DEFAULT_SCRIPTED_CAMERA"), true);
            RAGE.Game.Cam.SetCamCoord(_cameraId, camCord.X, camCord.Y, camCord.Z);
            RAGE.Game.Cam.PointCamAtCoord(_cameraId, pointAt.X, pointAt.Y, pointAt.Z);
        }

        public void SetActive(bool state)
        {
            if (state)
            {
                RAGE.Game.Cam.SetCamActive(_cameraId, true);
                RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
            }
            else
            {
                RAGE.Game.Cam.DestroyCam(_cameraId, true);
                RAGE.Game.Cam.RenderScriptCams(false, false, 0, true, false, 0);
            }
        }
    }
}
