using Fusion;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.ScriptableObjects
{
    [CreateAssetMenu(menuName = "SangoUtils/Fusions/Fusion Session ID")]
    public class FusionSessionID : FusionScriptableObject
    {
        [SerializeField] private string _ID = "";
        
        public string ID => _ID;

        public FusionSessionID()
        {
            _ID = FusionPartyCodeGenerator.Create(8, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789");
        }
    }
}
