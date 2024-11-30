using UnityEngine;
using BaseGame;
using AvatarController.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComplements.Cheats
{
    public class Cheat_PlayerPowers : Cheat, ISingleton<Cheat_PlayerPowers>
    {
        private const string KEYBOARD_CHEAT_CODE = "GODMODE";
        private const string GAMEPAD_CHEAT_CODE = "[Insert code without spaces]";

        public ISingleton<Cheat_PlayerPowers> Instance => this;

        protected override string KeyboardCheatReference => KEYBOARD_CHEAT_CODE;
        protected override string GamepadCheatReference => GAMEPAD_CHEAT_CODE;
        //protected override CheatTypeEnum CheatType => base.CheatType;

        #region UnityLogic

        private void Awake()
        {
            Instance.Instantiate();
        }

        //protected override void Start()
        //{
        //    base.Start();
        //}

        //protected override void Update()
        //{
        //    base.Update();
        //}

        private void OnDestroy()
        {
            Instance.RemoveInstance();
        }
        #endregion

        #region Public Methods //Uncomment this if you dont want to delete the gameObject. Destroys only the component.
        public void Invalidate() => Destroy(this);
        #endregion

        #region Private Methods
        protected override void ActivateCheat()
        {
            GameManager.GetGameManager().PlayerInstance.DataContainer.Powers.CHEAT_testPowers = true;
        }

        /*
        protected override void DeactivateCheat()
        {
            //Write here if want a toogleCheat(?, delete it otherwise
        }
        */
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        [MenuItem(itemName: "Cheats/Cheat_PlayerPowers", isValidateFunction: false, priority = 1)]
        private static void ActivateCheatFromMenu()
        {
            if (!ISingleton<Cheat_PlayerPowers>.TryGetInstance(out var cheat))
                return;

            cheat.CorrectCombination();
        }
#endif
        #endregion
    }
}
