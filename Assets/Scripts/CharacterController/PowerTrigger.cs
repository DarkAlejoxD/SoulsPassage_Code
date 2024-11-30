using UnityEngine;
using BaseGame;

namespace AvatarController.Powers
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    public class PowerTrigger : MonoBehaviour
    {
        private enum PowerType
        {
            Poltergeist,
            GhostView
        }
        #region Fields
        [SerializeField] private PowerType _powerType;
        private bool _activated;
        #endregion    

        #region Unity Logic
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
            _activated = false;
        }

        //private void OnValidate()
        //{
        //    var list = GameObject.FindObjectsOfType<PowerTrigger>();
        //    foreach (var item in list)
        //    {
        //        if (item != this)
        //        {
        //            if (item._powerType == this._powerType)
        //                Debug.Log($"It already Exists one PowerTrigger Of type: {_powerType}", item);
        //        }
        //    }
        //}

        private void OnTriggerEnter(Collider other)
        {
            if (_activated)
                return;

            if (other.CompareTag("Player"))
            {
                _activated = true;
                switch (_powerType)
                {
                    case PowerType.Poltergeist:
                        GameManager.GetGameManager().PlayerInstance.DataContainer.Powers.UnlockPoltergeist();
                        break;
                    case PowerType.GhostView:
                        GameManager.GetGameManager().PlayerInstance.DataContainer.Powers.UnlockGhostView();
                        break;
                }
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
