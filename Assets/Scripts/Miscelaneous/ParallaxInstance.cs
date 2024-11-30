using System.Collections.Generic;
using UnityEngine;

namespace BaseGame //It should be in Miscelaneous, but...
{
    public class ParallaxInstance : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Transform _maximumHandler;
        [SerializeField] private List<Transform> _lvl1Variations;
        [SerializeField] private List<Transform> _lvl2Variations;
        [SerializeField] private List<Transform> _lvl3Variations;

        private Transform _currentVariation;
        public Vector3 Distance
        {
            get
            {
                if (!_maximumHandler)
                {
                    Debug.LogError("Check the maximumHandler, it's not assigned");
                    return Vector3.one;
                }

                return _maximumHandler.position - transform.position;
            }
        }
        #endregion    

        #region Public Methods
        public void ActivateRandomVariation(int lvl)
        {
            _currentVariation?.gameObject.SetActive(false);

            switch (lvl)
            {
                case 1:
                    ActivateVariation(ref _lvl1Variations);
                    break;
                case 2:
                    ActivateVariation(ref _lvl2Variations);
                    break;
                case 3:
                    ActivateVariation(ref _lvl3Variations);
                    break;
                default:
                    ActivateVariation(ref _lvl1Variations);
                    break;
            }

            _currentVariation?.gameObject.SetActive(true);
        }
        #endregion

        #region Private Methods
        private void ActivateVariation(ref List<Transform> variationList)
        {
            int variations = variationList.Count;
            int randomIndex = Random.Range(0, variations);
            _currentVariation = variationList[randomIndex];
        }
        #endregion
    }
}
