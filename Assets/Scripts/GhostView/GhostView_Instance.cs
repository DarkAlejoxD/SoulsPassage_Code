using System;
using System.Collections;
using UnityEngine;
using UtilsComplements;

namespace GhostView
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    public class GhostView_Instance : MonoBehaviour
    {
        #region Fields
        private const string ALPHA_VALUE = "_AlphaValue";
        private const string ALBEDO_COLOR = "_Albedo";

        [Header("Render References")]
        [SerializeField] private Transform _art;
        [Tooltip("true: appears when button down" +
                 "\nfalse: dissapears when button down")]
        [SerializeField, Obsolete] private bool _inversed;
        private Color _startColor;
        //private Renderer _renderer;
        private Collider _collider;

        private MaterialPropertyBlock _materialPropertyBlock;
        private MaterialPropertyBlock ThisMaterialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();

                return _materialPropertyBlock;
            }
        }
        #endregion

        #region Unity Logic
        private void Awake()
        {
            //_renderer = _art.GetComponent<Renderer>();
            _collider = _art.GetComponent<Collider>();
            GhostViewManager.OnActivateGhostView += GhostView;
            _startColor = new Color();// _renderer.sharedMaterial.GetColor(ALBEDO_COLOR);
        }

        private void Start()
        {
            //if (_inversed)
            //{
            //    SetActiveCollision(true);
            //    _art.gameObject.SetActive(true);
            //}
            //else
            {
                ApplyAlphaValue(1);
                SetActiveCollision(false);
                _art.gameObject.SetActive(false);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                _collider.isTrigger = true;
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                _collider.isTrigger = false;
        }

        private void OnDestroy()
        {
            GhostViewManager.OnActivateGhostView -= GhostView;
        }
        #endregion

        #region Private Methods
        private void ApplyAlphaValue(float value)
        {

            ThisMaterialPropertyBlock.SetFloat(ALPHA_VALUE, value);
            //Color color = _startColor;
            //color.a = Mathf.Lerp(0, color.a, 1-value);
            //ThisMaterialPropertyBlock.SetColor("_Color", color);
            _art.GetComponent<Renderer>().SetPropertyBlock(ThisMaterialPropertyBlock);
        }

        private void SetActiveCollision(bool state)
        {
            if (!_collider)
                return;

            if (state)
            {
                _collider.enabled = true;
            }
            else
            {
                _collider.enabled = false;
            }
        }

        private void GhostView(Vector3 origin, float radius)
        {
            //Debug.Log("Reach");
            float distance = Vector3.Distance(origin, transform.position);

            if (distance > radius)
                return;

            StopAllCoroutines();

            //if (_inversed)
            //{
            //    SetActiveCollision(false);
            //    StartCoroutine(DissapearCoroutine(() =>
            //    {
            //        _art.gameObject.SetActive(false);
            //        StartCoroutine(StayCoroutine(() =>
            //        {
            //            _art.gameObject.SetActive(true);
            //            StartCoroutine(AppearCoroutine(() =>
            //            {
            //                SetActiveCollision(true);
            //                ThisMaterialPropertyBlock.SetColor(ALPHA_VALUE, _startColor);
            //                _renderer.SetPropertyBlock(ThisMaterialPropertyBlock);
            //            }, false));
            //        }, false));
            //    }, false));
            //}
            //else
            {
                SetActiveCollision(true);
                _art.gameObject.SetActive(true);
                StartCoroutine(AppearCoroutine(() =>
                {
                    StartCoroutine(StayCoroutine(() =>
                    {
                        StartCoroutine(DissapearCoroutine(() =>
                        {
                            SetActiveCollision(false);
                            _art.gameObject.SetActive(false);
                            ApplyAlphaValue(0);
                        }));
                    }));
                }));
            }
        }

        private IEnumerator AppearCoroutine(Action end, bool firstAppear = true)
        {
            if (ISingleton<GhostViewManager>.TryGetInstance(out var manager))
            {
                float timeToAppear = manager._values.AppearTime;

                for (float i = 0; i <= timeToAppear; i += Time.deltaTime)
                {
                    ApplyAlphaValue(1 - (i / timeToAppear));
                    yield return new WaitForSeconds(Time.deltaTime);
                }

                end.Invoke();
            }
        }

        private IEnumerator StayCoroutine(Action end, bool firstAppear = true)
        {
            if (ISingleton<GhostViewManager>.TryGetInstance(out var manager))
            {
                ApplyAlphaValue(0);
                yield return new WaitForSeconds(manager._values.Staytime);
                end.Invoke();
            }
        }

        private IEnumerator DissapearCoroutine(Action end, bool firstAppear = true)
        {
            if (ISingleton<GhostViewManager>.TryGetInstance(out var manager))
            {
                float timeToAppear = manager._values.DisapearTime;

                for (float i = 0; i <= timeToAppear; i += Time.deltaTime)
                {
                    ApplyAlphaValue((i / timeToAppear));
                    yield return new WaitForSeconds(Time.deltaTime);
                }
                end.Invoke();
            }
        }
        #endregion
    }
}
