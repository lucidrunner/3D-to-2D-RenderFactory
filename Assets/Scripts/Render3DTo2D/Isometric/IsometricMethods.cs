using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using UnityEngine;

public static class IsometricMethods
{

    
    public static void MatchCameraWidthToBasePlate(Camera aOrthographicCamera, GameObject aModelBase)
    {
        //Matching width is easy
        Vector3 _localScale = aModelBase.transform.localScale;
        aOrthographicCamera.orthographicSize = _localScale.x / Mathf.Sqrt(2);
    }
    
    
    
    private static void FixClippingPlanes(Camera aOrthographicCamera, float aClippingSize)
    {
        aOrthographicCamera.farClipPlane = aClippingSize;
        aOrthographicCamera.nearClipPlane = -aClippingSize;
    }

    public static void MatchCameraWidthToBasePlate(List<Camera> aOrthographicCameras, GameObject aModelBase)
    {
        foreach (Camera _camera in aOrthographicCameras)
        {
            MatchCameraWidthToBasePlate(_camera, aModelBase);
        }
    }

    public static float AlignBasePlateWithViewportBottom(List<Camera> aOrthographicCameras, GameObject aModelBase)
    {
        if (aOrthographicCameras.Count == 0)
            return -1f;
        
        
        //Find our alignment camera - which is the camera with the lowest aModelBase vertex y-value in its viewport
        Camera _alignmentCamera = FindAlignmentCamera(aOrthographicCameras, aModelBase);
        //Align that camera and get our aligned y value back
        float _toReturn = AlignBasePlateWithViewportBottom(_alignmentCamera, aModelBase); 
        float _y = _alignmentCamera.transform.localPosition.y;
        //For all other cameras except the one we just aligned, set their y manually to match it
        foreach (Camera _camera in aOrthographicCameras.Where(aOrthoCam => aOrthoCam != _alignmentCamera))
        {
               _camera.transform.localPosition = new Vector3(0, _y, 0);
        }
        //Since each camera will now have the center of the aModelBase on the same y
        //this px offset for the center is valid for all of them 
        return _toReturn;
    }

    private static Camera FindAlignmentCamera(List<Camera> aOrthographicCameras, GameObject aModelBase)
    {
        float _selectedY = 1;
        Camera _selectedCamera = aOrthographicCameras[0];
        //Match all the camera widths to the base plate
        MatchCameraWidthToBasePlate(aOrthographicCameras, aModelBase);
        float _orthoSize = aModelBase.transform.localScale.x / 2;
        List<Vector3> _baseVertices = GetBaseVertices(aModelBase, _orthoSize);
        foreach (Camera _camera in aOrthographicCameras)
        {
            //We reset the heights when doing more than 1 camera since any difference in height would mess up the measurements
            var _transform = _camera.transform;
            Vector3 _pos = _transform.localPosition;
            _pos.y = 0;
            _transform.localPosition = _pos;
            //Then we do the Bottom base check on each camera
            FindBottomBaseVertex(_camera, _baseVertices, out float _cameraY);
            //and add an additional step where we compare the cameras to each other to find the lowest base vertex in the collection
            if (_cameraY < _selectedY)
            {
                _selectedY = _cameraY;
                _selectedCamera = _camera;
            }
        }

        return _selectedCamera;
    }

    public static float AlignBasePlateWithViewportBottom(Camera aOrthographicCamera, GameObject aModelBase)
    {
        float _stepSize = RenderingSettings.GetFor(aOrthographicCamera.transform).IsometricStepSize;
        //Piggyback onto the calculator max steps
        int _stepMax = RenderingSettings.GetFor(aOrthographicCamera.transform).IsometricMaxSteps;
        
        //Width needs to be matched for alignment to properly work so we call it here for safety's sake
        MatchCameraWidthToBasePlate(aOrthographicCamera, aModelBase);
        float _orthoSize = aModelBase.transform.localScale.x / 2;
        List<Vector3> _baseVertices = GetBaseVertices(aModelBase, _orthoSize);

        //Depending on the side of the reference plate we're looking at we're gonna need to use different corners for alignment
        Vector3 _chosenVertex = FindBottomBaseVertex(aOrthographicCamera, _baseVertices, out float _smallestY);

        int _currentTries = 0;
        //Move back into frame if we're not in it
        if (_smallestY < 0)
        {
            while (aOrthographicCamera.WorldToViewportPoint(_chosenVertex).y < 0 && _currentTries < _stepMax)
            {
                aOrthographicCamera.transform.localPosition -= new Vector3(0, _stepSize, 0);
                _currentTries++;
            }
        }

        //move to the bottom of the frame
        while (aOrthographicCamera.WorldToViewportPoint(_chosenVertex).y > 0 && _currentTries < _stepMax)
        {
            aOrthographicCamera.transform.localPosition += new Vector3(0, _stepSize, 0);
            _currentTries++;
        }

        
        //This returns the # height of it within the current viewport, we will need to calculate offset later via sprite size and sprite density in the scene
        return aOrthographicCamera.WorldToViewportPoint(_baseVertices[4]).y; 
    }

    private static Vector3 FindBottomBaseVertex(Camera aOrthographicCamera, IReadOnlyList<Vector3> aBaseVertices, out float aSmallestY)
    {
        Vector3 _chosenVertex = aBaseVertices[0];
        aSmallestY = 1;
        foreach (Vector3 _vertexPosition in aBaseVertices)
        {
            float _y = aOrthographicCamera.WorldToViewportPoint(_vertexPosition).y;
            if (_y < aSmallestY)
            {
                aSmallestY = _y;
                _chosenVertex = _vertexPosition;
            }
        }

        return _chosenVertex;
    }

    private static List<Vector3> GetBaseVertices(GameObject aModelBase, float aOrthoSize)
    {
        //Note that we need this to be in WorldSpace for our WorldToViewport checks later
        Vector3 _modelPosition = aModelBase.transform.position; 
        List<Vector3> _verticesPositions = new List<Vector3>
        {
            _modelPosition + new Vector3(-aOrthoSize, 0, -aOrthoSize),
            _modelPosition + new Vector3(-aOrthoSize, 0, aOrthoSize),
            _modelPosition + new Vector3(aOrthoSize, 0, -aOrthoSize),
            _modelPosition + new Vector3(aOrthoSize, 0, aOrthoSize),
            _modelPosition //the [4] we add represents the center of the base
        };
        return _verticesPositions;
    }

    public static void SetCameraAngle(List<Camera> aCameras, float aNewAngle)
    {
        FLogger.LogMessage(null, FLogger.Severity.Debug, 
            $"Setting angle {aNewAngle} for cameras.", nameof(IsometricMethods));
        foreach (Camera _camera in aCameras)
        {
            SetCameraAngle(_camera, aNewAngle);
        }
    }

    public static void SetCameraAngle(Camera aCamera, float aNewAngle)
    {
        var _transform = aCamera.transform;
            Vector3 _eulerAngles = _transform.eulerAngles;
            _eulerAngles.x = aNewAngle;
            _transform.eulerAngles = _eulerAngles;
    }

    public static void SetCameraDepth(IEnumerable<Camera> aCameras, GameObject aModelBase)
    {
        foreach (var _camera in aCameras)
        {
            FixClippingPlanes(_camera, aModelBase.transform.localScale.x);
        }
    }
}