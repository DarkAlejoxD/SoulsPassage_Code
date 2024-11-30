using UnityEngine;
using AvatarController.Data;
using AvatarController.PlayerFSM;
using static UtilsComplements.AsyncTimer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AvatarController
{
    [RequireComponent(typeof(PlayerController), typeof(CharacterController))]
    public class PlayerDive : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        private PlayerController _playerController;
        private CharacterController _characterController;

        private Vector3 _velocity;        

        private bool _isGrounded;
        private bool _canDive;
        public bool IsDiving { get; private set; }

        private PlayerData Data => _playerController.DataContainer;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _characterController = GetComponent<CharacterController>();

            _canDive = true;
        }

        private void OnEnable()
        {
            if (_playerController == null)
                _playerController = GetComponent<PlayerController>();

            _playerController.OnDive += OnDive;
        }

        private void OnDisable()
        {
            _playerController.OnDive -= OnDive;
        }

        private void Update()
        {
            if (IsDiving)
            {
                DivingMovement();
                CheckGrounded();
            }
        }
        #endregion

        #region Private Methods

        private void OnDive(bool active)
        {
            if (!_canDive)
                return;

            if (!active) return;

            if (IsDiving)
                return;

            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();

            _velocity = forward * Data.DefaultDiveValues.StartingSpeed;
            IsDiving = true;

            _playerController.StopVelocity();
            _playerController.ForceChangeState(PlayerStates.OnDive);
            _canDive = false;
            StartCoroutine(TimerCoroutine(Data.DefaultDiveValues.Cooldown, () =>
            {
                _canDive = true;
            }));
            //
        }

        private void DivingMovement()
        {
            float deceleration = _isGrounded ? Data.DefaultDiveValues.GroundDeceleration// * Time.deltaTime
                : Data.DefaultDiveValues.AirDeceleration;// * Time.deltaTime;

            _velocity -= Time.deltaTime * deceleration * _velocity.normalized;

            if (_velocity.magnitude < Data.DefaultDiveValues.MinSpeedThreshold)
            {
                IsDiving = false;
                //Debug.Log("Reach");
                //DEBUG //Moved to the PlayeState_OnDive.OnExit()
                //_animator.SetBool("Dive", false);
                //_animator.SetBool("Idle", true);
                //
            }

            Vector3 motion = _velocity * Time.deltaTime * Data.DefOtherValues.ScaleMultiplicator;
            _characterController.Move(motion);
        }

        private void CheckGrounded() => _isGrounded = _characterController.isGrounded;
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_playerController)
                _playerController = GetComponent<PlayerController>();

            if (!Data.DefaultDiveValues.DEBUG_DrawDiveDisplacement)
                return;

            float height = -1 * Data.DefOtherValues.ScaleMultiplicator;
            //v^2 - v0^2 = 2ax where v:0; v0:DiveSpeed; a:deceleration; x:?
            //x = (-v0^2) / 2a
            float v0 = Data.DefaultDiveValues.StartingSpeed;
            float a = Data.DefaultDiveValues.GroundDeceleration;
            float scale = Data.DefOtherValues.ScaleMultiplicator;

            float distanceInFloor = ((v0 * v0) / (2 * a)) * scale;
            distanceInFloor = Mathf.Abs(distanceInFloor);

            a = Data.DefaultDiveValues.AirDeceleration;
            float distanceInAir = ((v0 * v0) / (2 * a)) * scale;
            distanceInAir = Mathf.Abs(distanceInAir);
            float dotted = 3;

            Vector3 startPoint = transform.position + transform.up * height;
            Vector3 endPoint = startPoint + transform.forward * distanceInFloor;
            Handles.color = new(60 / 236f, 9 / 255f, 255 / 255f);
            Handles.DrawLine(startPoint, endPoint, dotted);

            startPoint = transform.position;
            endPoint = startPoint + transform.forward * distanceInAir;
            Handles.DrawLine(startPoint, endPoint, dotted);
            Handles.color = Color.white;
        }
#endif
        #endregion
    }
}
