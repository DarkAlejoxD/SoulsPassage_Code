using UnityEngine;

namespace MenuManagement.Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "GameData/New Settings")]
    public class Settings : ScriptableObject
    {
        //private bool _resetOnRestart = true;

        [Header("Screen Settings")]
        [SerializeField] private bool _cameraShake;
        [SerializeField] private bool _cameraTurbulence;

        [Header("Sound Settings")]
        [SerializeField, Range(0, 1)] private float _masterVolume = 1;
        [SerializeField, Range(0, 1)] private float _soundVolume;
        [SerializeField, Range(0, 1)] private float _ambientVolume;

        [Header("Graphic Settings")]
        [SerializeField] private Resolution _resolution;

        #region Getters
        //Sreen
        public bool CameraShake => _cameraShake;
        public bool CameraTurbulence => _cameraTurbulence;

        //Sound
        public float MasterVolume => _masterVolume;
        public float AmbientVolume => _ambientVolume;
        public float SoundVolume => _soundVolume;

        //Graphics
        public Resolution Resolution => _resolution;
        #endregion


        #region Public Methods
        //Screen
        public void SetCameraShake(bool b) { _cameraShake = b; }
        public void SetCameraTurbulence(bool b) { _cameraTurbulence = b; }

        //Sound
        public void SetMasterVolume(float v) { _masterVolume = v; } //Expand this later
        public void SetAmbientVolume(float v) { _ambientVolume = v; } //Expand this later
        public void SetSoundVolume(float v) { _soundVolume = v; } //Expand this later

        //Graphics

        //public void SetResolution(Resolution resolution) { _resolution = resolution; } //Expand this later

        #endregion

    }

    public enum Resolutions { r1920x1080, r1280x720 }

}
