using Fusion;
using SangoUtils.PhotonFusionHelpers.FusionUIPanels;
using SangoUtils.PhotonFusionHelpers.FusionConnections;
using SangoUtils.PhotonFusionHelpers.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers.FusionCommons
{
    #region Bases
    public abstract class FusionUIPanelBase : FusionMonoBehaviour
    {
        protected static readonly int HideAnimHash = Animator.StringToHash("Hide");
        protected static readonly int ShowAnimHash = Animator.StringToHash("Show");

        /// <summary>
        /// Is modal flag must be set for overlay screens.
        /// </summary>
        [SerializeField] private bool _isModal;
        /// <summary>
        /// Is default active flag means this panel will keep alive when init.
        /// </summary>
        [SerializeField] private bool _isDefaultActive;
        /// <summary>
        /// The list of screen plugins for the screen. The actual plugin scripts can be distributed insde the UI hierarchy but must be liked here.
        /// </summary>
        [SerializeField] private List<FusionPanelPlugin> _plugins;

        private Animator _animator;
        /// <summary>
        /// The hide animation coroutine.
        /// </summary>
        private Coroutine _hideCoroutine;

        /// <summary>
        /// The list of screen plugins.
        /// </summary>
        public List<FusionPanelPlugin> Plugins => _plugins;
        /// <summary>
        /// Is modal property.
        /// </summary>
        public bool IsModal => _isModal;
        /// <summary>
        /// Is default active property.
        /// </summary>
        public bool IsDefaultActive => _isDefaultActive;
        /// <summary>
        /// Is the screen currently showing.
        /// </summary>
        public bool IsShowing { get; private set; }
        /// <summary>
        /// The menu config, assigned by the <see cref="IFusionMenuUIController"/>.
        /// </summary>
        public IFusionConnectConfig Config { get; set; }
        /// <summary>
        /// The menu connection object, The menu config, assigned by the <see cref="IFusionMenuUIController"/>.
        /// </summary>
        public IFusionConnection Connection { get; set; }
        /// <summary>
        /// The menu connection args.
        /// </summary>
        public IFusionConnectArgs ConnectionArgs { get; set; }
        /// <summary>
        /// The menu UI controller that owns this screen.
        /// </summary>
        public IFusionUIPanelManager UIManager { get; set; }

        /// <summary>
        /// Unity start method to find the animator.
        /// </summary>
        public virtual void Start()
        {
            TryGetComponent(out _animator);
        }

        /// <summary>
        /// Unit awake method to be overwritten by derived screens.
        /// </summary>
        public virtual void Awake()
        {
        }

        /// <summary>
        /// The screen init method is called during <see cref="FusionMenuUIController{T}.Awake()"/> after all screen have been assigned and configured.
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// The screen hide method.
        /// </summary>
        public virtual void Hide()
        {
            if (_animator)
            {
                if (_hideCoroutine != null)
                {
                    StopCoroutine(_hideCoroutine);
                }

                _hideCoroutine = StartCoroutine(HideAnimCoroutine());
                return;
            }

            IsShowing = false;

            foreach (var p in _plugins)
            {
                p.Hide(this);
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// The screen show method.
        /// </summary>
        public virtual void Show()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                if (_animator.gameObject.activeInHierarchy && _animator.HasState(0, ShowAnimHash))
                {
                    _animator.Play(ShowAnimHash, 0, 0);
                }
            }

            gameObject.SetActive(true);

            IsShowing = true;

            foreach (var p in _plugins)
            {
                p.Show(this);
            }
        }

        /// <summary>
        /// Play the hide animation wrapped in a coroutine.
        /// Forces the target framerate to 60 during the transition animations.
        /// </summary>
        /// <returns>When done</returns>
        private IEnumerator HideAnimCoroutine()
        {
#if UNITY_IOS || UNITY_ANDROID
            var changedFramerate = false;
            if (Config.AdaptFramerateForMobilePlatform)
            {
                if (Application.targetFrameRate < 60)
                {
                    Application.targetFrameRate = 60;
                    changedFramerate = true;
                }
            }
#endif

            _animator.Play(HideAnimHash);
            yield return null;
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }

#if UNITY_IOS || UNITY_ANDROID
            if (changedFramerate)
            {
                new FusionUIPanelGraphicsSettings().Apply();
            }
#endif

            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Classes
    public partial class FusionUIPanelGraphicsSettings
    {
        /// <summary>
        /// Available framerates.
        /// -1 = platform default
        /// </summary>
        protected static int[] PossibleFramerates = new int[] { -1, 30, 60, 75, 90, 120, 144, 165, 240, 360 };

        /// <summary>
        /// Target framerate
        /// </summary>
        public virtual int Framerate
        {
            get
            {
                var f = PlayerPrefs.GetInt("Photon.Menu.Framerate", -1);
                if (PossibleFramerates.Contains(f) == false)
                {
                    return PossibleFramerates[0];
                }
                return f;
            }
            set => PlayerPrefs.SetInt("Photon.Menu.Framerate", value);
        }

        /// <summary>
        /// Fullscreen mode.
        /// Is not shown for mobile platforms.
        /// </summary>
        public virtual bool Fullscreen
        {
            get => PlayerPrefs.GetInt("Photon.Menu.Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
            set => PlayerPrefs.SetInt("Photon.Menu.Fullscreen", value ? 1 : 0);
        }

        /// <summary>
        /// Selected resolution index based on Screen.resolutions.
        /// Is not shown for mobile platforms.
        /// </summary>
        public virtual int Resolution
        {
            get => Math.Clamp(PlayerPrefs.GetInt("Photon.Menu.Resolution", GetCurrentResolutionIndex()), 0, Math.Max(0, Screen.resolutions.Length - 1));
            set => PlayerPrefs.SetInt("Photon.Menu.Resolution", value);
        }

        /// <summary>
        /// Select VSync.
        /// </summary>
        public virtual bool VSync
        {
            get => PlayerPrefs.GetInt("Photon.Menu.VSync", Math.Clamp(QualitySettings.vSyncCount, 0, 1)) == 1;
            set => PlayerPrefs.SetInt("Photon.Menu.VSync", value ? 1 : 0);
        }

        /// <summary>
        /// Select Unity quality level index based on QualitySettings.names.
        /// </summary>
        public virtual int QualityLevel
        {
            get
            {
                var q = PlayerPrefs.GetInt("Photon.Menu.QualityLevel", QualitySettings.GetQualityLevel());
                q = Math.Clamp(q, 0, QualitySettings.names.Length - 1);
                return q;
            }
            set => PlayerPrefs.SetInt("Photon.Menu.QualityLevel", value);
        }

        /// <summary>
        /// Return a list of possible framerates filtered by Screen.currentResolution.refreshRate.
        /// </summary>
        public virtual List<int> CreateFramerateOptions => PossibleFramerates.Where(f => f <=
#if UNITY_2022_2_OR_NEWER
          (int)Math.Round(Screen.currentResolution.refreshRateRatio.value)
#else
  Screen.currentResolution.refreshRate
#endif
          ).ToList();

        /// <summary>
        /// Returns a list of resolution option indices based on Screen.resolutions.
        /// </summary>
        public virtual List<int> CreateResolutionOptions => Enumerable.Range(0, Screen.resolutions.Length).ToList();

        /// <summary>
        /// Returns a list of graphics quality indices based on QualitySettings.names.
        /// </summary>
        public virtual List<int> CreateGraphicsQualityOptions => Enumerable.Range(0, QualitySettings.names.Length).ToList();

        /// <summary>
        /// A partial method to be implemented on the SDK level.
        /// </summary>
        partial void ApplyUser();

        /// <summary>
        /// Applies all graphics settings.
        /// </summary>
        public virtual void Apply()
        {
#if !UNITY_IOS && !UNITY_ANDROID
      if (Screen.resolutions.Length > 0) {
        var resolution = Screen.resolutions[Resolution < 0 ? Screen.resolutions.Length - 1 : Resolution];
        if (Screen.currentResolution.width != resolution.width || 
          Screen.currentResolution.height != resolution.height ||
          Screen.fullScreen != Fullscreen) { 
          Screen.SetResolution(resolution.width, resolution.height, Fullscreen);
        }
      }
#endif

            if (QualitySettings.GetQualityLevel() != QualityLevel)
            {
                QualitySettings.SetQualityLevel(QualityLevel);
            }

            if (QualitySettings.vSyncCount != (VSync ? 1 : 0))
            {
                QualitySettings.vSyncCount = VSync ? 1 : 0;
            }

            if (Application.targetFrameRate != Framerate)
            {
                Application.targetFrameRate = Framerate;
            }

            ApplyUser();
        }

        /// <summary>
        /// Return the current selected resolution index based on Screen.resolutions.
        /// </summary>
        /// <returns>Index into Screen.resolutions</returns>
        private int GetCurrentResolutionIndex()
        {
            var resolutions = Screen.resolutions;
            if (resolutions == null || resolutions.Length == 0)
                return -1;

            int currentWidth = Mathf.RoundToInt(Screen.width);
            int currentHeight = Mathf.RoundToInt(Screen.height);
#if UNITY_2022_2_OR_NEWER
            var defaultRefreshRate = resolutions[^1].refreshRateRatio;
#else
      var defaultRefreshRate = resolutions[^1].refreshRate;
#endif

            for (int i = 0; i < resolutions.Length; i++)
            {
                var resolution = resolutions[i];

                if (resolution.width == currentWidth
                  && resolution.height == currentHeight
#if UNITY_2022_2_OR_NEWER
                  && resolution.refreshRateRatio.denominator == defaultRefreshRate.denominator
                  && resolution.refreshRateRatio.numerator == defaultRefreshRate.numerator)
#else
          && resolution.refreshRate == defaultRefreshRate)
#endif
                    return i;
            }

            return -1;
        }
    }

    public class FusionUIPanelManager<T> : FusionMonoBehaviour, IFusionUIPanelManager where T : IFusionConnectArgs, new()
    {
        [SerializeField] protected FusionConnectConfig _config;
        /// <summary>
        /// The connection wrapper.
        /// </summary>
        [SerializeField] protected FusionConnectionBehaviour _connection;
        /// <summary>
        /// The list of screens. The first one is the default screen shown on start.
        /// </summary>
        [SerializeField] protected FusionUIPanelBase[] _panels;

        /// <summary>
        /// A type to screen lookup to support <see cref="Get{S}()"/>
        /// </summary>
        protected Dictionary<Type, FusionUIPanelBase> _screenLookup;
        /// <summary>
        /// The popup handler is automatically set if present based on the interface <see cref="IFusionUIPanelPopup"/>.
        /// </summary>
        protected IFusionUIPanelPopup _popupHandler;
        /// <summary>
        /// The current active screen.
        /// </summary>
        protected FusionUIPanelBase _activeScreen;

        /// <summary>
        /// A factory to create SDK dependend derived connection args.
        /// </summary>
        public virtual T CreateConnectArgs => new T();

        /// <summary>
        /// Unity awake method. Populates internal structures based on the <see cref="_panels"/> list.
        /// </summary>
        protected virtual void Awake()
        {
            var connectionArgs = CreateConnectArgs;
            _screenLookup = new Dictionary<Type, FusionUIPanelBase>();

            foreach (var screen in _panels)
            {
                screen.Config = _config;
                screen.Connection = _connection;
                screen.ConnectionArgs = connectionArgs;
                screen.UIManager = this;

                var t = screen.GetType();
                while (true)
                {
                    _screenLookup.Add(t, screen);
                    if (t.BaseType == null || typeof(FusionUIPanelBase).IsAssignableFrom(t) == false || t.BaseType == typeof(FusionUIPanelBase))
                    {
                        break;
                    }

                    t = t.BaseType;
                }

                if (typeof(IFusionUIPanelPopup).IsAssignableFrom(t))
                {
                    _popupHandler = (IFusionUIPanelPopup)screen;
                }
            }

            foreach (var screen in _panels)
            {
                screen.Init();
            }
        }

        /// <summary>
        /// The Unity start method to enable the default screen.
        /// </summary>
        protected virtual void Start()
        {
            if (_panels != null && _panels.Length > 0)
            {
                // First screen is displayed by default
                _panels[0].Show();
                _activeScreen = _panels[0];
            }
        }

        /// <summary>
        /// Show a sreen will automaticall disable the current active screen and call animations.
        /// </summary>
        /// <typeparam name="S">Screen type</typeparam>
        public virtual void Show<S>() where S : FusionUIPanelBase
        {
            if (_screenLookup.TryGetValue(typeof(S), out var result))
            {
                if (result.IsModal == false && _activeScreen != result && _activeScreen)
                {
                    _activeScreen.Hide();
                }
                if (_activeScreen != result)
                {
                    result.Show();
                }
                if (result.IsModal == false)
                {
                    _activeScreen = result;
                }
            }
            else
            {
                Debug.LogError($"Show() - Screen type '{typeof(S).Name}' not found");
            }
        }

        /// <summary>
        /// Get a screen based on type.
        /// </summary>
        /// <typeparam name="S">Screen type</typeparam>
        /// <returns>Screen object</returns>
        public virtual S Get<S>() where S : FusionUIPanelBase
        {
            if (_screenLookup.TryGetValue(typeof(S), out var result))
            {
                return result as S;
            }
            else
            {
                Debug.LogError($"Show() - Screen type '{typeof(S).Name}' not found");
                return null;
            }
        }

        /// <summary>
        /// Show the popup/notification.
        /// </summary>
        /// <param name="msg">Popup message</param>
        /// <param name="header">Popup header</param>
        public void Popup(string msg, string header = default)
        {
            if (_popupHandler == null)
            {
                Debug.LogError("Popup() - no popup handler found");
            }
            else
            {
                _popupHandler.OpenPopup(msg, header);
            }
        }

        /// <summary>
        /// Show the popup but wait until it hides.
        /// </summary>
        /// <param name="msg">Popup message</param>
        /// <param name="header">Popup header</param>
        /// <returns>When the user clicked okay.</returns>
        public Task PopupAsync(string msg, string header = default)
        {
            if (_popupHandler == null)
            {
                Debug.LogError("Popup() - no popup handler found");
                return Task.CompletedTask;
            }
            else
            {
                return _popupHandler.OpenPopupAsync(msg, header);
            }
        }
    }
    #endregion

    #region Interfaces
    public interface IFusionUIPanelManager
    {
        /// <summary>
        /// Show a screen by type.
        /// </summary>
        /// <typeparam name="S">Screentype</typeparam>
        void Show<S>() where S : FusionUIPanelBase;
        /// <summary>
        /// Start a popup.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="header">Header</param>
        void Popup(string msg, string header = default);
        /// <summary>
        /// Start and async popup.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="header">Header</param>
        /// <returns>When the popup is closed.</returns>
        public Task PopupAsync(string msg, string header = default);
        /// <summary>
        /// Get a screen by type.
        /// </summary>
        /// <typeparam name="S">Screentype</typeparam>
        /// <returns></returns>
        S Get<S>() where S : FusionUIPanelBase;
    }

    public interface IFusionUIPanelPopup
    {
        /// <summary>
        /// Open screen with message.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="header">Header</param>
        void OpenPopup(string msg, string header);
        /// <summary>
        /// Open screen and wait until it closed.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="header">Header</param>
        /// <returns>When the screen is closed.</returns>
        Task OpenPopupAsync(string msg, string header);
    }
    #endregion
}
