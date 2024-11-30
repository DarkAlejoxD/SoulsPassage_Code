using AvatarController;
using BaseGame;
using Unity.VisualScripting;
using UnityEngine;
using UtilsComplements;

namespace Interactable //add it to a concrete namespace
{
    //[RequireComponent(typeof(Transform))] //Add this if necessary, delete it otherwise
    public class Interactable_Pushable : Interactable_Base
    {
        [SerializeField] private Transform[] _pushPoints;
        private Transform _grabbedPoint;
        private PlayerController _player;

        private bool _isGrabbed;

        #region Public Methods

        private void Start()
        {
            _player = ISingleton<GameManager>.GetInstance().PlayerInstance;
        }


        public override void Interact()
        {
            base.Interact();

            if(_isGrabbed )
            {
                LetGo();
            }
            else
            {
                Grab();
            }
        }

        private void Grab()
        {
            _grabbedPoint = _pushPoints[GetClosestPushPoint()];

            Vector3 pos = _grabbedPoint.position;
            pos.y = _player.transform.position.y;
            _player.transform.position = pos;

            _player.OnMovement += OnMove;
            _player.transform.SetParent(transform, true);
            _isGrabbed = true;

            _player.BlockMovement(GetDirection());

        }

        private void LetGo()
        {
            _player.OnMovement -= OnMove;
            _isGrabbed = false;
            _player.UnBlockMovement();
            _player.transform.SetParent(null, true);

        }

        #endregion

        #region Private Methods

        private int GetClosestPushPoint()
        {
            return MathUtils.GetClosestPoint(_player.transform.position, _pushPoints);
        }

        private Vector3 GetDirection()
        {
            Vector3 dir = transform.position - _grabbedPoint.position;
            dir.Normalize();

            return dir;
        }

        private Vector3 GetDirectionAbsolute()
        {
            Vector3 dir = transform.position - _grabbedPoint.position;
            dir.Normalize();

            dir = new Vector3(Mathf.Abs(dir.x), 0, Mathf.Abs(dir.z));

            return dir;
        }

        private void OnMove(Vector2 input)
        {
            Vector3 movement = Vector3.zero;            

            movement = GetDirectionAbsolute() * input.y * (_player.DataContainer.DefaultMovement.MaxSpeed / 2) * Time.deltaTime;
            transform.position += movement;
        }

        #endregion

    }
}
