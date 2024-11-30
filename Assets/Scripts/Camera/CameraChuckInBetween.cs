using System;
using UnityEngine;

namespace Cameras
{
    [Obsolete]
    public class CameraChuckInBetween : MonoBehaviour
    {
        //[SerializeField] private CameraChunk _before;
        //[SerializeField] private CameraChunk _after;

        //private bool _isActive;

        //private void Start()
        //{
        //    _isActive = false;
        //    SetCamerasActive(false);
        //}

        //private void Update()
        //{
        //    if (_before.IsActive || _after.IsActive)
        //    {
        //        if (!_isActive)
        //        {
        //            SetCamerasActive(true);
        //            _isActive = true;
        //        }
        //    }
        //    else
        //    {
        //        if (_isActive)
        //        {
        //            SetCamerasActive(false);
        //            _isActive = false;
        //        }
        //    }
        //}

        //private void SetCamerasActive(bool value)
        //{
        //    foreach (Transform transform in transform)
        //    {
        //        transform.gameObject.SetActive(value);
        //    }
        //}
    }
}
