using SceneBehaviours;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(SceneBehaviour_Game))]
    internal class SceneBehaviour_GameInspector : Editor
    {
        private SerializedProperty _config;

        private SerializedProperty _gameLogicParent;
        private SerializedProperty _subspaceParent;
        private SerializedProperty _subspaceRefParent;
        private SerializedProperty _physicCameraParent;

        private SerializedProperty _subspaceVisualParent;
        private SerializedProperty _subspaceRefVisualParent;

        private void OnEnable()
        {
            serializedObject.Update();

            _config = serializedObject.FindProperty("_config");
            _gameLogicParent = serializedObject.FindProperty("_gameLogicParent");
            _subspaceParent = serializedObject.FindProperty("_subspaceParent");
            _subspaceRefParent = serializedObject.FindProperty("_subspaceRefParent");
            _physicCameraParent = serializedObject.FindProperty("_physicCameraParent");

            _subspaceVisualParent = serializedObject.FindProperty("_subspaceVisualParent");
            _subspaceRefVisualParent = serializedObject.FindProperty("_subspaceRefVisualParent");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_config);
            EditorGUILayout.PropertyField(_gameLogicParent);
            EditorGUILayout.PropertyField(_subspaceParent);
            EditorGUILayout.PropertyField(_subspaceRefParent);
            EditorGUILayout.PropertyField(_physicCameraParent);

            EditorGUILayout.PropertyField(_subspaceVisualParent);
            EditorGUILayout.PropertyField(_subspaceRefVisualParent);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (GUILayout.Button("SetDefault"))
                SetDefault();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Only");
            if (GUILayout.Button("Sync Subspace Pose"))
                SyncSubspacePose();
            if (GUILayout.Button("Configure Physic Camera"))
                SetConfigurePhysicCamera();
        }

        private void SetDefault()
        {
            ((GameObject)_gameLogicParent.objectReferenceValue).SetActive(true);
            ((GameObject)_subspaceParent.objectReferenceValue).SetActive(true);
            ((GameObject)_subspaceRefParent.objectReferenceValue).SetActive(true);
            ((GameObject)_physicCameraParent.objectReferenceValue).SetActive(false);

            ((GameObject)_subspaceVisualParent.objectReferenceValue).SetActive(false);
            ((GameObject)_subspaceRefVisualParent.objectReferenceValue).SetActive(false);
        }

        private void SetConfigurePhysicCamera()
        {
            ((GameObject)_gameLogicParent.objectReferenceValue).SetActive(false);
            ((GameObject)_subspaceParent.objectReferenceValue).SetActive(false);
            ((GameObject)_subspaceRefParent.objectReferenceValue).SetActive(false);
            ((GameObject)_physicCameraParent.objectReferenceValue).SetActive(true);

            ((GameObject)_subspaceVisualParent.objectReferenceValue).SetActive(false);
            ((GameObject)_subspaceRefVisualParent.objectReferenceValue).SetActive(false);
        }

        private void SyncSubspacePose()
        {
            Transform subspaceRefTrans = ((GameObject)_subspaceRefParent.objectReferenceValue).transform;
            ((GameObject)_subspaceParent.objectReferenceValue).transform.SetPositionAndRotation(subspaceRefTrans.position, subspaceRefTrans.rotation);
        }
    }
}