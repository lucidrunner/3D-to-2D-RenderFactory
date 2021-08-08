using System;
using UnityEngine;

namespace Render3DTo2D.Utility
{
    internal class StoredTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 RotationEuler;
        public Vector3 Scale;
        
        public float PositionDeltaX { get; private set; }
        public float PositionDeltaY { get; private set; }
        public float PositionDeltaZ { get; private set; }

        public float RotationDeltaX { get; private set; }
        public float RotationDeltaY { get; private set; }
        public float RotationDeltaZ { get; private set; }

        public float ScaleDeltaX { get; private set; }
        public float ScaleDeltaY { get; private set; }
        public float ScaleDeltaZ { get; private set; }
        
        public bool HasDeltaChange { get; private set; }
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

        public void SetDelta(StoredTransform aOtherTransform)
        {
            var _posDelta = Position - aOtherTransform.Position;
            var _rotDelta = RotationEuler - aOtherTransform.RotationEuler;
            var _scaleDelta = Scale - aOtherTransform.Scale;
 
            PositionDeltaX = _posDelta.x;
            PositionDeltaY = _posDelta.y;
            PositionDeltaZ = _posDelta.z;

            _rotDelta = GeneralUtilities.ClampRelativeVector(_rotDelta);
            RotationDeltaY = _rotDelta.y;
            RotationDeltaX = _rotDelta.x;
            RotationDeltaZ = _rotDelta.z;

            ScaleDeltaX = _scaleDelta.x;
            ScaleDeltaY = _scaleDelta.y;
            ScaleDeltaZ = _scaleDelta.z;

            if (Math.Abs(PositionDeltaX) + Math.Abs(PositionDeltaY) + Math.Abs(PositionDeltaZ) + 
                Math.Abs(RotationDeltaX) + Math.Abs(RotationDeltaY) + Math.Abs(RotationDeltaZ) + 
                Math.Abs(ScaleDeltaX) + Math.Abs(ScaleDeltaY) + Math.Abs(ScaleDeltaZ) > 0)
                HasDeltaChange = true;
        }

        
    }
}