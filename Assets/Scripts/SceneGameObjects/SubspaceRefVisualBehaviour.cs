using SceneBehaviours;
using UnityEngine;

namespace SceneGameObjects
{
    internal class SubspaceRefVisualBehaviour : MonoBehaviour
    {
        [SerializeField] private GameBebaviourConfig _config;

        [SerializeField] private SkinnedMeshRenderer _meshRenderer;

        [SerializeField] private Material _visualTexRes;
        [SerializeField] private Material _transparentTexRes;

        private void Awake()
        {
            if (_config.GameDeviceType == GameCongifs.GameDeviceType.PhysicsCameraComposition
                || _config.GameDeviceType == GameCongifs.GameDeviceType.RuntimeDebug)
                _meshRenderer.material = _visualTexRes;
            else if(_config.GameDeviceType == GameCongifs.GameDeviceType.RokidAR)
                _meshRenderer.material = _transparentTexRes;
        }
    }
}
