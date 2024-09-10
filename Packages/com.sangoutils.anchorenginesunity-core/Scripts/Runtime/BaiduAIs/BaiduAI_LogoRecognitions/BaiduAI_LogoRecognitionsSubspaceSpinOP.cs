using SangoUtils.AnchorEngines_Unity.Core.SubspaceOPs;
using SangoUtils.AnchorEngines_Unity.Core.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace SangoUtils.AnchorEngines_Unity.Core.BaiduAIs.LogoRecognitions
{
    internal class BaiduAI_LogoRecognitionsSubspaceSpinOP : MonoBehaviour
    {
        private enum InstantiateModeCode { DirectMode, ExactMode }
        private enum DirectionCode { Default, Forward, Back, Left, Right }

        private class InstantiatePose
        {
            public static InstantiatePose DirectForward { get; } = new(DirectionCode.Forward);
            public static InstantiatePose DirectBack { get; } = new(DirectionCode.Back);
            public static InstantiatePose DirectLeft { get; } = new(DirectionCode.Left);
            public static InstantiatePose DirectRight { get; } = new(DirectionCode.Right);

            private InstantiatePose(DirectionCode direction)
            {
                InstantiateMode = InstantiateModeCode.DirectMode;
                DirectionCode = direction;
            }
            public InstantiatePose(Vector3 position)
            {
                InstantiateMode = InstantiateModeCode.ExactMode;
                Position = position;
            }
            public InstantiatePose()
            {
                InstantiateMode = InstantiateModeCode.ExactMode;
            }

            public InstantiateModeCode InstantiateMode { get; } = InstantiateModeCode.DirectMode;
            public DirectionCode DirectionCode { get; } = DirectionCode.Forward;
            public Vector3 Position { get; set; } = Vector3.zero;
        }

        private Transform _speceSpinsParentTrans;
        private BaiduAI_LogoRecognitionsUploadRequest _request;

        private readonly Dictionary<string, string> _logoNamesToPrefabNames = new();
        private readonly ConcurrentDictionary<string, GameObject> _logoNamesToObjects = new();

        private uint _updateRefocusSpinTaskID = 0;
        private uint _updateRefocusSpinTimeLimitTaskID = 0;

        private BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode _currentMode = BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.WaitingMode;

        private Vector3 _defaultViewCenterObjectForward = Vector3.zero;

        private TimerAsyncOperation _uploadCameraTextureOperation;
        private TimerAsyncOperation _uploadCameraTextureTimeLimiteOperation;

        private void Awake()
        {
            BaiduAI_LogoRecognitionsConfig config = BaiduAI_LogoRecognitionsEventBus.Config;
            _request = gameObject.AddComponent<BaiduAI_LogoRecognitionsUploadRequest>();

            _speceSpinsParentTrans = BaiduAI_LogoRecognitionsEventBus.Config.SubspaceSpinsRootObject.transform;
            _defaultViewCenterObjectForward = BaiduAI_LogoRecognitionsEventBus.Config.ViewCenterObject.transform.forward;

            _logoNamesToPrefabNames.Add(config.XPositiveLogoName, nameof(config.XPositiveLogoName));
            _logoNamesToPrefabNames.Add(config.XNegativeLogoName, nameof(config.XNegativeLogoName));
            _logoNamesToPrefabNames.Add(config.ZPositiveLogoName, nameof(config.ZPositiveLogoName));
            _logoNamesToPrefabNames.Add(config.ZNegativeLogoName, nameof(config.ZNegativeLogoName));

            BaiduAI_LogoRecognitionsEventBus.OnAutoRefocusFoundedEvent.AddListener(OnAutoRefocusFounded);
            BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinedEvent.AddListener(OnSubspaceSpined);
            BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinAdjustmentedEvent.AddListener(OnSpinAdjustmented);
        }

        private void OnRefocusUpdate()
        {
            if (BaiduAI_LogoRecognitionsEventBus.Config.CameraTextureData is not null)
            {
                _request.Upload(BaiduAI_LogoRecognitionsEventBus.Config.CameraTextureData);
            }
            else
            {
                BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("Camera TextureData is null.");
            }
        }

        private void OnRefocusTimeLimite()
        {
            if (_currentMode is BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode)
            {
                _currentMode = BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.ManualMode;
                _uploadCameraTextureOperation.Cancel();
                GenerateOtherSpins();
                BaiduAI_LogoRecognitionsEventBus.Config.OnAutoRefocusToManual?.Invoke();
                BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("[Manual]: The Time is Over!");
            }

            void GenerateOtherSpins()
            {
                BaiduAI_LogoRecognitionsConfig config = BaiduAI_LogoRecognitionsEventBus.Config;
                List<string> logoNotFoundNames = new(4);
                foreach (var logoName in _logoNamesToPrefabNames.Keys)
                {
                    if (_logoNamesToObjects.ContainsKey(logoName) is false)
                    {
                        logoNotFoundNames.Add(logoName);
                    }
                }
                InstantiatePose instantiatePose = new();
                Transform trans0;
                Transform trans1;
                Vector3 centerPos;
                Vector3 vertexPos;
                switch (_logoNamesToObjects.Count)
                {
                    case 4: break;
                    case 3
                        when _logoNamesToObjects.ContainsKey(config.XPositiveLogoName) is false:
                        {
                            trans0 = _logoNamesToObjects[config.ZPositiveLogoName].transform;
                            trans1 = _logoNamesToObjects[config.ZNegativeLogoName].transform;
                            centerPos = SangoSubspaceGeneratorUtils.CalcCenterPosByChild(trans0, trans1);
                            vertexPos = SangoSubspaceGeneratorUtils.CalcVertexByCenter(centerPos, _logoNamesToObjects[config.XNegativeLogoName].transform);
                            instantiatePose.Position = vertexPos;
                            OnLogoNameConfirmed(config.XPositiveLogoName, instantiatePose);
                        }
                        break;
                    case 3
                        when _logoNamesToObjects.ContainsKey(config.ZNegativeLogoName) is false:
                        {
                            trans0 = _logoNamesToObjects[config.XPositiveLogoName].transform;
                            trans1 = _logoNamesToObjects[config.XNegativeLogoName].transform;
                            centerPos = SangoSubspaceGeneratorUtils.CalcCenterPosByChild(trans0, trans1);
                            vertexPos = SangoSubspaceGeneratorUtils.CalcVertexByCenter(centerPos, _logoNamesToObjects[config.ZPositiveLogoName].transform);
                            instantiatePose.Position = vertexPos;
                            OnLogoNameConfirmed(config.ZNegativeLogoName, instantiatePose);
                        }
                        break;
                    case 3
                        when _logoNamesToObjects.ContainsKey(config.XNegativeLogoName) is false:
                        {
                            trans0 = _logoNamesToObjects[config.ZPositiveLogoName].transform;
                            trans1 = _logoNamesToObjects[config.ZNegativeLogoName].transform;
                            centerPos = SangoSubspaceGeneratorUtils.CalcCenterPosByChild(trans0, trans1);
                            vertexPos = SangoSubspaceGeneratorUtils.CalcVertexByCenter(centerPos, _logoNamesToObjects[config.XPositiveLogoName].transform);
                            instantiatePose.Position = vertexPos;
                            OnLogoNameConfirmed(config.XNegativeLogoName, instantiatePose);
                        }
                        break;
                    case 3
                        when _logoNamesToObjects.ContainsKey(config.ZPositiveLogoName) is false:
                        {
                            trans0 = _logoNamesToObjects[config.XPositiveLogoName].transform;
                            trans1 = _logoNamesToObjects[config.XNegativeLogoName].transform;
                            centerPos = SangoSubspaceGeneratorUtils.CalcCenterPosByChild(trans0, trans1);
                            vertexPos = SangoSubspaceGeneratorUtils.CalcVertexByCenter(centerPos, _logoNamesToObjects[config.ZNegativeLogoName].transform);
                            instantiatePose.Position = vertexPos;
                            OnLogoNameConfirmed(config.ZPositiveLogoName, instantiatePose);
                        }
                        break;
                    case 2:
                        {
                            OnLogoNameConfirmed(logoNotFoundNames[0], InstantiatePose.DirectForward);
                            OnLogoNameConfirmed(logoNotFoundNames[1], InstantiatePose.DirectBack);
                        }
                        break;
                    case 1:
                        {
                            OnLogoNameConfirmed(logoNotFoundNames[2], InstantiatePose.DirectLeft);
                        }
                        goto case 2;
                    case 0:
                        {
                            OnLogoNameConfirmed(logoNotFoundNames[3], InstantiatePose.DirectRight);
                        }
                        goto case 1;
                }
                logoNotFoundNames.Clear();
            }
        }

        #region Actions
        private void OnAutoRefocusFounded(object sender, BaiduAI_LogoRecognitionsMessages.UploadEvtArgs eventArgs)
        {
            if (_currentMode is BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode
                && _logoNamesToObjects.ContainsKey(eventArgs.ResponseName) is false)
            {
                OnLogoNameConfirmed(eventArgs.ResponseName, InstantiatePose.DirectForward);
            }
        }

        private void OnSubspaceSpined(object sender, BaiduAI_LogoRecognitionsMessages.SubspaceSpinEvtArgs eventArgs)
        {
            if (eventArgs.SubspaceSpinMode is BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode)
            {
                switch (eventArgs.SubspaceSpinRefocus)
                {
                    case BaiduAI_LogoRecognitionsMessages.SubspaceSpinRefocusCode.StartRefocus
                        when _currentMode is BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.WaitingMode:
                        {
                            _currentMode = BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode;
                            _uploadCameraTextureOperation = UnityTimer.RepeatWaitForSecondsRealtime(
                                BaiduAI_LogoRecognitionsEventBus.Config.UpdateSubspaceSpinRefocusTaskInterval);
                            _uploadCameraTextureOperation.completed += delegate
                            {
                                OnRefocusUpdate();
                            };

                            _uploadCameraTextureTimeLimiteOperation = UnityTimer.WaitForSecondsRealtime(
                                BaiduAI_LogoRecognitionsEventBus.Config.UpdateSubspaceSpinRefocusTimeLimite);
                            _uploadCameraTextureTimeLimiteOperation.completed += delegate
                            {
                                OnRefocusTimeLimite();
                            };
                        }
                        break;
                    case BaiduAI_LogoRecognitionsMessages.SubspaceSpinRefocusCode.DoneRefocus
                        when _currentMode is BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode:
                        {
                            _uploadCameraTextureOperation.Cancel();
                            _currentMode = BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.WaitingMode;
                            BaiduAI_LogoRecognitionsEventBus.Config.OnAutoRefocusDone?.Invoke();
                            BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("[Done]: The Refocus is Done.");
                        }
                        break;
                }
            }
        }

        private void OnSpinAdjustmented(object sender, BaiduAI_LogoRecognitionsMessages.SubspaceSpinAdjustmentEvtArgs eventArgs)
        {
            BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("Now is AdjustmentMode Begin: On Message.");
            if (BaiduAI_LogoRecognitionsEventBus.Config.SubspaceSpinsRootObject.transform.childCount is 4)
            {
                BaiduAI_LogoRecognitionsEventBus.LogEvent.Invoke("Now is AdjustmentMode Begin: First Check.");
                BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinedEvent.Invoke(this, new BaiduAI_LogoRecognitionsMessages.SubspaceSpinEvtArgs
                    (BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.AdjustmentMode,
                    BaiduAI_LogoRecognitionsMessages.SubspaceSpinRefocusCode.NoRefocus));
            }
        }

        private void OnLogoNameConfirmed(string logoName, InstantiatePose instantiatePose)
        {
            if (_logoNamesToPrefabNames.TryGetValue(logoName, out var prefabName))
            {
                string path = BaiduAI_LogoRecognitionsEventBus.Config.RecognitionLogoPrefabPathRoot
                    + prefabName
                    + BaiduAI_LogoRecognitionsEventBus.Config.RecognitionLogoPrefabPathSuffix;

                ResourceRequest request = Resources.LoadAsync<GameObject>(path);
                request.completed += delegate
                {
                    AsyncInstantiateOperation<GameObject> operation = InstantiateAsync(request.asset as GameObject, BaiduAI_LogoRecognitionsEventBus.Config.SubspaceSpinsRootObject.transform);
                    operation.completed += delegate
                    {
                        foreach (var gameObject in operation.Result)
                        {
                            OnInstantiatedGameObject(gameObject, logoName, instantiatePose);
                        }
                    };
                };
            }
        }

        private void OnInstantiatedGameObject(GameObject gameObject, string responseName, InstantiatePose instantiatePose)
        {
            Transform viewCenterTrans = BaiduAI_LogoRecognitionsEventBus.Config.ViewCenterObject.transform;

            int distance = BaiduAI_LogoRecognitionsEventBus.Config.RecognitionLogoPrefabOffsetZToViewCenterObject;
            Vector3 targetPosition;
            if (instantiatePose.InstantiateMode is InstantiateModeCode.DirectMode)
            {
                targetPosition = instantiatePose.DirectionCode switch
                {
                    DirectionCode.Forward => viewCenterTrans.position + viewCenterTrans.forward * distance,
                    DirectionCode.Back => viewCenterTrans.position - viewCenterTrans.forward * distance,
                    DirectionCode.Right => viewCenterTrans.position + viewCenterTrans.right * distance,
                    DirectionCode.Left => viewCenterTrans.position - viewCenterTrans.right * distance,
                    DirectionCode.Default => throw new System.NotImplementedException(),
                    _ => throw new System.NotImplementedException()
                };
            }
            else
            {
                Transform parentTrans = BaiduAI_LogoRecognitionsEventBus.Config.SubspaceSpinsRootObject.transform;
                targetPosition = parentTrans.TransformPoint(instantiatePose.Position);
            }

            if (gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.MovePosition(targetPosition);
            }
            else
            {
                gameObject.transform.position = targetPosition;
            }

            if (_logoNamesToObjects.TryAdd(responseName, gameObject))
            {
                BaiduAI_LogoRecognitionsEventBus.Config.OnAutoRefocusLogoFound?.Invoke();
            }
            else
            {
                Destroy(gameObject);
            }

            if (BaiduAI_LogoRecognitionsEventBus.Config.SubspaceSpinsRootObject.transform.childCount is 4)
            {
                BaiduAI_LogoRecognitionsEventBus.OnSubspaceSpinedEvent.Invoke(this, new BaiduAI_LogoRecognitionsMessages.SubspaceSpinEvtArgs
                    (BaiduAI_LogoRecognitionsMessages.SubspaceSpinModeCode.RefocusMode,
                    BaiduAI_LogoRecognitionsMessages.SubspaceSpinRefocusCode.DoneRefocus));
            }
        }
        #endregion
    }
}