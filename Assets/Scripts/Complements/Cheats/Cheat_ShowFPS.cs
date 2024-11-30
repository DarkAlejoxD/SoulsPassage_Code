using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComplements.Cheats
{
    public class Cheat_ShowFPS : Cheat, ISingleton<Cheat_ShowFPS>
    {
        private const string KEYBOARD_CHEAT_CODE = "FPS";
        private const string GAMEPAD_CHEAT_CODE = "[Insert code without spaces]";

        [SerializeField] private GameObject _fpsCanvas;

        public ISingleton<Cheat_ShowFPS> Instance => this;

        protected override string KeyboardCheatReference => KEYBOARD_CHEAT_CODE;
        protected override string GamepadCheatReference => GAMEPAD_CHEAT_CODE;
        protected override CheatTypeEnum CheatType => CheatTypeEnum.TOOGLEABLE;

        #region UnityLogic

        private void Awake() => Instance.Instantiate();

        //protected override void Start()
        //{
        //    base.Start();
        //}

        //protected override void Update()
        //{
        //    base.Update();
        //}

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Public Methods //Uncomment this if you dont want to delete the gameObject. Destroys only the component.
        public void Invalidate()
        {
            Destroy(this);
        }
        #endregion

        #region Private Methods
        protected override void ActivateCheat()
        {
            _fpsCanvas.SetActive(true);
        }


        protected override void DeactivateCheat()
        {
            _fpsCanvas.SetActive(false);
            //Write here if want a toogleCheat(?, delete it otherwise
        }

        #endregion

        #region DEBUG
#if UNITY_EDITOR
        [MenuItem(itemName: "Cheats/Cheat_ShowFPS", isValidateFunction: false, priority = 1)]
        private static void ActivateCheatFromMenu()
        {
            if (!ISingleton<Cheat_ShowFPS>.TryGetInstance(out var cheat))
                return;

            cheat.CorrectCombination();
        }
#endif
        #endregion
    }
}
