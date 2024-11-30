using UnityEngine;

namespace BaseGame
{
    public class Deadzone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                GameManager.DeadSequence();
        }
    }
}