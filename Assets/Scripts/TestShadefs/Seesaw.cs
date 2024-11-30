using UnityEngine;

namespace Miscelaneous
{
    public class Seesaw : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Transform _test;

        [Header("Attributes")]
        [SerializeField] private float _radius;
        [SerializeField] private bool _slippery;
        private Vector2 _desiredWeight;
        private Vector2 _currentWeight;
        #endregion    

        #region Unity Logic
        private void Awake()
        {
        }

        private void Update()
        {
        }
        #endregion

        #region Static Methods
        public static void StaticMethod()
        {
        }
        #endregion

        #region Public Methods
        public void PublicMethod()
        {
        }
        #endregion

        #region Private Methods
        private void PrivateMethod()
        {
        }
        #endregion
    }
}
