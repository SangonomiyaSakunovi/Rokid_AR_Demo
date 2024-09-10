using UnityEngine;

namespace SceneBehaviours
{
    internal class SceneBehaviour_Game : MonoBehaviour, ISceneBehaviour
    {
        [SerializeField] private GameBebaviourConfig _config;

        [SerializeField] private GameObject _gameLogicParent;
        [SerializeField] private GameObject _subspaceParent;
        [SerializeField] private GameObject _subspaceRefParent;
        [SerializeField] private GameObject _physicCameraParent;

        [SerializeField] private GameObject _subspaceVisualParent;
        [SerializeField] private GameObject _subspaceRefVisualParent;

        public string SceneBehaviourName => nameof(SceneBehaviour_Game);

        private void Awake()
        {
            if (_config.GameDeviceType == GameCongifs.GameDeviceType.PhysicsCameraComposition)
            {
                _physicCameraParent.SetActive(true);
            }
            else if (_config.GameDeviceType == GameCongifs.GameDeviceType.RokidAR
                || _config.GameDeviceType == GameCongifs.GameDeviceType.RuntimeDebug)
            {

            }
        }

        private void Start()
        {
            if (SceneBehaviour_UI.Instance != null)
                SceneBehaviour_UI.Instance.OnNewSceneOverlayed(nameof(SceneBehaviour_Game));
        }

        public void OnNewSceneOverlayed(string sceneBehaviourName)
        {

        }
    }
}
