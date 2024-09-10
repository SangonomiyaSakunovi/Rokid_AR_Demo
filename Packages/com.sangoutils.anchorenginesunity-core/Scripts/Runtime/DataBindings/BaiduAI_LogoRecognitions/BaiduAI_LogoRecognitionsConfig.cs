using UnityEngine;
using UnityEngine.Events;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal class BaiduAI_LogoRecognitionsConfig : MonoBehaviour
    {
        #region Configs
        [Header("Target Size")]
        [SerializeField] private int _targetLogoWidthRange = 50;
        [SerializeField] private int _targetLogoHeightRange = 50;
        [SerializeField] private int _targetLogoLocationOffsetRange = 150;

        [Header("Logo Names")]
        [SerializeField] private string _serverHost = "https://192.168.0.43:52516/uploadTexture";
        [SerializeField] private string _xPositiveLogoName = "fos";
        [SerializeField] private string _xNegativeLogoName = "baidu_logo_1";
        [SerializeField] private string _zPositiveLogoName = "baidu_logo_2";
        [SerializeField] private string _zNegativeLogoName = "baidu_logo_3";
        [SerializeField] private float _probabilityThreshold = 0.6f;

        [Header("Refocus Args")]
        [SerializeField] private float _updateSubspaceSpinRefocusTaskInterval = 1f;
        [SerializeField] private float _updateSubspaceSpinRefocusTimeLimite = 1f * 60 * 5;

        [Header("UnityEvent")]
        [SerializeField] private UnityEvent _onAutoRefocusLogoFound = new UnityEvent();
        [SerializeField] private UnityEvent _onAutoRefocusDone = new UnityEvent();
        [SerializeField] private UnityEvent _onAutoRefocusToManual = new UnityEvent();

        [Header("Path Define")]
        [Tooltip("Define: Root + Tag + Suffix. \ne.g. Prefabs/SubspaceSpins/XNegativeLogoNameSpin")]
        [SerializeField] private string _recognitionLogoPrefabPathRoot = "Prefabs/SubspaceSpins/";
        [SerializeField] private string _recognitionLogoPrefabPathSuffix = "Spin";

        [Header("Generator Define")]
        [SerializeField] private int _recognitionLogoPrefabOffsetZToViewCenterObject = 1;
        [SerializeField] private GameObject _viewCenterObject;
        [SerializeField] private GameObject _subspaceSpinsRootObject;
        [SerializeField] private GameObject _sangoSubspaceRootObject;
        #endregion

        internal int TargetLogoWidthRange { get => _targetLogoWidthRange; }
        internal int TargetLogoHeightRange { get => _targetLogoHeightRange; }
        internal int TargetLogoLocationOffsetRange { get => _targetLogoLocationOffsetRange; }

        internal string SeverHost { get => _serverHost; }
        internal string ZPositiveLogoName { get => _zPositiveLogoName; }
        internal string ZNegativeLogoName { get => _zNegativeLogoName; }
        internal string XPositiveLogoName { get => _xPositiveLogoName; }
        internal string XNegativeLogoName { get => _xNegativeLogoName; }
        internal float ProbabilityThreshold { get => _probabilityThreshold; }

        internal float UpdateSubspaceSpinRefocusTaskInterval { get => _updateSubspaceSpinRefocusTaskInterval; }
        internal float UpdateSubspaceSpinRefocusTimeLimite { get => _updateSubspaceSpinRefocusTimeLimite; }
        internal UnityEvent OnAutoRefocusLogoFound { get => _onAutoRefocusLogoFound; }
        internal UnityEvent OnAutoRefocusDone { get => _onAutoRefocusDone; }
        internal UnityEvent OnAutoRefocusToManual { get => _onAutoRefocusToManual; }

        internal string RecognitionLogoPrefabPathRoot { get => _recognitionLogoPrefabPathRoot; }
        internal string RecognitionLogoPrefabPathSuffix { get => _recognitionLogoPrefabPathSuffix; }

        internal int RecognitionLogoPrefabOffsetZToViewCenterObject { get => _recognitionLogoPrefabOffsetZToViewCenterObject; }
        internal GameObject ViewCenterObject { get => _viewCenterObject; }
        internal GameObject SubspaceSpinsRootObject { get => _subspaceSpinsRootObject; }
        internal GameObject SangoSubspaceRootObject { get => _sangoSubspaceRootObject; }

        #region InternalVisibleToOther
        internal CameraTextureData CameraTextureData { get; set; } = null;
        #endregion
    }
}
