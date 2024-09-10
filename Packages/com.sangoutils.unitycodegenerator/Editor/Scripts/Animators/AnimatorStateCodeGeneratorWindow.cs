using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace SangoUtils.UnityCodeGenerator.AnimatorGens
{
    internal class AnimatorStateCodeGeneratorWindow : EditorWindow
    {
        private AnimatorController _animator;
        private string _namespaceName;

        [MenuItem("SangoUtils/Code Generators/Export Animator States")]
        private static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow<AnimatorStateCodeGeneratorWindow>();
            GUIContent content = new GUIContent("Export Animator Stator Infos");
            window.titleContent = content;
            window.Show();
        }

        private void OnGUI()
        {
            _animator = (AnimatorController)EditorGUILayout.ObjectField("Animator", _animator, typeof(AnimatorController), true);
            _namespaceName = EditorGUILayout.TextField("NamespaceName", _namespaceName);

            if (GUILayout.Button("Generate"))
            {
                AnimatorStateCodeGenerator.Execute(_animator, _namespaceName);
            }
        }
    }
}
