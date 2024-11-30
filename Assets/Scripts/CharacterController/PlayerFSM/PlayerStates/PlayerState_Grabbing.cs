using UnityEngine;
using InputController;
using static UtilsComplements.AsyncTimer;
using AvatarController.LedgeGrabbing;

namespace AvatarController.PlayerFSM
{
    public class PlayerState_Grabbing : PlayerState
    {
        //[Range(0, 1)] private const float MIN_DOT_TO_LET_GO = 0.5f;
        private const string GRAB_ANIM_TRIGGER = "Grabbing";
        private const string GRAB_ANIM_BOOL = "OnGrab";
        private const string MOVEMENT_VALUE = "GrabSpeed";
        private const string AFK_ON_TRIGGER = "AFKon";
        private const string AFK_OFF_TRIGGER = "AFKoff";
        private const float SMOOTH = 0.2f;

        private float _animControl = 0;

        private float _timeToIdle = 0;
        private float _timeControl = 0;

        private bool IsAFK
        {
            get
            {
                if (!Anim)
                    return false;

                return Anim.GetBool("isAFK");
            }
        }

        public override string Name => "OnGrab";
        public PlayerState_Grabbing(PlayerController playerController) : base(playerController)
        {
            playerController.CanGrab = true;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Anim)
            {
                Anim.SetTrigger(GRAB_ANIM_TRIGGER);
                Anim.SetBool(GRAB_ANIM_BOOL, true);
            }
            _playerController._wasGrabbed = true;
            //_playerController.SetGravityActive(false);
            //_playerController.VelocityY = 0;

            _timeControl = 0;
            _timeToIdle = Data.DefOtherValues.TimeBreakIdle;
            _playerController.StopVelocity();
            //_playerController.StopFalling();

            PlayerLedgeGrab.ActivateLimits?.Invoke();
        }

        public override void OnPlayerStay(InputValues inputs)
        {
            _playerController.OnGrabUpdate?.Invoke();

            if (_timeControl > _timeToIdle)
            {
                _timeControl = 0;
                _timeToIdle = Data.DefOtherValues.TimeBreakIdle;
                if (Anim)
                    Anim.SetTrigger(AFK_ON_TRIGGER);
            }

            //if (_playerController.Velocity.magnitude > Data.DefaultMovement.MinSpeedToMove)
            if (inputs.MoveInput.magnitude > 0)
            {
                _timeControl = 0;
                if (Anim && IsAFK)
                    Anim.SetTrigger(AFK_OFF_TRIGGER);
                if (!IsAFK && Anim)
                    Anim.ResetTrigger(AFK_OFF_TRIGGER);
            }
            _timeControl += Time.deltaTime;

            if (Anim)
            {
                // 0----min(x)----max(y)
                float speed = _playerController.Velocity.magnitude;
                float minSpeed = Data.GrabbingLedgeMovement.MinSpeedToMove;
                float maxSpeed = Data.GrabbingLedgeMovement.MaxSpeed;
                float value;

                //min(0) ---- max(y-x)
                maxSpeed -= minSpeed;
                speed -= minSpeed;
                value = speed / maxSpeed;
                value = Mathf.Clamp01(value);
                _animControl = Mathf.Lerp(_animControl, value, SMOOTH);

                Anim.SetFloat(MOVEMENT_VALUE, _animControl);
            }

            _playerController.OnMovement?.Invoke(inputs.MoveInput);
            _playerController.OnJump?.Invoke(inputs.JumpInput);
            if (Data.Powers.HasGhostView)
                _playerController.OnGhostView?.Invoke(inputs.GhostViewInput);

            //float dot = Vector2.Dot(inputs.MoveInput, Vector2.down);
            //if (dot > MIN_DOT_TO_LET_GO)
            //    _playerController.GetComponent<PlayerLedgeGrab>().LetGoLedge();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (Anim)
            {
                Anim.ResetTrigger(GRAB_ANIM_TRIGGER);
                Anim.SetBool(GRAB_ANIM_BOOL, false);
            }

            //_playerController.SetGravityActive(true);
            _playerController.CanGrab = false;

            _playerController.StartCoroutine(TimerCoroutine(Data.DefOtherValues.GrabCD,
                () => _playerController.CanGrab = true));

            PlayerLedgeGrab.DeactivateLimits?.Invoke();
        }
    }
}