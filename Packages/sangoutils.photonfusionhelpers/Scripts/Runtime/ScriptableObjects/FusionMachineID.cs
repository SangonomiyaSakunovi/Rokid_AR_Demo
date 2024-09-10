using Fusion;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "SangoUtils/Fusions/Fusion Machine ID")]
    public class FusionMachineID : FusionScriptableObject
    {
        [SerializeField] private string _ID = "";

        public string ID => _ID;

        public FusionMachineID()
        {
            _ID = FusionPartyCodeGenerator.Create(8, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789");
        }
    }
}
