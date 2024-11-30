using UnityEngine;
using AvatarController.LedgeGrabbing;

namespace BaseGame
{
    [ExecuteAlways]
    public class DEBUG_Ledges : MonoBehaviour
    {
        private void OnEnable() => PlayerLedgeGrab.DEBUG_Ledges.Add(transform);

        private void OnDisable() => PlayerLedgeGrab.DEBUG_Ledges.Remove(transform);
    }
}
