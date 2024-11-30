using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UtilsComplements;

namespace AudioController
{
    public class AudioManager : MonoBehaviour, ISingleton<AudioManager>
    {
        #region Fields
        [Header("References")]
        private AudioReferences _playerAudioData;
        private AudioReferences _sfxAudioData;
        private Dictionary<EventReference, EventInstance> _audioInstances = new();
        private EventInstance? _currentAmbienceSound = null;

        public ISingleton<AudioManager> Instance => this;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();
            _audioInstances = new();
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            _playerAudioData = Resources.Load<AudioReferences>("AudioData/PlayerAudioData");
            _sfxAudioData = Resources.Load<AudioReferences>("AudioData/SFXAudioData");
        }

        private void OnDestroy()
        {
            Instance.RemoveInstance();
            CleanUp();
        }
        #endregion

        #region Static Methods

        public static AudioManager GetAudioManager()
        {
            if (Singleton.TryGetInstance(out AudioManager manager))
                return manager;

            GameObject gameObject = new("AudioManager");
            gameObject.AddComponent<AudioManager>();
            return GetAudioManager();
        }

        #endregion

        #region Public Methods
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }

        public void PlayOneShot(Database bank, string name, Vector3 worldPos)
        {
            EventReference audioEvent = default;
            switch (bank)
            {
                case Database.Player:
                    audioEvent = _playerAudioData.GetEvent(name);
                    break;
                case Database.SFX:
                    audioEvent = _sfxAudioData.GetEvent(name);
                    break;
                case Database.Music:
                    break;
            }
            PlayOneShot(audioEvent, worldPos);
        }

        public void CrossFadeMusic(EventReference eventRef, float time = 1)
        {
            EventInstance instance;

            if (_audioInstances.ContainsKey(eventRef))
                instance = _audioInstances[eventRef];
            else
                instance = CreateEventInstance(eventRef);

            if (_currentAmbienceSound == null)
            {
                _currentAmbienceSound = instance;
                _currentAmbienceSound.Value.start();
            }
        }

        public EventInstance CreateEventInstance(EventReference eventRef)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
            _audioInstances.Add(eventRef, eventInstance);
            return eventInstance;
        }

        public void StopAmbience(EventReference eventRef)
        {
            if (!_audioInstances.ContainsKey(eventRef))
                return;
            EventInstance instance = _audioInstances[eventRef];
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }

        public EventInstance CreateEventInstance(Database bank, string name)
        {
            EventReference eventRef = default;

            switch (bank)
            {
                case Database.Player:
                    eventRef = _playerAudioData.GetEvent(name);
                    break;
                case Database.Ambience:
                    break;
                case Database.Music:
                    break;
            }

            return CreateEventInstance(eventRef);
        }

        public bool CheckIfEventInstanceExists(EventReference eventRef)
        {
            return _audioInstances.ContainsKey(eventRef);
        }

        public void RemoveInstance(EventReference eventRef)
        {
            if (!_audioInstances.ContainsKey(eventRef))
                return;

            EventInstance instance = _audioInstances[eventRef];
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();

            _audioInstances.Remove(eventRef);
        }
        #endregion

        #region Private Methods
        private void CleanUp()
        {
            foreach (var item in _audioInstances)
            {
                item.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                item.Value.release();
            }
        }
        #endregion
    }
}