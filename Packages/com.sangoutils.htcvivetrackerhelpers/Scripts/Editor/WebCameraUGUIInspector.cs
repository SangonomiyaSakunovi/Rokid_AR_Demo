using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SangoUtils.HTCViveTrackerHelpers
{
    [CustomEditor(typeof(WebCameraUGUI))]
    internal class WebCameraUGUIInspector : Editor
    {
        private WebCameraUGUI _webCameraUGUI;
        private SerializedProperty _isWebCameraViewToggle;

        private SerializedProperty _webCameraUGUIRenderType;
        private SerializedProperty _cameraRawImage;
        private SerializedProperty _cameraQuad;

        private SerializedProperty _cameraWidth;
        private SerializedProperty _cameraHeight;
        private SerializedProperty _cameraFPS;
        private SerializedProperty _cameraLateFrames;

        //Autofind.
        private Canvas _canvas;

        private float _depth = -1;

        private void OnEnable()
        {
            serializedObject.Update();

            _webCameraUGUI = (WebCameraUGUI)target;

            _isWebCameraViewToggle = serializedObject.FindProperty("_isWebCameraViewToggle");

            _webCameraUGUIRenderType = serializedObject.FindProperty("_webCameraUGUIRenderType");
            _cameraRawImage = serializedObject.FindProperty("_cameraRawImage");
            _cameraQuad = serializedObject.FindProperty("_cameraQuad");

            _cameraWidth = serializedObject.FindProperty("_cameraWidth");
            _cameraHeight = serializedObject.FindProperty("_cameraHeight");
            _cameraFPS = serializedObject.FindProperty("_cameraFPS");
            _cameraLateFrames = serializedObject.FindProperty("_cameraLateFrames");

            //isNew is false, due to this is OnEnable
            GetOrUpdateDepth(ref _depth);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Description: This is a tool for managing Web Camera data.");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            bool isWebCameraViewToggleValue = EditorGUILayout.BeginToggleGroup("Web Camera View", _isWebCameraViewToggle.boolValue);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_webCameraUGUIRenderType);

            switch ((WebCameraUGUI.WebCameraUGUIRenderType)_webCameraUGUIRenderType.enumValueIndex)
            {
                case WebCameraUGUI.WebCameraUGUIRenderType.Canvas_RawImage:
                    EditorGUILayout.PropertyField(_cameraRawImage);
                    break;
                case WebCameraUGUI.WebCameraUGUIRenderType.Object_Quad:
                    EditorGUILayout.PropertyField(_cameraQuad);
                    break;
            }

            Vector2Int cameraReso = EditorGUILayout.Vector2IntField("Resolution", new Vector2Int(_cameraWidth.intValue, _cameraHeight.intValue));

            EditorGUILayout.PropertyField(_cameraFPS);
            EditorGUILayout.PropertyField(_cameraLateFrames);

            if (_depth >= 0)
                _depth = EditorGUILayout.FloatField("Depth", _depth);

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset LateFrames (Runtime)"))
                _webCameraUGUI.ResetLateFrames();

            EditorGUILayout.EndToggleGroup();

            if (EditorGUI.EndChangeCheck())
            {
                _isWebCameraViewToggle.boolValue = isWebCameraViewToggleValue;
                _cameraWidth.intValue = cameraReso.x;
                _cameraHeight.intValue = cameraReso.y;
                //isNew = true. Because that is update.
                GetOrUpdateDepth(ref _depth, true);
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// On Enable, the isNew should be false; On update, it need true.
        /// Be careful, this is ref param.
        /// </summary>
        /// <param name="depth">Ref this, if isNew, it will override the serialization, otherwise, will be reload.</param>
        /// <param name="isNew">The tag to clam if depath is new.</param>
        private void GetOrUpdateDepth(ref float depth, in bool isNew = false)
        {
            switch ((WebCameraUGUI.WebCameraUGUIRenderType)_webCameraUGUIRenderType.enumValueIndex)
            {
                case WebCameraUGUI.WebCameraUGUIRenderType.Canvas_RawImage:
                    if (_cameraRawImage.objectReferenceValue != null
                        && TryFindCanvas(((RawImage)_cameraRawImage.objectReferenceValue).transform, out _canvas))
                    {
                        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
                        {
                            if (isNew && depth >= 0)
                                _canvas.planeDistance = depth;
                            else
                                depth = _canvas.planeDistance;
                            return;
                        }
                    }
                    break;
                case WebCameraUGUI.WebCameraUGUIRenderType.Object_Quad:
                    break;
            }
            depth = -1;
        }

        /// <summary>
        /// Traverse the Canvas, use as other Try~.
        /// </summary>
        private bool TryFindCanvas(Transform trans, out Canvas canvas)
        {
            if (trans == null)
            {
                canvas = null;
                return false;
            }

            canvas = trans.GetComponent<Canvas>();
            if (canvas != null)
                return true;

            return TryFindCanvas(trans.parent, out canvas);
        }
    }
}