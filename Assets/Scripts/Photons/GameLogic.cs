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

        [SerializeField] private int _peerIDPrefix;

        private int _peerIDIndex = 0;

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
            
        }

        public void PlayerJoined(PlayerRef player)
        {
            //Only the host care about who join this game.
            if (HasStateAuthority)
            {
                string strIndex = "";
                if (_peerIDIndex < 10)
                    strIndex = "00" + _peerIDIndex;
                else if (_peerIDIndex < 100)
                    strIndex = "0" + _peerIDIndex;
                else if (_peerIDIndex < 1000)
                    strIndex = _peerIDIndex.ToString();
                else
                    UnityLogger.Error("[Sango] The index overflow in " + nameof(ObjectManager));

                _peerIDIndex++;

                GetSpawnPoint(Pose.identity, out Vector3 position, out Quaternion rotation);

                NetworkObject playerObj = Runner.Spawn(playerPrefab, position, rotation, player);
                playerObj.transform.SetParent(_objPlayersParent.transform);
                var component = playerObj.GetComponent<Player>();
                component.LocalPlayerRef = player;
                component.PeerID = int.Parse(_peerIDPrefix + strIndex);
                Players.Add(player, component);

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

        public void SetActiveObj<T>(int name, bool isActive = true) where T : IObjBehaviour
        {
            ObjectManager.SetActiveObj<T>(name, isActive);
        }

        public void SyncPose(int name, Vector3 position, Quaternion rotation)
        {
            ObjectManager.SyncPose(name, position, rotation);
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
