using System;
using UnityEngine;
using UtilsComplements;

namespace InputController
{
    public class InputManager : MonoBehaviour, ISingleton<InputManager>
    {
        public Action<InputValues> OnInputDetected;

        private PlayerMap _playerMap;
        private InputValues _inputValues;

        public ISingleton<InputManager> Instance => this;

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();

            _playerMap = new();
            SetPlayerMapActive(true);
            SetPoltergeistActive(false);
            _inputValues = new InputValues();
        }

        private void Update()
        {
            //Reset Inputs & then read them
            _inputValues.ResetInputs();

            //PlayerMove
            MoveUpdate();
            JumpUpdate();
            CrounchDive();
            SprintUpdate();
            InteractUpdate();
            GhostViewUpdate();
            PoltActiveUpdate();

            //PoltergeistMove
            PolterCancelUpdate();
            SelectDeselectUpdate();
            PoltergeistMovementUpdate();
            PoltergeistYMovementUpdate();

            //Send Inputs
            OnInputDetected?.Invoke(_inputValues);
        }

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Public Methods
        public void SetPlayerMapActive(bool value)
        {
            if (value)
                _playerMap.PlayerMove.Enable();
            else
                _playerMap.PlayerMove.Disable();
        }

        public void SetPoltergeistActive(bool value)
        {
            if (value)
                _playerMap.Poltergeist.Enable();
            else
                _playerMap.Poltergeist.Disable();
        }
        #endregion

        #region Private Methods
        private void MoveUpdate()
        {
            Vector2 movement = _playerMap.PlayerMove.Move.ReadValue<Vector2>();
            if (movement.magnitude > 1)
                movement.Normalize();

            _inputValues.MoveInput = movement;
        }

        private void JumpUpdate()
        {
            float value = _playerMap.PlayerMove.Jump.ReadValue<float>();
            _inputValues.JumpInput = value > 0;
        }

        private void InteractUpdate()
        {
            bool triggered = _playerMap.PlayerMove.Interact.WasReleasedThisFrame();
            _inputValues.InteractInput = triggered;
        }

        private void GhostViewUpdate()
        {
            bool triggered = _playerMap.PlayerMove.SurvivalInstinct.WasReleasedThisFrame();
            _inputValues.GhostViewInput = triggered;
        }

        private void CrounchDive()
        {
            bool triggered = _playerMap.PlayerMove.CrouchDive.IsPressed();
            _inputValues.CrounchDiveInput = triggered;
        }

        private void SprintUpdate()
        {
            bool triggered = _playerMap.PlayerMove.Sprint.IsPressed();
            _inputValues.SprintInput = triggered;
        }

        private void PoltActiveUpdate()
        {
            bool triggered = _playerMap.PlayerMove.Poltergeist.WasReleasedThisFrame();
            _inputValues.Poltergeist = triggered;
        }

        private void SelectDeselectUpdate()
        {
            bool triggered = _playerMap.Poltergeist.SelectDeselect.WasReleasedThisFrame();

            _inputValues.SelectDeselectInput = triggered;
        }

        private void PoltergeistMovementUpdate()
        {
            Vector2 movement = _playerMap.Poltergeist.Move.ReadValue<Vector2>();
            if (movement.magnitude > 1)
                movement.Normalize();

            _inputValues.PoltergeistXZAxis = movement;
        }

        private void PoltergeistYMovementUpdate()
        {
            float y = _playerMap.Poltergeist.YAxis.ReadValue<float>();

            _inputValues.PoltergeistYAxis = y;
        }

        private void PolterCancelUpdate()
        {
            bool triggered = _playerMap.Poltergeist.Cancel.WasReleasedThisFrame();

            _inputValues.Cancel = triggered;
        }
        #endregion
    }
}
