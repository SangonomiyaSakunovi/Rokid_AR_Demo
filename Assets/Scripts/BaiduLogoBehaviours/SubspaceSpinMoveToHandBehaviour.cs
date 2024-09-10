using Rokid.UXR.Interaction;
using Rokid.UXR.Native;
using SangoProjects.RokidARDemo.EventBus;
using SangoUtils.UnityDevelopToolKits.Computes.Lerps;
using UnityEngine;

namespace SangoProjects.RokidARDemo.Behaviours
{
    internal partial class SubspaceSpinMoveToHandBehaviour : MonoBehaviour
    {
        private enum MoveToHandBehaviourModeCode
        {
            None,
            Lerp
        }

        [field: SerializeField] private float Speed { get; set; }
        [field: SerializeField] private float LerpPrecision { get; set; }

        private MoveToHandBehaviourModeCode MoveToHandBehaviourMode { get; set; } = MoveToHandBehaviourModeCode.None;

        private Pose StartPose { get; set; } = Pose.identity;
        private Pose TargetPose { get; set; } = Pose.identity;

        private float ElapsedTime { get; set; } = 0;

        private void Update()
        {
            switch (MoveToHandBehaviourMode)
            {
                case MoveToHandBehaviourModeCode.Lerp:
                    UpdatePose();
                    break;
            }
        }

        #region Events
        public void OnSubspaceSpinMoveToHand()
        {
            StartPose = transform.GetPose();
            MoveToHandBehaviourMode = MoveToHandBehaviourModeCode.Lerp;
        }
        #endregion

        private void UpdatePose()
        {
            var getstureNum = NativeInterface.NativeAPI.GetTrackingHandNum();
            float distance;
            float lerpIndex;
            if (getstureNum > 0)
            {
                Pose handPose = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.WRIST, HandType.RightHand);
                TargetPose = new Pose(handPose.position + handPose.up * 0.1f, handPose.rotation);
            }

            distance = Vector3.Distance(transform.position, TargetPose.position);
            if (distance <= LerpPrecision)
            {
                MoveToHandBehaviourMode = MoveToHandBehaviourModeCode.None;
                ElapsedTime = 0;
                SubspaceEventBus.Instance.OnStartWaittingSubspaceSpinDissolveOnHandEvent?.Invoke(this, new EventMessages.SubspaceEventArgs.OnWaittingSubspaceSpinDissolveOnHandEventArgs());
            }
            else
            {
                lerpIndex = LerpAnim.GetSinDisplacementLerpIndex(distance, Speed, ElapsedTime);
                Pose resPos = Pose.identity;
                PoseUtils.Lerp(StartPose, TargetPose, lerpIndex, ref resPos);
                transform.SetPose(resPos);
                ElapsedTime += Time.deltaTime;
            }
        }
    }
}
