using System;
using UnityEngine;
using BaseGame;
using FSM;
using AvatarController;
using UtilsComplements;

namespace Poltergeist
{
    [DisallowMultipleComponent]
    public class PoltegeistStencilControl : MonoBehaviour, ISingleton<PoltegeistStencilControl>
    {
        [Header("References")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Transform _radiusHandler;
        private FSM_Default<SphereControlStates> _sphereBrain;

        [Header("Attributes")]
        [SerializeField, Min(0.01f)] private float _radiusThreshold = 0.1f;
        [SerializeField, Min(0.01f)] private float _radiusSmooth = 0.1f;

        [Header("Anims")]
        [SerializeField] private AnimationCurve _expansionCurve;
        [SerializeField, Min(0.01f)] private float _expansionTime;
        //[SerializeField] private AnimationCurve _reductionCurve;
        [SerializeField, Min(0.01f)] private float _reductionTime;
        private float _timeControl = 0;

        private float _desiredRadius = 0;
        private float _currentScaleControl = 0;
        private float? _ratioScaleDistance = null; //In scale 1, what should be the distance

        private float RatioScaleDistance
        {
            get
            {
                if (_ratioScaleDistance == null)
                    return 0;
                return _ratioScaleDistance.Value;
            }
            set
            {
                if (_ratioScaleDistance == null)
                    _ratioScaleDistance = value;
            }
        }

        public ISingleton<PoltegeistStencilControl> Instance => this;

        private void Awake()
        {
            Instance.Instantiate();
            PoltergeistManager.OnPolterEnter += ActivateZone;
            PoltergeistManager.OnPolterExit += DeactivateZone;

            FSMInit();
        }

        private void Start()
        {
            float scale = 1;
            transform.localScale = Vector3.one * scale;
            float distance = Vector3.Distance(transform.position, _radiusHandler.position);
            RatioScaleDistance = distance / scale;

            transform.localScale = Vector3.zero;
            _currentScaleControl = 0;
            _timeControl = 0;
        }

        private void Update() => _sphereBrain.OnStay();

        private void OnDestroy()
        {
            Instance.RemoveInstance();
            PoltergeistManager.OnPolterEnter -= ActivateZone;
            PoltergeistManager.OnPolterExit -= DeactivateZone;
        }

        private void ActivateZone()
        {
            _sphereBrain.RequestChange(SphereControlStates.EXPANDING);
        }

        private void DeactivateZone()
        {
            _sphereBrain.RequestChange(SphereControlStates.REDUCING);
        }

        #region FSM
        private void FSMInit()
        {
            _sphereBrain = new();

            Action empty = () => { };

            _sphereBrain.SetRoot(SphereControlStates.NONE, new State(empty, empty, empty, "NONE"));

            _sphereBrain.AddState(SphereControlStates.EXPANDING, new State(
                () =>
                {
                    gameObject.SetActive(true);
                    float distance = _playerController.DataContainer.DefPoltValues.PoltergeistRadius;
                    _desiredRadius = distance;
                    //_timeControl = _currentScaleControl / _desiredRadius;
                    float pctFromReduce = Mathf.Clamp01(_timeControl / _reductionTime);
                    _timeControl = _expansionTime * pctFromReduce;
                },
                () =>
                {
                    _timeControl += Time.deltaTime;
                    float value = _expansionCurve.Evaluate(Mathf.Clamp(_timeControl / _expansionTime, 0, 1));
                    _currentScaleControl = _desiredRadius * value;
                    transform.localScale = Vector3.one * _currentScaleControl * 2;
                },
                () =>
                {
                    gameObject.SetActive(true);
                }, "EXPANDING"));

            _sphereBrain.AddState(SphereControlStates.REDUCING, new State(
               () =>
               {
                   gameObject.SetActive(true);
                   float distance = _playerController.DataContainer.DefPoltValues.PoltergeistRadius;
                   _desiredRadius = distance;
                   //_timeControl = _currentScaleControl / _desiredRadius;
                   float pctFromExpansion = Mathf.Clamp01(_timeControl / _expansionTime);
                   _timeControl = _reductionTime * pctFromExpansion;
               },
               () =>
               {
                   _timeControl -= Time.deltaTime;
                   float value = _expansionCurve.Evaluate(Mathf.Clamp(_timeControl / _reductionTime, 0, 1));
                   _currentScaleControl = _desiredRadius * value;
                   transform.localScale = Vector3.one * _currentScaleControl * 2;
               },
               () =>
               {
                   gameObject.SetActive(false);
                   if (Singleton.TryGetInstance(out PoltergeistManager manager))
                       manager.OnPoltergeistEventExit?.Invoke();
               }, "REDUCING"));

            Transition expandEnd = new(() =>
            {
                return _currentScaleControl > _desiredRadius - _radiusThreshold;
            });

            Transition reduceEnd = new(() =>
            {
                return _currentScaleControl < _radiusThreshold;
            });

            _sphereBrain.AddAutoTransition(SphereControlStates.EXPANDING, expandEnd, SphereControlStates.NONE);
            _sphereBrain.AddAutoTransition(SphereControlStates.REDUCING, reduceEnd, SphereControlStates.NONE);

            _sphereBrain.OnEnter();
        }
        #endregion
    }
}
