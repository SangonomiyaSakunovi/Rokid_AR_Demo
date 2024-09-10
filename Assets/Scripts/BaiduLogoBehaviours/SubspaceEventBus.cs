using SangoProjects.RokidARDemo.EventMessages;
using SangoUtils.UnitySourceGenerators.Generateds;
using UnityEngine;
using UnityEngine.Events;

namespace SangoProjects.RokidARDemo.EventBus
{
    [UnityInstance]
    internal partial class SubspaceEventBus : MonoBehaviour
    {
        public UnityEvent<object, SubspaceEventArgs.OnRefocusAllDoneEventArgs> OnRefocusAllDoneEvent { get; set; } = new();
        public UnityEvent<object, SubspaceEventArgs.OnWaittingSubspaceSpinDissolveOnHandEventArgs> OnStartWaittingSubspaceSpinDissolveOnHandEvent { get; set; } = new();
    }
}
