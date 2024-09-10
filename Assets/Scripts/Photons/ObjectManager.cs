using Fusion;
using SceneGameObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Photons
{
    internal class ObjectManager : NetworkBehaviour
    {
        [SerializeField] private Transform _npcParentTrans;

        [SerializeField] private GameObject[] _objPrefabs;
        [SerializeField] private GameObject[] _objEnvs;

        private Dictionary<string, GameObject> PrefabsDict { get; } = new();
        [Networked] private NetworkDictionary<string, GrabableObjectBehaviour> GrabableObjectBehavioursDict => default;
        private Dictionary<string, EnvObjectBehaviour> EnvObjectBehavioursDict { get; } = new();

        public override void Spawned()
        {
            foreach (var item0 in _objPrefabs)
                PrefabsDict.Add(item0.name, item0);

            foreach (var item1 in _objEnvs)
            {
                if (item1.TryGetComponent<EnvObjectBehaviour>(out var behaviour))
                {
                    behaviour.ObjectID = item1.name;
                    behaviour.GameObject = item1.gameObject;
                    EnvObjectBehavioursDict.Add(behaviour.ObjectID, behaviour);

                    if (behaviour.IsHideDefault)
                        behaviour.GameObject.SetActive(false);
                }
            }
        }

        public void InstantiateObjAsyc<T>(string name, string ID, Vector3 position, Quaternion rotation) where T : IObjBehaviour
        {
            if (typeof(T).Name == nameof(IGrabableObjBehaviour))
                RPC_InstantiateObjAsyc_IGrabableObj(name, ID, position, rotation);
        }

        public void SetActiveObj<T>(string name, bool isActive = true) where T : IObjBehaviour
        {
            if (typeof(T).Name == nameof(IEnvObjBehaviour))
                RPC_SetActiveObj_IEnvObj(name, isActive);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_SetActiveObj_IEnvObj(string name, bool isActive = true)
        {
            if (EnvObjectBehavioursDict.TryGetValue(name, out var behaviour0))
            {
                behaviour0.gameObject.SetActive(isActive);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RPC_InstantiateObjAsyc_IGrabableObj(string name, string ID, Vector3 position, Quaternion rotation)
        {
            if (HasStateAuthority)
            {
                if (PrefabsDict.TryGetValue(name, out var obj))
                {
                    NetworkObject networkObj = Runner.Spawn(obj, position, rotation);
                    if (networkObj.TryGetComponent<GrabableObjectBehaviour>(out var behaviour))
                    {
                        networkObj.transform.SetParent(_npcParentTrans);
                        behaviour.ObjectID = ID;
                        behaviour.GameObject = networkObj.gameObject;
                        GrabableObjectBehavioursDict.Add(behaviour.ObjectID, behaviour);
                    }
                }
            }
        }
    }
}
