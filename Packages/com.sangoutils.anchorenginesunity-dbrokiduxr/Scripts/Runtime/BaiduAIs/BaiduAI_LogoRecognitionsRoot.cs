using SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.DB_RokidUXR.BaiduAIs
{
    [RequireComponent(typeof(BaiduAI_LogoRecognitionsConfig))]
    public class BaiduAI_LogoRecognitionsRoot : MonoBehaviour
    {
        [Header("Optional")]
        [SerializeField] private TMP_Text _logTxt;
        [SerializeField] private int _logMaxCount = 20;

        private readonly Queue<string> _logQueue = new(10);

        private void Awake()
        {
            BaiduAI_LogoRecognitionsEventBus.Config = GetComponent<BaiduAI_LogoRecognitionsConfig>();
            BaiduAI_LogoRecognitionsEventBus.LogEvent.AddListener(Log);

            gameObject.AddComponent<CameraTextureBehaviour>();
            gameObject.AddComponent<BaiduAI_LogoRecognitionsSubspaceSpinOP>();
            new BaiduAI_LogoRecognitionsSubspaceOP();

            BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinedEvent.Invoke(this, new BaiduAI_LogoRecognitionsMessages.SubspaceSpinEvtArgs
                (BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode,
                BaiduAI_LogoRecognitionsMessages.SubspaceSpinRefocusCode.StartRefocus));

            BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("StartRefocus");
        }

        private void Log(string log)
        {
            if (_logTxt == null) return;

            if (_logQueue.Count > _logMaxCount)
            {
                _logQueue.Dequeue();
            }
            _logQueue.Enqueue(log);

            StringBuilder stringBuilder = new();
            foreach (var item in _logQueue)
            {
                stringBuilder.AppendLine(item);
            }
            _logTxt.text = stringBuilder.ToString();
        }
    }
}