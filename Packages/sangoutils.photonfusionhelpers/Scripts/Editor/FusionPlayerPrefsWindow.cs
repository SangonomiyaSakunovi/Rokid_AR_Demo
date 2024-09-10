using UnityEditor;
using UnityEngine;

namespace SangoUtils.PhotonFusionHelpers
{
    internal class FusionPlayerPrefsWindow : EditorWindow
    {
        [MenuItem("SangoUtils/Fusions/PlayerPrefs")]
        private static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<FusionPlayerPrefsWindow>();
            GUIContent content = new GUIContent("Fusion PlayerPrefs");
            window.titleContent = content;
            window.Show();
        }

        private const string PrefPrefix = "SangoProjects.PhotonPref.";
        private string _txt = "";

        private void OnGUI()
        {
            GUILayout.Label("UserNames", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check"))
            { GetPref("Username", out string str); _txt = str; }
            if (GUILayout.Button("Set"))
                SetPref("Username", _txt);
            if (GUILayout.Button("Delet"))
                DelPref("Username");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("Region", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check"))
            { GetPref("Region", out string str); _txt = str; }
            if (GUILayout.Button("Set"))
                SetPref("Region", _txt);
            if (GUILayout.Button("Delet"))
                DelPref("Region");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("AppVersion", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check"))
            { GetPref("AppVersion", out string str); _txt = str; }
            if (GUILayout.Button("Set"))
                SetPref("AppVersion", _txt);
            if (GUILayout.Button("Delet"))
                DelPref("AppVersion");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("MaxPlayerCount", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check"))
            { GetPref("MaxPlayerCount", out string str); _txt = str; }
            if (GUILayout.Button("Set"))
                SetPref("MaxPlayerCount", _txt);
            if (GUILayout.Button("Delet"))
                DelPref("MaxPlayerCount");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("Scene", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check"))
            { GetPref("Scene", out string str); _txt = str; }
            if (GUILayout.Button("Set"))
                SetPref("Scene", _txt);
            if (GUILayout.Button("Delet"))
                DelPref("Scene");
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.Label("PlayerPref Info", EditorStyles.boldLabel);
            GUILayout.TextField(_txt);
        }

        private void GetPref(string key, out string value)
        {
            value = PlayerPrefs.GetString(PrefPrefix + key);
        }
        private void GetPref(string key, out int value)
        {
            value = PlayerPrefs.GetInt(PrefPrefix + key);
        }
        private void GetPref(string key, out float value)
        {
            value = PlayerPrefs.GetFloat(PrefPrefix + key);
        }

        private void DelPref(string key)
        {
            PlayerPrefs.DeleteKey(PrefPrefix + key);
        }

        private void SetPref(string key, string value)
        {
            PlayerPrefs.SetString(PrefPrefix + key, value);
        }
    }
}
