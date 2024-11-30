using System.Collections;
using UnityEngine;
using AvatarController.Data;
using GhostView;
#if UNITY_EDITOR
using UtilsComplements.Editor;
#endif

namespace AvatarController //add it to a concrete namespace
{
    [RequireComponent(typeof(PlayerController))] //Add this if necessary, delete it otherwise
    public class PlayerGhostView : MonoBehaviour
    {
        #region Fields
        private bool _canInspect;
        private PlayerController _controller;
        private PlayerData DataContainer => _controller.DataContainer;

        [Header("DEBUG")]
        [SerializeField] private Color DEBUG_GizmosColor;
        #endregion

        #region Unity Logic
        private void OnEnable()
        {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();

            _controller.OnGhostView += Inspect;
            _canInspect = true;
        }

        private void OnDisable()
        {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();

            _controller.OnGhostView -= Inspect;
        }
        #endregion

        #region Private Methods
        private void Inspect(bool value)
        {
            if (!value)
                return;

            if (!_canInspect)
                return;

            GhostViewManager.RequestGhostView(transform.position, DataContainer.DefOtherValues.GhostViewRadius);
            StartCoroutine(GhostViewCooldownCoroutine());
        }

        private IEnumerator GhostViewCooldownCoroutine()
        {
            _canInspect = false;
            yield return new WaitForSeconds(DataContainer.DefOtherValues.GhostViewCooldown);
            _canInspect = true;
        }
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_controller == null)
                _controller = GetComponent<PlayerController>();

            GizmosUtilities.DrawSphere(transform.position, DEBUG_GizmosColor,
                                       DataContainer.DefOtherValues.GhostViewRadius,
                                       DataContainer.DefOtherValues.DEBUG_ShowGhostRadius);
        }
#endif
        #endregion
    }
}