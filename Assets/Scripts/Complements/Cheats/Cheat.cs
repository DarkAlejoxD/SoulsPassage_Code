using UnityEngine;

namespace UtilsComplements.Cheats
{
    /// <summary>
    /// Mickey Tool to easy create Cheats
    /// </summary>
    public abstract class Cheat : MonoBehaviour
    {
        protected enum CheatTypeEnum
        {
            ONCE, REINVOKE, TOOGLEABLE
        }

        [Header("DEBUG", order = 22)]
        [SerializeField] private bool DEBUG_TestCheat = false;

        private int _cheatIndexControl;
        protected bool AlreadyInvoked { get; private set; }
        protected virtual CheatTypeEnum CheatType => CheatTypeEnum.ONCE;
        protected abstract string KeyboardCheatReference { get; }
        protected abstract string GamepadCheatReference { get; }

        protected virtual void Start()
        {
            _cheatIndexControl = 0;
        }

        protected virtual void Update()
        {
            KeyboardCheatUpdate();
        }

        private void KeyboardCheatUpdate()
        {
            if (!Input.anyKeyDown)
                return;

            if (CheatType != CheatTypeEnum.TOOGLEABLE)
            {
                if (AlreadyInvoked)
                    return;
            }

            char expectedChar = KeyboardCheatReference[_cheatIndexControl];

            //if (DEBUG_TestCheat)
            //    Debug.Log("Expected Char: " + expectedChar, this);

            if (!Input.GetKeyDown(expectedChar.ToString().ToLower()))
            {
                _cheatIndexControl = 0;
                return;
            }

            int cheatLenght = KeyboardCheatReference.Length;

            if (_cheatIndexControl >= cheatLenght - 1)
                CorrectCombination();

            else
                _cheatIndexControl++;
        }

        protected void CorrectCombination()
        {
            switch (CheatType)
            {
                case CheatTypeEnum.ONCE:
                    ActivateCheat();
                    AlreadyInvoked = true;
                    break;

                case CheatTypeEnum.REINVOKE:
                    AlreadyInvoked = false;
                    ActivateCheat();
                    break;

                case CheatTypeEnum.TOOGLEABLE:
                    if (!AlreadyInvoked)
                    {
                        ActivateCheat();
                        AlreadyInvoked = true;
                    }
                    else
                    {
                        DeactivateCheat();
                        AlreadyInvoked = false;
                    }
                    break;
                default:
                    Debug.LogError("Not expected CheatType");
                    break;
            }

            if (DEBUG_TestCheat)
                Debug.Log("CheatActivated");
            _cheatIndexControl = 0;
        }

        protected abstract void ActivateCheat();
        protected virtual void DeactivateCheat()
        {
            Debug.Log("Not implemented, try override DeactivateCheat() Method");
        }
    }
}