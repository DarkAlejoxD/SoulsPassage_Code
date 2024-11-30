using UnityEngine;

namespace BaseGame
{
    [ExecuteAlways]
    public class ShadowPosition : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _footHandler;
        [SerializeField] private Transform _shadow;
        [SerializeField, Min(0.01f)] private float _offset = 0.1f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField, Min(0.1f)] private float _maxDistanceToRender = 0.1f;
        [SerializeField] private Vector3 _maxScale;

        private void LateUpdate()
        {
            Ray ray = new(_footHandler.position, -transform.up);

            bool hit = Physics.Raycast(ray, out RaycastHit info, _maxDistanceToRender, _layerMask);

            if (hit)
            {
                _shadow.gameObject.SetActive(true);
                _shadow.position = info.point + info.normal * _offset;
                _shadow.forward = info.normal;

                float distanceToPoint = info.distance;
                _shadow.localScale = _maxScale * (1 - (distanceToPoint / _maxDistanceToRender));
            }
            else
            {
                _shadow.gameObject.SetActive(false);
            }
        }
    }
}
