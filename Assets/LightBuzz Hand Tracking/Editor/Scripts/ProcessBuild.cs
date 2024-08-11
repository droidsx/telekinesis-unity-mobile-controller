using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LightBuzz.HandTracking.Editor
{
    /// <summary>
    /// Copies DirectML.dll next to the executable after a successful build.
    /// </summary>
    [InitializeOnLoadAttribute]
    public class ProcessBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.StandaloneWindows64) return;

            string source = PlatformCheck.GetDirectMLPath();
            string folder = Path.GetDirectoryName(report.summary.outputPath);
            string destination = Path.Combine(folder, "DirectML.dll");

            try
            {
                File.Copy(source, destination);
            }
            catch (Exception e)
            {
                Debug.LogError($"[LIGHTBUZZ HAND TRACKING] Failed to copy DirectML.dll: {e.Message}. Copy DirectML.dll next to your executable manually.");
            }
        }
    }
}