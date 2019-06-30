using RAGE;

namespace RPServerClient.Globals
{
    internal class CustomCamera
    {
        private int _cameraId;
        private readonly Vector3 _camPosition;
        private readonly Vector3 _pointAtPosition;
        private bool _cameraStatus;

        public CustomCamera(Vector3 camPos, Vector3 pointAt)
        {
            if (camPos == null || pointAt == null) return;
            _camPosition = camPos;
            _pointAtPosition = pointAt;
        }

        public void SetActive(bool activate)
        {
            if (activate)
            {
                if (_cameraStatus) return;

                _cameraId = RAGE.Game.Cam.CreateCamera(RAGE.Game.Misc.GetHashKey("DEFAULT_SCRIPTED_CAMERA"), true);
                RAGE.Game.Cam.SetCamCoord(_cameraId, _camPosition.X, _camPosition.Y, _camPosition.Z);
                RAGE.Game.Cam.PointCamAtCoord(_cameraId, _pointAtPosition.X, _pointAtPosition.Y, _pointAtPosition.Z);
                RAGE.Game.Cam.SetCamActive(_cameraId, true);
                RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
                _cameraStatus = true;
            }
            else
            {
                if (!_cameraStatus) return;

                RAGE.Game.Cam.DestroyCam(_cameraId, true);
                RAGE.Game.Cam.RenderScriptCams(false, false, 0, true, false, 0);
                _cameraStatus = false;

            }
        }
    }
}