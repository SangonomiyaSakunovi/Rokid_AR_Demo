using Fusion;
using SangoUtils.PhotonFusionHelpers.FusionCommons;
using System.Collections.Generic;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "SangoUtils/Fusions/Fusion Connect Config")]
    public class FusionConnectConfig : FusionScriptableObject, IFusionConnectConfig
    {
        [SerializeField] protected int _maxPlayers = 6;
        [SerializeField] protected bool _adaptFramerateForMobilePlatform = true;
        [SerializeField] protected List<string> _availableAppVersions;
        [SerializeField] protected List<string> _availableRegions;
        [SerializeField] protected List<FusionSceneInfo> _availableScenes;
        [SerializeField] protected FusionMachineID _machineID;
        [SerializeField] protected FusionSessionID _sessionID;
        [SerializeField] protected FusionPartyCodeGenerator _codeGenerator;

        public List<string> AvailableAppVersions => _availableAppVersions;
        public List<string> AvailableRegions => _availableRegions;
        public List<FusionSceneInfo> AvailableScenes => _availableScenes;
        public int MaxPlayerCount => _maxPlayers;
        public virtual string MachineID => _machineID?.ID;
        public virtual string SessionID => _sessionID?.ID;
        public FusionPartyCodeGenerator CodeGenerator => _codeGenerator;
        public bool AdaptFramerateForMobilePlatform => _adaptFramerateForMobilePlatform;
    }
}

