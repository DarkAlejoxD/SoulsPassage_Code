using InputController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilsComplements;


namespace Pause
{
    [Obsolete("Using PauseManager")]
    public class PauseController : MonoBehaviour
    {

        #region Fields

        [SerializeField] private GameObject _pausePanel;

        #endregion




        #region Unity Logic

        private void Awake()
        {
            //ISingleton<InputManager>.GetInstance().OnInputDetected += OnGetInputs;
        }

        private void OnDisable()
        {
            //ISingleton<InputManager>.GetInstance().OnInputDetected -= OnGetInputs;
        }

        // Update is called once per frame
        void Update()
        {
            //DEBUG
            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause();
            }
        }

        #endregion


        #region Public Methods

        public void Unpasue()
        {
            _pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }

        #endregion



        #region Private Methods

        private void Pause()
        {
            _pausePanel.SetActive(true);
            Time.timeScale = 0.0f;
        }



        private void OnGetInputs(InputValues inputs)
        {
            //if(inputs.)
        }

        #endregion
    }
}
