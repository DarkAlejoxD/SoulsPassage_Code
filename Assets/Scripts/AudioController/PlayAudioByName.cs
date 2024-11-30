using UnityEngine;

namespace AudioController
{
    public class PlayAudioByName : AbsPlayAudio
    {
        [Header("References")]
        [SerializeField] private Database _bank;
        [SerializeField] private string _name;

        public override void PlaySound()
        {
            AudioManager.GetAudioManager().PlayOneShot(_bank, _name.ToUpper(), transform.position);
        }
    }
}