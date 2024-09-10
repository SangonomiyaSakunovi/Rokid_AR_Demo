using Fusion;
using SangoUtils.UnityDevelopToolKits.Loggers;
using UnityEngine;

namespace SceneGameObjects
{
    internal class GrabableObjectBehaviour : NetworkBehaviour, IGrabableObjBehaviour
    {
        public string ObjectID { get; set; }
        public GameObject GameObject { get; set; }

        private bool _isGrabUpdate = false;

        public override void FixedUpdateNetwork()
        {
            if (_isGrabUpdate)
                RPC_SyncGrabPose(Runner.UserId, transform.localPosition, transform.localRotation);
        }

        public void OnGrabStart()
        {
            _isGrabUpdate = true;
            UnityLogger.Color(LoggerColor.Cyan, "OnGrabStart");
        }

        public void OnGrabEnd()
        {
            _isGrabUpdate = false;
            UnityLogger.Color(LoggerColor.Orange, "OnGrabEnd");
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_SyncGrabPose(string id, Vector3 position, Quaternion rotation)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
    }
}
