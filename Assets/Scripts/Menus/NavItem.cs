using UnityEngine;
using UnityEngine.UI;
using MenuManagement;

public class NavItem : MonoBehaviour
{
    public bool _active;

    [Header("Refereces")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _background;

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _disabledColor;

    private NavBarController _controller;

    //Pa cosas futuras

    private void Awake()
    {
        Transform currentParent = transform.parent;
        while (currentParent != null)
        {
            if (currentParent.TryGetComponent(out NavBarController navBar))
            {
                _controller = navBar;
                return;
            }

            currentParent = currentParent.parent;
        }
        Debug.LogWarning("There is no Nav Bar Controller!");
    }


    public void Activate()
    {
        _active = true;
        _background.color = _activeColor;
        _panel.SetActive(true);
    }

    public void Deactivate()
    {
        _active = false;
        _background.color = _disabledColor;
        _panel.SetActive(false);
    }

    public void TryPanelActivation()
    {
        _controller.SetNavPanel(this);
    }
}
