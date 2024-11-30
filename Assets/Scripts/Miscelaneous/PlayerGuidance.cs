using UnityEngine;
using UtilsComplements;

namespace Miscelaneous
{    
    public class PlayerGuidance : MonoBehaviour, ISingleton<PlayerGuidance>
    {
        #region Fields
        [Header("Section1")]
        [SerializeField] private float _privateAttribute;

        public ISingleton<PlayerGuidance> Instance => this;
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
