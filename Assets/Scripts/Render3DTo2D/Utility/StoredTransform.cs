using UnityEngine;

namespace Render3DTo2D.Utility
{
    internal class StoredTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 RotationEuler;
        public Vector3 Scale; //

        public StoredTransform()
        {
            Position = Vector3.zero;
            Rotation = Quaternion.Euler(Vector3.zero);
            RotationEuler = Vector3.zero;
            Scale = Vector3.zero;
        }
        
        public StoredTransform(Transform aOriginTransform)
        {
            Position = aOriginTransform.localPosition;
            Rotation = aOriginTransform.localRotation;
            RotationEuler = aOriginTransform.localEulerAngles;
            Scale = aOriginTransform.localScale;
        }

        public static StoredTransform operator- (StoredTransform a, StoredTransform b)
        {
            StoredTransform _storedTransform = new StoredTransform();
            _storedTransform.Position = a.Position - b.Position;
            _storedTransform.Scale = a.Scale - b.Scale;
            _storedTransform.RotationEuler = a.RotationEuler - b.RotationEuler;
            _storedTransform.Rotation = a.Rotation * Quaternion.Inverse(b.Rotation);
            return _storedTransform;
        }

    }
}