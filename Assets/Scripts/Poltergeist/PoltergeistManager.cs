using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UtilsComplements;
using static UtilsComplements.AsyncTimer;
using BaseGame;

namespace Poltergeist
{
    public class PoltergeistManager : MonoBehaviour, ISingleton<PoltergeistManager>
    {
        private const float VERTICAL_DIVISION = 1f / 16;
        private const float HORIZONTAL_DIVISION = 1f / 32;

        [Header("References")]
        [SerializeField] private Transform _particleFather;
        [SerializeField] private ParticleSystem[] _selectionParticleSystem;

        [Header("Lists")]
        private List<Poltergeist_Item> _poltergeistList = new();
        private List<Poltergeist_Item> _nearPoltergeists = new();

        private int _indexControl;
        private bool _evaluating;

        [Header("Attributes")]
        [SerializeField, Min(0.01f)] private float _timerToDie = 2;

        public static Action OnPolterEnter;
        public static Action OnPolterExit;

        [Header("Delegates")]
        public UnityEvent OnPoltergeistEventEnter;
        public UnityEvent OnPoltergeistEventExit;
        public UnityEvent OnPoltergeistEventEnterDelayed;
        [SerializeField, Min(0.1f)] private float _delayedTime = 1f;

        public ISingleton<PoltergeistManager> Instance => this;

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();
            _evaluating = false;
            OnPoltergeistEventExit?.Invoke();
            DeactivateParticles();
        }

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Public Methods
        public void ActivateParticles()
        {
            foreach (var item in _selectionParticleSystem)
            {
                item.Play();
            }
        }

        public void DeactivateParticles()
        {
            foreach (var item in _selectionParticleSystem)
            {
                item.Stop();
            }
        }

        public void SetParticlePosition(Vector3 position)
        {
            _particleFather.position = position;
        }

        /// <summary>
        /// Should be triggered at the beginning of the poltergeistMode
        /// </summary>
        public void StartPoltergeist(Transform target, float radius)
        {
            OnPolterEnter?.Invoke();
            OnPoltergeistEventEnter?.Invoke();
            _evaluating = false;
            //_nearPoltergeists = GetNearPoltergeist(target, radius);
            UpdateNearestObjects(target, radius);
            StopAllCoroutines();

            StartCoroutine(TimerCoroutine(_delayedTime,
                () => OnPoltergeistEventEnterDelayed?.Invoke()));

            if (_poltergeistList == null)
            {
                GameManager.GetGameManager().PlayerInstance.PolterNotFound();
                return;
            }

            if (_nearPoltergeists.Count <= 0)
                GameManager.GetGameManager().PlayerInstance.PolterNotFound();

            foreach (var item in _nearPoltergeists)
            {
                item.StartPoltergeist();
            }
            _indexControl = SensingUtils.GetNearestIndex(_nearPoltergeists.ToArray(), target,
                                                         Camera.main.transform.right);
        }

        /// <summary> Get next node </summary>
        /// <param name="direction"> should be a number between -1 & 1</param>
        public Poltergeist_Item GetNext(int direction)
        {
            if (!_evaluating) // if it's not initialized
            {
                _evaluating = true;
                if (direction < 0)
                    _indexControl--;
            }
            else
            {
                if (direction > 0)
                    _indexControl++;
                else
                    _indexControl--;
            }

            _indexControl = Math.Clamp(_indexControl, 0, _nearPoltergeists.Count - 1);
            Debug.Log("IndexControl: " + _indexControl +
                      "\nNear: " + _nearPoltergeists.Count +
                      "\nTotal: " + _poltergeistList.Count);
            return _nearPoltergeists[_indexControl];
        }

        public void UpdateOrder(Poltergeist_Item item)
        {
            _nearPoltergeists = _nearPoltergeists.OrderBy(item =>
                Camera.main.WorldToScreenPoint(item.transform.position).x).ToList();
            _indexControl = _nearPoltergeists.IndexOf(item);
        }

        public void EndPoltergeist()
        {
            OnPolterExit?.Invoke();
            StartCoroutine(TimerCoroutine(_timerToDie, () =>
            {
                foreach (var item in _nearPoltergeists)
                {
                    item.EndPoltergeist();
                }
            }));
        }

        public void ActivatePoltergeist(Poltergeist_Item item) => item.Manipulate();

        public void DeactivateManipulation(Poltergeist_Item item) => item.NoManipulating();

        internal void AddPoltergeist(Poltergeist_Item item)
        {
            if (_poltergeistList == null)
                _poltergeistList = new List<Poltergeist_Item>();

            if (!_poltergeistList.Contains(item))
                _poltergeistList.Add(item);

            else
                Debug.LogWarning("You're trying to ad an existing polter Item in the Polter " +
                    "manager", item);
        }

        internal void RemovePoltergeist(Poltergeist_Item item)
        {
            if (_poltergeistList == null || _poltergeistList.Count <= 0)
                return;

            if (_poltergeistList.Contains(item))
                _poltergeistList.Remove(item);
        }

        public static Vector3 ScreenPosCorrection(Vector3 position)
        {
            //Check if is in the deadzone or not
            Camera cam = Camera.main;

            Vector3 screenPos = cam.WorldToViewportPoint(position);

            float minW = HORIZONTAL_DIVISION * 1;
            float maxW = 1 - minW;
            float minH = VERTICAL_DIVISION * 1;
            float maxH = 1 - minH;

            //Checks up Deadzone
            if (screenPos.y < minH)
                screenPos.y = minH;
            else if (screenPos.y > maxH)
                screenPos.y = maxH;

            if (screenPos.x < minW)
                screenPos.x = minW;
            else if (screenPos.x > maxW)
                screenPos.x = maxW;

            if (screenPos.z < 1)
                screenPos.z = 1;

            return cam.ViewportToWorldPoint(screenPos);
        }
        #endregion

        #region Private Methods
        private void UpdateNearestObjects(Transform target, float radius)
        {
            //Check nullity
            if (_poltergeistList.Count <= 0)
                return;

            //GetNearest
            List<Poltergeist_Item> nearList = new(_poltergeistList.Count);

            for (int i = 0; i < _poltergeistList.Count; i++)
            {
                float distance = Vector3.Distance(_poltergeistList[i].transform.position,
                                                  target.position);
                if (distance < radius)
                {
                    if (CheckIfIsInCamera(_poltergeistList.ElementAt(i).transform))
                        nearList.Add(_poltergeistList[i]);
                }
            }

            _nearPoltergeists.Clear();
            _nearPoltergeists = nearList.OrderBy(item =>
                Camera.main.WorldToScreenPoint(item.transform.position).x).ToList();
        }

        private static bool CheckIfIsInCamera(Transform obj)
        {
            Camera cam = Camera.main;
            Vector3 viewportPos = cam.WorldToViewportPoint(obj.transform.position);
            if (viewportPos.z < 0)
                return false;

            return true;
        }
        #endregion
    }
}