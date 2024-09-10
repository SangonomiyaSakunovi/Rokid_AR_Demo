using Fusion;
using SceneBehaviours;
using UnityEngine;

namespace SceneGameObjects
{
    internal class SyncPoseObjectPeriodicallyBehaviour : NetworkBehaviour
    {
        [SerializeField] private GameBebaviourConfig _config;
        [SerializeField] private float _syncCD = 5f;

        [Networked] private TickTimer SyncCD { get; set; }

        public override void Spawned()
        {
            SyncCD = TickTimer.CreateFromSeconds(Runner, _syncCD);
        }

        public override void FixedUpdateNetwork()
        {
            if (_config.GameDeviceType == GameCongifs.GameDeviceType.Editor
                || _config.GameDeviceType == GameCongifs.GameDeviceType.PhysicsCameraComposition)
                return;

            if (SyncCD.ExpiredOrNotRunning(Runner))
            {
                RPC_SyncObjectPose(Runner.UserId, transform.localPosition, transform.localRotation);
                SyncCD = TickTimer.CreateFromSeconds(Runner, _syncCD);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_SyncObjectPose(string id, Vector3 position, Quaternion rotation)
        {
            if (!id.Equals(Runner.UserId))
            {
                transform.localPosition = position;
                transform.localRotation = rotation;
            }
        }
    }
}
