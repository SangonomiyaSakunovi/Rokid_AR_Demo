using SangoUtils.HTCViveTrackerHelpers;
using UnityEngine;

namespace SangoProjects.HTCBehaviours
{
    internal class PhysicsCameraBehaviour : MonoBehaviour
    {
        [SerializeField] private TrackerDataReceiver trackerDataReceiver;
        [SerializeField] private Transform visualTrans;

        [SerializeField] private Vector3 _offset = Vector3.zero;

        private (Vector3, Quaternion) _camPose;

        private void Awake()
        {
            trackerDataReceiver.OnTrackerDataUpdate += delegate (Vector3 pos, Quaternion rot)
            {
                _camPose.Item1 = pos;
                _camPose.Item2 = rot;
            };
        }

        private void Update()
        {
            visualTrans.SetLocalPositionAndRotation(_camPose.Item1 + _offset, _camPose.Item2);
        }
    }
}