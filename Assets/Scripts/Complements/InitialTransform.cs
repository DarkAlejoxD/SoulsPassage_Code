using UnityEngine;

namespace UtilsComplements
{
    public struct InitialTransform
    {
        public Transform TransformReference;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 LocalScale;
        public Transform Parent;

        public InitialTransform(Transform evaluatedObj)
        {
            TransformReference = evaluatedObj;

            Position = evaluatedObj.transform.position;
            Rotation = evaluatedObj.transform.rotation;
            LocalScale = evaluatedObj.transform.localScale;
            Parent = evaluatedObj.parent;
        }

        public void ResetTransfrom()
        {
            TransformReference.SetPositionAndRotation(Position, Rotation);
            TransformReference.localScale = LocalScale;
            TransformReference.SetParent(Parent);
        }
    }
}