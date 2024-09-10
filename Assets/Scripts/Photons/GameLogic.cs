using Fusion;
using SceneGameObjects;
using SangoUtils.UnityDevelopToolKits.Loggers;
using UnityEngine;

namespace Photons
{
    internal class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef playerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameObject _objPlayersParent;

        [field: SerializeField] private ObjectManager ObjectManager { get; set; }

        /// <summary>
        /// This dict should carry by GameLogic, but not ObjectManager.
        /// Because some important instance will store in Player.
        /// </summary>
        [Networked, Capacity(6)] private NetworkDictionary<PlayerRef, Player> Players => default;

        public static GameLogic Instance
        {
            get => _instance;
            set
            {
                if (value == null)
                    _instance = null;
                else if (_instance == null)
                    _instance = value;
                else if (_instance != value)
                {
                    Destroy(value);
                    UnityLogger.Error($"There should only ever be one instance of {nameof(GameLogic)}!");
                }
            }
        }
        private static GameLogic _instance;

        public override void Spawned()
        {
            Runner.SetIsSimulated(Object, true);

            Instance = this;
        }

        public override void FixedUpdateNetwork()
        {
            if (Players.Count < 1)
                return;

        }

        public void PlayerJoined(PlayerRef player)
        {
            //Only the host care about who join this game.
            if (HasStateAuthority)
            {
                GetSpawnPoint(Pose.identity, out Vector3 position, out Quaternion rotation);

                NetworkObject playerObj = Runner.Spawn(playerPrefab, position, rotation, player);
                playerObj.transform.SetParent(_objPlayersParent.transform);
                Players.Add(player, playerObj.GetComponent<Player>());

                UnityLogger.Color(LoggerColor.Purple, Runner.SessionInfo.ToString());
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            //Also only the host care about who left this game.
            if (!HasStateAuthority)
                return;

            if (Players.TryGet(player, out var behaviour))
            {
                Players.Remove(player);
                Runner.Despawn(behaviour.Object);

                if (Players.Count == 0)
                    Runner.SessionInfo.IsOpen = false;
            }
        }

        private void GetSpawnPoint(Pose pose, out Vector3 position, out Quaternion rotation)
        {
            position = _spawnPoint.position + pose.position;
            rotation = pose.rotation * _spawnPoint.rotation;
        }

        public void InstantiateObjAsyc<T>(string name, string ID, Vector3 position, Quaternion rotation) where T : IObjBehaviour
        {
            ObjectManager.InstantiateObjAsyc<T>(name, ID, position, rotation);
        }

        public void SetActiveObj<T>(string name, bool isActive = true) where T : IObjBehaviour
        {
            ObjectManager.SetActiveObj<T>(name, isActive);
        }

        private void OnApplicationQuit()
        {
            if(!HasStateAuthority)
                { return; }

            //Change state authority hands
            if (Players.Count > 0)
            {
                Object.ReleaseStateAuthority();
                foreach (var player in Players)
                {
                    player.Value.Object.RequestStateAuthority();
                    break;
                }
            }
            else
            {
                Runner.SessionInfo.IsOpen = false;
            }
        }
    }
}
