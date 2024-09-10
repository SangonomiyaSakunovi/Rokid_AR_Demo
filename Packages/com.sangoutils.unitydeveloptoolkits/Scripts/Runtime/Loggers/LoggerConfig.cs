using System.IO;
using TMPro;
using UnityEngine;

namespace SangoUtils.UnityDevelopToolKits.Loggers
{
    public class LoggerConfig
    {
        public bool IsLogConsole { get; set; } = true;
        public bool IsLogCanvas { get; set; } = false;
        public int CanvasLogMaxCount { get; set; } = 10;
        public TextMeshProUGUI TextMeshProUGUI { get; set; } = null;

        public bool IsLogTimestamp { get; set; } = true;
        public bool IsLogThreadID { get; set; } = true;

        public string LogPreTag { get; set; } = "Sango";
        public string LogPrefix { get; set; } = "#";
        public string LogSeparate { get; set; } = ">>";

        public bool IsOutputLogFile { get; set; } = true;
        public bool IsOverrideLogFile { get; set; } = true;
        public string OutputLogPath { get; set; } = Path.Combine(Application.dataPath, "Logs");
        public string OutputLogName { get; set; } = "UnityLog.txt";
    }

    public enum LoggerColor
    {
        None,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow,
        Purple,
        Orange
    }

    internal interface IUnityLogWriter
    {
        void Log(string message, LoggerColor color, bool isImmediate);
        void Warning(string message, bool isImmediate);
        void Error(string message, bool isImmediate);
    }
}