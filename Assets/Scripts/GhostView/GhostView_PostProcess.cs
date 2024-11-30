using BaseGame;
using FSM;
using System;
using UnityEngine;
using UtilsComplements;

namespace GhostView
{
    public class GhostView_PostProcess : MonoBehaviour, ISingleton<GhostView_PostProcess>
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Transform _volumeRef;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _distance = 1;
        private float _timeControl = 0;

        private FSM_Default<SphereControlStates> _ghostBrain;

        public ISingleton<GhostView_PostProcess> Instance => this;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();

            GhostViewManager.OnActivate += Activate;
            GhostViewManager.OnDeactivate += Deactivate;

            FSMInit();
            _timeControl = 0;
            _volumeRef.gameObject.SetActive(false);
        }

        private void Update()
        {
            _ghostBrain.OnStay();
        }

        private void OnDestroy()
        {
            Instance.RemoveInstance();

            GhostViewManager.OnActivate -= Activate;
            GhostViewManager.OnDeactivate -= Deactivate;
        }
        #endregion

        #region Static Methods
        public static void StaticMethod()
        {
        }
        #endregion

        #region Public Methods
        public void PublicMethod()
        {
        }
        #endregion

        #region Private Methods
        private void Activate()
        {
            _ghostBrain.RequestChange(SphereControlStates.EXPANDING);
        }

        private void Deactivate()
        {
            _ghostBrain.RequestChange(SphereControlStates.REDUCING);
        }

        private void FSMInit()
        {
            _ghostBrain = new();

            Action empty = () => { };

            _ghostBrain.SetRoot(SphereControlStates.NONE, new State(() => _timeControl = 0, empty, empty));

            _ghostBrain.AddState(SphereControlStates.EXPANDING, new State(
                () =>
                {
                    _timeControl = 0;
                    _volumeRef.gameObject.SetActive(true);
                },
                () =>
                {
                    float curveValue = _curve.Evaluate(1 - (_timeControl / GhostViewManager.Values.AppearTime));

                    _volumeRef.position = transform.position + transform.up * _distance * curveValue;

                    _timeControl += Time.deltaTime;
                },
                () =>
                {
                    _volumeRef.gameObject.SetActive(true);
                }));

            _ghostBrain.AddState(SphereControlStates.REDUCING, new State(
                () =>
                {
                    _timeControl = 0;
                    _volumeRef.gameObject.SetActive(true);
                },
                () =>
                {
                    float curveValue = _curve.Evaluate((_timeControl / GhostViewManager.Values.DisapearTime));

                    _volumeRef.position = transform.position + transform.up * _distance * (curveValue);

                    _timeControl += Time.deltaTime;
                },
                () =>
                {
                    _volumeRef.gameObject.SetActive(false);
                }));

            Transition appearEnded = new(() =>
            {
                return _timeControl > GhostViewManager.Values.AppearTime;
            });

            Transition disappearEnded = new(() =>
            {
                return _timeControl > GhostViewManager.Values.DisapearTime;
            });

            _ghostBrain.AddAutoTransition(SphereControlStates.EXPANDING, appearEnded, SphereControlStates.NONE);
            _ghostBrain.AddAutoTransition(SphereControlStates.REDUCING, disappearEnded, SphereControlStates.NONE);

            _ghostBrain.OnEnter();
        }
        #endregion
    }
}
