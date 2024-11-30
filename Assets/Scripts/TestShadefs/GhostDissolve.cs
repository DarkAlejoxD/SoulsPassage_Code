using System;
using System.Collections;
using UnityEngine;
using UtilsComplements;

namespace GameShaders //add it to a concrete namespace
{
    [Obsolete("Not util for the prototype")]
    public class GhostDissolve : MonoBehaviour, ISingleton<GhostDissolve>
    {
        #region Fields
        private const string DISSOLVE_PROPERTY = "_AlphaValue";

        [Header("Renderer")]
        [SerializeField] Material _material;
        [SerializeField] Shader _shader;

        [Header("Dissolve")]
        [SerializeField] private float _timeToAppear;
        [SerializeField] private float _timeToStay;
        [SerializeField] private float _timeToDissapear;

        private float DissolveValue
        {
            get
            {
                if (_material != null)
                    return _material.GetFloat(DISSOLVE_PROPERTY);
                else
                    return -1;
            }
        }

        public ISingleton<GhostDissolve> Instance => this;
        #endregion

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();
            //_material = _renderer.sharedMaterial;
            //_renderer.sharedMaterial.SetFloat(DISSOLVE_PROPERTY, 1.0f);
            _material.SetFloat(DISSOLVE_PROPERTY, 1.0f);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.O))
                RequestAppear();
        }

        private void OnDestroy()
        {
            _material.SetFloat(DISSOLVE_PROPERTY, -1.0f);
            Instance.RemoveInstance();
        }
        #endregion

        #region Static Methods
        public static void StaticMethod()
        {
        }
        #endregion

        #region Public Methods
        public void RequestAppear()
        {
            StopAllCoroutines();
            StartCoroutine(AppearCoroutine());
        }

        public void Invalidate()
        {
            Destroy(this);
        }
        #endregion

        #region Private Methods
        private IEnumerator AppearCoroutine()
        {
            const float min = -1;
            const float max = 1;

            float actualValue = DissolveValue;
            float speed = (max - min) / _timeToAppear;

            #region Appear
            for (float i = 0; i <= _timeToAppear; i += Time.deltaTime)
            {
                actualValue -= speed * Time.deltaTime;
                //actualValue = Mathf.Clamp(actualValue, min, max);
                _material.SetFloat(DISSOLVE_PROPERTY, actualValue);
                Debug.Log(actualValue);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            #endregion

            #region Stay
            yield return new WaitForSeconds(_timeToStay);
            #endregion

            #region Dissapear
            for (float i = _timeToDissapear; i >= 0; i -= Time.deltaTime)
            {
                actualValue = Mathf.Lerp(max, min, i / _timeToDissapear);
                _material.SetFloat(DISSOLVE_PROPERTY, actualValue);
                yield return new WaitForSeconds(Time.deltaTime);
            }
            #endregion
        }
        #endregion
    }
}
