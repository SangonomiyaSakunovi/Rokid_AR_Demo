namespace SangoUtils.UnityDevelopToolKits.Loggers
{
    public static class UnityLogger
    {
        public static void Initialize(LoggerConfig cfg = null)
        {
            UnityLoggerProcessor.Initialize(cfg);
        }

        public static void Log(string log, bool isImmediate = false, params object[] args)
        {
            UnityLoggerProcessor.Log(log, isImmediate, args);
        }
        public static void Log(object logObj, bool isImmediate = false)
        {
            UnityLoggerProcessor.Log(logObj, isImmediate);
        }

        public static void Warning(string log, bool isImmediate = false, params object[] args)
        {
            UnityLoggerProcessor.Warning(log, isImmediate, args);
        }
        public static void Warning(object logObj, bool isImmediate = false)
        {
            UnityLoggerProcessor.Warning(logObj, isImmediate);
        }

        public static void Error(string log, bool isImmediate = false, params object[] args)
        {
            UnityLoggerProcessor.Error(log, isImmediate, args);
        }
        public static void Error(object logObj, bool isImmediate = false)
        {
            UnityLoggerProcessor.Error(logObj, isImmediate);
        }

        public static void Trace(string log, bool isImmediate = false, params object[] args)
        {
            UnityLoggerProcessor.Trace(log, isImmediate, args);
        }
        public static void Trace(object logObj, bool isImmediate = false)
        {
            UnityLoggerProcessor.Trace(logObj, isImmediate);
        }

        public static void Color(LoggerColor color, string log, bool isImmediate = false, params object[] args)
        {
            UnityLoggerProcessor.Color(color, log, isImmediate, args);
        }
        public static void Color(LoggerColor color, object logObj, bool isImmediate = false)
        {
            UnityLoggerProcessor.Color(color, logObj, isImmediate);
        }
    }
}