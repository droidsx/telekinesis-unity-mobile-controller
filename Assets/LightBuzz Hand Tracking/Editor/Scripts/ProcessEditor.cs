using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LightBuzz.HandTracking.Editor
{
    /// <summary>
    /// Copies DirectML.dll next in Unity's installation folder.
    /// </summary>
    [InitializeOnLoadAttribute]
    public class ProcessEditor : MonoBehaviour
    {
        static ProcessEditor()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor) return;

            string unityInstallationFolder = Path.GetDirectoryName(EditorApplication.applicationPath);
            string plugin = Path.Combine(unityInstallationFolder, "DirectML.dll");

            if (File.Exists(plugin)) return;

            Debug.LogError($"[LIGHTBUZZ HAND TRACKING] Use `LightBuzz -> Copy DirectML` to copy DirectML.dll to Unity's installation folder");
        }

        [MenuItem("LightBuzz/Help and Support", false, 101)]
        private static void MenuItem_Support()
        {
            Application.OpenURL("https://lightbuzz.com/support/");
        }

        [MenuItem("LightBuzz/Copy DirectML", false, 0)]
        private static void MenuItem_CopyDirectML()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                Debug.LogWarning("[LIGHTBUZZ HAND TRACKING] DirectML.dll is required only on Windows Editor.");
                return;
            }

            string directMLPath = PlatformCheck.GetDirectMLPath();
            string unityInstallationFolder = Path.GetDirectoryName(EditorApplication.applicationPath);
            string plugin = Path.Combine(unityInstallationFolder, "DirectML.dll");

            if (File.Exists(plugin))
            {
                Debug.Log($"[LIGHTBUZZ HAND TRACKING] DirectML.dll already exists in Unity's installation folder");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("@ECHO OFF");

            string source = directMLPath.Replace('/', '\\');
            string destination = unityInstallationFolder.Replace('/', '\\');

            sb.AppendLine($"xcopy \"{source}\" \"{destination}\"");

            string batchFile = Path.Combine(Application.dataPath, "lightbuzz.bat");

            File.WriteAllText(batchFile, sb.ToString());

            try
            {
                Debug.Log($"[LIGHTBUZZ HAND TRACKING] Copying DirectML.dll to Unity's installation folder... You can delete the `lightbuzz.bat` file after the copy is complete.");
                var process = new System.Diagnostics.Process
                {
                    StartInfo =
                    {
                        FileName = batchFile,
                        Verb = "runas",
                        UseShellExecute = true
                    }
                };
                process.Start();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LIGHTBUZZ HAND TRACKING] Failed to copy DirectML.dll: {e.Message}. Copy DirectML.dll manually to {unityInstallationFolder}.");
            }
        }
    }
}