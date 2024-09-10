using SangoUtils.PhotonFusionHelpers.FusionCommons;
using SangoUtils.UnityDevelopToolKits.Loggers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonUIPanels
{
    internal class FusionUIPanel_PopUp : FusionUIPanelBase, IFusionUIPanelPopup
    {
        [SerializeField] protected Button _btn_ReConnect;

        /// <summary>
        /// The completion source will be triggered when the screen has been hidden.
        /// </summary>
        protected TaskCompletionSource<bool> _taskCompletionSource;

        public void OpenPopup(string msg, string header)
        {
            Show();
        }

        public Task OpenPopupAsync(string msg, string header)
        {
            _taskCompletionSource?.TrySetResult(true);
            _taskCompletionSource = new TaskCompletionSource<bool>();

            OpenPopup(msg, header);

            return _taskCompletionSource.Task;
        }

        /// <summary>
        /// Is called when the <see cref="_playButton"/> is pressed using SendMessage() from the UI object.
        /// Intitiates the connection and expects the connection object to set further screen states.
        /// </summary>
        protected virtual async void OnConnectButtonPressed()
        {
            UnityLogger.Color(LoggerColor.Cyan, "Connecting to Fusion, please wait.");

            ConnectionArgs.Session = null;
            ConnectionArgs.Creating = false;
            ConnectionArgs.Region = ConnectionArgs.PreferredRegion;

            Hide();
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
