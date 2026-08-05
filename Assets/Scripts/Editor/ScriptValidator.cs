using System;
using System.Collections.Generic;
using System.IO;
using StillTime.Game;
using StillTime.Sts.Commands;
using StillTime.Sts.Nodes;
using StillTime.Sts.Parsers;
using UnityEditor;
using UnityEngine;

namespace StillTime.Editor {
    [InitializeOnLoad]
    public static class ScriptValidator {
        private static readonly Dictionary<string, DateTime> LastValidatedFileDateTimes = new();

        static ScriptValidator() {
            EditorApplication.focusChanged += HandleFocusChanged;
            ValidateScripts();

            string[] ext = EditorSettings.projectGenerationUserExtensions;
            if (Array.IndexOf(ext, "sts") < 0) {
                Array.Resize(ref ext, ext.Length + 1);
                ext[^1] = "sts";
                EditorSettings.projectGenerationUserExtensions = ext;
            }
        }

        private static void HandleFocusChanged(bool value) {
            if (!value) return;
            ValidateScripts();
        }

        private static void ValidateScripts() {
            foreach (string scriptPath in Directory.GetFiles(Application.streamingAssetsPath, "*.sts", SearchOption.AllDirectories)) {
                DateTime lastModifiedTime = File.GetLastWriteTime(scriptPath);

                if (LastValidatedFileDateTimes.TryGetValue(scriptPath, out DateTime lastValidateTime) &&
                    lastValidateTime >= lastModifiedTime)
                    continue;

                ValidateScript(scriptPath);
                LastValidatedFileDateTimes[scriptPath] = lastModifiedTime;
            }
        }

        private static void ValidateScript(string path) {
            try {
                string scriptText = File.ReadAllText(path);
                List<Command> commands = ScriptParser.ParseScript(scriptText);
                GameGraph graph = GraphBuilder.BuildGraph(commands);
                graph.Validate();
            } catch (ParsingException ex) {
                string relativePath = Path.GetRelativePath(".", path);
                int line = ex.LineNumber + 1;
                Debug.LogError($"<a href=\"{relativePath}\" line=\"{line}\">{relativePath}</a>: {ex}");
            }
        }
    }
}
