using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace AudioController
{
    [CreateAssetMenu(fileName = "New Audio DataBase", menuName = "AudioController/New AudioDataBase", order = 1)]
    public class AudioReferences : ScriptableObject
    {
        [Header("References")]
        [SerializeField] private List<AudioFile> _audioFiles = new();
        private Dictionary<string, AudioFile> _audioFilesDictionary;

        private void OnEnable()
        {
            _audioFilesDictionary = new Dictionary<string, AudioFile>();

            foreach (var item in _audioFiles)
            {
                _audioFilesDictionary.Add(item.Name.ToUpper(), item);
            }
        }

        public EventReference GetEvent(string name)
        {
            return _audioFilesDictionary[name.ToUpper()].Event;
        }

        /// <summary>
        /// Use this to DEBUG
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public EventReference GetEvent(string name, UnityEngine.Object context)
        {
            if (!_audioFilesDictionary.ContainsKey(name.ToUpper()))
            {
                Debug.LogError("Don't found this index", context);
                return default;
            }

            return GetEvent(name);
        }
    }

    [Serializable]
    public class AudioFile
    {
        public string Name;
        public EventReference Event;
    }
}