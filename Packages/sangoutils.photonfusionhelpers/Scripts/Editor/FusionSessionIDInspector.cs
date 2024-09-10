using SangoUtils.PhotonFusionHelpers.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers
{
    [CustomEditor(typeof(FusionSessionID))]
    internal class FusionSessionIDInspector : Editor
    {
        private SerializedProperty _ID;

        private void OnEnable()
        {
            serializedObject.Update();

            _ID = serializedObject.FindProperty("_ID");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_ID);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Ramdom ID"))
            {
                string tempID = FusionPartyCodeGenerator.Create(8, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789");
                if(tempID != "")
                    _ID.stringValue = tempID;
            }   
            if (GUILayout.Button("Clear ID"))
                _ID.stringValue = "";
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
