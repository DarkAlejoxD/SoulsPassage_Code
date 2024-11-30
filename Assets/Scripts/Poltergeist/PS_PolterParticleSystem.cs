using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace Poltergeist.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PS_PolterParticleSystem : MonoBehaviour
    {
        private ParticleSystem _system;
        private float _startEmission;

        private void OnEnable()
        {
            _system = GetComponent<ParticleSystem>();
            _startEmission = _system.emission.rateOverTimeMultiplier;
            PS_PolterController.OnActivate += Activate;
            PS_PolterController.OnDeactivate += Deactivate;
        }

        private void OnDisable()
        {
            PS_PolterController.OnActivate -= Activate;
            PS_PolterController.OnDeactivate -= Deactivate;
        }

        private void Activate()
        {
            var emission = _system.emission;
            emission.rateOverTimeMultiplier = _startEmission;
        }

        private void Deactivate()
        {
            var emission = _system.emission;
            emission.rateOverTimeMultiplier = 0;
        }
    }
}
