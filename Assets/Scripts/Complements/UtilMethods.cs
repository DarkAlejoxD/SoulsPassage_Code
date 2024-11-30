using Cameras;
using UnityEditor;
using UnityEngine;

namespace UtilsComplements
{
    public static class UtilMethods
    {
#if UNITY_EDITOR
        [MenuItem(itemName: "Utils/Put Mesh Collider To Every MeshRenderer", isValidateFunction: false, priority = 1)]
        private static void MeshRendererToCollider()
        {
            var list = GameObject.FindObjectsOfType<MeshRenderer>();

            foreach (var item in list)
            {
                if (!item.TryGetComponent<Collider>(out var a))
                {
                    item.gameObject.AddComponent<MeshCollider>();
                }
            }
        }

        [MenuItem(itemName: "Utils/Check Parity MeshCollider to Renderer", isValidateFunction: false, priority = 1)]
        private static void CheckParityMeshColliderRenderer()
        {
            var list = GameObject.FindObjectsOfType<MeshCollider>();

            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];
                if (!item.TryGetComponent<MeshRenderer>(out var a))
                {
                    GameObject.DestroyImmediate(item);
                }
            }
        }

        [MenuItem(itemName: "Utils/Select Current Virtual Camera", isValidateFunction: false, priority = 1)]
        public static void SelectCurrenteVirtualCamera()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("This function only works in Play Mode");
                return;
            }

            if (!Singleton.TryGetInstance(out CameraManager cam))
            {
                Debug.LogWarning("Camera Manager Dont found");
                return;
            }

            GameObject camera = cam.ActiveCamera.gameObject;
            if (camera == null)
                return;

            Selection.activeGameObject = camera;
        }
#endif
    }
}