using AudioController;
using FMOD.Studio;
using System;
using UnityEngine;
using UtilsComplements;
using static UtilsComplements.AsyncTimer;

namespace GhostView
{
    public class GhostViewManager : MonoBehaviour, ISingleton<GhostViewManager>
    {
        #region Fields

        [Serializable]
        public class GhostValues
        {
            [Min(0.1f)] public float AppearTime;
            [Min(0.1f)] public float Staytime;
            [Min(0.1f)] public float DisapearTime;
        }

        public ISingleton<GhostViewManager> Instance => this;

        public static Action<Vector3, float> OnActivateGhostView;
        public static Action OnActivate;
        public static Action OnDeactivate;

        private EventInstance? _ghostModeAudio;

        public GhostValues _values;
        public static GhostValues Values
        {
            get
            {
                if (!ISingleton<GhostViewManager>.TryGetInstance(out var manager))
                    return null;
                return manager._values;
            }
        }
        #endregion

        #region Unity Logic
        private void Awake() => Instance.Instantiate();

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Static Methods
        public static void RequestGhostView(Vector3 origin, float radius)
        {
            if (!ISingleton<GhostViewManager>.TryGetInstance(out var manager))
                return;

            manager.ActivateGhostView(origin, radius);
        }
        #endregion

        #region Public Methods
        public void ActivateGhostView(Vector3 origin, float radius)
        {
            if (_ghostModeAudio == null)
                _ghostModeAudio = AudioManager.GetAudioManager().CreateEventInstance(Database.Player, "GhostMode");

            _ghostModeAudio.Value.start();
            OnActivateGhostView?.Invoke(origin, radius);
            OnActivate?.Invoke();
            StartCoroutine(TimerCoroutine(_values.AppearTime + _values.Staytime,
                () =>
                {
                    OnDeactivate?.Invoke();
                    _ghostModeAudio.Value.stop(STOP_MODE.ALLOWFADEOUT);
                }));
        }
        #endregion
    }
}
