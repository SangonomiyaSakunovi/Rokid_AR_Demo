using Fusion;
using SangoUtils.PhotonFusionHelpers.FusionCommons;

namespace SangoUtils.PhotonFusionHelpers.FusionUIPanels
{
    public class FusionPanelPlugin : FusionMonoBehaviour
    {
        /// <summary>
        /// The parent screen is shown.
        /// </summary>
        /// <param name="screen">Parent screen</param>
        public virtual void Show(FusionUIPanelBase screen)
        {
        }

        /// <summary>
        /// The parent screen is hidden.
        /// </summary>
        /// <param name="screen">Parent screen</param>
        public virtual void Hide(FusionUIPanelBase screen)
        {
        }
    }
}
