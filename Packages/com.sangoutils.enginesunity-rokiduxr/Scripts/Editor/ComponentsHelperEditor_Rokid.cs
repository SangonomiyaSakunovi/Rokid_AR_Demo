using UnityEditor;

namespace SangoUtils.Engines_Unity.Rokid_UXR.Interactables
{
#if UNITY_EDITOR
    internal class ComponentsHelperEditor_Rokid
    {
        [MenuItem("GameObject/SangoUtils/Components/Add Grab InteractableObject Components", false, 10)]
        private static void AddGrabInteractableObjectComponents()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponentsHelper<GrabInteractableObjectComponentsHelper_Rokid>();
            }
        }

        [MenuItem("GameObject/SangoUtils/Components/Remove Grab InteractableObject Components", false, 10)]
        private static void RemoveGrabInteractableObjectComponents()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.RemoveComponentsHelper<GrabInteractableObjectComponentsHelper_Rokid>();
            }
        }

        [MenuItem("GameObject/SangoUtils/Components/Add Pressed InteractableObject Components", false, 10)]
        private static void AddPressedInteractableObjectComponents()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.AddComponentsHelper<PressedInteractableObjectComponentsHelper_Rokid>();
            }
        }

        [MenuItem("GameObject/SangoUtils/Components/Remove Pressed InteractableObject Components", false, 10)]
        private static void RemovePressedInteractableObjectComponents()
        {
            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.RemoveComponentsHelper<PressedInteractableObjectComponentsHelper_Rokid>();
            }
        }
    }
#endif
}