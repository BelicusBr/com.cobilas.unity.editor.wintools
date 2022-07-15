using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Cobilas.Unity.Utility;
using UnityEditor.Build.Reporting;
using Cobilas.Unity.Management.Build;

namespace Cobilas.Unity.Editor.BuildCount {
    public class BuildCountWin : EditorWindow {

        private static string PersistentBuildCountFolder => CobilasPaths.Combine(CobilasPaths.ProjectFolderPath, "BuildCount");
        private static string PersistentBuildCountFile => CobilasPaths.Combine(PersistentBuildCountFolder, "MyCount.txt");
        private static int Count;

        [MenuItem("Window/Cobilas/Build count")]
        private static void Init() {
            BuildCountWin build = GetWindow<BuildCountWin>();
            build.titleContent = new GUIContent("Build count");
            build.minSize = build.maxSize = new Vector2(300, 100);
            build.Show();
        }

        [InitializeOnLoadMethod]
        private static void buildCount() {
            CobilasBuildProcessor.EventOnPostprocessBuild += (p, b) => {
                if (p != CobilasEditorProcessor.PriorityProcessor.Low) return;
                if (b.summary.totalErrors != 0) return;
                if (!Directory.Exists(PersistentBuildCountFolder))
                    Directory.CreateDirectory(PersistentBuildCountFolder);

                if (File.Exists(PersistentBuildCountFile))
                    using (StreamReader reader = new StreamReader(File.OpenRead(PersistentBuildCountFile)))
                        BuildCountWin.Count = reader.ReadLine().ToInt();

                using (StreamWriter reader = new StreamWriter(File.Create(PersistentBuildCountFile)))
                    reader.Write(BuildCountWin.Count + 1);
            };
        }

        private string countDisplay;

        private void Awake() {
            if (File.Exists(PersistentBuildCountFile))
                using (StreamReader reader = new StreamReader(File.OpenRead(PersistentBuildCountFile)))
                    countDisplay = reader.ReadLine();
        }

        private void OnGUI() {
            EditorGUILayout.LabelField($"Build count : {countDisplay}");
        }
    }
}
