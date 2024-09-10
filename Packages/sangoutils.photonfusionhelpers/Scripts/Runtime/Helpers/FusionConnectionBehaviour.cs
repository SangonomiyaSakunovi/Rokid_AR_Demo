using Fusion;
using SangoUtils.PhotonFusionHelpers.ScriptableObjects;
using SangoUtils.PhotonFusionHelpers.FusionCommons;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.FusionConnections
{
    public class FusionConnectionBehaviour : FusionConnectionBehaviourBase
    {
        [SerializeField] private FusionConnectConfig config;
        [SerializeField] private NetworkRunner networkRunnerPrefab;

        private void Awake()
        {
            if (!config)
                Log.Error("Fusion configuration file not provided.");
        }

        public override IFusionConnection Create()
        {
            return new FusionConnection(config, networkRunnerPrefab);
        }
    }
}
