using System;
using Microsoft.Extensions.Logging;

namespace StillTime.Sts {
    public static class StsLibrary {
        public static Action<string> Log;
        public static Action<string> LogWarning;
        public static Action<string> LogError;
        public static Action<Exception> LogException;
    }
}
