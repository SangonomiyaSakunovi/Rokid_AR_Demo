using GameCongifs;
using UnityEngine;

namespace SceneBehaviours
{
    [CreateAssetMenu(menuName = "SangoProjects/Scenes/Game Behaviour Config")]
    internal class GameBebaviourConfig : ScriptableObject
    {
        [SerializeField] private GameDeviceType _gameDeviceType = GameDeviceType.Editor;

        public GameDeviceType GameDeviceType => _gameDeviceType;
    }
}
