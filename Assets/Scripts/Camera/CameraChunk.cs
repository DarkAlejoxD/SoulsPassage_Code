using UnityEngine;
using System.Collections.Generic;
using System;

namespace Cameras
{
    /// <summary>
    /// Put all the cameras inside a gameObject with the CameraChunk
    /// </summary>
    [Obsolete]
    [RequireComponent(typeof(BoxCollider))]
    public class CameraChunk : MonoBehaviour
    {
        //[SerializeField] private bool _isStartingChunk = false;
        //public bool IsActive { get; private set; }

        //private void Awake()
        //{
        //    if (!_isStartingChunk)
        //        SetCamerasActive(false);
        //}

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (!other.CompareTag("Player"))
        //        return;

        //    IsActive = true;
        //    SetCamerasActive(true);
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    if (!other.CompareTag("Player"))
        //        return;

        //    IsActive = false;
        //    SetCamerasActive(false);
        //}
        //private void SetCamerasActive(bool value)
        //{
        //    foreach (Transform child in transform)
        //    {
        //        child.gameObject.SetActive(value);
        //    }
        //}
    }
}
