using System;
using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class WebcamSource : StreamSource
    {
        [SerializeField] private int _webcamIndex = 0;
        [SerializeField] private int _requestedWidth = 1280;
        [SerializeField] private int _requestedHeight = 720;
        [SerializeField] private int _requestedFps = 30;

        [SerializeField] private bool _openCameraOnStart = true;

        private WebCamTexture _webcam;

        private DateTime _timestamp;
        private Color32[] _pixels;
        private IntPtr _pointer;

        public int WebcamIndex => _webcamIndex;

        public WebCamTexture WebcamTexture => _webcam;

        public override DateTime Timestamp => _timestamp;

        public bool IsOpen => _webcam != null && _webcam.isPlaying && _webcam.width > 16 && _webcam.height > 16;

        public bool IsPlaying => _webcam != null && _webcam.isPlaying;

        public override int Width => _webcam?.width ?? 0;

        public override int Height => _webcam?.height ?? 0;

        public override int Rotation => _webcam?.videoRotationAngle ?? 0;

        public override Color32[] Pixels => _pixels;

        public IntPtr Pointer => _pointer;

        private void OnApplicationFocus(bool focus)
        {
            if (!Application.isMobilePlatform) return;
            
            if (focus)
            {
                Open();
            }
            else
            {
                Pause();
            }
        }

        private void Start()
        {
            if (_openCameraOnStart)
            {
                Open();
            }
        }

        private void Update()
        {
            if (!IsOpen) return;
            if (!IsPlaying) return;
            if (!_webcam.didUpdateThisFrame) return;

            _timestamp = DateTime.Now;
            _pixels = _webcam.GetPixels32();
            _pointer = _webcam.GetNativeTexturePtr();
        }

        private void OnApplicationQuit()
        {
            Close();
        }

        private void OnDestroy()
        {
            Close();
        }

        private void OpenCamera()
        {
            if (_webcam != null && _webcam.isPlaying) return;
            
            var devices = WebCamTexture.devices;

            if (devices != null && devices.Length > 0)
            {
                string cameraName =
                    _webcamIndex >= 0 ? devices[_webcamIndex].name :
                    string.Empty;

                _webcam = new WebCamTexture(cameraName, _requestedWidth, _requestedHeight, _requestedFps);

                while (!Application.RequestUserAuthorization(UserAuthorization.WebCam).isDone)
                {
                    System.Threading.Thread.Sleep(1);
                }
                
                _webcam.Play();
            }
            else
            {
                Debug.LogError("No webcam found!");
            }
        }

        public void Open()
        {
            Application.RequestUserAuthorization(UserAuthorization.WebCam).completed += (operation) =>
            {
                if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                {
                    OpenCamera();
                }
                else
                {
                    Debug.LogError("Webcam access denied!");
                }
            };
        }

        public void Close()
        {
            if (_webcam == null) return;

            _webcam.Stop();
        }

        public void Pause()
        {
            if (_webcam == null) return;

            _webcam.Pause();
        }

        public void SwitchCamera(int index)
        {
            _webcamIndex = index;
            Close();
            Open();
        }
    }
}