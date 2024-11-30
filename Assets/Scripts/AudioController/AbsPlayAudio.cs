using UnityEngine;

namespace AudioController
{
    public abstract class AbsPlayAudio : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private bool _playOnAwake = false;

        private void Start()
        {
            if (_playOnAwake)
                PlaySound();
        }

        public abstract void PlaySound();
    }
}