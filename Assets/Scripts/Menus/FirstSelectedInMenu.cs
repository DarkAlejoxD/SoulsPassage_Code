using UnityEngine;
using UnityEngine.EventSystems;

namespace MenuManagement.Settings
{
    public class FirstSelectedInMenu : MonoBehaviour
    {
        [SerializeField] GameObject _selected;
        EventSystem _eventSystem;

        private void Awake()
        {
            _eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }

        private void OnEnable()
        {
            _eventSystem.SetSelectedGameObject(_selected);
        }
    }

}
