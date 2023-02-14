
using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility.IO;
using Render3DTo2D.Render_Info_Export;
using UnityEngine;

namespace Render3DTo2D.Rigging
{
    public class CameraRig: MonoBehaviour
    {
        #region Inspector

        [SerializeField, HideInInspector]
        private GameObject cameraAnchor = null;

        [SerializeField, HideInInspector]
        private RigScaleCalculator rigScaleCalculator = null;

        [SerializeField, HideInInspector] 
        private RigRenderer rigRenderer = null;
        
        [SerializeField, HideInInspector]
        private string rigTag = "";

        [SerializeField, HideInInspector] private bool manualPlacementMode = false;

        [SerializeField, HideInInspector] private CameraRigger.SetupInfo.RigType rigType;
        
        [SerializeField, HideInInspector]
        private List<CameraRenderer> cameras = new List<CameraRenderer>();
        
        #endregion

        #region Properties

        internal GameObject CameraAnchor => cameraAnchor;

        public string RigTag => rigTag;

        #endregion

        #region Private Fields


        private string lastOutputPath = "";
        

        #endregion

        #region Public Methods

        private void Reset()
        {
            GetComponentReferences();
        }

        private void GetComponentReferences()
        {
            cameraAnchor = GetComponentInChildren<CameraAnchor>().gameObject;
            rigScaleCalculator = GetComponent<RigScaleCalculator>();
            rigRenderer = GetComponent<RigRenderer>();
        }

        //Generic startup for static rendering

        public void Startup()
        {
            //Get our components if they're not linked
            GetComponentReferences();
            
            //Do the static rig startup
            rigScaleCalculator.Startup();
            
            //Create a static output path if none exists
            lastOutputPath = FactoryFolderCreator.CreateStaticFolderForRig(this);
            //Run the renderer startup
            rigRenderer.Startup(rigScaleCalculator, rigTag);
        }

        public void Startup(StopMotionAnimatorInfo aSMAnimatorInfo, bool aForceRecalculate)
        {
            //Get our components if they're not linked
            GetComponentReferences();
            
            //Link our scale calculator with the animator & set the starting scale
            rigScaleCalculator.Startup(aSMAnimatorInfo, aForceRecalculate);
            rigRenderer.Startup(rigScaleCalculator, rigTag);
        }

        //Generic Render startup for static rendering
        public void RenderStartup()
        {
            lastOutputPath = FactoryFolderCreator.CreateStaticFolderForRig(this);
            rigRenderer.RenderStartup(lastOutputPath);
        }

        public void RenderStartup(StopMotionAnimatorInfo aSMAnimatorInfo)
        {
            lastOutputPath = null;
            //Create the output folders and link our renderer to them
            FolderSettings _folderSettings = FolderSettings.GetFor(transform);
            lastOutputPath = FactoryFolderCreator.CreateFolderForRig(this);
                
            if (_folderSettings.CreateAnimationSubfolders)
            {
                foreach (string _animationName in aSMAnimatorInfo.AnimationNames)
                {
                    FactoryFolderCreator.CreateAnimationFolderForRig(this, _animationName);
                }
            }

            rigRenderer.RenderStartup(lastOutputPath);
        }

        public void SetRigTag(string aRigTag)
        {
            rigTag = aRigTag;
        }

        public void SetupName(int aRigNumber)
        {
            rigTag += aRigNumber;
            name = $"{name}_{rigTag}";
        }

        public void SetupManualPlacementMode()
        {
            manualPlacementMode = true;
        }

        public void SetupRigMode(CameraRigger.SetupInfo.RigType aRigType)
        {
            rigType = aRigType;
        }
        
        public void ExportAnimationData(StopMotionAnimatorInfo aSmAnimatorInfo, string aRootMotionFilePath = null)
        {
            //Actually This is where we should export the data methinks?
            
            RigDataJsonExporter.Export(new RigRenderExportArgs(this, lastOutputPath, GetComponentInParent<RenderFactory>().GetRenderTimestamp(false), aSmAnimatorInfo, aRootMotionFilePath));
            RigDataXmlExporter.Export(new RigRenderExportArgs(this, lastOutputPath, GetComponentInParent<RenderFactory>().GetRenderTimestamp(false), aSmAnimatorInfo, aRootMotionFilePath));
        }
        
