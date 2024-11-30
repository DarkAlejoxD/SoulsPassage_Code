using AvatarController.Animations;
using UnityEditor;
using UnityEngine;

namespace AvatarController.Misc
{
    public class FootDetection : MonoBehaviour
    {
        [SerializeField] private LayerMask _floorsLayers;
        [SerializeField, Min(0.01f)] private float _detectionDistance = 0.1f;
        public FloorType FloorType { get; private set; }

        private void Awake()
        {
            FloorType = FloorType.WOODY;
        }

        private void Update()
        {
            Ray ray = new(transform.position + Vector3.up * _detectionDistance, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, _detectionDistance * 2, _floorsLayers))
            {
                if (hitInfo.collider.CompareTag("WOOD"))
                {
                    FloorType = FloorType.WOODY;
                    return;
                }

                if (hitInfo.collider.CompareTag("METAL"))
                {
                    FloorType = FloorType.METAL;
                    return;
                }

                if (hitInfo.collider.CompareTag("CARPET"))
                {
                    FloorType = FloorType.CARPET;
                    return;
                }

                if (hitInfo.collider.CompareTag("CLOUD"))
                {
                    FloorType = FloorType.CLOUD;
                    return;
                }
            }
            else
            {
                FloorType = FloorType.WOODY;
            }
        }

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.blue;

            Vector3 start = transform.position;
            Vector3 end = start + Vector3.down * _detectionDistance;

            Handles.DrawLine(start, end);
            Handles.color = Color.white;
        }
#endif
        #endregion
    }
}
