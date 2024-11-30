using UnityEngine;
using InputController;
using UtilsComplements;
using UnityEngine.EventSystems;

namespace BaseGame
{
    public class PauseManager : MonoBehaviour, ISingleton<PauseManager>
    {
        [Header("References")]
        [SerializeField] private GameObject _canvas;

        private bool _canPause = true;

        private Menus _pauseMap;

        public ISingleton<PauseManager> Instance => this;

        private bool _paused;

        public static bool IsPaused
        {
            get
            {
                if (Singleton.TryGetInstance(out PauseManager manager))
                    return manager._paused;

                return false;
            }
        }

        private void Awake()
        {
            Instance.Instantiate();
            _pauseMap = new();
            _pauseMap.Pause.Enable();
            _paused = true;
            SetPauseActive(false);
            _canPause = true;
        }

        private void Update()
        {
            if (!_paused)
                if (!_canPause)
                    return;

            if (_pauseMap.Pause.PauseUnpause.WasPerformedThisFrame())
            {
                SetPaused(!_paused);
            }
        }

        private void OnDestroy() => Instance.RemoveInstance();

        public static void SetCanPause(bool value)
        {
            if (Singleton.TryGetInstance<PauseManager>(out var manager))
                manager._canPause = value;
        }

        public static void SetPauseActive(bool active)
        {
            if (Singleton.TryGetInstance(out PauseManager manager))
            {
                manager.SetPaused(active);
            }
        }

        public void OnResume()
        {
            SetPauseActive(false);
        }

        public void SetPaused(bool active)
        {
            if (active)
            {
                if (_paused)
                    return;
                _paused = true;
                Time.timeScale = 0f;
                _canvas.SetActive(true);
            }
            else
            {
                if (!_paused)
                    return;
                _paused = false;
                _canvas.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}