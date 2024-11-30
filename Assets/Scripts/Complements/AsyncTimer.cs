using System;
using System.Collections;
using UnityEngine;

namespace UtilsComplements
{
    public static class AsyncTimer
    {
        public static IEnumerator TimerCoroutine(float time, Action onEnd)
        {
            yield return new WaitForSeconds(time);
            onEnd.Invoke();
        }
    }
}