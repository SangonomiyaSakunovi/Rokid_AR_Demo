using UnityEngine.Events;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal class BaiduAI_LogoRecognitionsEventBus
    {
        internal static bool IsInitialized { get; private set; } = false;

        private static BaiduAI_LogoRecognitionsConfig _config;
        internal static BaiduAI_LogoRecognitionsConfig Config
        {
            get => _config; 
            set
            {
                _config ??= value; IsInitialized = true;
            }
        }

        public static UnityEvent<string> LogEvent { get; } = new();
        public static UnityEvent<object, BaiduAI_LogoRecognitionsMessages.UploadEvtArgs> OnAutoRefocusFoundedEvent { get; } = new();
        public static UnityEvent<object, BaiduAI_LogoRecognitionsMessages.SubspaceSpinEvtArgs> OnSubspaceSpinedEvent { get; } = new();
        public static UnityEvent<object, BaiduAI_LogoRecognitionsMessages.SubspaceSpinAdjustmentEvtArgs> OnSubspaceSpinAdjustmentedEvent { get; } = new();
    }
}
