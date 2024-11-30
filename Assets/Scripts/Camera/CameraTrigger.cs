using UnityEngine;
using Cinemachine;
using UtilsComplements;

namespace Cameras
{
    public class CameraTrigger : MonoBehaviour
    {
        private CameraManager _manager;
        [SerializeField] private CinemachineVirtualCamera _cam;
        [SerializeField] private bool isStartingCamera = false;

        private void Start()
        {
            _manager = ISingleton<CameraManager>.GetInstance();
            _cam.enabled = false;
            if (isStartingCamera)
            {
                _cam.enabled = true;
                _manager.SwitchCameras(_cam);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Camera confiner entered");
            if (other.CompareTag("Player"))
            {
                _manager.SwitchCameras(_cam);
            }
        }

        public void SetCamera()
        {
            _manager ??= Singleton.GetSingleton<CameraManager>();
            _manager.SwitchCameras(_cam);
        }
    }
}