using System;
using UnityEngine;
using BaseGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComplements.Cheats
{
    [DisallowMultipleComponent]
    public class Cheat_TimeScaleControl : Cheat, ISingleton<Cheat_TimeScaleControl>
    {
        private const string KEYBOARD_CHEAT_CODE = "CHRONOS";
        private const string GAMEPAD_CHEAT_CODE = "NOSEQUEHACERAQUI";

        private const float VARIATION = 0.5f;

        [Header("Time controller")]
        [SerializeField, Range(0, 5)] private float _timeScaleControl = 1;
        private bool _controlInGame;
        private float _lastTimeScale;

        public ISingleton<Cheat_TimeScaleControl> Instance => this;

        protected override string KeyboardCheatReference => KEYBOARD_CHEAT_CODE;
        protected override string GamepadCheatReference => GAMEPAD_CHEAT_CODE;
        #region UnityLogic

        private void Awake()
        {
            Instance.Instantiate();
            _lastTimeScale = Time.timeScale;
            _controlInGame = false;
        }

        protected override void Update()
        {
            base.Update();
            if (_controlInGame)
                ReadInputKeyboardUpdate();
        }

        private void LateUpdate()
        {
            if (PauseManager.IsPaused)
            {
                Time.timeScale = 0;
                return;
            }

            TimeScaleUpdate();
        }

        private void OnDestroy()
        {
            Instance.RemoveInstance();
        }
        #endregion

        #region Public Methods
        public void Invalidate()
        {
            Destroy(this);
        }
        #endregion

        #region Private Methods
        protected override void ActivateCheat()
        {
            _controlInGame = true;
        }

        private void ReadInputKeyboardUpdate()
        {
            if (!Input.anyKeyDown)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha0))
                _timeScaleControl = 0;
            if (Input.GetKeyDown(KeyCode.Alpha1))
                _timeScaleControl = 1;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                _timeScaleControl = 2;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                _timeScaleControl = 3;
            if (Input.GetKeyDown(KeyCode.Alpha4))
                _timeScaleControl = 4;
            if (Input.GetKeyDown(KeyCode.Alpha5))
                _timeScaleControl = 5;

            if (Input.GetKeyDown(KeyCode.Plus))
                _timeScaleControl += VARIATION;

            if (Input.GetKeyDown(KeyCode.Minus))
                _timeScaleControl -= VARIATION;
        }

        private void TimeScaleUpdate()
        {
            if (_timeScaleControl != _lastTimeScale)
            {
                Time.timeScale = _timeScaleControl;
                _lastTimeScale = _timeScaleControl;
            }
        }
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        [MenuItem(itemName: "Cheats/Cheat_TimeScaleControl", isValidateFunction: false, priority = 1)]
        private static void ActivateCheatFromMenu()
        {
            if (!ISingleton<Cheat_TimeScaleControl>.TryGetInstance(out var cheat))
                return;

            cheat.CorrectCombination();
        }
#endif
        #endregion
    }
}
