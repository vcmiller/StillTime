using System.IO;
using UnityEditor;
using UnityEngine;

namespace StillTime.Editor {
    public static class ScriptTools {
        [MenuItem("Tools/StillTime/Promote GameScript to Dev")]
        public static void PromoteGameScriptToDev() {
            PromoteScriptToDest("dev");
        }

        [MenuItem("Tools/StillTime/Promote GameScript to Prod")]
        public static void PromoteGameScriptToProduction() {
            PromoteScriptToDest("prod");
        }

        private static void PromoteScriptToDest(string destName) {
            string scriptRelativePath = Path.Combine("DialogScripts", "GameScript.sts");
            string srcPath = Path.Combine(Application.streamingAssetsPath, scriptRelativePath);
            string destPath = Path.Combine(Directory.GetCurrentDirectory(), "GithubPages", destName, "StreamingAssets",
                                           scriptRelativePath);

            File.Copy(srcPath, destPath, true);
        }

        [MenuItem("Tools/StillTime/Promote GameScript to Dev & Prod")]
        public static void PromoteGameScriptToBoth() {
            PromoteGameScriptToDev();
            PromoteGameScriptToProduction();
        }
    }
}
