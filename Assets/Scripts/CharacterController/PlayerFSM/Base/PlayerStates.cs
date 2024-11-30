namespace AvatarController.PlayerFSM
{
    public enum PlayerStates
    {
        OnGround,
        OnlyMove,
        Jumping,
        OnAir,
        OnDive,

        Grabbing,
        Pushing,
        OnPoltergeist
    }
}