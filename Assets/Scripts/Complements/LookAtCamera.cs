using UnityEngine;

namespace UtilsComplements
{
    [ExecuteAlways]
    public class LookAtCamera : MonoBehaviour
    {        
        #region Unity Logic
        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
        #endregion
    }
}
