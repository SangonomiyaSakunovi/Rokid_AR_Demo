using Rokid.UXR.Interaction;
using System;
using System.Reflection;
using UnityEngine;

namespace SangoUtils.Engines_Unity.Rokid_UXR.Interactables
{
    [RequireComponent(typeof(RayInteractable), typeof(ColliderSurface))]
    [RequireComponent(typeof(InteractableUnityEventWrapper))]
    [DisallowMultipleComponent]
    internal class PressedInteractableObjectComponentsHelper_Rokid : MonoBehaviour, IComponentsHelper
    {
        public Type[] GetReleventComponents() => new Type[]
        {
            typeof(InteractableUnityEventWrapper),
            typeof(ColliderSurface),
            typeof(RayInteractable)
        };

        public void OnInitialize()
        {
            FieldInfo surfaceField = typeof(RayInteractable).GetField("_surface",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
            surfaceField.SetValue(GetComponent<RayInteractable>(), GetComponent<ColliderSurface>());

            if (TryGetComponent<Collider>(out var collider))
            {
                FieldInfo colliderField = typeof(ColliderSurface).GetField("_collider",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
                colliderField.SetValue(GetComponent<ColliderSurface>(), collider);
            }

            FieldInfo interactableViewField = typeof(InteractableUnityEventWrapper).GetField("_interactableView",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField | BindingFlags.GetField);
            interactableViewField.SetValue(GetComponent<InteractableUnityEventWrapper>(), GetComponent<RayInteractable>());
        }
    }
}
