using SangoUtils.PhotonFusionHelpers.FusionCommons;

namespace PhotonUIPanels
{
    public class FusionUIPanelManager : FusionUIPanelManager<FusionConnectArgs>
    {
        protected override void Awake()
        {
            base.Awake();

            InitAllPanels();
        }

        private void InitAllPanels()
        {
            foreach (var panel in _panels)
            {
                if (panel.IsDefaultActive)
                    panel.gameObject.SetActive(true);
                else
                    panel.gameObject.SetActive(false);
            }
        }
    }
}
