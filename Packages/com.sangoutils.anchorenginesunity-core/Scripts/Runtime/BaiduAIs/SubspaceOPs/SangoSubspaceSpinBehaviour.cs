using SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.SubspaceOPs
{
    internal class SangoSubspaceSpinBehaviour : MonoBehaviour
    {
        public void OnUnSelect()
        {
            OnMoveSpin();
        }

        public void OnDropDown()
        {
            OnMoveSpin();
        }

        private void OnMoveSpin()
        {
            BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("On Spin OP!");
            BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinAdjustmentedEvent.Invoke(this, new BaiduAI_LogoRecognitionsMessages.SubspaceSpinAdjustmentEvtArgs(gameObject));
        }
    }
}
