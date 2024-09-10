using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SangoUtils.HTCViveTrackerHelpers
{
    public class WebCameraUGUI : MonoBehaviour
    {
        internal enum WebCameraUGUIRenderType
        {
            Canvas_RawImage,
            Object_Quad
        }

        [SerializeField] private bool _isWebCameraViewToggle = false;

        [Header("The Render Type")]
        [SerializeField] private WebCameraUGUIRenderType _webCameraUGUIRenderType = WebCameraUGUIRenderType.Canvas_RawImage;
        /// <summary>
        /// Only one will be use, if you want both, you need to modify this script.
        /// </summary>
        [SerializeField] private RawImage _cameraRawImage;
        [SerializeField] private GameObject _cameraQuad;

        [SerializeField] private int _cameraWidth = 1024;
        [SerializeField] private int _cameraHeight = 768;
        [SerializeField] private int _cameraFPS = 30;
        [SerializeField] private int _cameraLateFrames = 0;

        /// <summary>
        /// Share Color32 data of the webCamera Pixs.
        /// </summary>
        public Action<Color32[]> OnWebCamPixsUpdate;

        private WebCamTexture _webCamTexRaw;
        private Texture2D _webCamTexUGUI;
        private bool _isGotWebCamTex = false;

        private Queue<Color32[]> _webCamTexPixsQueue = new Queue<Color32[]>();

        private void Start()
        {
            if (_isWebCameraViewToggle)
                OpenCameraAsync();
        }

        private void FixedUpdate()
        {
            if (_isGotWebCamTex)
            {
                Color32[] colors = _webCamTexRaw.GetPixels32();
                _webCamTexPixsQueue.Enqueue(colors);
                if (_webCamTexPixsQueue.Count > _cameraLateFrames)
                {
                    var pix = _webCamTexPixsQueue.Dequeue();
                    _webCamTexUGUI.SetPixels32(pix);
                    _webCamTexUGUI.Apply();
                    OnWebCamPixsUpdate?.Invoke(pix);
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (_webCamTexRaw != null)
            {
                if (pause)
                {
                    _isGotWebCamTex = false;
                    _webCamTexRaw.Pause();
                }
                else
                {
                    _webCamTexRaw.Play();
                    _isGotWebCamTex = true;
                }
            }

        }

        private void OnApplicationQuit()
        {
            if (_webCamTexRaw != null)
            {
                _webCamTexRaw.Stop();
            }
        }

        /// <summary>
        /// Open camera with IEnumerator
        /// </summary>
        private void OpenCameraAsync()
        {
            StartCoroutine(OpenCamera());

            IEnumerator OpenCamera()
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                {
                    if (_webCamTexRaw != null)
                    {
                        _webCamTexRaw.Stop();
                    }

                    // Maybe the we get the autorization, but the camera we can`t find.
                    // We try to refound it 300 times, if still no device, perhaps really no. :)
                    int i = 0;
                    while (WebCamTexture.devices.Length <= 0 && 1 < 300)
                    {
                        yield return new WaitForEndOfFrame();
                        i++;
                    }

                    if (WebCamTexture.devices.Length <= 0)
                    {
                        Debug.LogError("[Sango] There is no camera device. :(");
                    }
                    else
                    {
                        string devicename = WebCamTexture.devices[0].name;

                        _webCamTexRaw = new WebCamTexture(devicename, _cameraWidth, _cameraHeight, _cameraFPS)
                        {
                            wrapMode = TextureWrapMode.Repeat,
                        };
                        //Warnning: You must Play this texture, then it will auto get width and height.
                        //These two paras will not same as default.
                        _webCamTexRaw.Play();

                        _webCamTexUGUI = new Texture2D(_webCamTexRaw.width, _webCamTexRaw.height);
                        _webCamTexUGUI.wrapMode = TextureWrapMode.Clamp;

                        switch (_webCameraUGUIRenderType)
                        {
                            //We use TexUGUI to play, this will used to store the TexRaw.
                            case WebCameraUGUIRenderType.Canvas_RawImage:
                                _cameraRawImage.gameObject.SetActive(true);
                                _cameraRawImage.texture = _webCamTexUGUI;
                                break;
                            case WebCameraUGUIRenderType.Object_Quad:
                                _cameraQuad.gameObject.SetActive(true);
                                _cameraQuad.GetComponent<Renderer>().material.mainTexture = _webCamTexUGUI;
                                break;
                        }

                        _isGotWebCamTex = true;
                    }
                }
                else
                {
                    Debug.LogError("[Sango] There is no Authorization to use Camera. :(");
                }
            }
        }

        /// <summary>
        /// You can reset the late frames any time.
        /// </summary>
        internal void ResetLateFrames()
        {
            Debug.Log("[Sango] You have reset the LateFrames.");
            _webCamTexPixsQueue.Clear();
        }
    }
}
