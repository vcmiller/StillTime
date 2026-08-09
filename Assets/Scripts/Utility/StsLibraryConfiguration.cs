using StillTime.Sts;
using UnityEngine;

namespace StillTime.Utility {
    public static class StsLibraryConfiguration {
        public static void Run() {
            StsLibrary.Log ??= Debug.Log;
            StsLibrary.LogWarning ??= Debug.LogWarning;
            StsLibrary.LogError ??= Debug.LogError;
            StsLibrary.LogException ??= Debug.LogException;
        }
    }
}
