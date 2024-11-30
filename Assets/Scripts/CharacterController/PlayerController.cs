using System;
using System.Collections;
using UnityEngine;
using BaseGame;
using FSM;
using InputController;
using AvatarController.Data;
using AvatarController.PlayerFSM;
using static UtilsComplements.AsyncTimer;

namespace AvatarController
{
    //[ExecuteAlways]
    [RequireComponent(typeof(InputManager), typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private PlayerData _dataContainer;
        [SerializeField] private Animator _animator;
        private CharacterController _characterController;
        private InputManager _inputManager;

        public PlayerData DataContainer => _dataContainer;

        [Header("Delegates")]
        public Action<Vector2> OnMovement;
        public Action<bool> OnJump;
        public Action<bool> OnDive;
        public Action<bool> OnInteract;
        public Action<bool> OnGhostView;
        public Action<bool> OnSprint;
        public Action OnGrabCheck;
        public Action OnGrabUpdate;

        [Header("Random Attributes")]
        private PlayerJump _playerJump;
        internal bool _wasGrabbed;
        internal bool CanGrab { get; set; }

        [Header("CanTransitionValues")]
        internal bool _polterFound;
        internal bool _canActivatePoltergeist { get; private set; }

        [Header("Velocity Attributes")]
        internal Vector3 Velocity;
        internal float VelocityY;
        private bool _useGravity;
        internal bool OnGround;

        internal float Gravity => Physics.gravity.y * DataContainer.DefaultJumpValues.GravityMultiplier;
        /// <summary> Meant to be the decceleration when the player released the button to reach the 
        /// peak of the jump earlier </summary>
        internal float TwistGravity { get; set; }
        internal bool UseTwikedGravity { get; set; }
        internal Animator ThisAnimator => _animator;

        [Header("FSM")]
        private FSM_Player<PlayerStates> _playerFSM;

        public PlayerStates CurrentState => _playerFSM.CurrentState;
        public PlayerStates LastState => _playerFSM.LastState;

        [Header("DEBUG")]
        [SerializeField] TMPro.TMP_Text DEBUG_TextTest;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            GameManager.GetGameManager().SetPlayerInstance(this);
            _characterController = GetComponent<CharacterController>();
            _inputManager = GetComponent<InputManager>();

            _playerJump = GetComponent<PlayerJump>();
            UseTwikedGravity = false;
            TwistGravity = 0;

            FSMInit();
        }

        private void OnEnable()
        {
            if (_inputManager == null)
                _inputManager = GetComponent<InputManager>();

            _inputManager.OnInputDetected += OnGetInputs;
        }

        private void OnDisable()
        {
            if (_inputManager == null)
                _inputManager = GetComponent<InputManager>();

            _inputManager.OnInputDetected -= OnGetInputs;
        }

        private void Start()
        {
            Velocity = Vector3.zero;
            VelocityY = 0;
            _useGravity = true;
            _canActivatePoltergeist = true;
            if (DEBUG_TextTest)
                DEBUG_TextTest.text = "";
        }

        private void Update()
        {
            UpdateVy();

#if UNITY_EDITOR
            if (!DEBUG_TextTest)
                return;
            if (!_playerFSM.Equals(null))
                DEBUG_TextTest.text = "Current State: " + _playerFSM.Name;

            //Debug.Log("Velocity: " + new Vector3(Velocity.x, VelocityY, Velocity.z) +
            //          "Magnitude: " + Velocity.magnitude +
            //          "\nDeltaTime: " + Time.deltaTime);
#endif
        }
        #endregion

        #region Public Methods

        #region Movement
        public void BlockMovement() => _characterController.enabled = false;

        public void UnBlockMovement() => _characterController.enabled = true;

        /// <summary></summary>
        /// <param name="dir"> the direction the player should look at when block</param>
        public void BlockMovement(Vector3 dir)
        {
            BlockMovement();

            Quaternion desiredRotation = Quaternion.LookRotation(dir);
            transform.rotation = desiredRotation;
        }

        public void RequestTeleport(Vector3 position)
        {
            BlockMovement();
            StopVelocity();
            StopFalling();
            transform.position = position;
            UnBlockMovement();
        }

        public void RequestTeleport(Vector3 position, Vector3 forward)
        {
            BlockMovement();
            StopVelocity();
            StopFalling();
            transform.position = position;
            forward.y = 0;
            transform.rotation = Quaternion.LookRotation(forward);

            StartCoroutine(TimerCoroutine(0.2f, UnBlockMovement));
            //UnBlockMovement();
        }

        public void SetOnlyMove()
        {
            _playerFSM.ForceChange(PlayerStates.OnlyMove);
        }

        public void SetDefaultMovement()
        {
            _playerFSM.ForceChange(PlayerStates.OnGround);
        }

        public void StopVelocity() => Velocity = Vector3.zero;

        public void StopFalling() => VelocityY = 0;

        public void SetGravityActive(bool state)
        {
            _useGravity = state;
            if (!state) StopVelocity();
        }

        public void PolterNotFound() => _polterFound = false;
        #endregion

        #region Physics
        public Vector3 GetVelocity() => Velocity;

        /// <summary>
        /// Adds velocity to the current Velocity
        /// </summary>
        public void AddImpulse(Vector3 impulse)
        {
            float y = impulse.y;
            impulse.y = 0;
            Velocity += impulse;
            VelocityY += y;
        }

        /// <summary>
        /// Adds velocity to the current Velocity with a limit
        /// </summary>
        public void AddImpulse(Vector3 impulse, float maxValue)
        {
            Velocity += impulse;
            if (Velocity.magnitude > maxValue)
                Velocity = Velocity.normalized * maxValue;
        }

        /// <summary>
        /// Caution! This abruptly changes the velocity of the target
        /// </summary>
        public void OverrideVelocity(Vector3 velocity)
        {
            Velocity = velocity;
        }

        /// <summary>
        /// This will add a force. Meant to be a continuous call on another script.
        /// </summary>
        /// <param name="acceleration"></param>
        public void AddForce(Vector3 acceleration, float dt)
        {
            Velocity += acceleration * dt;
        }

        /// <summary>
        /// Meant to be an asyn call on another script
        /// </summary>
        /// <param name="acceleration"></param>
        /// <param name="time"></param>
        public void AddForceAsync(Vector3 acceleration, float time)
        {
            StartCoroutine(ForceAsyncCoroutine(acceleration, time));
        }
        #endregion

        #region FSM
        internal void ForceChangeState(PlayerStates state) => _playerFSM.ForceChange(state);

        internal void RequestChangeState(PlayerStates state)
        {
            if (state == PlayerStates.OnPoltergeist)
            {
                if (!_canActivatePoltergeist)
                {
                    return;
                }
            }
            _playerFSM.RequestChange(state);
        }

        internal void ReturnState() => _playerFSM.ReturnLastState();

        internal void StartPoltergeist()
        {
            _inputManager.SetPlayerMapActive(false);
            _inputManager.SetPoltergeistActive(true);
        }

        internal void EndPoltergeist()
        {
            _inputManager.SetPlayerMapActive(true);
            _inputManager.SetPoltergeistActive(false);

            _canActivatePoltergeist = false;
            StartCoroutine(TimerCoroutine(_dataContainer.DefPoltValues.PoltergeistCD,
                                          () => _canActivatePoltergeist = true));
        }
        #endregion

        #endregion

        #region Private Methods
        private void FSMInit()
        {
            _playerFSM = new();

            _playerFSM.SetRoot(PlayerStates.OnGround, new PlayerState_DefaultMovement(this));
            _playerFSM.AddState(PlayerStates.OnlyMove, new PlayerState_OnlyMove(this));
            //Debug.Log("a");
            _playerFSM.AddState(PlayerStates.OnAir, new PlayerState_OnAir(this));
            _playerFSM.AddState(PlayerStates.Grabbing, new PlayerState_Grabbing(this));
            _playerFSM.AddState(PlayerStates.OnDive, new PlayerState_OnDive(this));
            _playerFSM.AddState(PlayerStates.OnPoltergeist, new PlayerState_Poltergeist(this));
            _playerFSM.AddState(PlayerStates.Jumping, new PlayerState_Jumping(this));

            Transition toAir = new(() =>
            {
                return !_playerJump.CanJump();
            });

            Transition grounded = new(() =>
            {
                return OnGround;
            });

            _playerFSM.AddAutoTransition(PlayerStates.OnGround, toAir, PlayerStates.OnAir);
            _playerFSM.AddAutoTransition(PlayerStates.OnAir, grounded, PlayerStates.OnGround);

            //Manual Transitions should be named here:
            // - When grab a GrabLedge and LetGoLedge
            // - Dive enter and exit
            // - OnPoltergeistEnter --> OnGroundState
            // - OnPoltergeistExit --> OnPoltergeist
            // - Jump() in PlayerJump --> Jumping
            // - in Jumping_State --> OnAir

            _playerFSM.OnEnter();
        }

        private void OnGetInputs(InputValues inputs) //TODO: Separate States
        {
            _playerFSM.StayPlayer(inputs);

            #region OLD
            //switch (_currentState)
            //{
            //    case PlayerStates.OnPoltergeist:
            //        {
            //            float upDown = 0;
            //            if (inputs.JumpInput) upDown += 1;
            //            if (inputs.CrounchDiveInput) upDown -= 1;
            //            OnPoltergeistStay?.Invoke(inputs.MoveInput, upDown);
            //            OnPoltergeistExit?.Invoke(inputs.CancelInput);
            //        }
            //        break;
            //    case PlayerStates.OnDive:
            //        break;
            //    default:
            //        {
            //            OnMovement?.Invoke(inputs.MoveInput);
            //            OnJump?.Invoke(inputs.JumpInput);
            //            OnDive?.Invoke(inputs.CrounchDiveInput);
            //            OnInteract?.Invoke(inputs.InteractInput);
            //            OnGhostView?.Invoke(inputs.GhostViewInput);
            //            //OnSprint?.Invoke(inputs.SprintInput);
            //        }
            //        break;
            //}
            #endregion
        }

        private void UpdateVy()
        {
            if (!_useGravity)
                return;

            if (VelocityY > DataContainer.DefaultJumpValues.MaxVySpeed)
                VelocityY = DataContainer.DefaultJumpValues.MaxVySpeed;

            else if (VelocityY < -DataContainer.DefaultJumpValues.MaxVySpeed)
                VelocityY = -DataContainer.DefaultJumpValues.MaxVySpeed;

            float deltaTime = Time.deltaTime;
            float vy0 = VelocityY;
            float acc;

            if (VelocityY <= 0)
            {
                acc = Gravity;
                VelocityY += Gravity * deltaTime * DataContainer.DefaultJumpValues.DownGravityMultiplier;
                UseTwikedGravity = false;
            }
            else
            {
                if (UseTwikedGravity)
                    VelocityY += TwistGravity * deltaTime;
                else
                    VelocityY += Gravity * deltaTime;

                acc = UseTwikedGravity ? TwistGravity : Gravity;
            }

            float variation = vy0 * deltaTime + (acc * deltaTime * deltaTime) / 2;

            CollisionFlags movement = _characterController.Move(new Vector3(0, variation, 0) *
                                                                DataContainer.DefOtherValues.ScaleMultiplicator);

            if (movement == (CollisionFlags.Above))
            {
                _characterController.Move(-transform.up * 0.001f);
                VelocityY = 0;
            }

            if (movement == CollisionFlags.Below)
            {
                OnGround = true;
                VelocityY = 0;
            }
            else
            {
                OnGround = false;
            }
        }

        private IEnumerator ForceAsyncCoroutine(Vector3 acceleration, float time)
        {
            for (float i = 0; i < time; i += Time.fixedDeltaTime)
            {
                Velocity += acceleration * Time.fixedDeltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }
        #endregion
    }
}