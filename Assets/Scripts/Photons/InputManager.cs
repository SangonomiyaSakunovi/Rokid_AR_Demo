using Fusion;
using Fusion.Sockets;
using PhotonUIPanels;
using SangoUtils.PhotonFusionHelpers.FusionCommons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photons
{
    internal class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
    {
        private Player _localPlayer;
        private NetInput _accumulatedInput;

        public Player LocalPlayer
        {
            get => _localPlayer;
            internal set
            {
                _localPlayer = value;
            }
        }

        public void BeforeUpdate()
        {
            if(_localPlayer == null)
                return;
        }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public async void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic)
            {
                await FindFirstObjectByType<FusionConnectionBehaviourBase>(FindObjectsInactive.Include).DisconnectAsync(ConnectFailReason.Disconnect);
                FindFirstObjectByType<FusionUIPanel_Game>(FindObjectsInactive.Include).UIManager.Show<FusionUIPanel_Main>();
            }
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    }
}
