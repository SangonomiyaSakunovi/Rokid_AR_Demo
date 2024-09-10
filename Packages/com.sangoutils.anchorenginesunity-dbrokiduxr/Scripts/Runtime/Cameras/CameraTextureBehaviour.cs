using Rokid.UXR.Native;
using Rokid.UXR.Utility;
using SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions;
using System;
using UnityEngine;

public class CameraTextureBehaviour : MonoBehaviour
{
    private bool _isInit = false;

    private void Awake()
    {
#if UNITY_EDITOR
        return;
#endif
        NativeInterface.NativeAPI.StartCameraPreview();
    }

    private void Update()
    {
        if (!_isInit && NativeInterface.NativeAPI.IsPreviewing() && BaiduAI_LogoRecognitionsEventBus.IsInitialized)
        {
            Init();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Release();
        }
        else
        {
            NativeInterface.NativeAPI.StartCameraPreview();
        }
    }

    private void OnDestroy()
    {
        Release();
    }

    private void Init()
    {
        int width = NativeInterface.NativeAPI.GetPreviewWidth();
        int height = NativeInterface.NativeAPI.GetPreviewHeight();

        NativeInterface.NativeAPI.SetCameraPreviewDataType(2);

        BaiduAI_LogoRecognitionsEventBus.Config.CameraTextureData = new(width, height, new byte[width * height * 4]);

        NativeInterface.NativeAPI.OnCameraDataUpdate += OnCameraDataUpdate;

        _isInit = true;
    }

    private void OnCameraDataUpdate(int cameraWidth, int cameraHeight, byte[] cameraData, long timestamp)
    {
        Loom.QueueOnMainThread(() =>
        {
            Array.Copy(cameraData, BaiduAI_LogoRecognitionsEventBus.Config.CameraTextureData.Data, cameraData.Length);
        });
    }

    private void Release()
    {
        if (_isInit)
        {
            NativeInterface.NativeAPI.OnCameraDataUpdate -= OnCameraDataUpdate;
            NativeInterface.NativeAPI.StopCameraPreview();
            NativeInterface.NativeAPI.ClearCameraDataUpdate();
            _isInit = false;
        }
    }
}
