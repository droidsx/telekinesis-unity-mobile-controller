using System;
using System.Collections.Generic;
using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class HandViewer : MonoBehaviour
    {
        [SerializeField] private ImageView _image;
        [SerializeField] private StreamSource _source;
        [SerializeField] private HandManager[] _handManagers;

        private DateTime _timestamp;

        private List<Hand> _hands;

        private readonly HandTracker _handTracker = new HandTracker();

        public List<Hand> Hands => _hands;

        private void Awake()
        {
            if (_source == null)
            {
                _source = FindObjectOfType<WebcamSource>();

                if (_source == null)
                {
                    _source = FindObjectOfType<VideoSource>();

                    if (_source == null)
                    {
                        _source = FindObjectOfType<SpriteSource>();

                        if (_source == null)
                        {
                            Debug.LogError("No stream source found.");
                        }
                    }
                }
            }

            if (_image == null)
            {
                _image = FindObjectOfType<ImageView>();

                if (_image == null)
                {
                    Debug.LogError("No image view found.");
                }
            }

            if (_handManagers == null || _handManagers.Length == 0)
            {
                _handManagers = FindObjectsOfType<HandManager>();

                if (_handManagers == null)
                {
                    Debug.LogError("No hand manager found.");
                }
            }
        }

        private void Update()
        {
            if (_source == null || _image == null || _handManagers == null || _handManagers.Length == 0) return;

            if (_source.Timestamp < _timestamp) return;

            // 1. Draw the texture.
            _image.Load(_source);

            // 2. Search for hands in the current stream.
            _hands = _handTracker.Load(_image.Texture);

            // 3. Draw the hands.
            foreach (HandManager handManager in _handManagers)
            {
                handManager.Load(_hands, Offsets());
            }

            _timestamp = _source.Timestamp;
        }

        private List<Vector3> Offsets()
        {
            List<Vector3> offsets = new List<Vector3>();

            foreach (Hand hand in _hands)
            {
                Vector3 offset = Offset(hand);

                offsets.Add(offset);
            }

            return offsets;
        }

        private Vector3 Offset(Hand hand)
        {
            float width = _source.Width;
            float height = _source.Height;

            Vector2 root2D = hand[FingerJointType.Root].Position2D;
            float offsetX = (root2D.x / width) - 0.5f;
            float offsetY = (root2D.y / height) - 0.5f;

            return new Vector3(offsetX, offsetY, 0);
        }
    }
}
