using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace AudioController
{
    public class PlayAudioByRef : AbsPlayAudio
    {
        [Header("References")]
        [SerializeField] private EventReference _event;

        public override void PlaySound()
        {
            AudioManager.GetAudioManager().PlayOneShot(_event, transform.position);
        }
    }
}