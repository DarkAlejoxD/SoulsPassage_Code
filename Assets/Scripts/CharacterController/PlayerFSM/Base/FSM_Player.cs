using FSM;
using InputController;

namespace AvatarController.PlayerFSM
{
    /// <summary>
    /// Use this as root because it's a bit different from the base FSM.
    /// It uses FSM_Base as base
    /// </summary>
    public class FSM_Player<TKey> : FSM_Base<TKey, PlayerState>
    {
        //TODO: Test, and maybe write all the logic here?
        public FSM_Player(string name = "FSM_Player") : base(name)
        {
        }

        public void StayPlayer(InputValues inputs)
        {
            TransitionsUpdate();
            this[_currentState].OnPlayerStay(inputs);
        }
    }
}