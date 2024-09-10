using Fusion;
using Fusion.Photon.Realtime;
using SangoUtils.PhotonFusionHelpers.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.FusionCommons
{
    #region Bases
    public abstract class FusionConnectionBehaviourBase : FusionMonoBehaviour, IFusionConnection
    {
        public IFusionConnection Connection;
        public string SessionName => Connection.SessionName;
        public bool IsVisible => Connection.IsVisible;
        public bool EnableClientSessionCreation => Connection.EnableClientSessionCreation;
        public MatchmakingMode MatchmakingMode => Connection.MatchmakingMode;
        public int MaxPlayerCount => Connection.MaxPlayerCount;
        public string Region => Connection.Region;
        public string AppVersion => Connection.AppVersion;
        public List<string> Usernames => Connection.Usernames;
        public bool IsConnected => Connection != null && Connection.IsConnected;
        public int Ping => Connection.Ping;

        public Action<IFusionConnectArgs> OnBeforeConnect;
        public Action<IFusionConnection, int> OnBeforeDisconnect;

        public abstract IFusionConnection Create();
        public virtual Task<ConnectResult> ConnectAsync(IFusionConnectArgs connectionArgs)
        {
            if (Connection == null)
            {
                Connection = Create();
            }

            if (OnBeforeConnect != null)
            {
                try
                {
                    OnBeforeConnect.Invoke(connectionArgs);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                    return Task.FromResult(new ConnectResult { FailReason = ConnectFailReason.Disconnect, DebugMessage = e.Message });
                }
            }

            return Connection.ConnectAsync(connectionArgs);
        }

        public virtual Task DisconnectAsync(int reason)
        {
            if (Connection != null)
            {
                if (OnBeforeDisconnect != null)
                {
                    try
                    {
                        OnBeforeDisconnect.Invoke(Connection, reason);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogException(e);
                    }
                }

                return Connection.DisconnectAsync(reason);
            }

            return Task.CompletedTask;
        }

        public virtual Task<List<FusionOnlineRegion>> RequestAvailableOnlineRegionsAsync(IFusionConnectArgs connectionArgs)
        {
            if (Connection == null)
            {
                Connection = Create();
            }

            return Connection.RequestAvailableOnlineRegionsAsync(connectionArgs);
        }
    }
    #endregion

    #region Classes
    public partial class FusionConnectArgs : IFusionConnectArgs
    {
        private const string PrefPrefix = "SangoProjects.PhotonPref.";

        public virtual string Username
        {
            get => PlayerPrefs.GetString(PrefPrefix + "Username");
            set => PlayerPrefs.SetString(PrefPrefix + "Username", value);
        }

        /// <summary>
        /// The session that the client wants to join. Is not persisted. Use ReconnectionInformation instead to recover it between application shutdowns.
        /// </summary>
        public virtual string Session { get; set; }
        public virtual bool IsVisible { get; set; }
        public virtual bool EnableClientSessionCreation { get; set; }
        public virtual MatchmakingMode MatchmakingMode { get; set; }

        /// <summary>
        /// The preferred region the user selected in the menu.
        /// </summary>
        public virtual string PreferredRegion
        {
            get => PlayerPrefs.GetString(PrefPrefix + "Region");
            set => PlayerPrefs.SetString(PrefPrefix + "Region", string.IsNullOrEmpty(value) ? value : value.ToLower());
        }

        /// <summary>
        /// The actual region that the client will connect to.
        /// </summary>
        public virtual string Region { get; set; }

        /// <summary>
        /// The app version used for the Photon connection.
        /// </summary>
        public virtual string AppVersion
        {
            get => PlayerPrefs.GetString(PrefPrefix + "AppVersion");
            set => PlayerPrefs.SetString(PrefPrefix + "AppVersion", value);
        }

        /// <summary>
        /// The max player count that the user selected in the menu.
        /// </summary>
        public virtual int MaxPlayerCount
        {
            get => PlayerPrefs.GetInt(PrefPrefix + "MaxPlayerCount");
            set => PlayerPrefs.SetInt(PrefPrefix + "MaxPlayerCount", value);
        }

        /// <summary>
        /// The map or scene information that the user selected in the menu.
        /// </summary>
        public virtual FusionSceneInfo Scene
        {
            get
            {
                try
                {
                    return JsonUtility.FromJson<FusionSceneInfo>(PlayerPrefs.GetString(PrefPrefix + "Scene"));
                }
                catch
                {
                    return default(FusionSceneInfo);
                }
            }
            set => PlayerPrefs.SetString(PrefPrefix + "Scene", JsonUtility.ToJson(value));
        }

        /// <summary>
        /// Toggle to create or join-only game sessions/rooms.
        /// </summary>
        public virtual GameMode GameMode { get; set; }

        /// <summary>
        /// Partial method to expand defaults to SDK variations.
        /// </summary>
        /// <param name="config"></param>
        partial void SetDefaultsUser(IFusionConnectConfig config);

        /// <summary>
        /// Make sure that all configuration have a default settings.
        /// </summary>
        /// <param name="config">The menu config.</param>
        public virtual void SetDefaults(IFusionConnectConfig config)
        {
            GameMode = config.GameMode;

            if (string.IsNullOrEmpty(config.SessionID))
            {
                Session = null;
            }
            else
            {
                Session = config.SessionID;
            }

            IsVisible = config.IsSessionVisible;
            EnableClientSessionCreation = config.IsEnableClientCreateNewSession;
            MatchmakingMode = config.SessionMatchmakingMode;

            if (string.IsNullOrEmpty(AppVersion))
            {
                AppVersion = config.AvailableAppVersions[0];
            }

            if (PreferredRegion != null && config.AvailableRegions.Contains(PreferredRegion) == false)
            {
                PreferredRegion = string.Empty;
            }

            if (MaxPlayerCount <= 0 || MaxPlayerCount > config.SessionMaxPlayerCount)
            {
                MaxPlayerCount = config.SessionMaxPlayerCount;
            }

            if (string.IsNullOrEmpty(Username))
            {
                Username = $"Player{config.CodeGenerator.Create(3)}";
            }

            if (string.IsNullOrEmpty(Scene.Name) && config.AvailableScenes.Count > 0)
            {
                Scene = config.AvailableScenes[0];
            }
            else
            {
                var index = config.AvailableScenes.FindIndex(s => s.Name == Scene.Name);
                if (index >= 0)
                {
                    // Overwrite anything in storage with fresh information from the config
                    Scene = config.AvailableScenes[Mathf.Clamp(index, 0, config.AvailableScenes.Count - 1)];
                }
            }

            SetDefaultsUser(config);
        }
    }

    public class ConnectResult
    {
        public bool Success;
        public int FailReason;
        public int DisconnectCause;
        public string DebugMessage;
        public bool CustomResultHandling;
        public Task WaitForCleanup;
    }

    public class ConnectFailReason
    {
        public const int None = 0;
        public const int UserRequest = 1;
        public const int ApplicationQuit = 2;
        public const int Disconnect = 3;
    }
    #endregion

    #region Interfaces
    public interface IFusionConnection
    {
        string SessionName { get; }
        bool IsVisible { get; }
        bool EnableClientSessionCreation { get; }
        MatchmakingMode MatchmakingMode { get; }
        int MaxPlayerCount { get; }
        string Region { get; }
        string AppVersion { get; }
        List<string> Usernames { get; }
        bool IsConnected { get; }
        public int Ping { get; }
        Task<ConnectResult> ConnectAsync(IFusionConnectArgs connectArgs);
        Task DisconnectAsync(int reason);
        Task<List<FusionOnlineRegion>> RequestAvailableOnlineRegionsAsync(IFusionConnectArgs connectArgs);
    }

    public interface IFusionConnectArgs
    {
        string Username { get; set; }
        string Session { get; set; }
        bool IsVisible { get; set; }
        bool EnableClientSessionCreation { get; set; }
        MatchmakingMode MatchmakingMode { get; set; }
        string PreferredRegion { get; set; }
        string Region { get; set; }
        string AppVersion { get; set; }
        int MaxPlayerCount { get; set; }
        FusionSceneInfo Scene { get; set; }
        GameMode GameMode { get; set; }
        void SetDefaults(IFusionConnectConfig config);
    }

    public interface IFusionConnectConfig
    {
        List<string> AvailableAppVersions { get; }
        List<string> AvailableRegions { get; }
        List<FusionSceneInfo> AvailableScenes { get; }
        int SessionMaxPlayerCount { get; }
        string MachineID { get; }
        string SessionID { get; }
        bool IsSessionVisible { get; }
        bool IsEnableClientCreateNewSession { get; }
        MatchmakingMode SessionMatchmakingMode { get; }
        GameMode GameMode { get; }
        FusionPartyCodeGenerator CodeGenerator { get; }
        bool AdaptFramerateForMobilePlatform { get; }
    }
    #endregion

    #region Structs
    [Serializable]
    public struct FusionSceneInfo
    {
        public string Name;
        [ScenePath] public string ScenePath;

        public string SceneName => ScenePath == null ? null : Path.GetFileNameWithoutExtension(ScenePath);
        public Sprite Preview;
    }

    [Serializable]
    public struct FusionOnlineRegion
    {
        public string Code;
        public int Ping;
    }
    #endregion
}