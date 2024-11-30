using InputController;

namespace AvatarController.PlayerFSM
{
    public class PlayerState_OnAir : PlayerState
    {
        private const string ANIM_BOOL_ONAIR = "OnAir";
        public override string Name => "OnAir";

        private bool _jumpButtonPressed;

        public PlayerState_OnAir(PlayerController playerController) : base(playerController)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (Anim)
            {
                Anim.SetBool(ANIM_BOOL_ONAIR, true);
            }

            _jumpButtonPressed = true;
        }

        public override void OnPlayerStay(InputValues inputs)
        {
            if (!inputs.JumpInput)
                _jumpButtonPressed = false;

            if (!_jumpButtonPressed)
                _playerController.OnDive?.Invoke(inputs.CrounchDiveInput);

            _playerController.OnMovement?.Invoke(inputs.MoveInput);

            if (Data.Powers.HasGhostView)
                _playerController.OnGhostView?.Invoke(inputs.GhostViewInput);

            if (_playerController.CanGrab)
                _playerController.OnGrabCheck?.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();

            if (Anim)
            {
                Anim.SetBool(ANIM_BOOL_ONAIR, false);
            }
        }
    }
}