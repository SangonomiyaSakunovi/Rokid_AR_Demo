using Fusion;
using UnityEngine;

namespace Photons
{
    internal class Player : NetworkBehaviour
    {
        [SerializeField] private string _playerName = "Sango";

        [HideInInspector]
        [Networked] public string Name { get; private set; }

        private InputManager _inputManager;

        public override void Spawned()
        {
            if (HasInputAuthority)
            {
                _inputManager = Runner.GetComponent<InputManager>();
                _inputManager.LocalPlayer = this;
                Name = "Sango";
                RPC_SetPlayerName(Name);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetPlayerName(string name)
        {
            Name = name;
        }
    }
}
