using UnityEngine;

namespace UtilsComplements
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    public class DifferentColorMaterial : MonoBehaviour
    {
        private const string COLOR_ID = "_BaseColor";

        [Header("References")]
        private Color _startColor;
        [SerializeField] private Color _newColor;
        [SerializeField] private string _colorID = "_Color";
        private MeshRenderer _meshRenderer;

        private MaterialPropertyBlock _materialPropertyBlock;

        private MaterialPropertyBlock ThisMaterialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();
                return _materialPropertyBlock;
            }
        }

        private MeshRenderer ThisMeshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                    _meshRenderer = GetComponent<MeshRenderer>();
                return _meshRenderer;
            }
        }

        private void Awake()
        {
            _startColor = ThisMeshRenderer.sharedMaterial.GetColor(_colorID);
        }

        public void ResetColor()
        {
            ThisMaterialPropertyBlock.SetColor(_colorID, _startColor);
            ThisMeshRenderer.SetPropertyBlock(ThisMaterialPropertyBlock);
        }

        public void ChangeColorToNew()
        {
            ThisMaterialPropertyBlock.SetColor(_colorID, _newColor);
            ThisMeshRenderer.SetPropertyBlock(ThisMaterialPropertyBlock);
        }
    }
}
