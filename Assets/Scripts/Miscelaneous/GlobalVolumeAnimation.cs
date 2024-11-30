using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Miscelaneous
{
    /// <summary>
    /// 1. Añadir la variable reflejo en el script para que se muestre en el inspector.
    /// 2. Crear referencia al subapartado del perfil del Volume. (Variable)
    /// 3. Crear Propiedad para obtenerlo y que no sea nulo. (Comprobación de Variable)
    /// 4. Crear reflejo de variable en el Animator.
    /// 
    /// 5. Añadir logica en UpdateAnimator()
    /// 6. Añadir logica en el GLobalVopmueUpdate().
    /// 7. Testear que al tocar la variable en el script, se vea reflejado en el volumen.
    /// 
    /// 8. Hacer la animacion
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Animator))]
    public class GlobalVolumeAnimation : MonoBehaviour
    {
        #region Fields
        [Header("Global Volume")]
        [SerializeField] private Volume _globalVolume;
        private ColorAdjustments _gvColorAdjust;
        private DepthOfField _gvDepthOfField;
        private Tonemapping _gvTonemapping;
        //private DepthOfField _gvDepthOfField;

        [Header("Color Adjusment Control")]
        [SerializeField, Range(-100, 100)] private float _contrast;
        [SerializeField, Range(-10, 10)] private float _postexposure = 0;
        [SerializeField, Range(-100, 15)] private float _saturation = 10;


        [Header("Focal Lenght")] // Min(1) or Max(1)
        [SerializeField, Range(0.5f, 100)] private float _focusDistance = 0.86f;
        [SerializeField, Range(1f, 100)] private float _focalLength = 40;

        [Header("Tonemaping")]
        [SerializeField] private bool _toneActive;

        /*
        [Header("[NAME] Control")]
        [SerializeField, Range(-100, 100)] private float _contrast;
        */
        private Animator _animator;

        private ColorAdjustments GlobalColorAdjust
        {
            get
            {
                if (_gvColorAdjust == null)
                {
                    if (_globalVolume.profile.TryGet(out _gvColorAdjust))
                        return _gvColorAdjust;
                    else
                        return null;
                }
                return _gvColorAdjust;
            }
        }

        private DepthOfField DepthOfFieldControl
        {
            get
            {
                if (_gvDepthOfField == null)
                {
                    if (_globalVolume.profile.TryGet(out _gvDepthOfField))
                        return _gvDepthOfField;
                    else
                        return null;
                }
                return _gvDepthOfField;
            }
        }

        private Tonemapping TonemappingControl
        {
            get
            {
                if (_gvTonemapping == null)
                {
                    if (_globalVolume.profile.TryGet(out _gvTonemapping))
                        return _gvTonemapping;
                    else
                        return null;
                }
                return _gvTonemapping;
            }
        }

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
            UpdateGlobalVolumeValues();
        }

        private void Update()
        {
            if (UpdateAnimatorValues())
                UpdateGlobalVolumeValues();
        }
        #endregion

        #region Animator Logic //Do touch
        private bool UpdateAnimatorValues()
        {
            bool anyChange = false;
            //*******************

            const string POSTEXPOSURE_VALUE = "PostExposure";

            float lastPostExposure = AnimController.GetFloat(POSTEXPOSURE_VALUE);
            if (lastPostExposure != _postexposure)
            {
                AnimController.SetFloat(POSTEXPOSURE_VALUE, _postexposure);
                anyChange = true;
            }

            const string CONTRAST_VALUE = "Contrast";

            float lastContrastValue = AnimController.GetFloat(CONTRAST_VALUE);
            if (lastContrastValue != _contrast)
            {
                AnimController.SetFloat(CONTRAST_VALUE, _contrast);
                anyChange = true;
            }

            const string SATURATION_VALUE = "Saturation";

            float saturationValue = AnimController.GetFloat(SATURATION_VALUE);
            if (lastContrastValue != _saturation)
            {
                AnimController.SetFloat(SATURATION_VALUE, _saturation);
                anyChange = true;
            }

            const string DEPTH_FIELD_VALUE = "DepthField";

            float lastDepthField = AnimController.GetFloat(DEPTH_FIELD_VALUE);
            if (lastDepthField != _focalLength)
            {
                AnimController.SetFloat(DEPTH_FIELD_VALUE, _focalLength);
                anyChange = true;
            }

            const string FOCUS_DISTANCE_VALUE = "FocusDistance";

            float focusDistance = AnimController.GetFloat(FOCUS_DISTANCE_VALUE);
            if (lastDepthField != _focusDistance)
            {
                AnimController.SetFloat(FOCUS_DISTANCE_VALUE, _focusDistance);
                anyChange = true;
            }

            if (TonemappingControl != null)
            {
                bool tonemappingActive = TonemappingControl.active;
                if (tonemappingActive != _toneActive)
                    anyChange = true;
            }

            /*
            const string ANIM_VALUE = "[Parameter name]";
            float lastParamName = AnimController.GetFloat(ANIM_VALUE);
            if (lastParamName != _newParam)
            {
                AnimController.SetFloat(ANIM_VALUE, _newParam);
                anyChange = true;
            }
            */


            //********************
            return anyChange;
        }

        private void UpdateGlobalVolumeValues()
        {
            if (GlobalColorAdjust != null)
            {
                GlobalColorAdjust.contrast.Override(_contrast);
                GlobalColorAdjust.postExposure.Override(_postexposure);
                GlobalColorAdjust.saturation.Override(_saturation);
                //Write some code here
            }

            if (DepthOfFieldControl != null)
            {
                DepthOfFieldControl.focalLength.Override(_focalLength);
                DepthOfFieldControl.focusDistance.Override(_focusDistance);
            }

            if (TonemappingControl != null)
            {
                TonemappingControl.active = _toneActive;
                if (_toneActive)
                    TonemappingControl.mode.Override(TonemappingMode.ACES);
            }

            //Debug.Log("Something Changed");
        }
        #endregion
    }
}
