using System;
using UnityEngine;
using UnityEngine.UI;

namespace LightBuzz.HandTracking
{
    public class ImageView : MonoBehaviour
    {
        [SerializeField] protected bool _flipVertically = false;
        [SerializeField] protected bool _flipHorizontally = false;

        private RectTransform _rectTransform;

        private Quaternion _baseRotation = Quaternion.identity;

        private int _webcamRotation = 0;

        private Image _image;

        private AspectRatioFitter _arf;

        private Texture2D _texture;

        public Texture2D Texture => _texture;

        public int Width => _texture != null ? _texture.width : 0;

        public int Height => _texture != null ? _texture.height : 0;

        public bool FlipVertically
        {
            get => _flipVertically;
            set => _flipVertically = value;
        }

        public bool FlipHorizontally
        {
            get => _flipHorizontally;
            set => _flipHorizontally = value;
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            _arf = _image.GetComponent<AspectRatioFitter>();
            _rectTransform = (RectTransform)transform;
            _baseRotation = _rectTransform.rotation;
        }

        private void OnDestroy()
        {
            if (_texture == null) return;

            Destroy(_texture);
        }

        private void LateUpdate()
        {
            float scaleX = _flipHorizontally ? -1.0f : 1.0f;
            float scaleY = _flipVertically ? -1.0f : 1.0f;
            float scaleZ = 1.0f;

            Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

            if (_image.transform.localScale != scale)
            {
                _image.transform.localScale = scale;
            }

            _rectTransform.rotation = _baseRotation * Quaternion.AngleAxis(_webcamRotation, Vector3.back);
        }

        private void Create(int width, int height, int rotation)
        {
            if (_texture == null || _texture.width != width || _texture.height != height)
            {
                if (_texture != null)
                {
                    Destroy(_texture);
                }

                _rectTransform = (RectTransform)transform;

                _texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                _arf.aspectRatio = _texture.width / (float)_texture.height;
                _image.rectTransform.sizeDelta = new Vector2(_texture.width, _texture.height);
                _image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
            }

            _webcamRotation = rotation;
        }

        public void Load(Texture2D texture)
        {
            if (texture == null) return;

            Load(texture.GetPixels32(), texture.width, texture.height);
        }

        public void Load(StreamSource source)
        {
            if (source == null) return;

            Load(source.Pixels, source.Width, source.Height, source.Rotation);
        }

        public void Load(Color32[] pixels, int width, int height, int rotation = 0)
        {
            if (pixels == null || pixels.Length == 0 || width <= 16 || height <= 16) return;

            Create(width, height, rotation);

            _texture.SetPixels32(pixels);
            _texture.Apply();
        }

        public void Load(byte[] rgbData, int width, int height, int rotation = 0)
        {
            if (rgbData == null || rgbData.Length == 0 || width == 0 || height == 0) return;

            Create(width, height, rotation);

            _texture.LoadRawTextureData(rgbData);
            _texture.Apply();
        }

        /// <summary>
        /// Calculates the position in the local space of the image.
        /// </summary>
        /// <param name="point">The original 2D position of a point (e.g., in a 1280x720 frame).</param>
        /// <returns>The equivalent position within the scaled image view (e.g., in a scaled 2000x1500 window).</returns>
        public Vector2 GetPosition(Vector2 point)
        {
            if (_image == null) return Vector2.zero;

            float scaledWidth = _image.rectTransform.rect.width * _image.transform.localScale.x;
            float scaledHeight = _image.rectTransform.rect.height * _image.transform.localScale.y;

            float x = point.x / Width * scaledWidth - scaledWidth * 0.5f;
            float y = point.y / Height * scaledHeight - scaledHeight * 0.5f;

            Vector2 positionInFrame = new Vector2(_rectTransform.anchoredPosition.x + x, _rectTransform.anchoredPosition.y + y);

            float angle = _rectTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            float xRotated = cos * positionInFrame.x - sin * positionInFrame.y;
            float yRotated = sin * positionInFrame.x + cos * positionInFrame.y;

            return new Vector2(xRotated, yRotated);

        }
    }
}
