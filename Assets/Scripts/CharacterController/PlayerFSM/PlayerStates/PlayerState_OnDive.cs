using AudioController;
using InputController;
using UnityEngine;
using UtilsComplements;

namespace AvatarController.PlayerFSM
{
    public class PlayerState_OnDive : PlayerState
    {
        private const string DIVE_ANIM_TRIGGER = "Dive";
        private const string DIVE_END_ANIM_TRIGGER = "DiveEnd";

        public override string Name => "OnDive";
        private readonly CharacterController _characterController;
        private readonly PlayerDive _playerDive;
        private readonly float _initialHeight;

        public PlayerState_OnDive(PlayerController playerController) : base(playerController)
        {
            _characterController = playerController.GetComponent<CharacterController>();
            _playerDive = playerController.GetComponent<PlayerDive>();
            _initialHeight = _characterController.height;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _characterController.height = _initialHeight / 2;

            if (Anim)
            {
                Anim.SetTrigger(DIVE_ANIM_TRIGGER);
            }

            AudioManager.GetAudioManager().PlayOneShot(Database.Player, "JUMP", _playerController.transform.position);
        }
        public override void OnPlayerStay(InputValues inputs)
        {
            if (Data.Powers.HasGhostView)
                _playerController.OnGhostView?.Invoke(inputs.GhostViewInput);

            if (_characterController.isGrounded) //Maybe send a raycast?
                _playerController.RequestChangeState(PlayerStates.OnGround);

            else
                _playerController.RequestChangeState(PlayerStates.OnAir);

            if (_playerController.CanGrab)
                _playerController.OnGrabCheck?.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();
            _characterController.height = _initialHeight;
            _playerController.StopFalling();
            if (Anim)
            {
                Anim.SetTrigger(DIVE_END_ANIM_TRIGGER);
                float yImpulse = Data.DefaultDiveValues.VerticalImpulse *
                    Data.DefOtherValues.ScaleMultiplicator;
                _playerController.AddImpulse(new(0, yImpulse, 0));
            }
        }

        public override bool CanAutoTransition()
        {
            return !_playerDive.IsDiving;
        }
    }
}