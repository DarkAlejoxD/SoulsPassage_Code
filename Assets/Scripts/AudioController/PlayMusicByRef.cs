using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace AudioController
{
    public class PlayMusicByRef : AbsPlayMusic
    {
        [Header("References")]
        [SerializeField] private EventReference _event;
        EventInstance? _eventIns;

        public override void PlaySound()
        {
            var audioManager = AudioManager.GetAudioManager();
            if (_eventIns == null)
                _eventIns = audioManager.CreateEventInstance(_event);
            _eventIns.Value.start();
        }
        public override void StopSound()
        {
            _eventIns.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void StopInmediate()
        {
            _eventIns.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        }
    }
}