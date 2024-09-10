using Rokid.UXR.Interaction;
using System;
using System.Reflection;
using UnityEngine;

namespace SangoUtils.Engines_Unity.Rokid_UXR.Interactables
{
    [RequireComponent(typeof(Rigidbody), typeof(VelocityEstimator))]
    [RequireComponent(typeof(RayInteractable), typeof(ColliderSurface))]
    [RequireComponent(typeof(GrabInteractable))]
    [RequireComponent(typeof(Draggable), typeof(Throwable))]
    [RequireComponent(typeof(InteractableUnityEventWrapper))]
    [DisallowMultipleComponent]
    internal class GrabInteractableObjectComponentsHelper_Rokid : MonoBehaviour, IComponentsHelper
    {
        public Type[] GetReleventComponents() => new Type[]
        {
            typeof(InteractableUnityEventWrapper),
            typeof(Throwable),
            typeof(Draggable),
            typeof(GrabInteractable),
            typeof(ColliderSurface),
            typeof(RayInteractable),
            typeof(VelocityEstimator),
            typeof(Rigidbody)
        };

        public void OnInitialize()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            VelocityEstimator velocityEstimator = GetComponent<VelocityEstimator>();
            velocityEstimator.velocityAverageFrames = 3;
            velocityEstimator.angularVelocityAverageFrames = 5;

            GrabInteractable grabInteractable = GetComponent<GrabInteractable>();
            grabInteractable.attachEaseIn = true;

            Throwable throwable = GetComponent<Throwable>();
            throwable.releaseVelocityStyle = ReleaseStyle.ShortEstimation;
            throwable.scaleReleaseVelocity = 0;
            throwable.scaleReleaseAngularVelocity = 0;
            throwable.scaleReleaseVelocityThreshold = 1;
            throwable.scaleReleaseVelocityCurve = AnimationCurve.Linear(0, 1, 1, 1);
            throwable.restoreOriginalParent = true;

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