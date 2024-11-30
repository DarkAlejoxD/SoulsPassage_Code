using UnityEngine;
using AvatarController.LedgeGrabbing;

namespace BaseGame
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class LedgeLimit : MonoBehaviour
    {
        private void OnEnable()
        {
            PlayerLedgeGrab.ActivateLimits += ActivateLimit;
            PlayerLedgeGrab.DeactivateLimits += DeactivateLimit;
        }

        private void OnDisable()
        {
            PlayerLedgeGrab.ActivateLimits -= ActivateLimit;
            PlayerLedgeGrab.DeactivateLimits -= DeactivateLimit;
        }

        private void ActivateLimit()
        {
            Collider collider = GetComponent<Collider>();
            collider.enabled = true;
        }

        private void DeactivateLimit()
        {
            Collider collider = GetComponent<Collider>();
            collider.enabled = false;
        }
    }
}
