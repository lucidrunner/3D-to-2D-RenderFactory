using System.Collections.Generic;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility.Extensions;
using RootChecker;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Render3DTo2D.RigCamera
{
    public class CameraScaleCalculator : MonoBehaviour
    {
        #region Private Fields
        #region Serialized

        [SerializeField, HideInInspector]
        private BoundsCalculator boundsCalculator;

        #endregion
        
        private Camera orthoCam;
        private Vector3 boundsCenter;
        private Vector3[] boundsCorners;
        private List<Vector3> boundsInScreenSpace = new List<Vector3>();
        private float maxPadding = 5f;
        private Vector2 outerBounds;
        private float scalingStepPercentage = 0.04f;
        private Vector3[] screenCorners;
        private Vector3 upperRightScreenCorner;
        private float cameraInitialScale = -1f;
        public float CurrentScale => GetComponent<Camera>().orthographicSize;

        #endregion


        #region Public Methods
        public float CalculateScale()
        {
            SetScale(cameraInitialScale);
            
            //Calculate (or get the cache) the current bounds
            Bounds _modelBounds = boundsCalculator.CalculateAndReturn();

            //Declare their corners, center & extends
            boundsCorners = new Vector3[8];
            Vector3 _center = _modelBounds.center;
            Vector3 _extents = _modelBounds.extents;
            
            //Define the 8 corners of the bounds cube
            boundsCorners[0] = new Vector3(_center.x + _extents.x, _center.y + _extents.y, _center.z + _extents.z);
            boundsCorners[1] = new Vector3(_center.x + _extents.x, _center.y + _extents.y, _center.z - _extents.z);
            boundsCorners[2] = new Vector3(_center.x + _extents.x, _center.y - _extents.y, _center.z + _extents.z);
            boundsCorners[3] = new Vector3(_center.x - _extents.x, _center.y + _extents.y, _center.z + _extents.z);
            boundsCorners[4] = new Vector3(_center.x + _extents.x, _center.y - _extents.y, _center.z - _extents.z);
            boundsCorners[5] = new Vector3(_center.x - _extents.x, _center.y - _extents.y, _center.z + _extents.z);
            boundsCorners[6] = new Vector3(_center.x - _extents.x, _center.y + _extents.y, _center.z - _extents.z);
            boundsCorners[7] = new Vector3(_center.x - _extents.x, _center.y - _extents.y, _center.z - _extents.z);
            
            //Define the upper screen corner for 
            upperRightScreenCorner = orthoCam.ViewportToScreenPoint(new Vector3(1, 1, 0)); //Previously this had the legendary "Figure it out note" regarding a very confusing comment for a value that was never used again
            //Sadly that bit of fun has been removed from this math hole

            //Convert the 3D bounds to a 2D square representation relative to our viewport
            RecalculateScreenSpaceBounds();

            //I wasn't using do while's at the start of this project so we have a first time run here
            float _newScale = orthoCam.orthographicSize;
            float _distanceToEdge = FindDistanceToScreenEdge();

            int _numbOfChecks = 0;
            while(_distanceToEdge <= 0 || _distanceToEdge >= maxPadding)
            {
                _numbOfChecks++;
                if(_numbOfChecks > RenderingSettings.GetFor(transform).ScaleCalculatorMaxSteps)
                {
                    Debug.Log("Camera Scale Calculator for camera on " + RootFinder.FindFirstRoot(transform).name + " hit max number of attempts. Padding for object might need to be increased.");
                    break;
                }
                
                RecalculateScreenSpaceBounds();

                _newScale = orthoCam.orthographicSize;
                _distanceToEdge = FindDistanceToScreenEdge();
                if(_distanceToEdge >= maxPadding) //Change current scale depending on if we are below 0 / above the max padding (in pixels) in distance between bounds & edge
                {
                    _newScale -= _newScale * scalingStepPercentage; //Found this to be generally safer than halving / increasing based on current scale. At worst here we spin _numbOfChecks times rather than produce infinity errors.
                }
                else if(_distanceToEdge <= 0)
                {
                    _newScale += _newScale * (scalingStepPercentage * 1.25f); //Essentially, if we go under we take a slightly larger starting point and try again. After a couple of tries we will land in the allowed padding instead of under 0
                }
                else { continue; } //No need to set a new scale if the previous scale did the trick

                //Otherwise, set the new scale & position the camera correctly which will be used during the next time around
                orthoCam.orthographicSize = _newScale;
                var _transform = transform;
                Vector3 _localPosition = _transform.localPosition;
                _localPosition = new Vector3(_localPosition.x, _newScale, _localPosition.z);
                _transform.localPosition = _localPosition;
            }

            return _newScale;
        }

        public void ResetScale()
        {
            if (cameraInitialScale > 0)
            {
                SetScale(cameraInitialScale);
            }
        }

        public void SetScale(float aSetToScale)
        {
            if (orthoCam == null)
                orthoCam = GetComponent<Camera>();
            
            orthoCam.orthographicSize = aSetToScale;
            
            //Get the transform, position & set our y to match the ortho scale (2x ortho = viewport height in a side view so this centers the camera on the object)
            var _transform = transform;
            var _localPosition = _transform.localPosition;
            _localPosition = new Vector3(_localPosition.x, aSetToScale, _localPosition.z);
            _transform.localPosition = _localPosition;
        }

        /// <summary>
        /// Calculates the clipping planes based on bounding box
        /// </summary>
        public virtual void CalculateDepth()
        {
            //This is a pretty dirty solution tbh but it seems to always work
            var _currentBounds = boundsCalculator.CalculateAndReturn();
            var _transform = transform;
            var _checkedPosition = _transform.position;
            _checkedPosition.y = _currentBounds.center.y;
            float _boundsSizeModifier = RenderingSettings.GetFor(_transform).BoundsCalculatorSizeModifier;
            float _overDraw = 1.2f;
            orthoCam.SetClipPlanes((_boundsSizeModifier * _currentBounds.max.magnitude) * _overDraw  + Vector3.Distance(_checkedPosition, _currentBounds.center));
        }

        
        public void SetDepth(float aDepth)
        {
            orthoCam = GetComponent<Camera>();
            orthoCam.SetClipPlanes(aDepth);
        }


        public void Startup(BoundsCalculator aBoundsCalculator)
        {
            orthoCam = GetComponent<Camera>();
            cameraInitialScale = orthoCam.orthographicSize;
            boundsCalculator = aBoundsCalculator;
            scalingStepPercentage = RenderingSettings.GetFor(transform).ScaleCalculatorStepPercentage;
            maxPadding = RenderingSettings.GetFor(transform).ScaleCalculatorMaxPadding;
        }

        #endregion

        #region Private Methods

        private void RecalculateScreenSpaceBounds()
        {
            boundsInScreenSpace = new List<Vector3>();

            for(int _index = 0; _index < boundsCorners.Length; _index++)
            {
                Vector3 _toScreen = boundsCorners[_index];
                _toScreen = orthoCam.WorldToScreenPoint(_toScreen);
                _toScreen.z = orthoCam.nearClipPlane;
                if(boundsInScreenSpace.Contains(_toScreen) == false)
                {
                    boundsInScreenSpace.Add(_toScreen);
                }
            }
        }

        private float FindDistanceToScreenEdge()
        {
            if(boundsInScreenSpace.Count == 0)
            {
                return 0;
            }

            Vector3 _upperLeft = boundsInScreenSpace[0];
            Vector3 _upperRight = boundsInScreenSpace[0];

            for(int _index = 1; _index < boundsInScreenSpace.Count; _index++)
            {
                Vector3 _checkedBound = boundsInScreenSpace[_index];
                if(_checkedBound.x > _upperRight.x && _checkedBound.y >= _upperRight.y - 3f) //hard value due to float conversions, if the model is smaller than 3 pixels on the screen this might give it some trouble though
                {
                    _upperRight = _checkedBound;
                }
                else if(_checkedBound.x < _upperLeft.x && _checkedBound.y >= _upperLeft.y - 3f)
                {
                    _upperLeft = _checkedBound;
                }
            }

            //Get the smallest distance to the screen edge so we can return it
            float _distanceToEdge = upperRightScreenCorner.x - _upperRight.x;

            if(upperRightScreenCorner.y - _upperRight.y < _distanceToEdge) //We only need to do one y for the orthographic sideview of the cameras
            {
                _distanceToEdge = upperRightScreenCorner.y - _upperRight.y;
            }

            if(_upperLeft.x < _distanceToEdge)
            {
                _distanceToEdge = _upperLeft.x;
            }

            return _distanceToEdge;
        }

        #endregion

    }
}
