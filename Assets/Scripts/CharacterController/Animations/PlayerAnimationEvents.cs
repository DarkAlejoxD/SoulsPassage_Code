using UnityEngine;
using AudioController;
using AvatarController.Misc;

namespace AvatarController.Animations
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        [Header("Player References")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private FootDetection _foot;

        [Header("Step References")]
        [SerializeField] private ParticleSystem _stepsSmoke;
        [SerializeField, Range(0, 1)] private float _stepProbability = 0.7f;
        [SerializeField, Range(0, 1)] private float _stepSpeedThreshold = 0.01f;

        [Header("Jump References")]
        [SerializeField] private ParticleSystem _jumpSmoke;
        [SerializeField] private Transform _jumpPivot;
        [SerializeField, Min(1)] private int _jumpParticlesCount = 50;

        #region Unity Logic
        private void Awake()
        {
            _jumpSmoke.transform.SetParent(null, true);
        }
        #endregion

        #region Public Methods
        public void Step()
        {
            float current = _player.Velocity.magnitude;
            float minSpeed = _player.DataContainer.DefaultMovement.MinSpeedToMove;
            float maxSpeed = _player.DataContainer.DefaultMovement.MaxSpeed;
            float minSpeedPct = Mathf.Lerp(minSpeed, maxSpeed, _stepSpeedThreshold);

            string stepType;
            //Debug.Log("FloorType: " + _foot.FloorType);
            switch (_foot.FloorType)
            {
                case FloorType.METAL:
                    stepType = "STEP_METAL";
                    break;
                case FloorType.CARPET:
                    stepType = "STEP_CARPET";
                    break;
                case FloorType.CLOUD:
                    stepType = "STEP_CLOUD";
                    break;
                default:
                    stepType = "STEP_WOOD";
                    break;
            }

            if (current > minSpeedPct)
            {
                PlayOneShot(Database.Player, stepType, transform.position);
                _stepsSmoke.Play();
            }
            else
            {
                _stepsSmoke.Stop();
                return;
            }

            float rnd = Random.value;
            if (rnd < _stepProbability)
                _stepsSmoke.Emit(1);
            //_stepsSmoke.Play();
            //Debug.Log("Step SoundAndParticles");
        }

        public void Jump()
        {
            _jumpSmoke.transform.position = _jumpPivot.position;
            //PlayOneShot(Database.Player, "JUMP", transform.position);
            _jumpSmoke.Emit(_jumpParticlesCount);
        }

        public void FallHit()
        {
            PlayOneShot(Database.Player, "FALL_HIT", transform.position);
        }

        public void Frontflip()
        {
            PlayOneShot(Database.Player, "FRONTFLIP", transform.position);
        }
        #endregion

        #region Private Methods
        private void PlayOneShot(Database database, string name, Vector3 position)
        {
            AudioManager.GetAudioManager().PlayOneShot(database, name, position);
        }

        #endregion
    }

    public enum FloorType
    {
        WOODY,
        METAL,
        CARPET,
        CLOUD
    }
}
