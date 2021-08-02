using UnityEngine;

namespace Render3DTo2D.Utility.Extensions
{
    public static class CameraExtensions
    {
        public static void SetClipPlanes(this Camera aCamera, float aClipPlaneDistance)
        {
            aCamera.farClipPlane = aClipPlaneDistance;
            aCamera.nearClipPlane = -aClipPlaneDistance;
        }
    }
}