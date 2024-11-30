using System;
using UnityEngine;
using UtilsComplements;

namespace Poltergeist
{
    /// <summary> 
    /// Unique for alpha, shouldn't make it in beta. 
    /// </summary>
    [Obsolete("Poltergeist don't use Trigger zones anymore, you may want PoltergeistStencilControl")]
    [RequireComponent(typeof(Collider))]
    public class PoltergeistZone : MonoBehaviour
    {
        [SerializeField] private Poltergeist_Item _item;

        #region Unity Logic
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (Singleton.TryGetInstance(out PoltergeistManager manager))
            {
                //manager._evaluatedPoltergeist = _item;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (!Singleton.TryGetInstance(out PoltergeistManager manager))
                return;

            //if(manager._evaluatedPoltergeist == _item)
            //    return;

            //manager._evaluatedPoltergeist = _item;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            //if (Singleton.TryGetInstance(out PoltergeistManager manager))
            //{
            //    if (manager._evaluatedPoltergeist == _item)
            //        manager._evaluatedPoltergeist = null;
            //}
        }
        #endregion
    }
}