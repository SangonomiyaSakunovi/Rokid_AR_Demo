using Rokid.UXR.Interaction;
using SangoProjects.RokidARDemo.EventBus;
using SangoProjects.RokidARDemo.EventMessages;
using SangoUtils.UnityDevelopToolKits.Timers;
using SangoUtils.UnitySourceGenerators.Generateds;
using UnityEngine;

namespace SangoUtils.UnityDevelopToolKits.Computes.Effects
{
    internal partial class ThunderEffect : MonoBehaviour
    {
        [UnityInspector]
        private GameObject ThunderEffectObj { get => _ThunderEffectObj; }
        [UnityInspector]
        private float IntervalBetweenEachChangeTargetPos { get => _IntervalBetweenEachChangeTargetPos; }

        private LineRenderer LineRenderer { get; set; }

        private Vector3 HandPos { get; set; }
        //private Vector3 TargetPos { get; set; }
        private Transform TargetTrans { get; set; } = null;

        private TimerAsyncOperation ChangeTargetPosTimer { get; set; }
        private Vector3 TargetPosBiasVec { get; set; } = Vector3.zero;

        private void Awake()
        {
            SubspaceEventBus.Instance.OnStartWaittingSubspaceSpinDissolveOnHandEvent.AddListener(OnStartWaittingSubspaceSpinDissolveOnHand);
        }

        private void Start()
        {
            AsyncInstantiateOperation<GameObject> operation = InstantiateAsync<GameObject>(ThunderEffectObj);
            operation.completed += delegate
            {
                foreach (var gameObject in operation.Result)
                {
                    if (gameObject.TryGetComponent<LineRenderer>(out var lineRenderer))
                    {
                        LineRenderer = lineRenderer;
                        LineRenderer.enabled = false;
                        break;
                    }
                }
            };
        }

        private void Update()
        {
            if (TargetTrans != null)
            {
                HandPos = GesEventInput.Instance.GetSkeletonPose(SkeletonIndexFlag.WRIST, HandType.RightHand).position;
                LineRenderer.SetPositions(new Vector3[] { HandPos, TargetTrans.position + TargetPosBiasVec });
            }
        }

        public void SetTargetTrans(Transform transform)
        {
            TargetTrans = transform;
            ChangeTargetPosTimer = UnityTimer.RepeatWaitForSecondsRealtime(IntervalBetweenEachChangeTargetPos);
            ChangeTargetPosTimer.completed += delegate
            {
                TargetPosBiasVec = Vector3.zero;
            };
            //LineRenderer.enabled = true;
        }

        public void RemoveTargetTrans()
        {
            LineRenderer.enabled = false;
            TargetTrans = null;
            ChangeTargetPosTimer.Cancel();
        }

        private void OnStartWaittingSubspaceSpinDissolveOnHand(object sender, SubspaceEventArgs.OnWaittingSubspaceSpinDissolveOnHandEventArgs eventArgs)
        {
            RemoveTargetTrans();
        }
    }
}
