using Fusion;
using UnityEngine;

namespace SceneGameObjects
{
    internal interface IObjBehaviour
    {
        string ObjectID { get; set; }
        GameObject GameObject { get; set; }
    }

    internal interface IGrabableObjBehaviour : IObjBehaviour
    {

    }

    internal interface INPCObjBehaviour : IObjBehaviour
    {

    }

    internal interface IEnvObjBehaviour : IObjBehaviour
    {
        bool IsHideDefault { get; set; }
    }
}
