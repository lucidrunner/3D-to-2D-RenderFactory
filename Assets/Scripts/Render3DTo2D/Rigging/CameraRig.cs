using System;
using System.Collections.Generic;
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
        
        #endregion

        #region Properties

        internal GameObject CameraAnchor => cameraAnchor;

        public string RigTag => rigTag;

        #endregion

        #region Private Fields

        private List<GameObject> cameras = new List<GameObject>();

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

        #endregion

        #region Private Methods

        internal void AddCamera(GameObject addedCamera)
        {
            cameras.Add(addedCamera);
        
            CameraScaleCalculator _scaleCalculator = addedCamera.GetComponent<CameraScaleCalculator>();

            if(_scaleCalculator == null)
            {
                //Some weird setup error has happened
                Debug.LogWarning("Camera successfully added but missing a scale calculator. Something is probably wrong with the prefab");
                return;
            }

            rigScaleCalculator.AddCameraCalculator(_scaleCalculator);
        }

        #endregion

    }
}
