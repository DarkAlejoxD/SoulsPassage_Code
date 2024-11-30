using UnityEngine;
using UnityEngine.Events;

namespace BaseGame
{
    [RequireComponent(typeof(Collider))]
    public class Checkpoint : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [Tooltip("To try reproduce the events had happened before")]
        [SerializeField, HideInInspector] private Checkpoint _lastCheckpointReference;
        [SerializeField, HideInInspector] private UnityEvent _unityEvent;
        [SerializeField] private Vector3 _playerOffset = Vector3.zero;
        [SerializeField] private bool _isStartPoint = false;
        public static Checkpoint CurrentCheckpoint { get; private set; }
        #endregion

        #region Unity Logic
        static Checkpoint() => CurrentCheckpoint = null;
        private void Awake()
        {
            GetComponent<Collider>().isTrigger = true;

            if (_isStartPoint)
            {
                if (CurrentCheckpoint == null)
                    CurrentCheckpoint = this;
                else
                    Debug.LogWarning("More than one starting Checkpoint", this);
            }
        }

        private void Start()
        {
            if (CurrentCheckpoint == null)
            {
                Debug.Log("StartingCheckpoint don't assigned, a random one will be assigned");
                CurrentCheckpoint = this;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                CurrentCheckpoint = this;
        }
        #endregion

        #region Public Methods
        public Vector3 GetSpawnPosition()
        {
            return transform.position + _playerOffset;
        }

        public void ReproduceLastEvents()
        {
            _unityEvent?.Invoke();
            if (_lastCheckpointReference != null)
                _lastCheckpointReference.ReproduceLastEvents();
        }
        #endregion

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new(217 / 255f, 100 / 255f, 22 / 255);
            Gizmos.DrawWireSphere(GetSpawnPosition(), 0.2f);
            Gizmos.color = Color.white;

            UnityEditor.Handles.color = Gizmos.color;
            Gizmos.DrawLine(GetSpawnPosition(), GetSpawnPosition() + transform.forward * 1);
            UnityEditor.Handles.color = Color.white;
        }
#endif
        #endregion

    }
}
