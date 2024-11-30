using UnityEngine;
using AvatarController.Data;
using UtilsComplements.Editor;
using AvatarController.LedgeGrabbing;
using AudioController;

namespace AvatarController
{
    [RequireComponent(typeof(PlayerController), typeof(CharacterController))]
    public class PlayerJump : MonoBehaviour
    {
        private const string ANIM_JUMP_TRIGGER = "Jump";
        private const string ANIM_JUMPGRABBED_TRIGGER = "JumpGrabbed";

        #region Fields
        [Header("References")]
        private PlayerController _controller;
        private CharacterController _characterController;

        [Header("Control")]
        private float _lastTimeInGround;
        private bool _jumped;
        private bool _grabbingLedge;

        private PlayerData DataContainer => _controller.DataContainer;
        private float VelocityY
        {
            get => _controller.VelocityY;
            set => _controller.VelocityY = value;
        }
        private float Gravity => _controller.Gravity;
        public bool IsGrounded => _controller.OnGround;
        public bool IsFailling => VelocityY <= 0;

        private bool _buttonUp = true;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _buttonUp = true;
        }

        private void OnEnable()
        {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();

            _controller.OnJump += OnJump;
        }

        private void OnDisable()
        {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();

            _controller.OnJump -= OnJump;
        }

        private void Update()
        {
            if (IsGrounded)
            {
                _lastTimeInGround = Time.time;
                _jumped = false;
            }
        }
        #endregion

        #region Public Methods
        public void SetLedgeGrab(bool value)
        {
            _grabbingLedge = value;
        }

        public float GetTimeToPeak()
        {
            float vel0 = GetVelocity();
            //Calculus?
            //v = v0 + a * t //where v:0; v0:vel; a:Gravity; t:?
            //t = -v0 /-a
            return Mathf.Abs(vel0 / Gravity);
        }

        internal bool CanJump()
        {
            if (_grabbingLedge)
                return true;

            if (IsGrounded)
                return true;

            if (_jumped)
                return false;

            if (Time.time <= _lastTimeInGround + DataContainer.DefaultJumpValues.CoyoteTime)
                return true;

            return false;
        }
        #endregion

        #region Private Methods
        private void OnJump(bool active)
        {
            if (!_buttonUp)
            {
                if (active)
                    return;
                _buttonUp = true;
            }

            if (!active)
                return;

            if (!CanJump())
                return;

            Jump();
        }

        private void Jump()
        {
            VelocityY = GetVelocity();
            _jumped = true;
            _buttonUp = false;
            _controller.ForceChangeState(PlayerFSM.PlayerStates.Jumping);
            if (_grabbingLedge)
            {
                GetComponent<PlayerLedgeGrab>().LetGoLedge();
            }

            if (_controller.ThisAnimator)
            {
                if (_controller._wasGrabbed)
                    _controller.ThisAnimator.SetTrigger(ANIM_JUMPGRABBED_TRIGGER);
                else
                    _controller.ThisAnimator.SetTrigger(ANIM_JUMP_TRIGGER);
            }

            AudioManager.GetAudioManager().PlayOneShot(Database.Player, "JUMP", transform.position);

            #region DEBUG
#if UNITY_EDITOR
            lastPos = transform.position;
            lastDir = transform.forward;
#endif
            #endregion
        }


