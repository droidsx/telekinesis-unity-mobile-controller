#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LightBuzz.HandTracking.Editor
{
    [InitializeOnLoadAttribute]
    public class PlatformCheck
    {
        static PlatformCheck()
        {
            List<BuildTarget> targets = GetSupportedBuildTargets();

            Debug.Log($"[LIGHTBUZZ] The supported runtime platforms are: {DisplayBuildTargets(targets)}");
        }

        public static List<BuildTarget> GetSupportedBuildTargets()
        {
            string pluginsFolder = Path.Combine(Application.dataPath, "LightBuzz Hand Tracking", "Runtime", "Plugins");

            if (!Directory.Exists(pluginsFolder)) return new List<BuildTarget>();

            string[] directories = Directory.GetDirectories(pluginsFolder);
            List<BuildTarget> buildTargets = new List<BuildTarget>();

            foreach (string directory in directories)
            {
                string platform = Path.GetFileName(directory);

                if (platform == "Android")
                {
                    buildTargets.Add(BuildTarget.Android);
                }
                else if (platform == "iOS")
                {
                    buildTargets.Add(BuildTarget.iOS);
                }
                else if (platform == "macOS")
                {
                    buildTargets.Add(BuildTarget.StandaloneOSX);
                }
                else if (platform == "Windows")
                {
                    buildTargets.Add(BuildTarget.StandaloneWindows);
                }
            }

            return buildTargets;
        }

        public static string GetDirectMLPath()
        {
            string[] files = Directory.GetFiles(Application.dataPath, "DirectML.dll", SearchOption.AllDirectories);

            if (files == null || files.Length == 0)
            {
                Debug.LogError("[LIGHTBUZZ HAND TRACKING] DirectML.dll not found.");
                return null;
            }

            return files[0];
        }

        private static string DisplayBuildTargets(List<BuildTarget> buildTargets)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < buildTargets.Count; i++)
            {
                string buildTarget = buildTargets[i].ToString();
                string postfix = i == buildTargets.Count - 1 ? "." : ", ";

                sb.Append($"{buildTarget}{postfix}");
            }

            return sb.ToString();
        }
    }
}
#endif