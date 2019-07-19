using RAGE;

namespace RPServerClient.Globals
{
    public class Camera
    {
        private readonly uint _cameraHandle = RAGE.Game.Misc.GetHashKey("DEFAULT_SCRIPTED_CAMERA");
        private readonly int _cameraID;

        public Camera(Vector3 cameraPos, Vector3 cameraLookAt, bool active)
        {
            var camera = RAGE.Game.Cam.CreateCamera(_cameraHandle, true);

            RAGE.Game.Cam.SetCamCoord(camera, cameraPos.X, cameraPos.Y, cameraPos.Z);
            RAGE.Game.Cam.PointCamAtCoord(camera, cameraLookAt.X, cameraLookAt.Y, cameraLookAt.Z);
            RAGE.Game.Cam.SetCamActive(camera, active);
            RAGE.Game.Cam.RenderScriptCams(active, false, 0, true, true, 0);

            _cameraID = camera;
        }

        public void SetActive(bool state)
        {
            RAGE.Game.Cam.SetCamActive(_cameraID, state);
            RAGE.Game.Cam.RenderScriptCams(state, false, 0, true, true, 0);
        }

        public void SetPos(Vector3 pos, Vector3 rot)
        {
            RAGE.Game.Cam.SetCamCoord(_cameraID, pos.X, pos.Y, pos.Z);
            RAGE.Game.Cam.PointCamAtCoord(_cameraID, rot.X, rot.Y, rot.Z);
        }

        public void Destroy()
        {
            RAGE.Game.Cam.SetCamActive(_cameraID, false);
            RAGE.Game.Cam.DestroyCam(_cameraID, true);
            RAGE.Game.Cam.RenderScriptCams(false, false, 0, true, true, 0);
        }

        public Vector3 GetPosition(int camera)
        {
            return RAGE.Game.Cam.GetCamCoord(camera);
        }
    }
}