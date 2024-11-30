using InputController;
using UnityEngine;

namespace AvatarController.PlayerFSM
{
    /// <summary>
    /// FreeMove, the regular state of the player
    /// </summary>
    public class PlayerState_DefaultMovement : PlayerState
    {
        private const string MOVEMENT_VALUE = "Speed";
        private const string ONGROUND_ANIM = "OnGround";
        private const string AFK_ON_TRIGGER = "AFKon";
        private const string AFK_OFF_TRIGGER = "AFKoff";
        private const float SMOOTH = 0.3f;

        private float _animControl = 0;

        private float _timeToIdle = 0;
        private float _timeControl = 0;
        private float _jumpTimeControl = 0;

        private bool _poltergeistActivated;
        private bool _isAFK;
        private bool _canJump;

        public override string Name => "Default Movement";
        private bool IsAFK
        {
            get
            {
                if (!Anim)
                    return false;

                return Anim.GetBool("isAFK");
            }
        }
        private bool IsTrigger
        {
            get
            {
                if (!Anim)
                    return false;

                return Anim.GetBool("isAFK");
            }
        }

        public PlayerState_DefaultMovement(PlayerController playerController) : base(playerController)
        {
            _poltergeistActivated = false;
        }

        public override void OnEnter()
        {
            _playerController.UnBlockMovement();
            //If necessary change the playerMovementData
            _poltergeistActivated = false;

            if (Anim)
                Anim.SetBool(ONGROUND_ANIM, true);

            _isAFK = false;
            _timeControl = 0;
            _timeToIdle = Data.DefOtherValues.TimeBreakIdle;
            _jumpTimeControl = Time.time;
        }

        public override void OnPlayerStay(InputValues inputs)
        {
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

            //Debug.Log($"BreakTime: {_timeControl}");

            //Anim Logic
            if (Anim)
            {
                // 0----min(x)----max(y)
                float speed = _playerController.Velocity.magnitude;
                float minSpeed = Data.DefaultMovement.MinSpeedToMove;
                float maxSpeed = Data.DefaultMovement.MaxSpeed;
                float value;

                //min(0) ---- max(y-x)
                maxSpeed -= minSpeed;
                speed -= minSpeed;
                value = speed / maxSpeed;
                value = Mathf.Clamp01(value);
                _animControl = Mathf.Lerp(_animControl, value, SMOOTH);

                Anim.SetFloat(MOVEMENT_VALUE, _animControl);
            }

            //Inputs Logic
            _playerController.OnMovement?.Invoke(inputs.MoveInput);
            if (Time.time > _jumpTimeControl + Data.DefaultJumpValues.JumpCD)
                _playerController.OnJump?.Invoke(inputs.JumpInput);
            //_playerController.OnDive?.Invoke(inputs.CrounchDiveInput);

            //_playerController.OnInteract?.Invoke(inputs.InteractInput);

            if (Data.Powers.HasGhostView)
                _playerController.OnGhostView?.Invoke(inputs.GhostViewInput);

            if (!Data.Powers.HasPoltergeist)
                return;

            if (inputs.Poltergeist && !_poltergeistActivated &&
                _playerController._canActivatePoltergeist)
            {
                _poltergeistActivated = true;
                _playerController.RequestChangeState(PlayerStates.OnPoltergeist);
            }
            //_playerController.OnSprint?.Invoke(inputs.SprintInput); ???

        }

        public override void OnExit()
        {
            base.OnExit();

            if (Anim)
            {
                Anim.SetBool(ONGROUND_ANIM, false);
                Anim.ResetTrigger(AFK_ON_TRIGGER);
                Anim.ResetTrigger(AFK_OFF_TRIGGER);
            }

            _isAFK = false;
        }
    }
}