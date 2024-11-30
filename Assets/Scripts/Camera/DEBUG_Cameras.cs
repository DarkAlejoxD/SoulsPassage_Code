using Cinemachine;
using UnityEngine;

namespace Cameras
{
    public class DEBUG_Cameras : MonoBehaviour
    {
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyUp("o"))
            {
                var list = GameObject.FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.InstanceID);
                foreach (var item in list)
                {
                    if (item.enabled)
                        Debug.Log("This Camera is enabled:", item);
                }
            }
#endif
        }
    }
}
