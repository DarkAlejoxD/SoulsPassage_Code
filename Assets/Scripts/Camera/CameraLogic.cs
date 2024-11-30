using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cameras
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraLogic : MonoBehaviour
    {
        const float NEAR_PLANE = 1f;

        [Header("References")]
        [SerializeField] private Transform _lookAt;
        [SerializeField] private Transform DEBUG_InternalHandler;
        [SerializeField] private Transform DEBUG_ExternalHandler;
        [SerializeField] private Transform DEBUG_CenterHandler;
        private Camera _camera;

        [Header("References")]
        [Tooltip("Used to quickly look at the player if goes out the screen")]
        [SerializeField, Range(0.001f, 10)] private float _hardSmooth = 1;
        [Tooltip("Used to slowly return to localRot 0")]
        [SerializeField, Range(0.001f, 10)] private float _softSmooth = 2;

        [Header("DeadZone")]
        [SerializeField, Range(-1f, 1f)] private float _up = 0.05f;
        [SerializeField, Range(-1f, 1f)] private float _down = 0.05f;
        [SerializeField, Range(-1f, 1f)] private float _left = 0.05f;
        [SerializeField, Range(-1f, 1f)] private float _right = 0.05f;

        public bool IsPlayerOutScreen { get; private set; }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            IsPlayerOutScreen = false;
        }

        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
        }

        private void OnValidate()
        {
            if (_up < _down)
                _down = _up;

            if (_right < _left)
                _left = _right;
        }

        private void LateUpdate()
        {
            Vector3 pos = _camera.WorldToViewportPoint(DEBUG_InternalHandler.position);
            Debug.Log(pos);
            RotationLogic();
        }

        private void RotationLogic()
        {

            /* if the player is inside limits, return to local Rotation 0 and follow the virtual
             * cameras logic*/
            if (!CheckIfIsOutLimits(out Vector3 lookAtPosition))
            {
                //Quaternion desiredRotation = Quaternion.identity;
                //transform.localRotation = Quaternion.Slerp(transform.localRotation,
                //                                      desiredRotation,
                //                                      _softSmooth * Time.deltaTime);
                IsPlayerOutScreen = false;
            }
            // If the player is outside, look at him
            else
            {
                //Quaternion desiredRotation = Quaternion.LookRotation(lookAtPosition, transform.up);
                //transform.rotation = Quaternion.Slerp(transform.rotation,
                //                                      desiredRotation,
                //                                      _hardSmooth * Time.deltaTime);
                IsPlayerOutScreen = true;
            }
        }

        private bool CheckIfIsOutLimits(out Vector3 lookAtPosition)
        {
            bool isOutsideLimits = false;

            GetAllLimits(out Vector3 center, out Vector3 upLimit, out Vector3 downLimit,
                         out Vector3 rightLimit, out Vector3 leftLimit);

            Vector3 upViewport, downViewport, rightViewport, leftViewport;
            upViewport = _camera.WorldToViewportPoint(upLimit);
            downViewport = _camera.WorldToViewportPoint(downLimit);
            leftViewport = _camera.WorldToViewportPoint(leftLimit);
            rightViewport = _camera.WorldToViewportPoint(rightLimit);

            Vector3 player = _lookAt.position;
            Vector3 internalPos = _camera.WorldToViewportPoint(player);
            Vector3 externalPos = internalPos;

            if (internalPos.z < 0)
            {
                Vector3 cameraToPlayer = player - transform.position;
                cameraToPlayer.Normalize();

                float dotUp = Vector3.Dot(cameraToPlayer, transform.up);
                float dotRight = Vector3.Dot(cameraToPlayer, transform.right);

                internalPos.z = 0;

                if (dotUp < 0)
                {
                    internalPos.y = downViewport.y;
                    externalPos.y = 0;
                }
                else
                {
                    internalPos.y = upViewport.y;
                    externalPos.y = 1;
                }

                if (dotRight < 0)
                {
                    internalPos.x = leftViewport.x;
                    externalPos.x = 0;
                }
                else
                {
                    internalPos.x = rightViewport.x;
                    externalPos.x = 1;
                }

                Ray internalRayAlt = _camera.ViewportPointToRay(internalPos);
                Vector3 altDir = internalRayAlt.direction;
                Vector3 altHandlerPos = transform.position + altDir.normalized * 1f;

                Ray externalRayAlt = _camera.ViewportPointToRay(externalPos);
                Vector3 externalDirAlt = externalRayAlt.direction;
                Vector3 altExternalHandlerPos = transform.position + externalDirAlt * 1;

                DEBUG_InternalHandler.position = altHandlerPos;
                DEBUG_ExternalHandler.position = altExternalHandlerPos;

                isOutsideLimits = true;
            }
            else
            {
                if (internalPos.y > upViewport.y)
                {
                    isOutsideLimits = true;
                    internalPos.y = upViewport.y;
                }
                else if (internalPos.y < downViewport.y)
                {
                    isOutsideLimits = true;
                    internalPos.y = downViewport.y;
                }

                if (internalPos.x < leftViewport.x)
                {
                    isOutsideLimits = true;
                    internalPos.x = leftViewport.x;
                }
                else if (internalPos.x > rightViewport.x)
                {
                    isOutsideLimits = true;
                    internalPos.x = rightViewport.x;
                }

                internalPos.z = 0;

                Ray internalRay = _camera.ViewportPointToRay(internalPos);
                Vector3 dir = internalRay.direction;
                Vector3 handlerPos = transform.position + dir.normalized * 1f;

                Ray externalRay = _camera.ViewportPointToRay(externalPos);
                Vector3 externalDir = externalRay.direction;
                Vector3 externalHandlerPos = transform.position + externalDir * 1;

                DEBUG_InternalHandler.position = handlerPos;
                DEBUG_ExternalHandler.position = externalHandlerPos;
            }

            Vector3 lookAtCenter = new(0.5f, 0.5f, 0);

            Vector3 exPos = externalPos;
            exPos.z = 0;
            Vector3 inPos = internalPos;
            inPos.z = 0;
            Vector3 difference = exPos - inPos;

            //if(difference.magnitude > 0.1f)
            //{
            //    isOutsideLimits = true;
            //}
            //else
            //{
            //    isOutsideLimits=false;
            //}

            Vector3 desiredlookAt = lookAtCenter + difference;
            Vector3 lookAt = Vector3.Slerp(lookAtCenter, desiredlookAt, _hardSmooth * Time.deltaTime);
            Ray ray = _camera.ViewportPointToRay(lookAt);
            Vector3 lookAtDir = ray.direction;

            lookAtPosition = transform.position + lookAtDir * 1f;
            DEBUG_CenterHandler.position = lookAtPosition;
            return isOutsideLimits;
        }

        private void GetAllLimits(out Vector3 center, out Vector3 upLimit, out Vector3 downLimit,
                                  out Vector3 rightLimit, out Vector3 leftLimit)
        {
            float aspect = _camera.aspect;
            float halfHeight = Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * NEAR_PLANE;
            float halfWidth = halfHeight * aspect;

            center = transform.position + transform.forward * NEAR_PLANE;
            upLimit = center + transform.up * _up * halfHeight;
            downLimit = center + transform.up * _down * halfHeight;
            rightLimit = center + transform.right * _right * halfWidth;
            leftLimit = center + transform.right * _left * halfWidth;
        }

        #region DEBUG
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.red;

            const float thickness = 5;

            var zTest = Handles.zTest;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

            float aspect = _camera.aspect;
            float halfHeight = Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * NEAR_PLANE;
            float halfWidth = halfHeight * aspect;
            Vector3 start, end;

            Vector3 center = transform.position + transform.forward * NEAR_PLANE;

            #region up
            Vector3 up = center + transform.up * _up * halfHeight;
            start = up - transform.right * halfWidth;
            end = up + transform.right * halfWidth;

            Handles.DrawLine(start, end, thickness);
            #endregion

            #region down
            Vector3 down = center + transform.up * _down * halfHeight;
            start = down - transform.right * halfWidth;
            end = down + transform.right * halfWidth;

            Handles.DrawLine(start, end, thickness);
            #endregion

            #region right
            Vector3 right = center + transform.right * _right * halfWidth;
            start = right - transform.up * halfHeight;
            end = right + transform.up * halfHeight;

            Handles.DrawLine(start, end, thickness);
            #endregion

            #region left
            Vector3 left = center + transform.right * _left * halfWidth;
            start = left - transform.up * halfHeight;
            end = left + transform.up * halfHeight;

            Handles.DrawLine(start, end, thickness);
            #endregion
            Handles.zTest = zTest;
            Handles.color = Color.white;
        }
#endif
        #endregion
    }
}