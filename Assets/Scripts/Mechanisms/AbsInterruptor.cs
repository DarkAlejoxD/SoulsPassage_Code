using UnityEngine;
using BaseGame;

namespace Mechanisms
{
    [ExecuteAlways]
    public abstract class AbsInterruptor : MonoBehaviour, IKeyInterruptor, IResetable
    {
        #region Fields
        private KeyManager _keyManager;
        public bool IsKeyActivated { get; set; }
        #endregion

        #region Unity Logic
        private void Awake() => IsKeyActivated = false;

        private void OnEnable()
        {
            _keyManager = GetComponentInParent<KeyManager>();
            _keyManager.AddToInterruptorList(this);
        }

        private void OnDisable()
        {
            _keyManager.RemoveFromInterruptorList(this);
        }
        #endregion

        #region Public Methods
        public /*virtual*/ void Activate()
        {
            IsKeyActivated = true;
            _keyManager.SendSignal();
        }

        public /*virtual*/ void Deactivate()
        {
            IsKeyActivated = false;
            _keyManager.SendSignal();
        }

        public void Reset()
        {
            Debug.Log("Reset not implemented");
        }
        #endregion
    }
}
