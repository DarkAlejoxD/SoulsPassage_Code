using System.Collections.Generic;
using UnityEngine;
using BaseGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mechanisms
{
    [DisallowMultipleComponent]
    public class KeyManager : MonoBehaviour, IResetable
    {
        #region Fields
        [Header("Type")]
        private List<IKeyInterruptor> _keyInterruptors = new(1);
        private List<IMechanism> _mechanisms = new(1);
        private bool _isActive;
        #endregion

        #region Unity Logic
        private void Start() => SendSignal();
        public void Reset()
        {
            Debug.Log("Reset Not implemented in KeyManager");
        }
        #endregion

        #region Public Methods
        public void AddToInterruptorList(IKeyInterruptor key)
        {
            _keyInterruptors ??= new List<IKeyInterruptor>(1);

            if (!_keyInterruptors.Contains(key))
                _keyInterruptors.Add(key);
        }

        public void RemoveFromInterruptorList(IKeyInterruptor key)
        {
            if (_keyInterruptors.Contains(key))
                _keyInterruptors.Remove(key);
        }

        public void AddToMechanismList(IMechanism mec)
        {
            _mechanisms ??= new(1);

            if (!_mechanisms.Contains(mec))
                _mechanisms.Add(mec);
        }

        public void RemoveFromMechanismList(IMechanism mec)
        {
            if (_mechanisms.Contains(mec))
                _mechanisms.Remove(mec);
        }

        public void SendSignal() => ReceiveSignal();
        #endregion

        #region Private Methods
        private void ReceiveSignal()
        {
            if (_keyInterruptors.Count <= 0)
            {
                _isActive = true;
            }
            else
            {
                _isActive = true;
                foreach (var item in _keyInterruptors)
                {
                    if (!item.IsKeyActivated)                    
                        _isActive = false;                    
                }
            }
            foreach (var item in _mechanisms)
            {
                item.SendSignal(_isActive);
            }
        }
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        [Header("DEBUG")]
        [SerializeField] private bool DEBUG_draw = false;

        private void OnDrawGizmos()
        {
            if (!DEBUG_draw)
                return;

            Vector3 center = Vector3.zero;
            float spaceDots = 20;

            //Calculate the center
            foreach (var item in _keyInterruptors)
            {
                if (item is Component comp)
                    center += comp.transform.position;
            }
            foreach (var item in _mechanisms)
            {
                if (item is Component comp)
                    center += comp.transform.position;
            }
            center /= (_keyInterruptors.Count + _mechanisms.Count);

            //Draw Lines
            foreach (var item in _keyInterruptors)
            {
                Handles.color = item.IsKeyActivated ? Color.green : Color.red;
                if (item is Component comp)
                    Handles.DrawDottedLine(comp.transform.position, center, spaceDots);
            }
            foreach (var item in _mechanisms)
            {
                Handles.color = item.IsActive ? Color.green : Color.gray;
                if (item is Component comp)
                    Handles.DrawDottedLine(comp.transform.position, center, spaceDots);
            }
            Handles.color = Color.white;
        }
#endif
        #endregion
    }
}
