using Photons;
using SangoUtils.UnityDevelopToolKits.Loggers;
using UnityEngine;

namespace SceneGameObjects
{
    internal class GrabableObjectBehaviour : MonoBehaviour, IGrabableObjBehaviour
    {
        public int ObjectID { get; set; }
        public GameObject GameObject { get; set; }

        [field: SerializeField] public bool IsHideDefault { get; set; }

        private bool _isGrabUpdate = false;

        private void FixedUpdate()
        {
            if (_isGrabUpdate)
            {
                GameLogic.Instance.SyncPose(ObjectID, transform.localPosition, transform.localRotation);
            }
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

        public void SyncPose(Vector3 position, Quaternion rotation)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
    }
}
