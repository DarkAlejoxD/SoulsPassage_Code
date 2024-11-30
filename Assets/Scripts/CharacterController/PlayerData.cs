using System;
using UnityEngine;

namespace AvatarController.Data
{
    [CreateAssetMenu(fileName = "New PlayerData", menuName = "GameData/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        private const int SPACES = 6;

        #region Powers

        [Serializable]
        public class PlayerPowers
        {
            [SerializeField] private bool DEBUG_testPowers;

            public bool CHEAT_testPowers { get; set; }

            internal bool _hasPoltergeist = false;
            internal bool _hasGhostView = false;

            public bool HasPoltergeist
            {
                get
                {
#if UNITY_EDITOR
                    if (DEBUG_testPowers || CHEAT_testPowers)
                        return true;
#endif
                    if (CHEAT_testPowers)
                        return true;

                    return _hasPoltergeist;
                }
            }

            public bool HasGhostView
            {
                get
                {
#if UNITY_EDITOR
                    if (DEBUG_testPowers || CHEAT_testPowers)
                        return true;
#endif
                    if (CHEAT_testPowers)
                        return true;

                    return _hasGhostView;
                }
            }

            public void UnlockGhostView() => _hasGhostView = true;
            public void UnlockPoltergeist() => _hasPoltergeist = true;
        }
        #endregion

        #region Player Movement Nested Class
        [Serializable]
        public class PlayerMovementData
        {
            [Header("Linear Attributes")]
            [SerializeField] internal float _acceleration = 20;
            [SerializeField] internal float _linearDecceleration = 10;
            [SerializeField] internal float _minSpeedToMove = 2;
            [SerializeField] internal float _maxSpeed = 5;
            [SerializeField] internal float _rotationLerp = 5;
            [SerializeField] internal bool _canSprint = true;
            [SerializeField] internal float _sprintMaxSpeed = 5;
            [SerializeField] internal bool _canJump = true;

            [Header("Rotation Attributes")]
            [SerializeField, Min(0.1f)] internal float _angularAcceleration;
            [SerializeField] internal float _maxAngularSpeed;

            [Header("DEBUG")]
            public bool DEBUG_DrawMovementPerSecond;

            public float Acceleration => _acceleration;
            public float LinearDecceleration => _linearDecceleration;
            public float MinSpeedToMove => _minSpeedToMove;
            public float MaxSpeed => _maxSpeed;
            public float RotationLerp => _rotationLerp;
            public bool CanSprint => _canSprint;
            public float SprintMaxSpeed => _sprintMaxSpeed;
            public bool CanJump => _canJump;
        }
        #endregion

        #region Jump Nested Class
        [Serializable]
        public class JumpValues
        {
            [Header("Main attributes")]
            [SerializeField, Min(0.1f)] private float _maxHeight;
            [SerializeField, Min(0.01f)] private float _coyoteTime;
            [SerializeField] private float _gravityMultiplier = 2.0f;
            [SerializeField] private float _downGravityMultiplier = 1.5f;

            [Header("Secondary")]
            [Tooltip("The time the player will be on air before starts falling." +
                     "\nThis variable will be used to compute the deceleration when the player release the button" +
                     "before reaching the peak")]
            [SerializeField, Min(0.01f)] private float _releasePenaltyTime = 0.3f;
            [SerializeField, Min(0.01f)] private float _maxVySpeed = 20;
            [SerializeField, Min(0.01f)] private float _jumpCD = 0.01f;

            [Header("DEBUG")]
            public bool DEBUG_drawHeight;
            public bool DEBUG_drawCurve;
            [Range(0, 50)] public int DEBUG_definitionOfJump;
            [Tooltip("Draws the definition depending on the percentage of the max speed the player has")]
            [Range(0, 1)] public float DEBUG_forwardMovementPct;
            [HideInInspector] public bool DEBUG_drawCoyote;

            public float MaxHeight => _maxHeight;
            public float CoyoteTime => _coyoteTime;
            public float GravityMultiplier => _gravityMultiplier;
            public float DownGravityMultiplier => _downGravityMultiplier;
            public float ReleasePenalty => _releasePenaltyTime;
            public float MaxVySpeed => _maxVySpeed;
            public float JumpCD => _jumpCD;
        }
        #endregion

        #region Dive Nested Class
        [Serializable]
        public class DiveValues
        {
            [Header("Attributes")]
            [SerializeField] private float _startingSpeed;
            [SerializeField] private float _airDeceleration;
            [SerializeField] private float _groundDeceleration;
            [SerializeField] private float _cooldown;
            [Tooltip("If speed is inferior to this threshold, it should return to normal")]
            [SerializeField] private float _speedThreshold = 0.2f;
            [SerializeField, Min(0.01f)] private float _verticalImpulse = 1;

            [Header("DEBUG")]
            [SerializeField] public bool DEBUG_DrawDiveDisplacement = true;

            public float StartingSpeed => _startingSpeed;
            public float AirDeceleration => _airDeceleration;
            public float GroundDeceleration => _groundDeceleration;
            public float MinSpeedThreshold => _speedThreshold;
            public float Cooldown => _cooldown;
            public float VerticalImpulse => _verticalImpulse;
        }
        #endregion

        #region Interaction Nested Class
        [Serializable]
        public class InteractionValues
        {
            [SerializeField] private float _interactionRange;
            //[SerializeField] private float _interactionCooldown;

            public float InteractionRange => _interactionRange;
            //public float InteractionCooldown => _interactionCooldown;            
        }
        #endregion

        #region Poltergeist Nested Class
        [Serializable]
        public class PoltergeistValues
        {
            #region Poltergeist
            [Header("Poltergeist Control")]
            [SerializeField, Min(0.01f), Tooltip("Security Cooldown to not spam it")]
            private float _poltergeistSpamCooldown = 0.1f;
            [SerializeField, Range(0.1f, 1)] private float _dotJoystickThreshold = 0.3f;

            [Header("Polt Attributes")]
            [SerializeField, Min(0.01f)] private float _poltergeistCooldown;
            [SerializeField, Min(0.01f)] private float _poltergeistRadius;
            [SerializeField, Min(0.01f)] private float _playerRadius;
            [SerializeField, Min(0.01f)] private float _speed;

            public float PoltergeistSpamCD => _poltergeistSpamCooldown;
            public float JoystickThreshold => _dotJoystickThreshold;
            public float PoltergeistCD => _poltergeistCooldown;
            public float PoltergeistRadius => _poltergeistRadius;
            public float PlayerRadius => _playerRadius;
            public float Speed => _speed;

            [Header("DEBUG Poltergeist")]
            public bool DEBUG_DrawPoltergeistRadius = true;
            #endregion
        }
        #endregion

        #region Other Nested Class
        [Serializable]
        public class OtherValues
        {
            #region Grab
            [SerializeField, Min(0.01f)] private float _grabCD = 0.5f;

            public float GrabCD => _grabCD;
            #endregion

            #region Scale
            [Header("Scale")]
            [SerializeField, Range(0.01f, 1)] private float _scaleMultiplicator;
            [SerializeField, Min(0.01f)] private float _minTimeBreakIdle = 1;
            [SerializeField, Min(0.01f)] private float _maxTimeBreakIdle = 10;

            public float ScaleMultiplicator => _scaleMultiplicator;
            public float TimeBreakIdle => UnityEngine.Random.Range(_minTimeBreakIdle, _maxTimeBreakIdle);
            #endregion

            #region Ghost
            [Header("GhostViewValues")]
            [SerializeField, Min(0.01f)] private float _ghostViewCooldown;
            [SerializeField, Min(0.01f)] private float _ghostViewRadius;

            public float GhostViewCooldown => _ghostViewCooldown;
            public float GhostViewRadius => _ghostViewRadius;

            [Header("DEBUG GhostView")]
            public bool DEBUG_ShowGhostRadius;
            #endregion
        }
        #endregion

        #region Powers 
        [Header("Powers")]
        [SerializeField, Space(SPACES)] private PlayerPowers _powers;

        public PlayerPowers Powers => _powers;
        #endregion

        #region Movement Fields
        [Header("Movement Attributes")]
        [SerializeField, Space(SPACES)] private PlayerMovementData _defaultMovement;
        [SerializeField, Space(SPACES)] private PlayerMovementData _grabbingLedgeMovement;
        [SerializeField, Space(SPACES), HideInInspector] private PlayerMovementData _pushingMovement;
        [SerializeField, Space(SPACES), HideInInspector] private PlayerMovementData _crounchMovement;
        [SerializeField, Space(SPACES), HideInInspector] private PlayerMovementData _onAirMovement;

        public PlayerMovementData DefaultMovement => _defaultMovement;
        public PlayerMovementData GrabbingLedgeMovement => _grabbingLedgeMovement;
        public PlayerMovementData PushingMovement => _pushingMovement;
        public PlayerMovementData CrounchMovement => _crounchMovement;
        public PlayerMovementData OnAirMovement => _onAirMovement;
        #endregion

        #region JumpValues
        [SerializeField, Space(SPACES)] JumpValues _jumpValues;

        [SerializeField, Space(SPACES)] DiveValues _diveValues;

        public JumpValues DefaultJumpValues => _jumpValues;
        public DiveValues DefaultDiveValues => _diveValues;
        #endregion

        #region Other Values
        //[Header("Interaction Attributes")]
        [SerializeField, Space(SPACES), HideInInspector] InteractionValues _interactionValues;
        //[Header("Poltergeist")]
        [Header("OtherValues")]
        [SerializeField, Space(SPACES)] PoltergeistValues _poltergeistValues;
        [SerializeField, Space(SPACES)] private OtherValues _otherValues;

        public InteractionValues DefaultInteractionValues => _interactionValues;
        public PoltergeistValues DefPoltValues => _poltergeistValues;
        /// <summary>
        /// Contains:
        ///     - GhostView
        ///     - Poltergeist
        /// </summary>
        public OtherValues DefOtherValues => _otherValues;
        #endregion

        private void OnEnable() => _powers.CHEAT_testPowers = false;
        /// <summary>
        /// This should be called once, on the start or awake of the player
        /// </summary>
        public void DisablePowers()
        {
            _powers._hasPoltergeist = false;
            _powers._hasGhostView = false;
        }
    }
}


#if UNITY_EDITOR

namespace AvatarController.Data
{
    using UnityEditor;

    [CustomEditor(typeof(PlayerData))]
    public class PlayerDataEditor : Editor
    {
        private PlayerData data;
        private float Scale => data.DefOtherValues.ScaleMultiplicator;
        private float TimeToPeak => 0;

        private void OnEnable()
        {
            data = ((PlayerData)target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("If you pass the mouse over some variables, you will get a few " +
                                    "description", MessageType.Info);

            EditorGUILayout.HelpBox($"The final result of the movement will scalate into {(Scale * 10)}/10, " +
                                    $"\nso 1 meter for the player is {Scale * 1} meter in the rest of the " +
                                    "game", MessageType.Info);

            EditorGUILayout.HelpBox($"The height of the jump is: {data.DefaultJumpValues.MaxHeight * Scale}",
                                    MessageType.Info);

            EditorGUILayout.HelpBox($"The max displacement per second is: {data.DefaultMovement.MaxSpeed * Scale}" +
                                    $" meters per second", MessageType.Info);

            //EditorGUILayout.HelpBox($"This is the time to the peak of the jump: {TimeToPeak}", MessageType.Info);

            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}

#endif