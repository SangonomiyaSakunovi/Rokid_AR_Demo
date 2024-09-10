using SangoUtils.AnchorEngines_Unity.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal class BaiduAI_LogoRecognitionsUploadRequest : MonoBehaviour
    {
        private IEnumerable<string> _validLogoNames;
        private readonly ConcurrentQueue<string> _logoNamesMessages = new();

        private void Awake()
        {
            _validLogoNames = new List<string>() {
                BaiduAI_LogoRecognitionsEventBus.Config.XPositiveLogoName,
                BaiduAI_LogoRecognitionsEventBus.Config.XNegativeLogoName,
                BaiduAI_LogoRecognitionsEventBus.Config.ZPositiveLogoName,
                BaiduAI_LogoRecognitionsEventBus.Config.ZNegativeLogoName };
        }

        private void Update()
        {
            if (_logoNamesMessages.Count > 0 && _logoNamesMessages.TryDequeue(out var logoName))
            {
                BaiduAI_LogoRecognitionsEventBus.OnAutoRefocusFoundedEvent.Invoke(this, new BaiduAI_LogoRecognitionsMessages.UploadEvtArgs(logoName));
            }
        }

        internal void Upload(CameraTextureData data)
        {
            WWWForm form = new();

            form.AddField("width", data.Width);
            form.AddField("height", data.Height);
            form.AddBinaryData("file", data.Data);

            UnityWebRequest unityWebRequest = UnityWebRequest.Post(BaiduAI_LogoRecognitionsEventBus.Config.SeverHost, form);
            unityWebRequest.certificateHandler = new WebReqSkipCert();
            UnityWebRequestAsyncOperation operation = unityWebRequest.SendWebRequest();
            operation.completed += delegate
            {
                if (unityWebRequest.result is UnityWebRequest.Result.ProtocolError
                || unityWebRequest.result is UnityWebRequest.Result.ConnectionError)
                {
                    BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("[Error]: " + unityWebRequest.error);
                }
                else
                {
                    OnLogoRecognized(unityWebRequest.downloadHandler.text, data.Width, data.Height);
                }
            };
        }

        private void OnLogoRecognized(string content, int width, int height)
        {
            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(content);
                BaiduAI_LogoRecognitionsDataDefs.RspData baiduLogoRspData =
                    JsonSerializer.Deserialize<BaiduAI_LogoRecognitionsDataDefs.RspData>(jsonDocument.RootElement.GetString());

                if (baiduLogoRspData.ErrorCode is 0
                    && baiduLogoRspData.ResultNum > 0)
                {
                    int targetLeftMin = width / 2 - BaiduAI_LogoRecognitionsEventBus.Config.TargetLogoLocationOffsetRange;
                    int targetLeftMax = width / 2 + BaiduAI_LogoRecognitionsEventBus.Config.TargetLogoLocationOffsetRange;
                    int targetTopMin = height / 2 - BaiduAI_LogoRecognitionsEventBus.Config.TargetLogoLocationOffsetRange;
                    int targetTopMax = height / 2 + BaiduAI_LogoRecognitionsEventBus.Config.TargetLogoLocationOffsetRange;
                    foreach (var res in baiduLogoRspData.Results)
                    {
                        if (res.Probability < BaiduAI_LogoRecognitionsEventBus.Config.ProbabilityThreshold
                            || _validLogoNames.Contains(res.Name) is false) continue;

                        int centerX = res.Location.Left + res.Location.Width / 2;
                        int centerY = res.Location.Top + res.Location.Height / 2;

                        if (centerX > targetLeftMin
                            && centerX < targetLeftMax
                            && centerY > targetTopMin
                            && centerY < targetTopMax
                            && res.Location.Width > BaiduAI_LogoRecognitionsEventBus.Config.TargetLogoWidthRange
                            && res.Location.Height > BaiduAI_LogoRecognitionsEventBus.Config.TargetLogoHeightRange)
                        {
                            _logoNamesMessages.Enqueue(res.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("[error]: Json Decompile, message: " + ex);
            }
        }
    }
}