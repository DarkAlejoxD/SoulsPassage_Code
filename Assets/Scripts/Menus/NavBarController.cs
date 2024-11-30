using System.Collections.Generic;
using UnityEngine;

namespace MenuManagement //add it to a concrete namespace
{
    public class NavBarController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform _content;
        private List<NavItem> _navItems;

        //ButtonNavigation
        private int _selectedIndex;

        #endregion

        #region Unity Logic

        private void Start()
        {
            _navItems = new List<NavItem>();
            foreach (Transform child in _content)
            {
                if (child.gameObject.activeSelf)
                    _navItems.Add(child.GetComponent<NavItem>());
            }

            DisableAll();
            _navItems[0].Activate();
        }

        private void Update()
        {
            //DEBUG: Change to new input system y soporte de mando
            if (Input.GetKeyDown(KeyCode.Q)) //Derecha
            {
                if (_selectedIndex > 0)
                    _selectedIndex--;

                DisableAll();
                _navItems[_selectedIndex].Activate();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (_selectedIndex < (_navItems.Count - 1))
                    _selectedIndex++;

                DisableAll();
                _navItems[_selectedIndex].Activate();
            }
        }

        #endregion


        #region Public Methods

        public void SetNavPanel(NavItem item)
        {
            DisableAll();
            item.Activate();
        }

        #endregion

        #region Private Methods

        private void DisableAll()
        {
            foreach (NavItem navItem in _navItems)
            {
                navItem.Deactivate();
            }
        }

        #endregion
    }
}
