using System;
using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class SpriteSource : StreamSource
    {
        [SerializeField] private Sprite _sprite;

        private DateTime _timestamp = DateTime.MinValue;

        public override DateTime Timestamp => _timestamp;

        public override Color32[] Pixels => _sprite?.texture?.GetPixels32();

        public override int Width => _sprite?.texture?.width ?? 0;

        public override int Height => _sprite?.texture?.height ?? 0;

        private void Start()
        {
            _timestamp = _sprite != null && _sprite.texture != null ? DateTime.Now : DateTime.MinValue;
        }

        private void OnValidate()
        {
            _timestamp = _sprite != null && _sprite.texture != null ? DateTime.Now : DateTime.MinValue;
        }
    }
}