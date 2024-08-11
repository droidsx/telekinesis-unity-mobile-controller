using System;
using System.Collections.Generic;
using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class StreamSource : MonoBehaviour
    {
        public virtual DateTime Timestamp { get; protected set; }
        public virtual Color32[] Pixels { get; protected set; }
        public virtual int Width { get; protected set; }
        public virtual int Height { get; protected set; }
        public virtual int Rotation { get; protected set; }
    }
}
