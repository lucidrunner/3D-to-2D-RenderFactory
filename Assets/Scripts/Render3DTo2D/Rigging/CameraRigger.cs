using System;
using System.Collections.Generic;
using Render3DTo2D.Isometric;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using Render3DTo2D.Setup;
using UnityEngine;

namespace Render3DTo2D.Rigging
{
    public static class CameraRigger
    {
        #region Public Methods
        public static CameraRig AddRigToModel(GameObject aRenderFactoryObject, SetupInfo aSetupInfo)
        {
            //Right so we're finally gonna actually add the rigs now
            //If we're doing a prefab rig - load that and return
            if (aSetupInfo.Rig == SetupInfo.RigType.Prefab)
            {
                CameraRig _prefab = aSetupInfo.GetData<CameraRig>(aSetupInfo.Prefab);
                if (_prefab != null)
                    return _prefab;
                
                //If we've failed with loading the prefab, log & return null
                FLogger.LogMessage(null, FLogger.Severity.FatalError, "Failed with loading prefab during setup, rig not added to the current factory.", nameof(CameraRigger));
            }
            
            //Add the rig prefab object to the aAttachTo
            GameObject _rigObject = SetupResources.Instance.AddCameraRigToGameObject(aRenderFactoryObject, aSetupInfo.Rig);
            CameraRig _cameraRig = _rigObject.GetComponent<CameraRig>();

            //Set the tag
            SetupRigMode(_cameraRig, ref aSetupInfo);

            //If we're supposed to setup cameras on the rig, do so 
            if (aSetupInfo.SetPlacementMode == SetupInfo.PlacementMode.Generated)
                SetupCamerasOnRig(_cameraRig, ref aSetupInfo);
            else
                SetupManualPlacementOnRig(_cameraRig);
                //Depending on type, get the correct initial scale from the global / specific rendering settings
            var _renderingSettings = RenderingSettings.GetFor(aRenderFactoryObject.transform);
            float _initalScale = 1;
            switch (aSetupInfo.Rig)
            {
                case SetupInfo.RigType.TopView:
                case SetupInfo.RigType.SideView:
                    _initalScale = _renderingSettings.BaselineScale;
                    break;
                case SetupInfo.RigType.Isometric: //We've already set all the scales via the refresh method for isometric rigs
                    _initalScale = -1;
                    break;
            }
            
            if(_initalScale > 0)
                _cameraRig.GetComponent<RigScaleCalculator>().SetCameraScales(_initalScale);
            
            //Return the finished CameraRig so it can be coupled with the RenderFactory
            return _cameraRig;
        }

        #endregion

        #region Private Methods

        private static void SetupCamerasOnRig(CameraRig aCameraRig, ref SetupInfo aSetupInfo)
        {
            
            //Get the camera anchor
            GameObject _anchor = aCameraRig.CameraAnchor;
            
            //Get the relevant info
            int _numberOfCameras = aSetupInfo.NumberOfCameras;
            float _yOffset = aSetupInfo.InitialOffset;
            bool _invertRotation = aSetupInfo.InvertRotation;
            float _cameraStepSize = 360f / _numberOfCameras;

            
            
            if (aSetupInfo.SetPlacementType == SetupInfo.PlacementType.Manual)
                _cameraStepSize = (float)aSetupInfo.GetData(aSetupInfo.ManualAngle);
            else if (aSetupInfo.GetData<bool>(aSetupInfo.HalfWrap) && _numberOfCameras > 1)
                _cameraStepSize = 180f / (_numberOfCameras -1); // -1 because we want to go the full 180%

            //Get the cameras and set them to their correct rotation
            for(int _index = 0; _index < _numberOfCameras; _index++)
            {
                //Get the correct camera for the rig type
                GameObject _addedCamera = SetupResources.Instance.AddRenderCameraToAnchor(SetupResources.Instance.GetRenderCameraPrefab(aSetupInfo.Rig), _anchor);
                
                //Set the camera rotation to correctly match its index
                Vector3 _eulerAngles = _addedCamera.transform.eulerAngles;
                _eulerAngles.y = _yOffset;
                if (!_invertRotation)
                {
                    _eulerAngles.y += _cameraStepSize * _index;
                }
                else
                {
                    _eulerAngles.y -= _cameraStepSize * _index;
                }
                
                //If we're dealing with an isometric camera, also set it to the default angle for the factory / scene
                if (aSetupInfo.Rig == SetupInfo.RigType.Isometric)
                {
                    _eulerAngles.x = aSetupInfo.GetData<float>(aSetupInfo.IsometricAngle);
                }

                //And if we're dealing with a top down view, rotate the cameras to look down at it
                if (aSetupInfo.Rig == SetupInfo.RigType.TopView)
                {
                    _eulerAngles.x = 90;
                }
                
                //Set the rotation and add the camera
                _addedCamera.transform.eulerAngles = _eulerAngles;
                aCameraRig.AddCamera(_addedCamera);
            }

            //If we're dealing with an isometric rig - align them properly in width and height with the reference base
            if (aSetupInfo.Rig == SetupInfo.RigType.Isometric)
            {
                //Sync the angle & pass on the plate base line scale so it's updated
                aCameraRig.GetComponent<ModelBaseManager>().RefreshAlignment(aSetupInfo.GetData<float>(aSetupInfo.IsometricBaseSize));
            }
        }

