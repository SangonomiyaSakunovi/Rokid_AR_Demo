using Rokid.UXR.Interaction;
using Rokid.UXR.Native;
using SangoProjects.RokidARDemo.Behaviours;
using SangoProjects.RokidARDemo.EventBus;
using SangoProjects.RokidARDemo.EventMessages;
using SangoUtils.UnityDevelopToolKits.Computes.Effects;
using SangoUtils.UnityDevelopToolKits.Timers;
using UnityEngine;

namespace SangoProjects.RokidARDemo.Controllers
{
    internal partial class HandGestureController : MonoBehaviour
    {
        private enum GestureListenModeCode
        {
            None,
            WaittingSubspaceSpinMoveToHand,
            WaittingSubspaceSpinDissolveOnHand
        }

        [field: SerializeField] private GameObject SubspaceObjRoot { get; set; }
        [field: SerializeField] private float WaittingSubspaceSpinMoveToHandDistanceLimit { get; set; }
        [field: SerializeField] private float IntervalBeforeGetHandPosition { get; set; }

        [field: SerializeField] private ThunderEffect ThunderEffect { get; set; }

        private GestureListenModeCode GestureListenMode { get; set; } = GestureListenModeCode.None;

        private void Awake()
        {
            SubspaceEventBus.Instance.OnRefocusAllDoneEvent.AddListener(OnRefocusAllDone);
            SubspaceEventBus.Instance.OnStartWaittingSubspaceSpinDissolveOnHandEvent.AddListener(OnStartWaittingSubspaceSpinDissolveOnHand);
        }

        private void Update()
        {
            switch (GestureListenMode)
            {
                case GestureListenModeCode.WaittingSubspaceSpinMoveToHand when SubspaceObjRoot != null:
                    OnWaittingSubspaceSpinMoveToHand();
                    break;
                case GestureListenModeCode.WaittingSubspaceSpinDissolveOnHand when SubspaceObjRoot != null:
                    OnWaittingSubspaceSpinDissolveOnHand();
                    break;
            }
        }

        #region Events
        private void OnRefocusAllDone(object sender, SubspaceEventArgs.OnRefocusAllDoneEventArgs eventArgs)
        {
            TimerAsyncOperation operation = UnityTimer.WaitForSecondsRealtime(IntervalBeforeGetHandPosition);
            operation.completed += delegate
            {
                GestureListenMode = GestureListenModeCode.WaittingSubspaceSpinMoveToHand;
            };
        }
        private void OnStartWaittingSubspaceSpinDissolveOnHand(object sender, SubspaceEventArgs.OnWaittingSubspaceSpinDissolveOnHandEventArgs eventArgs)
        {
            GestureListenMode = GestureListenModeCode.WaittingSubspaceSpinDissolveOnHand;
        }
        #endregion

        private void OnWaittingSubspaceSpinMoveToHand()
        {
            var getstureNum = NativeInterface.NativeAPI.GetTrackingHandNum();
            if (getstureNum == 0) return;

            var gestureType = GesEventInput.Instance.GetGestureType(HandType.RightHand);
            var gesturePos = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.WRIST, HandType.RightHand);
            float distance;

            if (gestureType != GestureType.Palm) return;

            for (int i = 0; i < SubspaceObjRoot.transform.childCount; i++)
            {
                Transform subspaceSpinTrans = SubspaceObjRoot.transform.GetChild(i);
                distance = Vector3.Distance(gesturePos.position, subspaceSpinTrans.position);
                if (distance < WaittingSubspaceSpinMoveToHandDistanceLimit)
                {
                    if (subspaceSpinTrans.TryGetComponent<SubspaceSpinMoveToHandBehaviour>(out var behaviour))
                    {
                        behaviour.OnSubspaceSpinMoveToHand();
                        ThunderEffect.SetTargetTrans(subspaceSpinTrans);
                        GestureListenMode = GestureListenModeCode.None;
                    }
                }
            }
        }

        private void OnWaittingSubspaceSpinDissolveOnHand()
        {
            var getstureNum = NativeInterface.NativeAPI.GetTrackingHandNum();
            if (getstureNum == 0) return;

            var gestureType = GesEventInput.Instance.GetGestureType(HandType.RightHand);
            if (gestureType == GestureType.Pinch)
            {
                for (int i = 0; i < SubspaceObjRoot.transform.childCount; i++)
                {
                    Transform subspaceSpinTrans = SubspaceObjRoot.transform.GetChild(i);
                    subspaceSpinTrans.gameObject.SetActive(false);
                    GestureListenMode = GestureListenModeCode.None;
                }
            }
        }
    }
}