        private float GetVelocity()
        {
            //VelocityCalculus
            //v^2 - (v0)^2 = 2 * a * Dx
            //V^2 = 0; v0 = ?; a = gravity; Dx = Data
            //v0 = maths.sqrt ( -2 * a * Dx )

            float vel = -2 * Gravity * DataContainer.DefaultJumpValues.MaxHeight;
            return Mathf.Sqrt(Mathf.Abs(vel));

            //OtherVelocityCalculus
            //v = v0 + gt
            //0 = v0 + gt
            //v0 = -gt

            //y = y0 + v0 * t + a/2 * t^2
            //y=Data; y0 = 0; v0=? 
            //return -Gravity * DataContainer.DefaultJumpValues.TimeToReachHeight;
        }
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_controller == null)
            {
                _controller = GetComponent<PlayerController>();
            }

            DrawHeight();
            DrawCurve();
        }

        private void DrawHeight()
        {
            Vector3 height = lastPos + Vector3.up * DataContainer.DefaultJumpValues.MaxHeight *
                             DataContainer.DefOtherValues.ScaleMultiplicator;
            Color color = Color.green;
            GizmosUtilities.DrawSphere(height, color, 0.3f,
                                       DataContainer.DefaultJumpValues.DEBUG_drawHeight);
        }

        #region Curve
        Vector3 lastPos = Vector3.zero;
        Vector3 lastDir = Vector3.zero;

        private void DrawCurve() //TODO: Represent the actual curve with the gravity twink
        {
            if (Application.isPlaying)
            {
                if (lastPos == Vector3.zero)
                    lastPos = transform.position;
                if (lastDir == Vector3.zero)
                    lastDir = transform.forward;
            }
            else
            {
                lastDir = transform.forward;
                lastPos = transform.position;
            }

            Color color = Color.blue;

            //Time calculus
            //v = v0 + a * t
            //0 = vel + g * t --> t = vel / g
            float time = GetTimeToPeak();
            //Seconde time Calculus
            //x = x0 + a/2 * time? * time?
            //time = mathf.sqrt((x - x0)*2/a);
            //float secondTime = (0 - DataContainer.DefaultJumpValues.MaxHeight) * 2
            //                   / (Gravity * DataContainer.DefaultJumpValues.DownGravityMultiplier);
            //;
            //secondTime = Mathf.Sqrt(Mathf.Abs(secondTime));

            GizmosUtilities.DrawCurveProperties curveProperties =
                new(GizmosUtilities.DrawCurveProperties.DefaultValues)
                {
                    MinValue = 0,
                    MaxValue = time * 2,
                    DefinitionOfCurve = DataContainer.DefaultJumpValues.DEBUG_definitionOfJump,
                    DotRadius = 0.01f
                };

            GizmosUtilities.DrawCurve(Curve, color, curveProperties,
                                      DataContainer.DefaultJumpValues.DEBUG_drawCurve);
        }

        private Vector3 Curve(float time)
        {
            Vector3 xzPos = Vector3.zero;
            Vector3 yPos = Vector3.zero;

            #region Y Axis
            float accA = Gravity;
            ////float accB = Gravity * DataContainer.DefaultJumpValues.DownGravityMultiplier;
            float vel = GetVelocity();
            //v = v0 + a*t; 
            //v = 0; v0 = vel; a = gravity;
            //float timeWhen0 = Mathf.Abs(vel / accA);
            float y;
            //if (time <= timeWhen0)
            y = vel * time + accA / 2 * time * time;
            y = lastPos.y + (-1 + y) * DataContainer.DefOtherValues.ScaleMultiplicator;
            //else
            //{
            //    float yPosVel0 = lastPos.y + vel * timeWhen0 + accA / 2 * timeWhen0 * timeWhen0;
            //    float dt = (time - timeWhen0);
            //    y = yPosVel0 + accB * dt * dt;
            //}

            yPos = new(0, y, 0);
            #endregion

            #region XY Axis
            float speed = DataContainer.DefaultMovement.MaxSpeed *
                          DataContainer.DefaultJumpValues.DEBUG_forwardMovementPct;

            Vector3 direction = lastDir;
            Vector3 pos = new(lastPos.x, 0, lastPos.z);

            xzPos = speed * time * direction;
            xzPos = pos + xzPos * DataContainer.DefOtherValues.ScaleMultiplicator;
            #endregion

            return xzPos + yPos;
        }

        #endregion
#endif
        #endregion
    }
}

