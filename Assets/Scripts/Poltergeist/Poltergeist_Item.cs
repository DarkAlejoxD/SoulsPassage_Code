using UnityEngine;
using UtilsComplements;

namespace Poltergeist
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Poltergeist_Item : MonoBehaviour
    {
        private const int POLTER_LAYER = 11;

        [Header("Start Attributes")]
        [SerializeField, Tooltip("It won't use gravity if is kinematic")] private bool _useGravity;
        [SerializeField] private bool _isKinematic;
        [SerializeField] private Transform _particlesPos;
        [SerializeField] private GameObject _art;
        [SerializeField] private bool _changeAllChildren = false;
        private int _startLayer;

        Rigidbody _rb;
        bool _freezePosition;
        Vector3 _position;

        public Transform ParticleTransform => _particlesPos;

        #region Unity Logic
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            EndPoltergeist();
            _position = transform.position;
        }

        private void OnEnable()
        {
            if (Singleton.TryGetInstance(out PoltergeistManager manager))
            {
                manager.AddPoltergeist(this);
                manager.OnPoltergeistEventExit.AddListener(Deselect);
            }
        }

        private void OnDisable()
        {
            if (Singleton.TryGetInstance(out PoltergeistManager manager))
            {
                manager.RemovePoltergeist(this);
                manager.OnPoltergeistEventExit.RemoveListener(Deselect);
            }
        }

        private void Start()
        {
            if (_art)
                _startLayer = _art.layer;
        }

        private void Update()
        {
            if (_freezePosition)
            {
                transform.position = _position;
            }
        }
        #endregion

        #region Public Methods
        public void StartPoltergeist()
        {
            _rb.isKinematic = false;
            _rb.useGravity = false;
            _rb.freezeRotation = true;
            _freezePosition = true;
            _position = transform.position;
            if (_art)
            {
                _art.layer = POLTER_LAYER;
                if (_changeAllChildren)
                {
                    var list = gameObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (var item in list)
                    {
                        item.gameObject.layer = POLTER_LAYER;
                    }
                }
            }
        }

        public void EndPoltergeist()
        {
            _rb.isKinematic = _isKinematic;
            _rb.useGravity = _useGravity;
            _freezePosition = false;
            _rb.freezeRotation = false;
        }

        public void Deselect()
        {
            if (_art)
            {
                _art.layer = _startLayer;
                if (_changeAllChildren)
                {
                    var list = gameObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (var item in list)
                    {
                        item.gameObject.layer = _startLayer;
                    }
                }
            }
        }

        public void Manipulate()
        {
            //Debug.Log("Wow, outline chido");
            _freezePosition = false;
        }

        public void NoManipulating()
        {
            //Debug.Log("Desactiva, outline chido");
            _position = transform.position;
            _freezePosition = true;
        }
        #endregion
    }
}