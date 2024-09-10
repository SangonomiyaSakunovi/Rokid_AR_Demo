using SangoUtils.HTCViveTrackerHelpers;
using UnityEngine;

namespace SangoProjects.HTCBehaviours
{
    internal class PhysicsCameraBehaviour : MonoBehaviour
    {
        [SerializeField] private TrackerDataReceiver trackerDataReceiver;
        [SerializeField] private Transform visualTrans;

        /// <summary>
        /// Use for offset, in my origin design, you can get it by other way.
        /// </summary>
        [SerializeField] private Vector3 _offset = Vector3.zero;
        /// <summary>
        /// Be careful, how to define this is important, I found if the coordinate is changed, just use left multiply will sick trouble.
        /// Result to that will rotate in a strange direction, when you rotate left, but the reaction is up? :(
        /// </summary>
        [SerializeField] private Quaternion _rotM = Quaternion.identity;

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
            visualTrans.SetLocalPositionAndRotation(_camPose.Item1 + _offset, _rotM * _camPose.Item2);
        }
    }
}