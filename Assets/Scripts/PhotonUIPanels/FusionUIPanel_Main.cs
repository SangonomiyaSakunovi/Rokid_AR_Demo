using SangoUtils.PhotonFusionHelpers.FusionCommons;
using SangoUtils.UnityDevelopToolKits.Loggers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonUIPanels
{
    internal partial class FusionUIPanel_Main : FusionUIPanelBase
    {
        [SerializeField] protected Button _btn_Connect;

        partial void AwakeUser();
        partial void InitUser();
        partial void ShowUser();
        partial void HideUser();

        /// <summary>
        /// The Unity awake method. Calls partial method <see cref="AwakeUser"/> to be implemented on the SDK side.
        /// Applies the current selected graphics settings (loaded from PlayerPrefs)
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            new FusionUIPanelGraphicsSettings().Apply();

            AwakeUser();
        }

        /// <summary>
        /// The screen init method. Calls partial method <see cref="InitUser"/> to be implemented on the SDK side.
        /// Initialized the default arguments.
        /// </summary>
        public override void Init()
        {
            base.Init();

            ConnectionArgs.SetDefaults(Config);

            InitUser();
        }

        /// <summary>
        /// The screen show method. Calls partial method <see cref="ShowUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Show()
        {
            base.Show();

            if (string.IsNullOrEmpty(ConnectionArgs.Scene.Name))
            {
                _btn_Connect.interactable = false;
                Debug.LogWarning("No game scene to start found.");
            }

            ShowUser();
        }

        /// <summary>
        /// The screen hide method. Calls partial method <see cref="HideUser"/> to be implemented on the SDK side.
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            HideUser();
        }

        /// <summary>
        /// Is called when the <see cref="_playButton"/> is pressed using SendMessage() from the UI object.
        /// Intitiates the connection and expects the connection object to set further screen states.
        /// </summary>
        protected virtual async void OnConnectButtonPressed()
        {
            UnityLogger.Color(LoggerColor.Cyan, "Connecting to Fusion, please wait.");

            ConnectionArgs.Region = ConnectionArgs.PreferredRegion;

            UIManager.Show<FusionUIPanel_Loading>();

            var result = await Connection.ConnectAsync(ConnectionArgs);

            await HandleConnectionResult(result, this.UIManager);
        }

        /// <summary>
        /// Default connection error handling is reused in a couple places.
        /// </summary>
        /// <param name="result">Connect result</param>
        /// <param name="manager">UI Controller</param>
        /// <returns>When handling is completed</returns>
        public static async Task HandleConnectionResult(ConnectResult result, IFusionUIPanelManager manager)
        {
            if (result.CustomResultHandling == false)
            {
                if (result.Success)
                {
                    UnityLogger.Color(LoggerColor.Green, "Server Connected!");

                    manager.Show<FusionUIPanel_Game>();
                }
                else if (result.FailReason != ConnectFailReason.ApplicationQuit)
                {
                    UnityLogger.Color(LoggerColor.Red, $"Server Connect Failed! Code: [{result.FailReason}].", true);

                    var popup = manager.PopupAsync(result.DebugMessage, "Connection Failed");
                    if (result.WaitForCleanup != null)
                    {
                        await Task.WhenAll(result.WaitForCleanup, popup);
                    }
                    else
                    {
                        await popup;
                    }
                }
            }
        }
    }
}
