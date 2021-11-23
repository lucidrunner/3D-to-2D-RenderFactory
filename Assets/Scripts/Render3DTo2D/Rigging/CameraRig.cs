using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility.IO;
using Render3DTo2D.XML_Render_Info_Export;
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
            RigDataXmlExporter.Export(new RigDataXmlExporter.RigRenderExportArgs(this, lastOutputPath, GetComponentInParent<RenderFactory>().GetRenderTimestamp(false), aSmAnimatorInfo, aRootMotionFilePath));
        }
        
        //TODO This needs to be called at the end of normal rigging too
        public void ValidateCameraSetup()
        {
            //If we're not doing manual placement this is just a reload of the camera lists in this / the calculator class
            cameras = new List<CameraRenderer>();
            var _anchorTransform = cameraAnchor.transform;
            for (var _index = 0; _index < _anchorTransform.childCount; _index++)
            {
                var _camera = cameraAnchor.transform.GetChild(_index);
                Debug.Log(_camera.name);
                var _renderer = _camera.GetComponent<CameraRenderer>();
                if(_renderer != null)
                {
                    cameras.Add(_renderer);
                    _renderer.SetCameraNumber(_index);
                }
            }
            
            

            rigScaleCalculator.ValidateSetup(cameras);
            Debug.Log("Cameras validated: " + cameras.Count);

            //If we're doing manual placement we should do an additional validation on the cameras to make sure they're also correctly setup according to the rig type
            //TODO
        }

        #endregion

        #region Private Methods

        public void AddCamera(GameObject aAddedCamera)
        {
            var _renderer = aAddedCamera.GetComponent<CameraRenderer>();
            if (_renderer == null)
            {
                //TODO Show warning message
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
