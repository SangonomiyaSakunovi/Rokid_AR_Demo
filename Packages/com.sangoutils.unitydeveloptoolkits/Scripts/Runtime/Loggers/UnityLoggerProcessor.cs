using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SangoUtils.UnityDevelopToolKits.Loggers
{
    internal class UnityLoggerProcessor : MonoBehaviour
    {
        private static LoggerConfig _config;
        private static ConsoleLogWriter _consoleWriter;
        private static CanvasLogWriter _canvasWriter;
        private static StreamWriter _logFileWriter;
        private static StringBuilder _sb;

        public static void Initialize(LoggerConfig cfg)
        {
            GameObject go = new GameObject($"[{nameof(UnityLoggerProcessor)}]");
            go.AddComponent<UnityLoggerProcessor>();
            DontDestroyOnLoad(go);
            _sb = new StringBuilder(100);

            cfg ??= new LoggerConfig();
            _config = cfg;

            string preTag = $"[{_config.LogPreTag}] ";

            if (_config.IsLogConsole)
            {
                _consoleWriter = go.AddComponent<ConsoleLogWriter>();
                _consoleWriter.Initialize(preTag);
            }

            if (_config.IsLogCanvas)
            {
                _canvasWriter = go.AddComponent<CanvasLogWriter>();
                _canvasWriter.Initialize(preTag, _config.CanvasLogMaxCount, _config.TextMeshProUGUI);
            }

            if (_config.IsOutputLogFile == false)
                return;

            if (_config.IsOverrideLogFile)
            {
                string path = Path.Combine(_config.OutputLogPath, _config.OutputLogName);
                try
                {
                    if (Directory.Exists(_config.OutputLogPath))
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                    else
                        Directory.CreateDirectory(_config.OutputLogPath);

                    _logFileWriter = File.AppendText(path);
                    _logFileWriter.AutoFlush = true;
                }
                catch
                {
                    _logFileWriter = null;
                }
            }
            else
            {
                string prefix = DateTime.Now.ToString("yyyyMMdd@HH-mm-ss");
                string path = Path.Combine(_config.OutputLogPath, prefix + _config.OutputLogName);
                try
                {
                    if (Directory.Exists(_config.OutputLogPath) == false)
                        Directory.CreateDirectory(_config.OutputLogPath);

                    _logFileWriter = File.AppendText(path);
                    _logFileWriter.AutoFlush = true;
                }
                catch
                {
                    _logFileWriter.Close();
                    _logFileWriter = null;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _logFileWriter.Close();
            _logFileWriter = null;
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        #region Log
        public static void Log(string log, bool isImmediate, params object[] args)
        {
            if (_config != null)
            {
                if (args != null && args.Length > 0)
                    log = string.Format(log, args);

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Log(log, LoggerColor.None, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Log(log, LoggerColor.None, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Log] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Log(object logObj, bool isImmediate)
        {
            if (_config != null)
            {
                string log = logObj.ToString();

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Log(log, LoggerColor.None, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Log(log, LoggerColor.None, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Log] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Warning(string log, bool isImmediate, params object[] args)
        {
            if (_config != null)
            {
                if (args != null && args.Length > 0)
                    log = string.Format(log, args);

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Warning(log, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Warning(log, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Warning] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Warning(object logObj, bool isImmediate)
        {
            if (_config != null)
            {
                string log = logObj.ToString();

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Warning(log, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Warning(log, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Warning] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Error(string log, bool isImmediate, params object[] args)
        {
            if (_config != null)
            {
                if (args != null && args.Length > 0)
                    log = string.Format(log, args);

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Error(log, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Error(log, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Error] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Error(object logObj, bool isImmediate)
        {
            if (_config != null)
            {
                string log = logObj.ToString();

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Error(log, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Error(log, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Error] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Trace(string log, bool isImmediate, params object[] args)
        {
            if (_config != null)
            {
                if (args != null && args.Length > 0)
                    log = string.Format(log, args);

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Log(log, LoggerColor.Cyan, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Log(log, LoggerColor.Cyan, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Trace] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Trace(object logObj, bool isImmediate)
        {
            if (_config != null)
            {
                string log = logObj.ToString();

                DecorateLog(ref log, true);

                if (_config.IsLogConsole)
                    _consoleWriter.Log(log, LoggerColor.Cyan, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Log(log, LoggerColor.Cyan, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Trace] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Color(LoggerColor color, string log, bool isImmediate, params object[] args)
        {
            if (_config != null)
            {
                if (args != null && args.Length > 0)
                    log = string.Format(log, args);

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Log(log, color, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Log(log, color, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Log] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        public static void Color(LoggerColor color, object logObj, bool isImmediate)
        {
            if (_config != null)
            {
                string log = logObj.ToString();

                DecorateLog(ref log);

                if (_config.IsLogConsole)
                    _consoleWriter.Log(log, color, isImmediate);

                if (_config.IsLogCanvas)
                    _canvasWriter.Log(log, color, isImmediate);

                if (_config.IsOutputLogFile)
                    WriteToFile(string.Format("[Log] {0}", log));
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }
        #endregion

        #region Decorate
        private static void DecorateLog(ref string log, bool isTraceInfo = false)
        {
            if (_config != null)
            {
                _sb.Append(_config.LogPrefix);

                if (_config.IsLogTimestamp)
                    _sb.AppendFormat(" {0}", DateTime.Now.ToString("hh:mm:ss--fff"));

                if (_config.IsLogThreadID)
                    _sb.AppendFormat(" {0}", GetThreadID());

                _sb.AppendFormat(" {0} {1}", _config.LogSeparate, log);

                if (isTraceInfo)
                    _sb.AppendFormat(" \nStackTrace: {0}", GetTraceInfo());

                log = _sb.ToString();
                _sb.Clear();
            }
            else
                throw new ArgumentNullException(nameof(_config));
        }

        private static string GetThreadID()
        {
            return string.Format("ThreadID:{0}", Environment.CurrentManagedThreadId);
        }

        private static string GetTraceInfo()
        {
            StackTrace st = new StackTrace(3, true);    //The method called DecorateLog has 3 calls should be ignore
            string traceInfo = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame sf = st.GetFrame(i);
                traceInfo += string.Format("\n    {0}::{1}  line:{2}", sf.GetFileName(), sf.GetMethod(), sf.GetFileLineNumber());
            }
            return traceInfo;
        }
        #endregion

        #region Tools
        private static void WriteToFile(string log)
        {
            if (_logFileWriter != null)
            {
                try
                {
                    _logFileWriter.WriteLine(log);
                }
                catch
                {
                    _logFileWriter.Close();
                    _logFileWriter = null;
                }
            }
        }
        #endregion
    }
}