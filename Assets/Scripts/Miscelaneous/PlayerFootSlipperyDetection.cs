using UnityEngine;

namespace AvatarController.Misc
{
    [RequireComponent(typeof(Collider))]
    public class PlayerFootSlipperyDetection : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private PlayerController _controller;
        [SerializeField] private LayerMask _slipperyLayers;        
        [SerializeField, Range(0, 0.5f)] private float _slipperyThreshold;
        private Collider _collider;

        [Header("Attributes")]
        [SerializeField] private float _force;
        [SerializeField] private float _radiusLedge = 0.18116f / 2f;
        #endregion    

        #region Unity Logic
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (DetectGround(_slipperyLayers, out Collider col))
            {
                if (col.CompareTag("Walkable"))
                    return;
                AddForce(col);
            }

            Ray ray = new(transform.position, Vector3.down);


            //Debug.Log(Camera.main.WorldToViewportPoint(_collider.transform.position));
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == _slipperyLayers.value)
            {
                Debug.Log("Collides");
                if (other.CompareTag("Walkable"))
                    return;
                AddForce(other);
            }
        }

        private void AddForce(Collider other)
        {
            //Calculates the normal and get the component based on the normal axis
            Vector3 normal = other.transform.right;
            Vector3 directionToPlayer = transform.position - other.transform.position;
            float dot = Vector3.Dot(normal, directionToPlayer.normalized);
            float distance = directionToPlayer.magnitude * dot;

            //Calculates the distance and applies force
            float forcePct = distance / _radiusLedge;
            if (forcePct < 0)
                forcePct = Mathf.Clamp(forcePct, -1, -_slipperyThreshold);
            else
                forcePct = Mathf.Clamp(forcePct, _slipperyThreshold, 1);

            _controller.AddForce(_force * forcePct * normal, Time.deltaTime);
        }
        #endregion

        #region 
        private bool DetectGround(LayerMask mask, out Collider collider)
        {
            Vector3 center = _collider.bounds.center;
            Vector3 halfExtents = _collider.bounds.extents;

            Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, mask);

            if (hits.Length > 0)
            {
                collider = hits[0];
                return true;
            }
            else
            {
                collider = null;
                return false;
            }
        }
        #endregion

        
    }
}
