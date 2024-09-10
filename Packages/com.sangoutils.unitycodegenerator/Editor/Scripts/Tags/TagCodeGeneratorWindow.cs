using UnityEditor;
using UnityEngine;

namespace SangoUtils.UnityCodeGenerator.TagGens
{
    internal class TagCodeGeneratorWindow : EditorWindow
    {
        [MenuItem("SangoUtils/Code Generators/Tag Generator")]
        private static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<TagCodeGeneratorWindow>();
            GUIContent content = new GUIContent("Generate Tags");
            window.titleContent = content;
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Generate"))
            {
                TagCodeGenerator.Execute();
            }
        }
    }
}
