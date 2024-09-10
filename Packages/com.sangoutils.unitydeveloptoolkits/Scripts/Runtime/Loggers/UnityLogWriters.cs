using System;
using System.Collections.Concurrent;
using System.Text;
using TMPro;
using UnityEngine;

namespace SangoUtils.UnityDevelopToolKits.Loggers
{
    internal class ConsoleLogWriter : MonoBehaviour, IUnityLogWriter
    {
        private string _preTag;
        private ConcurrentQueue<string> _messages;
        private bool _isMessageReceived = false;

        public void Initialize(string preTag)
        {
            _preTag = preTag;
        }

        private void Awake()
        {
            _messages = new ConcurrentQueue<string>();
        }

        private void Update()
        {
            while (_messages.Count > 0)
            {
                if (_messages.TryDequeue(out var res))
                    Debug.Log(res);
            }
        }

        public void Log(string log, LoggerColor color, bool isImmediate)
        {
            GetUnityLogColorString(ref log, color);

            if (isImmediate)
                Debug.Log(log);
            else
                _messages.Enqueue(log);
        }

        public void Warning(string log, bool isImmediate)
        {
            GetUnityLogColorString(ref log, LoggerColor.Yellow);

            if (isImmediate)
                Debug.Log(log);
            else
                _messages.Enqueue(log);
        }

        public void Error(string log, bool isImmediate)
        {
            GetUnityLogColorString(ref log, LoggerColor.Red);

            if (isImmediate)
                Debug.Log(log);
            else
                _messages.Enqueue(log);
        }

        private void GetUnityLogColorString(ref string log, LoggerColor color)
        {
            switch (color)
            {
                case LoggerColor.None:
                    log = string.Format("<color=#4E88DD>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Yellow:
                    log = string.Format("<color=#FFFF00>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Red:
                    log = string.Format("<color=#FF0000>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Green:
                    log = string.Format("<color=#00FF00>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Blue:
                    log = string.Format("<color=#0000FF>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Magenta:
                    log = string.Format("<color=#FF00FF>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Cyan:
                    log = string.Format("<color=#00FFFF>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Purple:
                    log = string.Format("<color=#5000B8>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Orange:
                    log = string.Format("<color=#FF8C00>{0}</color>{1}", _preTag, log);
                    break;
            }
        }
    }

    internal class CanvasLogWriter : MonoBehaviour, IUnityLogWriter
    {
        private string _preTag;
        private int _maxMessageCount;
        private TextMeshProUGUI _textMeshProUGUI;
        private ConcurrentQueue<string> _messages;
        private bool _isMessageReceived = false;
        private StringBuilder _sb;

        public void Initialize(string preTag, int maxMessageCount, TextMeshProUGUI textMeshProUGUI)
        {
            _preTag = preTag;
            _maxMessageCount = maxMessageCount;
            _textMeshProUGUI = textMeshProUGUI;
        }

        private void Awake()
        {
            _messages = new ConcurrentQueue<string>();
            _sb = new StringBuilder();
        }

        public void Update()
        {
            if (_isMessageReceived)
                UpdateCanvas();
        }

        public void Log(string log, LoggerColor color, bool isImmediate)
        {
            GetUnityLogColorString(ref log, color);
            AddLogMessages(log);

            if (isImmediate)
                UpdateCanvas();
        }

        public void Warning(string log, bool isImmediate)
        {
            GetUnityLogColorString(ref log, LoggerColor.Yellow);
            AddLogMessages(log);

            if (isImmediate)
                UpdateCanvas();
        }

        public void Error(string log, bool isImmediate)
        {
            GetUnityLogColorString(ref log, LoggerColor.Red);
            AddLogMessages(log);

            if (isImmediate)
                UpdateCanvas();
        }

        private void GetUnityLogColorString(ref string log, LoggerColor color)
        {
            switch (color)
            {
                case LoggerColor.None:
                    log = string.Format("<color=#4E88DD>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Yellow:
                    log = string.Format("<color=#FFFF00>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Red:
                    log = string.Format("<color=#FF0000>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Green:
                    log = string.Format("<color=#00FF00>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Blue:
                    log = string.Format("<color=#0000FF>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Magenta:
                    log = string.Format("<color=#FF00FF>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Cyan:
                    log = string.Format("<color=#00FFFF>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Purple:
                    log = string.Format("<color=#5000B8>{0}</color>{1}", _preTag, log);
                    break;
                case LoggerColor.Orange:
                    log = string.Format("<color=#FF8C00>{0}</color>{1}", _preTag, log);
                    break;
            }
        }

        private void AddLogMessages(string log)
        {
            _messages.Enqueue(log);

            while (_messages.Count > _maxMessageCount)
                _messages.TryDequeue(out var _);

            _isMessageReceived = true;
        }

        private void UpdateCanvas()
        {
            if (_textMeshProUGUI == null)
                throw new NullReferenceException(nameof(TextMeshPro));

            foreach (var message in _messages)
                _sb.AppendLine(message.ToString());

            _textMeshProUGUI.text = _sb.ToString();
            _sb.Clear();
            _isMessageReceived = false;
        }
    }
}
