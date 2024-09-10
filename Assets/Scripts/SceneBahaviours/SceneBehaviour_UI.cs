using GameCongifs;
using SangoUtils.UnityDevelopToolKits.Loggers;
using TMPro;
using UnityEngine;

namespace SceneBehaviours
{
    internal class SceneBehaviour_UI : MonoBehaviour, ISceneBehaviour
    {
        [SerializeField] private GameBebaviourConfig _config;
        
        [SerializeField] private TextMeshProUGUI _loggerTxt;
        [SerializeField] private GameObject _rokidRKComponentParent;
        [SerializeField] private GameObject _fusionBehaviourParent;
        [SerializeField] private GameObject _uiPanelsParent;

        public GameObject RokidRKComponentParent => _rokidRKComponentParent;
        public string SceneBehaviourName => nameof(SceneBehaviour_UI);

        public static SceneBehaviour_UI Instance
        {
            get => _instance;
            set
            {
                if (value == null)
                    _instance = null;
                else if (_instance == null)
                    _instance = value;
                else if (_instance != null)
                {
                    Destroy(value);
                    UnityLogger.Error($"There should only ever be one instance of {nameof(SceneBehaviour_UI)}!");
                }
            }
        }

        private static SceneBehaviour_UI _instance;

        private void Awake()
        {
            Instance = this;

            var cfg = new LoggerConfig();

            if (_config.GameDeviceType == GameDeviceType.RuntimeDebug)
            {
                cfg.IsLogCanvas = true;
                cfg.CanvasLogMaxCount = 12;
                cfg.TextMeshProUGUI = _loggerTxt;
            }
            else
            {
                _loggerTxt.gameObject.SetActive(false);
            }

            UnityLogger.Initialize(cfg);
        }

        private void Start()
        {
            UnityLogger.Color(LoggerColor.Green, "Game Start.");
        }

        public void OnNewSceneOverlayed(string sceneBehaviourName)
        {
            if (sceneBehaviourName == nameof(SceneBehaviour_Game))
            {
                if (_config.GameDeviceType == GameDeviceType.PhysicsCameraComposition)
                {
                    _rokidRKComponentParent.SetActive(false);
                    _uiPanelsParent.SetActive(false);
                }
            }
        }
    }
}