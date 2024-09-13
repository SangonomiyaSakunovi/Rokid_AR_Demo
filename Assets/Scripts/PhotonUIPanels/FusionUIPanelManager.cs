using SangoUtils.PhotonFusionHelpers.FusionCommons;
using UnityEngine;

namespace PhotonUIPanels
{
    public class FusionUIPanelManager : FusionUIPanelManager<FusionConnectArgs>
    {
        [SerializeField] private GameObject _canvasRoot;

        public static FusionUIPanelManager Instance;
        
        protected override void Awake()
        {
            base.Awake();

            Instance = this;

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

        public void HideCanvas()
        {
            _canvasRoot.gameObject.SetActive(false);
        }
    }
}
