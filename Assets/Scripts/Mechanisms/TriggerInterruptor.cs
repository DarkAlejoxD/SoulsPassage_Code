using UnityEngine;

namespace Mechanisms
{
    [RequireComponent(typeof(Collider))]
    public class TriggerInterruptor : AbsInterruptor
    {
        private void Awake() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") && !other.CompareTag("ActivationProp"))
                return;

            base.Activate();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player") && !other.CompareTag("ActivationProp"))
                return;

            base.Deactivate();
        }
    }
}
