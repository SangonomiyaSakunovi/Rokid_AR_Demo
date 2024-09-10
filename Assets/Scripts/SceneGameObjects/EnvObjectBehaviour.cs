using Fusion;
using UnityEngine;

namespace SceneGameObjects
{
    internal class EnvObjectBehaviour : NetworkBehaviour, IEnvObjBehaviour
    {
        public string ObjectID { get; set; }
        public GameObject GameObject { get; set; }

        [field: SerializeField] public bool IsHideDefault { get; set; }
    }
}
