using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UtilsComplements.AsyncTimer;

namespace UtilsComplements
{
    public class ShowFPS : MonoBehaviour
    {
        [SerializeField] TMP_Text _textReference;
        [SerializeField, Range(0, 1)] float _recomputeTime;
        private bool _recompute;

        private void Start()
        {
            _recompute = true;
        }

        private void Update()
        {
            if (!_recompute)
                return;

            _textReference.text = "FPS: " + (int)(1 / Time.deltaTime);
            _recompute = false;
            StartCoroutine(TimerCoroutine(_recomputeTime, () =>
            {
                _recompute = true;
            }));
        }
    }
}