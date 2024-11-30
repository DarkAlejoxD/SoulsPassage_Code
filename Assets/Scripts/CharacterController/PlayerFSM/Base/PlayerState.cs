using InputController;
using FSM;
using AvatarController.Data;
using UnityEngine;

namespace AvatarController.PlayerFSM //Adaption of the FSM to the playerstats
{
    /// <summary>
    /// Call ReadInputs before OnStay
    /// </summary>
    public abstract class PlayerState : IState
    {
        protected PlayerController _playerController;

        public abstract string Name { get; }
        protected PlayerData Data => _playerController.DataContainer;
        protected Animator Anim => _playerController.ThisAnimator;

        public PlayerState(PlayerController playerController)
        {
            _playerController = playerController;
        }
        public virtual bool CanAutoTransition() => true;
        public virtual void OnEnter() { }
        public void OnStay() { }
        public abstract void OnPlayerStay(InputValues inputs);
        public virtual void OnExit() { }
    }
}