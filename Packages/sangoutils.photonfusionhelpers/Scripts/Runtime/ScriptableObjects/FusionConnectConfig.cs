using Fusion;
using Fusion.Photon.Realtime;
using SangoUtils.PhotonFusionHelpers.FusionCommons;
using System.Collections.Generic;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "SangoUtils/Fusions/Fusion Connect Config")]
    public class FusionConnectConfig : FusionScriptableObject, IFusionConnectConfig
    {
        [Header("AppConfigs")]
        [SerializeField] protected List<string> _availableAppVersions;
        [SerializeField] protected List<string> _availableRegions;
        [SerializeField] protected List<FusionSceneInfo> _availableScenes;
        [SerializeField] protected GameMode _gameMode;

        [Header("SessionConfigs")]
        [SerializeField] protected FusionSessionID _sessionID;
        [SerializeField] protected int _sessionMaxPlayers = 6;
        [SerializeField] protected bool _isSessionVisible; //Default: true
        [SerializeField] protected bool _isEnabaleClientCreateNewSession; //Default: false
        [SerializeField] protected MatchmakingMode _sessionMatchmakingMode; //Default: fillRoom

        [Header("OtherConfigs")]
        [SerializeField] protected FusionMachineID _machineID;
        [SerializeField] protected bool _adaptFramerateForMobilePlatform = true;
        [SerializeField] protected FusionPartyCodeGenerator _codeGenerator;

        public List<string> AvailableAppVersions => _availableAppVersions;
        public List<string> AvailableRegions => _availableRegions;
        public List<FusionSceneInfo> AvailableScenes => _availableScenes;
        public int SessionMaxPlayerCount => _sessionMaxPlayers;
        public virtual string MachineID => _machineID?.ID;
        public virtual string SessionID => _sessionID?.ID;
        public bool IsSessionVisible => _isSessionVisible;
        public bool IsEnableClientCreateNewSession => _isEnabaleClientCreateNewSession;
        public MatchmakingMode SessionMatchmakingMode => _sessionMatchmakingMode;
        public GameMode GameMode => _gameMode;
        public FusionPartyCodeGenerator CodeGenerator => _codeGenerator;
        public bool AdaptFramerateForMobilePlatform => _adaptFramerateForMobilePlatform;
    }
}

