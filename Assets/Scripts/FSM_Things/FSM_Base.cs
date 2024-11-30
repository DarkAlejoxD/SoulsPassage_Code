using System.Collections.Generic;

namespace FSM
{
    /// <summary>
    /// Base to easy create FSM
    /// Maybe can be optimized
    /// </summary>
    /// <typeparam name="TKey">Key, probably you can use any type as key</typeparam>
    /// <typeparam name="TValue">The value you wanna save and implement IState, you can use IState as TValue or use the FSM_Default Class</typeparam>
    public class FSM_Base<TKey, TValue> : IState where TValue : IState
    {
        //TODO: Test it
        private struct AutoTransition
        {
            public Transition TransitionTo;
            public TKey OtherState;

            public AutoTransition(Transition transition, TKey other)
            {
                TransitionTo = transition;
                OtherState = other;
            }
        }

        protected TKey _currentState;
        protected TKey _lastState;

        private Dictionary<TKey, TValue> _keyStatePair;
        private Dictionary<TKey, List<AutoTransition>> _autoTransitions;
        private bool _autoTransition = true;

        private readonly string NAME;

        public string Name => NAME + this[CurrentState].Name;
        public TKey CurrentState => _currentState;
        public TKey LastState => _lastState;

        #region FSM Logic
        public FSM_Base(string name = "Default FSM")
        {
            NAME = name;
            _keyStatePair = new();
            _autoTransitions = new();
        }

        public FSM_Base(TKey rootKey, TValue rootState, string name = "Default FSM")
        {
            _keyStatePair = new();
            _autoTransitions = new();
            SetRoot(rootKey, rootState);
            NAME = name;
        }

        protected TValue this[TKey key] => _keyStatePair[key];

        public virtual bool CanAutoTransition() => true;
        public virtual void OnEnter() => this[_currentState].OnEnter();
        public virtual void OnStay()
        {
            this[_currentState].OnStay();
            TransitionsUpdate();
        }

        public virtual void OnExit() => this[_currentState].OnExit();
        #endregion

        #region Public Methods
        public void AddState(TKey key, TValue value)
        {
            if (!_keyStatePair.ContainsKey(key))
                _keyStatePair.Add(key, value);

            else
                DEBUG_Warning("You are trying to add an existing key");
        }

        public void AddAutoTransition(TKey from, Transition condition, TKey to)
        {
            if (!_keyStatePair.ContainsKey(to))
            {
                DEBUG_Warning("This state doesn't exists in the curren context, try add the state before adding the transition");
                return;
            }

            if (!_autoTransitions.ContainsKey(from))
            {
                if (!_keyStatePair.ContainsKey(from))
                {
                    DEBUG_Warning("This state doesn't exists in the curren context, try add the state before adding the transition");
                    return;
                }
                _autoTransitions.Add(from, new());
            }
            AutoTransition transition = new(condition, to);
            _autoTransitions[from].Add(transition);
        }

        public void SetRoot(TKey rootKey, TValue rootState)
        {
            if (_keyStatePair.ContainsKey(rootKey))
            {
                DEBUG_Warning("This key already exists");
                return;
            }
            _keyStatePair.Add(rootKey, rootState);
            _currentState = rootKey;
            this[_currentState].OnEnter();
        }

        /// <summary>
        /// Use this if you don't care if the current state is currently doing something
        /// </summary>
        /// <param name="state"></param>
        public void ForceChange(TKey state)
        {
            if (_keyStatePair.ContainsKey(state))
                ChangeState(state);

            else
                DEBUG_Warning("This key doesn't exist, remain in the same state");
        }

        /// <summary>
        /// Use this if you do care if the current state is currently doing smthgh
        /// </summary>
        /// <param name="state"></param>
        public void RequestChange(TKey state)
        {
            if (!_keyStatePair.ContainsKey(state))
            {
                DEBUG_Warning("This key doesn't exist, remain in the same state");
                return;
            }

            if (!this[_currentState].CanAutoTransition())
                return;

            ChangeState(state);
        }

        public void ReturnLastState()
        {
            TKey handler = _currentState;
            this[_currentState].OnExit();
            _currentState = _lastState;
            _lastState = handler;
            this[_currentState].OnEnter();
        }
        #endregion

        protected void ChangeState(TKey state)
        {
            if (state.Equals(_currentState))
            {
                DEBUG_Warning("Demanding change to the same State");
                return;
            }

            this[_currentState].OnExit();
            _lastState = _currentState;
            _currentState = state;
            this[_currentState].OnEnter();
            _autoTransition = true;
        }

        protected void TransitionsUpdate()
        {
            if (!_autoTransition)
                return;

            if (!_autoTransitions.ContainsKey(_currentState))
            {
                _autoTransition = false;
                return;
            }

            if (!this[_currentState].CanAutoTransition())
                return;

            var list = _autoTransitions[_currentState];

            for (int i = 0; i < list.Count; i++)
            {
                AutoTransition trans = list[i];
                if (trans.TransitionTo.Condition())
                {
                    trans.TransitionTo.OnEndAction?.Invoke();
                    ForceChange(trans.OtherState);
                    break;
                }
            }
        }

        protected static void DEBUG_Warning(string text)
        {
#if UNITY_2017_1_OR_NEWER
            UnityEngine.Debug.LogWarning(text);
#else
            System.Console.WriteLine(text);
#endif
        }
    }

    /// <summary>
    /// Shortcut to the FSM_Base where the state is the base state
    /// </summary>
    /// <typeparam name="TKey">The key value you wanna index your FSM</typeparam>
    public class FSM_Default<TKey> : FSM_Base<TKey, IState>
    {
    }
}
