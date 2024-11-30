using UnityEngine;
using AvatarController.Data;
using UtilsComplements.Editor;
using Poltergeist;

namespace AvatarController
{
    /// <summary>
    /// If the player has this component, enters the poltergeist and does the logic
    /// </summary>
    //Maybe i shouldn't separate the poltergeist logic, but It's already done so... xd
    [RequireComponent(typeof(PlayerController))]
    public class PlayerPoltergeist : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        private PlayerController _controller;
        [HideInInspector] public Poltergeist_Item Item;

        private PlayerData DataContainer => _controller.DataContainer;
        private Transform CameraTransform => Camera.main.transform;

        [Header("DEBUG")]
        [SerializeField] private Color DEBUG_gizmosColor;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
        }
        #endregion

        #region Private Methods
        public void PoltergeistModeUpdate(Vector2 xzDirection, float yDirection)
        {
            //Transform the input by the camera
            Vector3 forward = CameraTransform.forward;
            forward.y = 0;
            forward.Normalize();
            Vector3 right = CameraTransform.right;
            right.y = 0;
            right.Normalize();

            Vector3 movement = xzDirection.y * forward + xzDirection.x * right + yDirection * Vector3.up;
            movement.Normalize();

            //Realize the movement
            Rigidbody rb = Item.GetComponent<Rigidbody>();// _evaluatedPoltergeistZone.ObjectAttached;
            Vector3 motion = DataContainer.DefPoltValues.Speed * Time.deltaTime * movement;
            Vector3 newPos = rb.position + motion;

            //Check if is far
            float distance = Vector3.Distance(transform.position, newPos);
            if (distance > DataContainer.DefPoltValues.PoltergeistRadius)
            {
                Vector3 direction = newPos - transform.position;
                direction.Normalize();
                newPos = transform.position + direction * DataContainer.DefPoltValues.PoltergeistRadius;
            }

            if (distance < DataContainer.DefPoltValues.PlayerRadius)
            {
                Vector3 direction = newPos - transform.position;
                direction.Normalize();
                newPos = transform.position + direction * DataContainer.DefPoltValues.PlayerRadius;
            }

            newPos = Vector3.Lerp(newPos,
                                  PoltergeistManager.ScreenPosCorrection(newPos),
                                  0.5f);
            rb.MovePosition(newPos);
            rb.velocity = Vector3.zero;
        }
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (!_controller)
                _controller = GetComponent<PlayerController>();

            if (_controller.CurrentState != PlayerFSM.PlayerStates.OnPoltergeist)
                return;

            if (!_controller.DataContainer.DefPoltValues.DEBUG_DrawPoltergeistRadius)
                return;

            GizmosUtilities.DrawSphere(transform.position, DEBUG_gizmosColor,
                                       DataContainer.DefPoltValues.PoltergeistRadius,
                                       DataContainer.DefPoltValues.DEBUG_DrawPoltergeistRadius);
            GizmosUtilities.DrawSphere(transform.position, DEBUG_gizmosColor,
                                       DataContainer.DefPoltValues.PlayerRadius,
                                       DataContainer.DefPoltValues.DEBUG_DrawPoltergeistRadius);

            Transform selection = Item ? Item.transform : transform;

            GizmosUtilities.DrawSphere(selection.position, Color.yellow, 0.2f);
        }
#endif
        #endregion
    }
}