        private static void SetupManualPlacementOnRig(CameraRig aCameraRig)
        {
            aCameraRig.SetupManualPlacementMode();
        }

        private static void SetupRigMode(CameraRig aCameraRig, ref SetupInfo aSetupInfo)
        {
            aCameraRig.SetupRigMode(aSetupInfo.Rig);
            switch (aSetupInfo.Rig)
            {
                case SetupInfo.RigType.SideView:
                    aCameraRig.SetRigTag(GlobalFolderSettings.Instance.SideViewRigTag);
                    break;
                case SetupInfo.RigType.Isometric:
                    aCameraRig.SetRigTag(GlobalFolderSettings.Instance.IsometricRigTag);
                    break;
                case SetupInfo.RigType.TopView:
                    aCameraRig.SetRigTag(GlobalFolderSettings.Instance.TopDownRigTag);
                    break;
                case SetupInfo.RigType.Prefab:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
        
        
        //So this whole class was constructed when I expected greater parity in camera types & different setup classes etc
        //Since I instead opted for a slightly clunkier partial class for the ModelRenderer and realised everything was a bit simpler this is a bit dumb
        //For the dangers of opting for a non-generic builder pattern though see the RigCamera/RenderingInfo class :p
        public class SetupInfo
        {
            //For any new camera setup, add it to the enum below and handle it in the AddRigToModel on top of the class
            public enum RigType
            {
                SideView,
                Isometric,
                TopView,
                Prefab
            }

            public enum PlacementMode
            {
                Generated,
                CustomPlacement
            }

            //Whether or not the cameras should be placed with an automatically 360 / cameras wrap or via a manual offset + desired angle between each camera
            public enum PlacementType
            {
                AutoWrap,
                Manual
            }
            
            //For any new setting, add a flag below, a property to unpack it via, and make sure to pack it in the ModelRenderer and unpack it in setup above
            private Dictionary<string, object> rigFlags = new Dictionary<string, object>();
            public readonly string NumberOfCamerasInt = "cams";
            public readonly string InitialOffsetFloat = "offset";
            public readonly string InvertRotationBool = "invert";
            public readonly string ManualAngle = "manualA";
            public readonly string IsometricAngle = "isometricAngle";
            public readonly string IsometricBaseSize = "isometricBase";
            public readonly string HalfWrap = "halfWrap";
            public readonly string Prefab = "prefab";
            
            public SetupInfo(RigType aRigType, PlacementMode aPlacementMode, PlacementType aSetPlacementTypeType)
            {
                Rig = aRigType;
                SetPlacementType = aSetPlacementTypeType;
                SetPlacementMode = aPlacementMode;
            }

            public void AddSetupFlag(string aFlag, object aData)
            {
                rigFlags[aFlag] = aData;
            }

            public object GetData(string aFlag)
            {
                return rigFlags.TryGetValue(aFlag, out var _data) ? _data : null;
            }

            public T GetData<T>(string aFlag)
            {
                if (rigFlags.TryGetValue(aFlag, out var _data) && _data is T _value)
                {
                    return _value;
                }
                
                LogSetupError(aFlag);
                return default;
            }


            public RigType Rig { get; }
            
            public PlacementType SetPlacementType { get; }
            
            public PlacementMode SetPlacementMode { get; }

            //These properties make it a bit easier to visualise when using but could have been replaced with a generic UnPack(flag) instead
            public int NumberOfCameras
            {
                get
                {
                    if(rigFlags.ContainsKey(NumberOfCamerasInt))
                    {
                        return (int)rigFlags[NumberOfCamerasInt];
                    }

                    LogSetupError("Number of Cameras");
                    return 0;
                }
            }
            public float InitialOffset
            {
                get
                {

                    if(rigFlags.ContainsKey(InitialOffsetFloat))
                    {
                        return (float)rigFlags[InitialOffsetFloat];
                    }

                    LogSetupError("Initial Offset");
                    return 0;
                }
            }

            public bool InvertRotation 
            { 
                get
                {

                    if(rigFlags.ContainsKey(InvertRotationBool))
                    {
                        return (bool)rigFlags[InvertRotationBool];
                    }
                    
                    LogSetupError("Invert Rotation");
                    return false;
                }
            }

            public void AddIsometricInfo(float aIsometricAngle, float aIsometricBaseSize)
            {
                rigFlags.Add(IsometricAngle, aIsometricAngle);
                rigFlags.Add(IsometricBaseSize, aIsometricBaseSize);
            }

            private static void LogSetupError(string aErrorOrigin)
            {
                FLogger.LogMessage(null, FLogger.Severity.Error,$"Internal error for CameraRiggers setup info. {aErrorOrigin} was accessed in an invalid state. This is most likely an oversight in the setup functions for the different rigs.", nameof(CameraRigger));
            }
        }


    }
}
