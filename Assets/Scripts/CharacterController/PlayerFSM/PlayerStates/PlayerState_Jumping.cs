using UnityEngine;
using InputController;
using Unity.VisualScripting;

namespace AvatarController.PlayerFSM
{
    public class PlayerState_Jumping : PlayerState
    {
        //private const string ANIM_JUMP_TRIGGER = "Jump";

        private readonly PlayerJump _playerJump;
        private float _timeToPeak;
        private float _timeWhenJumpStarted;
        private bool _isJumping;

        public override string Name => "Jumping";

        public PlayerState_Jumping(PlayerController playerController) : base(playerController)
        {
            if (!playerController.TryGetComponent(out _playerJump))
            {
                _playerJump = playerController.AddComponent<PlayerJump>();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _timeToPeak = _playerJump.GetTimeToPeak();
            _timeWhenJumpStarted = Time.time;
            _isJumping = true;

            //if (Anim)
            //    Anim.SetTrigger(ANIM_JUMP_TRIGGER);
        }

        public override void OnPlayerStay(InputValues inputs)
        {
            _playerController.OnMovement?.Invoke(inputs.MoveInput);

            if (Data.Powers.HasGhostView)
                _playerController.OnGhostView?.Invoke(inputs.GhostViewInput);

            if (!_isJumping)
                return;

            if (!inputs.JumpInput)
            {
                _isJumping = false;
                float timePassed = Time.time - _timeWhenJumpStarted;
                float remainingTime = _timeToPeak - timePassed;

                remainingTime = Mathf.Clamp(remainingTime, 0, Data.DefaultJumpValues.ReleasePenalty);

                float Deceleration = (0 - _playerController.VelocityY) / remainingTime;

                _playerController.TwistGravity = Deceleration;
                _playerController.UseTwikedGravity = true;
                _playerController.ForceChangeState(PlayerStates.OnAir);
                return;
            }

            if (_timeWhenJumpStarted + _timeToPeak < Time.time)
            {
                _isJumping = false;
                _playerController.ForceChangeState(PlayerStates.OnAir);
                return;
            }

            if (_playerController.CanGrab)
                _playerController.OnGrabCheck?.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();
            _isJumping = false;
            _playerController._wasGrabbed = false;
        }
    }
}