using RootChecker;
using UnityEngine;

namespace Render3DTo2D.RigCamera
{
    //Note, due to depth rotation working and being very very simple we don't use this at all
    //However, we'll need something like this on isometric cameras so keeping it in the project
    //FUTURE NOTE, depth rotation worked for isometric cameras too so we really don't need this anymore
    //I'm pretty sure this class isn't fully working anymore so we'll just keep it around for historic reasons
    public class CameraRotator : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private Transform orbitPoint = null;
        [SerializeField]
        private float orbitOffset = 0;
        [SerializeField]
        private float autoRotationLength = 0f;
        #endregion

        #region Private Fields
        private float lastAngle = 0f;
        #endregion

        #region Public Enums
        public enum CameraRotationDirection
        {
            Right,
            Left
        }

        #endregion Public Enums

        #region Public Properties
        public bool CanOrbit => orbitPoint != null;

        #endregion
        #region Public Methods
        //Originally we used Orbit points for side view cameras but now we extend them back and rotate around their midpoint instead (if rotation is present)
        public void SetRotation(float aSetAngle, bool aUseOffset = true)
        {
            float _toSet = aSetAngle;
            if(aUseOffset == true)
            {
                _toSet += orbitOffset;
            }
            if(Mathf.Approximately(lastAngle, 0f) == false)
            {
                transform.Rotate(new Vector3(0, -lastAngle));
            }
            lastAngle = _toSet;
            transform.Rotate(new Vector3(0, _toSet));
        }

        public void RotateWith(float aSetAngle)
        {
            transform.Rotate(new Vector3(0, aSetAngle));
            lastAngle = transform.rotation.eulerAngles.y;
        }

        public void Rotate(CameraRotationDirection aRotationDirection)
        {
            if(Mathf.Approximately(autoRotationLength, 0f))
            {
                Debug.LogWarning("AutoRotationLength not set before attempting to AutoRotate Camera " + gameObject.name + " on object " + RootChecker.RootFinder.FindHighestRoot(this.transform).name);
                return;
            }

            if(aRotationDirection == CameraRotationDirection.Right)
            {
                transform.Rotate(new Vector3(0, autoRotationLength));
                lastAngle += autoRotationLength;
            }
            else
            {
                transform.Rotate(new Vector3(0, -autoRotationLength));
                lastAngle -= autoRotationLength;
            }
        }

        //I'm fairly sure this orbit isn't gonna work for traditional isometric cameras looking down btw
        //When implementing that testing and fixing this is a main problem
        public void OrbitCamera(float aSetAngle, bool aUseOffset = true)
        {
            float _toSet = aSetAngle;
            if(aUseOffset == true)
            {
                _toSet += orbitOffset;
            }
            if(Mathf.Approximately(lastAngle, 0) == false)
            {
                //For an Isometric camera Vector3.up is probably unusable since it would be aligned with a view like "/" looking down
                //Splitting it into a orbit point and a look-point could be a solution (orbit point would be same y as camera and camera would look at it, rotate, look at the other point again)
                //Otherwise just do the trig and calculate new positions, it's not a complicated formula anyway
                
                transform.RotateAround(orbitPoint.transform.position, Vector3.up, -lastAngle);
                transform.LookAt(orbitPoint);
            }
            lastAngle = _toSet;

            transform.RotateAround(orbitPoint.transform.position, Vector3.up, _toSet);
            transform.LookAt(orbitPoint);
        }

        public void OrbitCamera(CameraRotationDirection aRotationDirection)
        {
            if(Mathf.Approximately(autoRotationLength, 0f))
            {
                Debug.LogWarning("AutoRotationLength not set before attempting to AutoRotate Camera " + gameObject.name + " on object " + RootFinder.FindHighestRoot(this.transform).name);
                return;
            }


            if(aRotationDirection == CameraRotationDirection.Right)
            {
                transform.RotateAround(orbitPoint.transform.position, Vector3.up, autoRotationLength);
                transform.LookAt(orbitPoint);
                lastAngle += autoRotationLength;
            }
            else
            {
                transform.RotateAround(orbitPoint.transform.position, Vector3.up, -autoRotationLength);
                transform.LookAt(orbitPoint);
                lastAngle -= autoRotationLength;
            }
        }

        public void SetOrbitPointHeight(float aNewY)
        {
            orbitPoint.transform.localPosition = new Vector3(orbitPoint.transform.localPosition.x, aNewY, orbitPoint.transform.localPosition.z);
        }

        public void SetAutoRotationLength(float aNewRotationLength)
        {
            autoRotationLength = aNewRotationLength;
        }
        #endregion
    }
}
