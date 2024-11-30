using System.Collections.Generic;
using UnityEngine;
using UtilsComplements;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BaseGame //add it to a concrete namespace
{
    public class ParallaxController : MonoBehaviour, ISingleton<ParallaxController>
    {
        #region Fields
        [Header("References")]
        [SerializeField] private GameObject _prefab;
        private List<ParallaxInstance> _instances = new();

        [Header("Instance Attributes")]
        [SerializeField, Min(1)] private int _instancesForward = 1;
        [SerializeField, Min(1)] private int _instancesBackward = 1;
        [SerializeField] private bool _isDay = true;

        [Header("Movement Attributes")]
        //[SerializeField, Min(0.1f)] private float _distanceToDissapear = 1;
        //[SerializeField, Min(0.1f)] private float _distanceToAppear = 1;
        [Tooltip("Internally will clamp the speed by the dimensions of the prefab")]
        [SerializeField, Min(0.1f)] private float _speed = 1;
        [SerializeField] private float _directionX = 1;
        [SerializeField] private float _directionZ = 0;
        private Vector3 _direction = Vector3.forward;

        [Header("DEBUG")]
        [SerializeField] private Color DEBUG_color;
        [SerializeField] private bool DEBUG_drawGizmos = true;

        public ISingleton<ParallaxController> Instance => this;
        private Vector3 Direction
        {
            get
            {
                _direction.x = _directionX;
                _direction.z = _directionZ;
                _direction.y = 0;
                return _direction.normalized;
            }
        }

        private float MaxSpeed
        {
            get
            {
                const float _securityPct = 0.8f;

                return (InstanceDistance * _securityPct) / Time.fixedUnscaledDeltaTime;
            }
        }

        private float InstanceDistance
        {
            get
            {
                if (_instances.Count <= 0)
                    return 0;

                Vector3 distanceFromInstanceToHandler = _instances[0].Distance;

                float dot = Vector3.Dot(Direction.normalized, distanceFromInstanceToHandler.normalized);
                //the distance projection in the MovingDirectionAxis
                float distance = (Direction.normalized * distanceFromInstanceToHandler.magnitude * dot).magnitude;

                return distance;
            }
        }

        private Vector3 InstanceDirectionProjection
        {
            get => Direction * InstanceDistance;
        }
        #endregion

        #region Unity Logic
        private void Awake()
        {
            Instance.Instantiate();

            CreateInstances();
        }

        private void OnValidate()
        {
            //if (_instances == null)
            //    _instances = new List<GameObject>();

            //if (_instances.Count >= 1)
            //    return;

            //if (!_prefab)
            //    return;

            //GameObject go = Instantiate(_prefab, transform, true);
            //go.transform.position = transform.position;
            //_instances.Add(go);

            if (_directionX > 0)
                _directionX = 1;
            else if (_directionX < 0)
                _directionX = -1;

            if (_directionZ > 0)
                _directionZ = 1;
            else if (_directionZ < 0)
                _directionZ = -1;

            if (_directionX == 1)
                _directionZ = 0;
        }

        private void Update()
        {
            MoveAllParallax(Time.deltaTime);
        }

        private void OnDestroy() => Instance.RemoveInstance();
        #endregion

        #region Static Methods
        public static void StaticMethod()
        {
        }
        #endregion

        #region Public Methods
        public void PublicMethod()
        {
        }
        #endregion

        #region Private Methods
        private void CreateInstances()
        {
            #region First instance
            GameObject first = Instantiate(_prefab, transform.position, transform.rotation);
            ParallaxInstance parallaxInsComp = first.GetComponent<ParallaxInstance>();

            if (parallaxInsComp == null)
            {
                Debug.Log("The prefab doesn't have ParallaxInstance component, check it");
                return;
            }
            var component = first.GetComponent<ParallaxInstance>();
            component.ActivateRandomVariation(1);
            _instances.Add(parallaxInsComp);
            #endregion

            #region Frontward
            for (int i = 1; i < _instancesForward; i++)
            {
                Vector3 offset = InstanceDirectionProjection;
                Vector3 position = transform.position;

                Vector3 xAxis = transform.right.normalized;
                Vector3 zAxis = transform.forward.normalized;

                float dotX = Vector3.Dot(xAxis, offset.normalized);

                position += (xAxis * offset.magnitude * dotX) * i;

                float dotZ = Vector3.Dot(zAxis, offset.normalized);
                position += (zAxis * offset.magnitude * dotZ) * i;

                GameObject front = Instantiate(_prefab, transform.position, transform.rotation);
                front.transform.position = position;
                var comp = front.GetComponent<ParallaxInstance>();
                comp.ActivateRandomVariation(1);
                _instances.Insert(0, comp);
            }
            #endregion

            #region Backward
            for (int i = 1; i <= _instancesBackward; i++)
            {
                Vector3 offset = InstanceDirectionProjection;
                Vector3 position = transform.position;

                Vector3 xAxis = transform.right.normalized;
                Vector3 zAxis = transform.forward.normalized;

                float dotX = Vector3.Dot(xAxis, offset.normalized);

                position -= (xAxis * offset.magnitude * dotX) * i;

                float dotZ = Vector3.Dot(zAxis, offset.normalized);
                position -= (zAxis * offset.magnitude * dotZ) * i;

                GameObject front = Instantiate(_prefab, transform.position, transform.rotation);
                front.transform.position = position;
                var comp = front.GetComponent<ParallaxInstance>();
                comp.ActivateRandomVariation(1);
                _instances.Add(comp);
            }
            #endregion
        }

        private void MoveAllParallax(float dt)
        {
            float speedClamped = Mathf.Clamp(_speed, 0, MaxSpeed);
            float motion = speedClamped * dt;

            for (int i = 0; i < _instances.Count; i++)
            {
                _instances[i].transform.position += Direction * motion;
            }

            bool checkedFirst = false;
            float maxDistance = InstanceDistance * (_instancesForward + 2);

            while (!checkedFirst)
            {
                ParallaxInstance first = _instances.First();
                float distanceToFirst = Vector3.Distance(transform.position, first.transform.position);

                if (distanceToFirst > maxDistance)
                {
                    _instances.Remove(first);
                    ParallaxInstance last = _instances.Last();

                    Vector3 offset = InstanceDirectionProjection;
                    Vector3 position = last.transform.position;

                    Vector3 xAxis = transform.right.normalized;
                    Vector3 zAxis = transform.forward.normalized;

                    float dotX = Vector3.Dot(xAxis, offset.normalized);

                    position -= (xAxis * offset.magnitude * dotX);

                    float dotZ = Vector3.Dot(zAxis, offset.normalized);
                    position -= (zAxis * offset.magnitude * dotZ);

                    first.transform.position = position;
                    first.ActivateRandomVariation(GameManager.Level);
                    _instances.Add(first);
                }
                else
                {
                    checkedFirst = true;
                }
            }
        }
        #endregion

        #region Debug
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!DEBUG_drawGizmos)
                return;

            if (_instances.Count <= 0)
                return;

            Handles.color = DEBUG_color;

            const float spaceDotted = 5;
            const float width = 10;

            Vector3 startPoint = transform.position;

            Vector3 endPosition = startPoint + Direction.normalized * InstanceDistance * (_instancesForward + 2);
            Handles.DrawDottedLine(startPoint, endPosition, spaceDotted);

            startPoint = endPosition + Vector3.Cross(Direction, Vector3.up).normalized * width;
            endPosition = endPosition - Vector3.Cross(Direction, Vector3.up).normalized * width;
            Handles.DrawDottedLine(startPoint, endPosition, spaceDotted);

            //startPoint = transform.position;
            //endPosition = startPoint - Direction.normalized * _distanceToAppear;
            //Handles.DrawDottedLine(startPoint, endPosition, spaceDotted);

            //startPoint = endPosition + Vector3.Cross(-Direction, Vector3.up).normalized * width;
            //endPosition = endPosition - Vector3.Cross(-Direction, Vector3.up).normalized * width;
            //Handles.DrawDottedLine(startPoint, endPosition, spaceDotted);

            const float radius = 1;
            const float dottedLines = 3;

            Transform lastTransform = _instances[0].transform;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastTransform.position, radius);
            Gizmos.color = DEBUG_color;
            for (int i = 1; i < _instances.Count; i++)
            {
                Transform next = _instances[i].transform;
                Handles.DrawDottedLine(lastTransform.position, next.position, dottedLines);
                Gizmos.DrawWireSphere(next.position, radius);
                lastTransform = next;
            }

            Handles.color = Color.white;
            Gizmos.color = Color.white;
        }
#endif
        #endregion
    }
}
