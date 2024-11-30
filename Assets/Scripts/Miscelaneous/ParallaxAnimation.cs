using System;
using UnityEngine;

namespace Miscelaneous
{
    [ExecuteAlways]
    [RequireComponent(typeof(Animator))]
    public class ParllaxAnimation : MonoBehaviour
    {
        private const string BLEND_VALUE = "_BlendFactor";
        private const string EXPOSURE1_VALUE = "_Exposure1";
        private const string EXPOSURE2_VALUE = "_Exposure2";

        private const string BLEND_ANIM_KEY = "BlendFactor";
        private const string EXPOSURE1_ANIM_KEY = "Exposure1";
        private const string EXPOSURE2_ANIM_KEY = "Exposure2";

        #region Fields
        [Header("Skybox")]
        [SerializeField] private Material _skyboxMaterial;

        [Header("Color Adjusment Control")]
        [SerializeField, Range(0, 1)] private float _blendFactor = 0;
        [SerializeField] private float _exposure1 = 0.41f;
        [SerializeField] private float _exposure2 = 0.41f;

        private Animator _animator;

        private Animator AnimController
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }

        #endregion    

        #region Unity Logic //Dont Touch
        private void OnValidate()
        {
            UpdateSkyboxValues();
        }

        private void Update()
        {
            if (UpdateAnimatorValues())
                UpdateSkyboxValues();
        }
        #endregion

        #region Animator Logic //Do touch
        private bool UpdateAnimatorValues()
        {
            bool anyChange = false;

            float lastBlendValue = AnimController.GetFloat(BLEND_ANIM_KEY);
            if (lastBlendValue != _blendFactor)
            {
                AnimController.SetFloat(BLEND_ANIM_KEY, _blendFactor);
                anyChange = true;
            }

            float lastExposure1Value = AnimController.GetFloat(EXPOSURE1_ANIM_KEY);
            if (lastExposure1Value != _exposure1)
            {
                AnimController.SetFloat(EXPOSURE1_ANIM_KEY, _exposure1);
                anyChange = true;
            }

            float lastExposure2Value = AnimController.GetFloat(EXPOSURE2_ANIM_KEY);
            if (lastExposure2Value != _exposure2)
            {
                AnimController.SetFloat(EXPOSURE2_ANIM_KEY, _exposure2);
                anyChange = true;
            }
            return anyChange;
        }

        private void UpdateSkyboxValues()
        {
            _skyboxMaterial.SetFloat(BLEND_VALUE, _blendFactor);
            _skyboxMaterial.SetFloat(EXPOSURE1_VALUE, _exposure1);
            _skyboxMaterial.SetFloat(EXPOSURE2_VALUE, _exposure2);
        }
        #endregion
    }
}