        public void ValidateCameraSetup()
        {
            //If we're not doing manual placement this is just a reload of the camera lists in this / the calculator class
            cameras = new List<CameraRenderer>();
            var _anchorTransform = cameraAnchor.transform;
            for (var _index = 0; _index < _anchorTransform.childCount; _index++)
            {
                var _camera = cameraAnchor.transform.GetChild(_index);
                var _renderer = _camera.GetComponent<CameraRenderer>();
                if (_renderer == null) continue;
                cameras.Add(_renderer);
                _renderer.SetCameraNumber(_index);
            }
            
            

            rigScaleCalculator.ValidateSetup(cameras);
            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Validated {cameras.Count} cameras.");

            if (!manualPlacementMode)
                return;

            //If we're doing manual placement we should do an additional validation on the cameras to make sure they're also correctly setup according to the rig type
            //Failing to pass this should provide a warning and we should also have the ability to suppress that error on the component if we ever wanna do something funky
            bool _setupValid = true;
            string _errorMessage = "";
            switch (rigType)
            {
                case CameraRigger.SetupInfo.RigType.SideView:
                    //If we're in side view, check that all cameras are set to X / Z rotation 0
                    _setupValid = cameras.Select(t => t.transform).All(aTransform => Mathf.Approximately(0, aTransform.localEulerAngles.x) && Mathf.Approximately(0, aTransform.localEulerAngles.z));
                    _errorMessage = "One or more Cameras in setup is not in a side view (X/Z-angles = 0), this might cause odd effects during calculations.";
                    break;
                case CameraRigger.SetupInfo.RigType.Isometric:
                    //If we're in isometric, we want our x to be in the 0 < x < 90 range
                    _setupValid = cameras.Select(t => t.transform).All(aTransform => aTransform.localEulerAngles.x > 0 && aTransform.localEulerAngles.x < 90  && Mathf.Approximately(0, aTransform.localEulerAngles.z));
                    _errorMessage = "One or more Cameras in setup is not in an isometric view (0 < x-angle < 90, z-angle = 0), this might cause odd effects during rendering.";
                    break;
                case CameraRigger.SetupInfo.RigType.TopView:
                    //If we're in isometric, we want our x to be in the 0 < x < 90 range
                    _setupValid = cameras.Select(t => t.transform).All(aTransform => Mathf.Approximately(90, aTransform.localEulerAngles.x) && Mathf.Approximately(0, aTransform.localEulerAngles.z));
                    _errorMessage = "One or more Cameras in setup is not in a topdown view (x-angle = 90, z-angle = 0), this might cause off effects during calculations.";
                    break;
            }

            if (!_setupValid)
            {
                FLogger.LogMessage(this, FLogger.Severity.Priority, _errorMessage);
            }
        }

        #endregion

        #region Private Methods

        public void AddCamera(GameObject aAddedCamera)
        {
            var _renderer = aAddedCamera.GetComponent<CameraRenderer>();
            if (_renderer == null)
            {
                FLogger.LogMessage(this, FLogger.Severity.LinkageError, $"{nameof(CameraRenderer)} missing on added camera. Is there something wrong with the prefab?");
            }
            cameras.Add(_renderer);
            CameraScaleCalculator _scaleCalculator = aAddedCamera.GetComponent<CameraScaleCalculator>();
            rigScaleCalculator.AddCameraCalculator(_scaleCalculator);
            ValidateCameraSetup();
        }

        public void RemoveCamera(CameraRenderer aRemovedCamera)
        {
            if (aRemovedCamera == null) return;
            CameraScaleCalculator _scaleCalculator = aRemovedCamera.GetComponent<CameraScaleCalculator>();
            rigScaleCalculator.RemoveCameraCalculator(_scaleCalculator);
            if (cameras.Contains(aRemovedCamera))
            {
                cameras.Remove(aRemovedCamera);
            }
            DestroyImmediate(aRemovedCamera.gameObject);
            ValidateCameraSetup();
        }

        #endregion
    }
}
