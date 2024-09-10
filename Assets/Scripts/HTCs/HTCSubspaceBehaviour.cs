using Fusion;
using Rokid.UXR.Interaction;
using Rokid.UXR.Native;
using SangoUtils.HTCViveTrackerHelpers;
using UnityEngine;

namespace SangoProjects.HTCBehaviours
{
    internal class HTCSubspaceBehaviour : NetworkBehaviour
    {
        private enum DeviceTypeCode
        {
            Rokid,
            Tower
        }

        [field: SerializeField] private Transform SubspaceParentTrans { get; set; }
        [field: SerializeField] private DeviceTypeCode DeviceType { get; set; } = DeviceTypeCode.Tower;
        [field: SerializeField] private TrackerDataReceiver TrackerDataReceiver { get; set; }

        [SerializeField] private float rokidRefocusSpaceCD = 5f;

        [Networked] private TickTimer RokidRefocusSpaceCD { get; set; }

        private (Vector3, Quaternion) _wristRealPose;

        public override void Spawned()
        {
            if (DeviceType == DeviceTypeCode.Rokid)
            {
                RokidRefocusSpaceCD = TickTimer.CreateFromSeconds(Runner, rokidRefocusSpaceCD);
                //OnHTCTrackerDataUpdate();
            }

            void OnHTCTrackerDataUpdate()
            {
                TrackerDataReceiver.OnTrackerDataUpdate += delegate (Vector3 pos, Quaternion rot)
                {
                    _wristRealPose.Item1 = pos;
                    _wristRealPose.Item2 = rot;
                };
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (DeviceType == DeviceTypeCode.Rokid)
            {
                if (RokidRefocusSpaceCD.ExpiredOrNotRunning(Runner))
                {
                    RokidRefocusSpace();
                    RokidRefocusSpaceCD = TickTimer.CreateFromSeconds(Runner, rokidRefocusSpaceCD);
                }
            }
        }

        private void RokidRefocusSpace()
        {
            if (NativeInterface.NativeAPI.GetTrackingHandNum() == 0
                || GesEventInput.Instance.GetGestureType(HandType.RightHand) == GestureType.None)
                return;

            Pose wirstSlamPose = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.WRIST, HandType.RightHand);

            if (wirstSlamPose == Pose.identity)
                return;

            RPC_RequestToTower(Runner.UserId, SubspaceParentTrans.position, SubspaceParentTrans.rotation, wirstSlamPose.position, wirstSlamPose.rotation);
        }

        /// <summary>
        /// To get pose in mapB
        /// If you do not know why, we suggest you`d better not modify it.
        /// </summary>
        private (Vector3, Quaternion) GetPoseInMapB(Vector3 positionInMapA, Quaternion rotationInMapA, Pose poseInMapA, Pose poseInMapB)
        {
            //Vector3 positionInMapB = transformB.position + transformB.rotation * (transformA.InverseTransformPoint(positionInMapA) - transformA.position);
            //Quaternion rotationInMapB = transformB.rotation * Quaternion.Inverse(transformA.rotation) * rotationInMapA;
            Vector3 positionInMapB = poseInMapB.position + poseInMapB.rotation * Quaternion.Inverse(poseInMapA.rotation) * (positionInMapA - poseInMapA.position);
            Quaternion rotationInMapB = poseInMapB.rotation * Quaternion.Inverse(poseInMapA.rotation) * rotationInMapA;
            return (positionInMapB, rotationInMapB);
        }
        /// <summary>
        /// To get pose in mapB
        /// If you do not know why, we suggest you`d better not modify it.
        /// </summary>
        private (Vector3, Quaternion) GetPoseInMapB(Vector3 positionInMapA, Quaternion rotationInMapA,
            Vector3 trackerPosInMapA, Quaternion trackerRotInMapA,
            Vector3 trackerPosInMapB, Quaternion trackerRotInMapB)
        {
            Vector3 positionInMapB = trackerPosInMapB + trackerRotInMapB * Quaternion.Inverse(trackerRotInMapA) * (positionInMapA - trackerPosInMapA);
            Quaternion rotationInMapB = trackerRotInMapB * Quaternion.Inverse(trackerRotInMapA) * rotationInMapA;
            return (positionInMapB, rotationInMapB);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_RequestToTower(string id, Vector3 subSpaceRootPos, Quaternion subSpaceRootRot, Vector3 trackerSlamPos, Quaternion trackerSlamRot)
        {
            if (DeviceType == DeviceTypeCode.Tower)
            {
                (Vector3, Quaternion) poseInMapB = GetPoseInMapB(subSpaceRootPos, subSpaceRootRot,
                    trackerSlamPos, trackerSlamRot,
                    _wristRealPose.Item1, _wristRealPose.Item2);

                RPC_ResponseToRokid(id, poseInMapB.Item1, poseInMapB.Item2);
            }
        }
        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_ResponseToRokid(string id, Vector3 subSpaceRootPos, Quaternion subSpaceRootRot)
        {
            if (DeviceType == DeviceTypeCode.Rokid && id.Equals(Runner.UserId))
            {
                SubspaceParentTrans.position = subSpaceRootPos;
                SubspaceParentTrans.rotation = subSpaceRootRot;
            }
        }
    }
}