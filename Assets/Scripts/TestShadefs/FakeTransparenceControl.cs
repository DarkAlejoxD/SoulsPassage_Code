using UnityEngine;
using UtilsComplements;
using System.Collections.Generic;
using static UtilsComplements.AsyncTimer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BaseGame
{
    public class FakeTransparenceControl : MonoBehaviour, ISingleton<FakeTransparenceControl>
    {
        [Header("Refrences")]
        [SerializeField] private Transform _player;
        [SerializeField] private GameObject _sphereMesh;
        [SerializeField] private LayerMask _layers;
        [SerializeField, Min(1)] private int _fakeTransparentLayer = 24;
        private Dictionary<GameObject, int> _hittedObjects = new();

        [Header("Attributes")]
        [SerializeField, Min(0.01f)] private float _diameter;
        [SerializeField, Min(0.01f)] private float _expansionVelocity = 0.01f;
        [SerializeField, Min(15)] private int _refreshRate = 24; // 24 fps/s
        private float _timeControl = 0;
        private float _radiusControl = 0;
        private SphereControlStates _state = SphereControlStates.NONE;

        private float Radius => _diameter / 2;
        private Camera CurrentCamera => Camera.main;
        private float TimeToRefreshRaycasts => 1f / _refreshRate;
        public ISingleton<FakeTransparenceControl> Instance => this;

        private void Awake() => Instance.Instantiate();
        private void OnDestroy() => Instance.RemoveInstance();

        private void Start()
        {
            _timeControl = Time.time;
            transform.localScale = Vector3.zero;
            _state = SphereControlStates.NONE;
            _hittedObjects = new();
        }

        private void FixedUpdate()
        {
            transform.position = _player.position;
            UpdateRaycast(Time.fixedDeltaTime);
            UpdateScale(Time.fixedDeltaTime);
        }

        public void SphereMeshSetActive(bool value)
        {
            if (_sphereMesh)            
                _sphereMesh.SetActive(value);            
        }

        private void UpdateScale(float dt)
        {
            switch (_state)
            {
                case SphereControlStates.EXPANDING:
                    {
                        transform.localScale = _radiusControl * Vector3.one;
                        if (_radiusControl > _diameter)
                        {
                            _radiusControl = _diameter;
                            _state = SphereControlStates.NONE;
                            transform.localScale = _radiusControl * Vector3.one;
                            return;
                        }
                        _radiusControl += _expansionVelocity * dt;
                    }
                    break;
                case SphereControlStates.REDUCING:
                    {
                        transform.localScale = _radiusControl * Vector3.one;
                        if (_radiusControl < 0)
                        {
                            _radiusControl = 0;
                            _state = SphereControlStates.NONE;
                            transform.localScale = _radiusControl * Vector3.one;

                            foreach (var item in _hittedObjects)
                            {
                                item.Key.layer = item.Value;
                            }

                            _hittedObjects.Clear();

                            return;
                        }
                        _radiusControl -= _expansionVelocity * dt;
                    }
                    break;
            }
        }

        private void UpdateRaycast(float dt)
        {
            _timeControl += dt;
            if (_timeControl >= TimeToRefreshRaycasts)
            {
                _timeControl -= TimeToRefreshRaycasts;

                //First deactivate all not touched objects                

                //From Center
                if (SendRaycastToCamera(transform.position))
                {
                    ActivateSphere();
                    return;
                }

                #region CercleRaycast
                /*
                Vector3 up = CurrentCamera.transform.up;

                //From up
                if (SendRaycastToCamera(transform.position + up * Radius))
                {
                    ActivateSphere();
                    return;
                }

                ////From down
                //if (SendRaycastToCamera(transform.position - up * Radius))
                //{
                //    ActivateSphere();
                //    return;
                //}

                Vector3 right = CurrentCamera.transform.right;

                //From right
                if (SendRaycastToCamera(transform.position + right * Radius))
                {
                    ActivateSphere();
                    return;
                }

                //From left
                if (SendRaycastToCamera(transform.position - right * Radius))
                {
                    ActivateSphere();
                    return;
                }
                */
                #endregion

                DeactivateSphere();

                //SendRaycastToCamera(transform.position);
            }
        }

        private void DeactivateSphere() => _state = SphereControlStates.REDUCING;

        private void ActivateSphere() => _state = SphereControlStates.EXPANDING;

        private bool SendRaycastToCamera(Vector3 position)
        {
            Vector3 final = CurrentCamera.transform.position;
            Vector3 direction = final - position;
            float offset = CurrentCamera.nearClipPlane + 0.1f;

            float distance = direction.magnitude;

            Ray ray = new(position, direction);
            var hits = Physics.RaycastAll(ray, distance, _layers);

            if (hits.Length >= 1)
            {
                foreach (var hit in hits)
                {
                    var go = hit.collider.gameObject;
                    if (!_hittedObjects.ContainsKey(go))
                    {
                        _hittedObjects.Add(go, go.layer);
                        go.layer = _fakeTransparentLayer;
                    }
                }
                return true;
            }

            return false;
        }

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color color = new(34 / 255f, 0f, 118 / 255f);

            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, Radius);
            Gizmos.color = Color.white;

            const float distance_Draw = 1f;
            const float distance_Bet_Lines = 5f;

            Vector3 center;
            Vector3 finalPos;
            Vector3 direction;
            Vector3 up = CurrentCamera.transform.up;
            Vector3 right = CurrentCamera.transform.right;

            float offset = CurrentCamera.nearClipPlane + 0.1f;
            float distance;
            Ray ray;
            bool hit;

            #region Center
            //Calculate Center
            center = transform.position;
            direction = CurrentCamera.transform.position - center;
            distance = direction.magnitude - offset;
            ray = new(center, direction);
            hit = Physics.Raycast(ray, distance, _layers);
            finalPos = center + direction.normalized * distance_Draw;

            Handles.color = !hit ? Color.green : Color.red;
            Handles.DrawDottedLine(center, finalPos, distance_Bet_Lines);
            #endregion

            #region Up
            //Calculate Up
            center = transform.position + up * Radius;
            direction = CurrentCamera.transform.position - center;
            distance = direction.magnitude - offset;
            ray = new(center, direction);
            hit = Physics.Raycast(ray, distance, _layers);
            finalPos = center + direction.normalized * distance_Draw;

            Handles.color = !hit ? Color.green : Color.red;
            Handles.DrawDottedLine(center, finalPos, distance_Bet_Lines);
            #endregion

            #region Down
            //Calculate Down
            center = transform.position - up * Radius;
            direction = CurrentCamera.transform.position - center;
            distance = direction.magnitude - offset;
            ray = new(center, direction);
            hit = Physics.Raycast(ray, distance, _layers);
            finalPos = center + direction.normalized * distance_Draw;

            Handles.color = !hit ? Color.green : Color.red;
            Handles.DrawDottedLine(center, finalPos, distance_Bet_Lines);
            #endregion

            #region Right
            //Calculate Down
            center = transform.position + right * Radius;
            direction = CurrentCamera.transform.position - center;
            distance = direction.magnitude - offset;
            ray = new(center, direction);
            hit = Physics.Raycast(ray, distance, _layers);
            finalPos = center + direction.normalized * distance_Draw;

            Handles.color = !hit ? Color.green : Color.red;
            Handles.DrawDottedLine(center, finalPos, distance_Bet_Lines);
            #endregion

            #region Left
            //Calculate Down
            center = transform.position - right * Radius;
            direction = CurrentCamera.transform.position - center;
            distance = direction.magnitude - offset;
            ray = new(center, direction);
            hit = Physics.Raycast(ray, distance, _layers);
            finalPos = center + direction.normalized * distance_Draw;

            Handles.color = !hit ? Color.green : Color.red;
            Handles.DrawDottedLine(center, finalPos, distance_Bet_Lines);
            #endregion

            Handles.color = Color.white;
        }
#endif
        #endregion
    }
}
