using UnityEditor;
using UnityEngine;

namespace SangoUtils.HTCViveTrackerHelpers
{
    [CustomEditor(typeof(TrackerDataReceiver))]
    internal class TrackerDataReceiverInspector : Editor
    {
        private TrackerDataReceiver _dataReceiver;
        private SerializedProperty _isGetTrackerDataToggle;

        private SerializedProperty _trackerDataLateFrames;

        private SerializedProperty _trackerDataIpType;
        private SerializedProperty _ipAddress;
        private SerializedProperty _port;

        private SerializedProperty _unityPosX;
        private SerializedProperty _unityPosY;
        private SerializedProperty _unityPosZ;
        private SerializedProperty _unityRotX;
        private SerializedProperty _unityRotY;
        private SerializedProperty _unityRotZ;
        private SerializedProperty _unityRotW;

        private SerializedProperty _trackerObject;

        private bool isPositionFoldout = false;
        private bool isRotationFoldout = false;

        private void OnEnable()
        {
            serializedObject.Update();

            _dataReceiver = (TrackerDataReceiver)target;

            _isGetTrackerDataToggle = serializedObject.FindProperty("_isGetTrackerDataToggle");

            _trackerDataLateFrames = serializedObject.FindProperty("_trackerDataLateFrames");

            _trackerDataIpType = serializedObject.FindProperty("_trackerDataIpType");
            _ipAddress = serializedObject.FindProperty("_ipAddress");
            _port = serializedObject.FindProperty("_port");

            _unityPosX = serializedObject.FindProperty("_unityPosX");
            _unityPosY = serializedObject.FindProperty("_unityPosY");
            _unityPosZ = serializedObject.FindProperty("_unityPosZ");

            _unityRotX = serializedObject.FindProperty("_unityRotX");
            _unityRotY = serializedObject.FindProperty("_unityRotY");
            _unityRotZ = serializedObject.FindProperty("_unityRotZ");
            _unityRotW = serializedObject.FindProperty("_unityRotW");

            _trackerObject = serializedObject.FindProperty("_trackerObject");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Description: This is a tool for managing HTC Vive Tracker data.");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_trackerDataLateFrames);

            bool isGetTrackerDataValue = EditorGUILayout.BeginToggleGroup("Get Tracker Data", _isGetTrackerDataToggle.boolValue);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_trackerDataIpType);
            switch ((TrackerDataReceiver.TrackerDataIPTypeCode)_trackerDataIpType.enumValueIndex)
            {
                case TrackerDataReceiver.TrackerDataIPTypeCode.AnyIP:
                    break;
                case TrackerDataReceiver.TrackerDataIPTypeCode.TargetIP:
                    EditorGUILayout.PropertyField(_ipAddress);
                    break;
            }
            EditorGUILayout.PropertyField(_port);

            isPositionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(isPositionFoldout, "Position Unity");
            if (isPositionFoldout)
            {
                EditorGUILayout.PropertyField(_unityPosX);
                EditorGUILayout.PropertyField(_unityPosY);
                EditorGUILayout.PropertyField(_unityPosZ);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            isRotationFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(isRotationFoldout, "Rotation Unity");
            if (isRotationFoldout)
            {
                EditorGUILayout.PropertyField(_unityRotX);
                EditorGUILayout.PropertyField(_unityRotY);
                EditorGUILayout.PropertyField(_unityRotZ);
                EditorGUILayout.PropertyField(_unityRotW);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.PrefixLabel("The following is Optional choice.");
            EditorGUILayout.PropertyField(_trackerObject);

            EditorGUILayout.Space();
            if (GUILayout.Button("Reset LateFrames (Runtime)"))
                _dataReceiver.ResetLateFrames();

            EditorGUILayout.EndToggleGroup();

            if (EditorGUI.EndChangeCheck())
            {
                _isGetTrackerDataToggle.boolValue = isGetTrackerDataValue;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
