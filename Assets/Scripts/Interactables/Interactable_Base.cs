using UnityEngine;

namespace Interactable //add it to a concrete namespace
{
    //Maybe make it abstract?
    //Btw srry for touching your scripts, for compensation i will touch you later on ;)
    //ok no xdxd
    public class Interactable_Base : MonoBehaviour, IInteractable
    {
        #region Fields
        private Material _outlineMaterial;
        #endregion    

        #region Unity Logic
        protected virtual void Awake()
        {
            _outlineMaterial = GetComponent<MeshRenderer>().materials[1];
        }
        #endregion

        #region Public Methods
        public virtual void Interact()
        {
            Debug.Log($"Interacted with {name}");
        }

        public virtual bool CanInteract()
        {
            return true;
        }

        public virtual void Select()
        {
            Debug.Log($"{name} is selected");
            if (_outlineMaterial) //sryy srrys rys
                _outlineMaterial.SetInt("_ShowOutline", 1);
        }

        public virtual void Unselect()
        {
            if (_outlineMaterial) //sryy srrys rys
                _outlineMaterial.SetInt("_ShowOutline", 0);
        }
        #endregion
    }
}
