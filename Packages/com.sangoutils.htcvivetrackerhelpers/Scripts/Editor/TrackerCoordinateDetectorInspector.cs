using UnityEditor;

namespace SangoUtils.HTCViveTrackerHelpers
{
    [CustomEditor(typeof(TrackerCoordinateDetector))]
    internal class TrackerCoordinateDetectorInspector : Editor
    {
        private TrackerCoordinateDetector _trackerCoordinateDetector;
        private SerializedProperty _isTrackerCoordinateDetectToggle;

        private SerializedProperty _originRange;
        private SerializedProperty _originPrecisionPos;
        private SerializedProperty _originPrecisionRotEular;

        private SerializedProperty _audioSource;
        private SerializedProperty _audioClipIdle;
        private SerializedProperty _audioClipPosDetecting;
        private SerializedProperty _audioClipRotDetecting;
        private SerializedProperty _audioClipDetected;

        private SerializedProperty _audioVolumMin;
        private SerializedProperty _audioVolumMax;
        private SerializedProperty _audioVolumDefault;
        private SerializedProperty _audioPitchMin;
        private SerializedProperty _audioPitchMax;
        private SerializedProperty _audioPitchDefault;

        private SerializedProperty _isCustomDetect;
        private SerializedProperty _onDetectedEvt;

        private void OnEnable()
        {
            serializedObject.Update();

            _trackerCoordinateDetector = (TrackerCoordinateDetector)target;
            _isTrackerCoordinateDetectToggle = serializedObject.FindProperty("_isTrackerCoordinateDetectToggle");

            _originRange = serializedObject.FindProperty("_originRange");
            _originPrecisionPos = serializedObject.FindProperty("_originPrecisionPos");
            _originPrecisionRotEular = serializedObject.FindProperty("_originPrecisionRotEular");

            _audioSource = serializedObject.FindProperty("_audioSource");
            _audioClipIdle = serializedObject.FindProperty("_audioClipIdle");
            _audioClipPosDetecting = serializedObject.FindProperty("_audioClipPosDetecting");
            _audioClipRotDetecting = serializedObject.FindProperty("_audioClipRotDetecting");
            _audioClipDetected = serializedObject.FindProperty("_audioClipDetected");

            _audioVolumMin = serializedObject.FindProperty("_audioVolumMin");
            _audioVolumMax = serializedObject.FindProperty("_audioVolumMax");
            _audioVolumDefault = serializedObject.FindProperty("_audioVolumDefault");
            _audioPitchMin = serializedObject.FindProperty("_audioPitchMin");
            _audioPitchMax = serializedObject.FindProperty("_audioPitchMax");
            _audioPitchDefault = serializedObject.FindProperty("_audioPitchDefault");

            _isCustomDetect = serializedObject.FindProperty("_isCustomDetect");
            _onDetectedEvt = serializedObject.FindProperty("_onDetectedEvt");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Description: This is a tool for detecting the definition of HTC VIVE Tracker Coordinate.");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            bool isTrackerCoordinateDetectToggleValue = EditorGUILayout.BeginToggleGroup("Tracker Coordinate Detect", _isTrackerCoordinateDetectToggle.boolValue);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_originRange);
            EditorGUILayout.PropertyField(_originPrecisionPos);
            EditorGUILayout.PropertyField(_originPrecisionRotEular);

            EditorGUILayout.LabelField("The following audio will be played on detecting update.");
            EditorGUILayout.PropertyField(_audioSource);
            EditorGUILayout.PropertyField(_audioClipIdle);
            EditorGUILayout.PropertyField(_audioClipPosDetecting);
            EditorGUILayout.PropertyField(_audioClipRotDetecting);
            EditorGUILayout.PropertyField(_audioClipDetected);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("You can customize your own prefer.");
            EditorGUILayout.PropertyField(_audioVolumMin);
            EditorGUILayout.PropertyField(_audioVolumMax);
            EditorGUILayout.PropertyField(_audioVolumDefault);
            EditorGUILayout.PropertyField(_audioPitchMin);
            EditorGUILayout.PropertyField(_audioPitchMax);
            EditorGUILayout.PropertyField(_audioPitchDefault);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("If you custom your own behaviour, you can try this.");
            EditorGUILayout.PropertyField(_isCustomDetect);
            EditorGUILayout.PropertyField(_onDetectedEvt);

            EditorGUILayout.EndToggleGroup();

            if (EditorGUI.EndChangeCheck())
            {
                _isTrackerCoordinateDetectToggle.boolValue = isTrackerCoordinateDetectToggleValue;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
