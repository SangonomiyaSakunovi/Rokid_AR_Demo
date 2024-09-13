using Fusion;
using UnityEngine;

namespace Photons
{
    internal class Player : NetworkBehaviour
    {
        [SerializeField] private string _playerName = "Sango";

        public PlayerRef LocalPlayerRef { get; set; }

        [HideInInspector]
        [Networked] public int PeerID { get; set; }
        [HideInInspector]
        [Networked] public string Name { get; private set; }

        private InputManager _inputManager;

        public override void Spawned()
        {
            if (HasInputAuthority)
            {
                _inputManager = Runner.GetComponent<InputManager>();
                _inputManager.LocalPlayer = this;
                Name = PlayerPrefs.GetString("SangoProjects.PhotonPref.Username");

                RPC_SetPlayerName(Name);
            }
        }

        public override void FixedUpdateNetwork()
        {
            
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetPlayerName(string name)
        {
            Name = name;
        }
    }
}
