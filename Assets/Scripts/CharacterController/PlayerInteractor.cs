using UnityEngine;
using Interactable;

namespace AvatarController.Interaction 
{
    [RequireComponent(typeof(PlayerController), typeof(CharacterController))]
    public class PlayerInteractor : MonoBehaviour
    {
        private const float CAST_SPHERE_EVERY_X_FRAMES = 15;

        #region Fields
        [SerializeField] private LayerMask _interactionLayer;
        [SerializeField] private Transform _interactionPoint;
        [SerializeField] private float _interactionRadius;

        [Header("DEBUG"), Space(10)]
        [SerializeField] private Color gizmosColor;

        private PlayerController _playerController;
        private IInteractable _selectedInteractable;


        #endregion

        #region Unity Logic
        private void OnEnable()
        {
            if (_playerController == null)
                _playerController = GetComponent<PlayerController>();

            _playerController.OnInteract += OnInteract;
        }

        private void OnDisable()
        {
            if (_playerController == null)
                _playerController = GetComponent<PlayerController>();

            _playerController.OnInteract -= OnInteract;
        }

        private void Awake()
        {    
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            
        }

        private void Update()
        {      
            //TODO: Every x frames cast sphere
            if(Time.frameCount % CAST_SPHERE_EVERY_X_FRAMES == 0)
            {
                CastInteractionSpehere();
            }
        }
        #endregion


        #region Public Methods
        public void PublicMethod()
        {

        }
        #endregion

        #region Private Methods
        private void OnInteract(bool active)
        {
            if (!active) return;
            //if (!_playerController.IsGrounded) return;

            if(_selectedInteractable != null)
            {
                _selectedInteractable.Interact();
            }
        }

        private void CastInteractionSpehere()
        {
            Collider[] col = Physics.OverlapSphere(_interactionPoint.position, _interactionRadius, _interactionLayer);
                        
            IInteractable interactable = null;
            
            //If no collisions detected, unselect previous one and return
            if(col.Length == 0)
            {
                if (_selectedInteractable == null) return; 
                _selectedInteractable.Unselect();
                _selectedInteractable = null;
                return;
            }
            else if(col.Length == 1)
            {
                interactable = col[0].GetComponent<IInteractable>();
            }
            else
            {
                //If there were more than 1 collision detected,
                //choose the closest to the player.
                interactable = GetClosestInteractable(col);
            }

            if (_selectedInteractable == null)
            {
                _selectedInteractable = interactable;
                _selectedInteractable.Select();
            }
            else if (interactable != _selectedInteractable)
            {
                _selectedInteractable.Unselect();
                _selectedInteractable = interactable;
                _selectedInteractable.Select();
            }
            
        }

        private IInteractable GetClosestInteractable(Collider[] col)
        {
            int index = 0;
            Vector3[] interactablePositions = new Vector3[col.Length];
            for (int i = 0; i < col.Length; i++)
            {                
                interactablePositions[i] = col[i].transform.position;
            }
            
            index = MathUtils.GetClosestPoint(_interactionPoint.position, interactablePositions);
            return col[index].GetComponent<IInteractable>();
        }
    


    #endregion
    #region DEBUG
        private void OnDrawGizmos()
        {
            if (_interactionPoint == null) return;
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionRadius);
        }
        #endregion
    }
}
