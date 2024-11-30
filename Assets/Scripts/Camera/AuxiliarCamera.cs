using UnityEngine;
using Cinemachine;
using FSM;
using UtilsComplements;
using static UtilsComplements.AsyncTimer;

namespace Cameras
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class AuxiliarCamera : MonoBehaviour, ISingleton<AuxiliarCamera>
    {
        #region Fields
        private enum CameraStates
        {
            On, Off
        }

        private const int PRIORITY_LOW = 1;
        private const int PRIORITY_HIGH = 100;

        [Header("References")]
        [SerializeField] private CameraLogic _mainCamera;
        private CinemachineBrain _cinemachineBrain;
        private CinemachineVirtualCamera _auxiliarCamera;
        private CinemachineVirtualCamera _activeCamera;

        [Header("Attributes")]
        [SerializeField, Range(0, 1)] private float _dotToTurnOff = 0.70711f; //30º
        [SerializeField] private bool _workOnBlendings;

        [Header("Composer InitialValues")]
        private CinemachineComposer _composer;
        private float _deadZoneWidth;
        private float _deadZoneHeight;

        private FSM_Default<CameraStates> _cameraBrain;

        public ISingleton<AuxiliarCamera> Instance => this;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();

            _cinemachineBrain = _mainCamera.GetComponent<CinemachineBrain>();
            _auxiliarCamera = GetComponent<CinemachineVirtualCamera>();
            _composer = _auxiliarCamera.GetCinemachineComponent<CinemachineComposer>();
            _deadZoneHeight = _composer.m_DeadZoneHeight;
            _deadZoneWidth = _composer.m_DeadZoneWidth;

            _auxiliarCamera.Priority = PRIORITY_LOW;

            StartCoroutine(TimerCoroutine(10, () => _auxiliarCamera.enabled = true));

            FSMInit();
        }

        //private void Start()
        //{
        //    _activeCamera = Singleton.GetSingleton<CameraManager>().ActiveCamera;
        //}

        private void Update()
        {
            if (_cinemachineBrain.IsBlending && !_workOnBlendings)
            {
                if (_auxiliarCamera.Priority == 100)
                {
                    _cameraBrain.OnStay();
                    return;
                }
                if (_cameraBrain.CurrentState == CameraStates.On)
                    _cameraBrain.RequestChange(CameraStates.Off);
            }
            else
            {
                _cameraBrain.OnStay();
            }
        }

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Public Methods
        public void SetAuxiliarActive(bool value)
        {
            gameObject.SetActive(value);
        }
        #endregion

        #region Private Methods
        private void FSMInit()
        {
            _cameraBrain = new();

            IState offState = new State(() =>
            {
                //_auxiliarCamera.Priority = PRIORITY_LOW;
                //_composer.m_DeadZoneHeight = 0;
                //_composer.m_DeadZoneWidth = 0;
            }, () => { }, () => { }, "AuxiliarCameraOff");


            IState onState = new State(() =>
            {
                _auxiliarCamera.Priority = PRIORITY_HIGH;
                _composer.m_DeadZoneHeight = _deadZoneHeight;
                _composer.m_DeadZoneWidth = _deadZoneWidth;
                _activeCamera = Singleton.GetSingleton<CameraManager>().ActiveCamera;
                if (_activeCamera)
                    _auxiliarCamera.Follow = _activeCamera.transform;
            }, () =>
            {
                _activeCamera = Singleton.GetSingleton<CameraManager>().ActiveCamera;
                if (_cinemachineBrain.IsBlending)
                {
                    //if (_mainCamera.transform != _auxiliarCamera.Follow)
                    //    _auxiliarCamera.Follow = _mainCamera.transform;
                }
                else
                {
                    //if (_activeCamera.transform != _auxiliarCamera.Follow)
                    _auxiliarCamera.Follow = _activeCamera.transform;
                }
            }, () =>
            {
                _auxiliarCamera.Priority = PRIORITY_LOW;
            }, "AuxiliarCameraOn");

            Transition turnOn = new(() =>
            {
                //_activeCamera = Singleton.GetSingleton<CameraManager>().ActiveCamera;
                //Vector3 activeCameraForward = _activeCamera.transform.forward;
                //Vector3 auxiliarForward = transform.forward;

                //float dot = Vector3.Dot(activeCameraForward, auxiliarForward);
                return _mainCamera.IsPlayerOutScreen;
            });

            Transition turnOff = new(() =>
            {
                _activeCamera = Singleton.GetSingleton<CameraManager>().ActiveCamera;
                Vector3 activeCameraForward = _activeCamera.transform.forward;
                Vector3 auxiliarForward = transform.forward;

                float dot = Vector3.Dot(activeCameraForward, auxiliarForward);

                return dot > _dotToTurnOff && !_mainCamera.IsPlayerOutScreen;
            });

            _cameraBrain.SetRoot(CameraStates.Off, offState);
            _cameraBrain.AddState(CameraStates.On, onState);

            _cameraBrain.AddAutoTransition(CameraStates.On, turnOff, CameraStates.Off);
            _cameraBrain.AddAutoTransition(CameraStates.Off, turnOn, CameraStates.On);

            _cameraBrain.OnEnter();
        }
        #endregion
    }
}