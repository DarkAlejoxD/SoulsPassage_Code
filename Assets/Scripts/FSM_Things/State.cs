using System;

namespace FSM
{
    public interface IState
    {
        string Name { get; }

        void OnEnter();
        void OnExit();
        void OnStay();

        bool CanAutoTransition();
    }

    public class State : IState
    {
        public Action OnEnterDelegate;
        public Action OnStayDelegate;
        public Action OnExitDelegate;

        private readonly string NAME;

        public string Name => NAME;

        public State(Action onEnter, Action onStay, Action onExit,
                     string name = "Default Delegates State")
        {
            OnEnterDelegate = onEnter;
            OnStayDelegate = onStay;
            OnExitDelegate = onExit;

            NAME = name;
        }

        public virtual bool CanAutoTransition() => true;

        public void OnEnter() => OnEnterDelegate?.Invoke();

        public void OnExit() => OnExitDelegate?.Invoke();

        public void OnStay() => OnStayDelegate?.Invoke();
    }

    public delegate bool TransitionCondition();

    public struct Transition
    {
        public readonly TransitionCondition Condition;
        public readonly Action OnEndAction;

        public Transition(TransitionCondition condition)
        {
            Condition = condition;
            OnEndAction = null;
        }

        public Transition(TransitionCondition condition, Action onEndAction)
        {
            Condition = condition;
            OnEndAction = onEndAction;
        }
    }
}
