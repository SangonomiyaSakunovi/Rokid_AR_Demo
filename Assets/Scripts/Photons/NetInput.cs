using Fusion;
using UnityEngine;

namespace Photons
{
    internal struct NetInput : INetworkInput
    {
        public int PeerID;
        public int GrabableName;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
