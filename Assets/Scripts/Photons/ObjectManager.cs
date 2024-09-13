using Fusion;
using SangoUtils.UnityDevelopToolKits.Loggers;
using SceneGameObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Photons
{
    internal partial class ObjectManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _objGrabableParent;

        [SerializeField] private int _objGrabablePrefixCode;

        private Dictionary<int, GameObject> PrefabsDict { get; } = new();
        private Dictionary<int, GrabableObjectBehaviour> GrabableObjectBehavioursDict { get; } = new();

        public override void Spawned()
        {
            string strIndex = "";

            for (int i = 0; i < _objGrabableParent.transform.childCount; i++)
            {
                var item = _objGrabableParent.transform.GetChild(i);
                if (item.TryGetComponent<GrabableObjectBehaviour>(out var behaviour))
                {
                    if (i < 10)
                        strIndex = "00" + i;
                    else if (i < 100)
                        strIndex = "0" + i;
                    else if (i < 1000)
                        strIndex = i.ToString();
                    else
                    {
                        UnityLogger.Error("[Sango] The index overflow in " + nameof(ObjectManager));
                        break;
                    }

                    behaviour.ObjectID = int.Parse(_objGrabablePrefixCode + strIndex);
                    behaviour.GameObject = item.gameObject;
                    GrabableObjectBehavioursDict.Add(behaviour.ObjectID, behaviour);

                    if (behaviour.IsHideDefault)
                        behaviour.GameObject.SetActive(false);
                }
            }
        }

        public void SetActiveObj<T>(int name, bool isActive = true) where T : IObjBehaviour
        {
            if (typeof(T).Name == nameof(IGrabableObjBehaviour))
                RPC_SetActiveObj_IGrabableObj(name, isActive);
        }

        public void SyncPose(int name, Vector3 position, Quaternion rotation)
        {
            RPC_SyncPose(name, position, rotation);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_SetActiveObj_IGrabableObj(int name, bool isActive = true)
        {
            if (GrabableObjectBehavioursDict.TryGetValue(name, out var behaviour0))
            {
                behaviour0.gameObject.SetActive(isActive);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_SyncPose(int name ,Vector3 position, Quaternion rotation)
        {
            if (GrabableObjectBehavioursDict.TryGetValue(name, out var behaviour0))
            {
                Debug.Log("SyncPoseRPC");
                behaviour0.SyncPose(position,rotation);
            }
        }
    }
}
