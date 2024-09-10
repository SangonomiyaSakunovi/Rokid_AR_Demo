using UnityEditor;
using UnityEngine;

namespace SangoUtils.Engines_Unity.Rokid_UXR
{
#if UNITY_EDITOR
    internal class EnvironmentsHelperEditor_Rokid
    {
        [MenuItem("GameObject/SangoUtils/Environment/Add CameraRig", false, 10)]
        private static void AddCameraRig()
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("Prefabs/BaseSetting/RKCameraRig");
            request.completed += delegate
            {
                AsyncInstantiateOperation<GameObject> operation = MonoBehaviour.InstantiateAsync(request.asset as GameObject);
                operation.completed += delegate
                {
                    foreach (var rig in operation.Result)
                    {
                        rig.name = rig.name.Replace("(Clone)", "");

                        if (Selection.activeGameObject != null)
                        {
                            rig.transform.SetParent(Selection.activeGameObject.transform);
                        }
                    }
                };
            };
        }

        [MenuItem("GameObject/SangoUtils/Environment/Add Input Manager", false, 10)]
        private static void AddInputManager()
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("Prefabs/RKInput/[RKInput]");
            request.completed += delegate
            {
                AsyncInstantiateOperation<GameObject> operation = MonoBehaviour.InstantiateAsync(request.asset as GameObject);
                operation.completed += delegate
                {
                    foreach (var input in operation.Result)
                    {
                        input.name = input.name.Replace("(Clone)", "");

                        if (Selection.activeGameObject != null)
                        {
                            input.transform.SetParent(Selection.activeGameObject.transform);
                        }
                    }
                };
            };
        }

        [MenuItem("GameObject/SangoUtils/Environment/Add Canvas", false, 10)]
        private static void AddLoggerManager()
        {
            ResourceRequest request = Resources.LoadAsync<GameObject>("Prefabs/UI/PointableUI/PointableUI");
            request.completed += delegate
            {
                AsyncInstantiateOperation<GameObject> operation = MonoBehaviour.InstantiateAsync(request.asset as GameObject);
                operation.completed += delegate
                {
                    foreach (var logger in operation.Result)
                    {
                        logger.name = logger.name.Replace("(Clone)", "");

                        if (Selection.activeGameObject != null)
                        {
                            logger.transform.SetParent(Selection.activeGameObject.transform);
                        }
                    }
                };
            };
        }
    }
#endif
}
