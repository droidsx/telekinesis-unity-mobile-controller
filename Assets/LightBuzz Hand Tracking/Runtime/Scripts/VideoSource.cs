using System;
using UnityEngine;
using UnityEngine.Video;

namespace LightBuzz.HandTracking
{
    public class VideoSource : StreamSource
    {
        [SerializeField] private VideoPlayer _videoPlayer;

        private Texture2D _texture;
        private DateTime _timestamp;
        private Color32[] _pixels;

        public override DateTime Timestamp => _timestamp;

        public override Color32[] Pixels => _pixels;

        public override int Width => _texture?.width ?? 0;

        public override int Height => _texture?.height ?? 0;

        private void Start()
        {
            _videoPlayer.sendFrameReadyEvents = true;
            _videoPlayer.prepareCompleted += VideoPrepared;
            _videoPlayer.frameReady += VideoFrameReady;
            _videoPlayer.Prepare();
        }

        private void OnDestroy()
        {
            _videoPlayer.prepareCompleted -= VideoPrepared;
            _videoPlayer.frameReady -= VideoFrameReady;

            Destroy(_texture);
        }

        private void VideoPrepared(VideoPlayer source)
        {
            _texture = new Texture2D((int)source.width, (int)source.height, TextureFormat.RGB24, false);
            _videoPlayer.Play();
        }

        private void VideoFrameReady(VideoPlayer source, long frameIdx)
        {
            RenderTexture renderTexture = (RenderTexture)_videoPlayer.texture;
            RenderTexture.active = renderTexture;

            _texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            _texture.Apply();

            _timestamp = DateTime.Now;
            _pixels = _texture.GetPixels32();
        }
    }
}