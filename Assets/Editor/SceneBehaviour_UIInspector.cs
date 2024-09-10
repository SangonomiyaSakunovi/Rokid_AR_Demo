using SceneBehaviours;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(SceneBehaviour_UI))]
    internal class SceneBehaviour_UIInspector : Editor
    {
        private SerializedProperty _config;

        private SerializedProperty _loggerTxt;
        private SerializedProperty _rokidRKComponentParent;
        private SerializedProperty _fusionBehaviourParent;
        private SerializedProperty _uiPanelsParent;

        private void OnEnable()
        {
            serializedObject.Update();

            _config = serializedObject.FindProperty("_config");
            _loggerTxt = serializedObject.FindProperty("_loggerTxt");
            _rokidRKComponentParent = serializedObject.FindProperty("_rokidRKComponentParent");
            _fusionBehaviourParent = serializedObject.FindProperty("_fusionBehaviourParent");
            _uiPanelsParent = serializedObject.FindProperty("_uiPanelsParent");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_config);
            EditorGUILayout.PropertyField(_loggerTxt);
            EditorGUILayout.PropertyField(_rokidRKComponentParent);
            EditorGUILayout.PropertyField(_fusionBehaviourParent);
            EditorGUILayout.PropertyField(_uiPanelsParent);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (GUILayout.Button("SetDefault"))
                SetDefault();
        }

        private void SetDefault()
        {
            ((GameObject)_rokidRKComponentParent.objectReferenceValue).SetActive(true);
            ((GameObject)_fusionBehaviourParent.objectReferenceValue).SetActive(true);
            ((GameObject)_uiPanelsParent.objectReferenceValue).SetActive(true);
        }
    }
}
