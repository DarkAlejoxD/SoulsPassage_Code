using System.Collections;
using UnityEngine;
using Cinemachine;
using BaseGame;
using UtilsComplements;
using AvatarController;

namespace Cameras
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraEvent : MonoBehaviour
    {
        [Header("References")]
        private CinemachineVirtualCamera _thisCamera;
        private CinemachineVirtualCamera _lastCamera;

        [SerializeField] private float _timeStolen = 3;

        private void Awake()
        {
            _thisCamera = GetComponent<CinemachineVirtualCamera>();
            _thisCamera.enabled = false;
        }

        public void ActivateCamera()
        {
            StartCoroutine(SwitchCameraCoroutine());
        }

        private IEnumerator SwitchCameraCoroutine()
        {
            if (Singleton.TryGetInstance(out CameraManager manager))
            {
                AuxiliarCamera auxiliar = Singleton.GetSingleton<AuxiliarCamera>();
                auxiliar?.SetAuxiliarActive(false);

                PlayerController player = GameManager.GetGameManager().PlayerInstance;
                player.SetOnlyMove();
                player.BlockMovement();

                _lastCamera = manager.ActiveCamera;
                manager.SwitchCameras(_thisCamera);

                yield return new WaitForSeconds(_timeStolen);
                manager.SwitchCameras(_lastCamera);
                player.UnBlockMovement();
                player.SetDefaultMovement();

                const float transition_time = 2;

                yield return new WaitForSeconds(transition_time);
                auxiliar?.SetAuxiliarActive(true);
            }
            else
                yield return null;
        }
    }
}
