using AvatarController;
using BaseGame;
using UnityEngine;

namespace Miscelaneous
{
    public class Platform : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            var player = other.GetComponentInParent<PlayerController>();
            if (!player)
                return;

            player.transform.SetParent(transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            var player = other.GetComponentInParent<PlayerController>();
            if (!player)
                return;

            player.transform.SetParent(null);
        }
    }
}