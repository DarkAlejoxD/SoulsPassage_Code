using FMOD.Studio;
using FMODUnity;
using InputController;
using UnityEngine;
using UnityEngine.UI;

namespace MenuManagement.Settings
{
    public class SettingsHandler : MonoBehaviour
    {
        [SerializeField] Settings _settings;
        [SerializeField] Slider _masterSlider;

        private Bus _masterBus;

        private Menus _menuInputs;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _masterSlider.value = _settings.MasterVolume;
            _menuInputs = new Menus();

            _masterBus = RuntimeManager.GetBus("bus:/");
        }
        private void OnEnable() => _menuInputs.Settings.Enable();

        private void OnDisable() => _menuInputs.Settings.Disable();

        private void Update()
        {
            if (!gameObject.activeSelf)
                return;

            bool triggered = _menuInputs.Settings.Cancel.WasPerformedThisFrame();
            if (triggered)
                _exitButton.onClick?.Invoke();
        }

        public void OnMasterChange()
        {
            _settings.SetMasterVolume(_masterSlider.value);
            _masterBus.setVolume(_masterSlider.value);
        }
    }
}
