using UnityEngine;

namespace Interactable //add it to a concrete namespace
{
    public interface IInteractable 
    {
        public void Interact();
        public bool CanInteract();

        public void Select();
        public void Unselect();
    }
}
