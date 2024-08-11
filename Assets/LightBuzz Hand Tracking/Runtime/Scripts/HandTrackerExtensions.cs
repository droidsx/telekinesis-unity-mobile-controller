using System.Collections.Generic;

namespace LightBuzz.HandTracking
{
    public static class HandTrackerExtensions
    {
        public static List<Hand> Load(this HandTracker tracker, StreamSource source)
        {
            return tracker.Load(source.Pixels, source.Width, source.Height);
        }
    }
}
