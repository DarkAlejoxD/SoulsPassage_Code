using System;
using UnityEngine;
using static UtilsComplements.AsyncTimer;

namespace Poltergeist.Particles
{
    public class PS_PolterController : MonoBehaviour
    {
        public static Action OnActivate;
        public static Action OnDeactivate;
        public Transform _parent;

        private void Awake()
        {
            //_parent = transform.parent;
            transform.SetParent(null, true);
            transform.position = Vector3.zero;
        }

        private void Start()
        {
            DeactivateParticles();
        }

        public void ActivateParticles()
        {
            transform.position = _parent.position;
            OnActivate?.Invoke();
        }

        public void DeactivateParticles()
        {
            OnDeactivate?.Invoke();
        }

    }
}
