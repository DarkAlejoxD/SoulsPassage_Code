using UnityEngine;
using UtilsComplements;

namespace Poltergeist
{
    public class CameraPolter : MonoBehaviour, ISingleton<CameraPolter>
    {
        #region Fields
        [Header("PostProcessVolume")]
        [SerializeField] private Transform _volume;
        [SerializeField] private float _distanceAway = 20;

        public ISingleton<CameraPolter> Instance => this;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();
            Deactivate();
        }

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Static Methods
        public static void ActivatePolterMode()
        {
            if (!Singleton.TryGetInstance(out CameraPolter camera))
                return;

            camera.Activate();
        }

        public static void DeactivatePolterMode()
        {
            if (!Singleton.TryGetInstance(out CameraPolter camera))
                return;

            camera.Deactivate();
        }
        #endregion

        #region Private Methods
        private void Activate()
        {
            _volume.position = transform.position;
        }

        private void Deactivate()
        {
            _volume.position = transform.position + transform.up * _distanceAway;
        }
        #endregion
    }
}
