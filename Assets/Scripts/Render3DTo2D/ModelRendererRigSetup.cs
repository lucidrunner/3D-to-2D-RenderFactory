using Render3DTo2D.Factory_Core;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.Setup;
using UnityEngine;

namespace Render3DTo2D
{
    public partial class ModelRenderer
    {

        #region Inspector

        public float IsometricSetupAngle => isometricAngle;
        public float IsometricSetupBaseSize => isometricBaseSize;
        

        [SerializeField] private CameraRigger.SetupInfo.RigType cameraRig = CameraRigger.SetupInfo.RigType.SideView;
        
        [SerializeField, Range(0,90)]
        private float isometricAngle = 32f;
        
        [SerializeField] private float isometricBaseSize = 1f;
        
        [SerializeField] private int numberOfCameras = 8;
        
        //Defaults to -90 since that's helpful for the side camera
        [SerializeField] private float initialCameraOffset = -90;
        
        [SerializeField] private bool invertCameraRotation = false;
        
        [SerializeField] private CameraRigger.SetupInfo.PlacementType placementType = CameraRigger.SetupInfo.PlacementType.AutoWrap;

        [SerializeField, Range(1, 360)]
        private float manualAngle = 30;
        
        [SerializeField] private bool halfWrap = false;

        [SerializeField] private CameraRig prefabRig;

        [SerializeField] private CameraRigger.SetupInfo.PlacementMode placementMode = CameraRigger.SetupInfo.PlacementMode.Generated;
        
        public void AddCameraRigToFactory()
        {
            //Android style packaging for setup info cause I did a bunch of mobile work right before factory 2.0
            CameraRigger.SetupInfo _setupInfo = new CameraRigger.SetupInfo(cameraRig, placementMode, placementType);
            
            //The always include settings
            _setupInfo.AddSetupFlag(_setupInfo.NumberOfCamerasInt, numberOfCameras);
            _setupInfo.AddSetupFlag(_setupInfo.InitialOffsetFloat, initialCameraOffset);
            _setupInfo.AddSetupFlag(_setupInfo.InvertRotationBool, invertCameraRotation);

            //Isometric settings
            if (cameraRig == CameraRigger.SetupInfo.RigType.Isometric)
                _setupInfo.AddIsometricInfo(isometricAngle, isometricBaseSize);

            //If needed - manual placement settings
            if(placementType == CameraRigger.SetupInfo.PlacementType.Manual)
                _setupInfo.AddSetupFlag(_setupInfo.ManualAngle, manualAngle);
            else if(placementType == CameraRigger.SetupInfo.PlacementType.AutoWrap)
                _setupInfo.AddSetupFlag(_setupInfo.HalfWrap, halfWrap);
            
            //If we're doing a prefab rig, add that
            if(cameraRig == CameraRigger.SetupInfo.RigType.Prefab)
                _setupInfo.AddSetupFlag(_setupInfo.Prefab, prefabRig);

            GetSelectedRenderFactory(setupFactoryType, out var _setupFactory);
            ModelSetupHelper.SetupCameraRig(_setupFactory ,gameObject, _setupInfo);
        }


        #region Inspector Conditionals
        
        private void LoadRigBaseSettings()
        {
            var _settings = RenderingSettings.GetFor(transform);
            if (_settings == null) return;

            isometricAngle = _settings.IsometricDefaultAngle;
            isometricBaseSize = _settings.IsometricBaseline;
        }

        public bool ShowAdd
        {
            get
            {
                var _factoryAdded = GetSelectedRenderFactory(setupFactoryType, out var _childRenderFactory);
                return _factoryAdded && !_childRenderFactory.Busy;
            }
        }
        
        #endregion

        #endregion
    }
}