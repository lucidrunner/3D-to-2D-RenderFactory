using System;
using System.Linq;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using UnityEngine;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.Single_Frame;
using Render3DTo2D.Utility;
using RootChecker;
using Shared_Scripts;
using UnityEditor;

namespace Render3DTo2D
{
    [RequireComponent( typeof(RootAnchor))]
    public partial class ModelRenderer : MonoBehaviour
    {
        
        #region Private Fields

        [SerializeField]
        private RenderFactory dynamicRenderFactory;

        [SerializeField]
        private RenderFactory staticRenderFactory;

        [SerializeField] private ModelInfo modelInfo;
        
        private bool enableStart = true;

        #endregion

        #region Inspector


        [SerializeField] private RenderFactoryType setupFactoryType = RenderFactoryType.Animated;

        public void SetupRenderFactory()
        {
            if (setupFactoryType == RenderFactoryType.Animated)
            {
                dynamicRenderFactory = ModelSetupHelper.AddAnimatedFactoryToObject(gameObject);
            }
            else if (setupFactoryType == RenderFactoryType.Static)
            {
                staticRenderFactory = ModelSetupHelper.AddStaticFactoryToObject(gameObject);
            }
        }
        
        
        public void AddRenderSettingsOverride()
        {
            gameObject.AddComponent<RenderingSettings>();
        }

        public void AddFolderSettingsOverride()
        {
            //Add the overriding settings script and copy the current base folder from the global settings
            gameObject.AddComponent<FolderSettings>().SetBaseFolder(GlobalFolderSettings.Instance.OutputBaseFolder);
        }

        public void AddAdvancedAnimationSettings()
        {
            gameObject.AddComponent<AdvancedAnimationSettings>().Setup(ModelInfo.GetFor(transform).Animator);
        }

        public void AddRootMotionSettings()
        {
            gameObject.AddComponent<RootMotionSettings>().Setup(ModelInfo.GetFor(transform).Animator);
        }
        
        [SerializeField]
        private RenderFactoryType runFactoryType = RenderFactoryType.Animated;
        
        public void RenderModel()
        {
            LinkComponents();
            SetLayer();
            SetPosition();
            switch (runFactoryType)
            {
                case RenderFactoryType.Animated:
                {
                    if (dynamicRenderFactory == null || !modelInfo.CanAnimate)
                        return;
                    dynamicRenderFactory.FinishedCallback += RenderFactoryOnFinishedCallback;
                    dynamicRenderFactory.RunFactory(modelInfo.Animator);
                    break;
                }
                case RenderFactoryType.Static:
                    staticRenderFactory.FinishedCallback += RenderFactoryOnFinishedCallback;
                    staticRenderFactory.RunFactory(modelInfo.Animator);
                    break;
            }
        }
        
        public void CalculateToggledRigs()
        {
            CalculateRigs(false);
        }

        public void CalculateAllRigs()
        {
            CalculateRigs(true);
        }

        private void CalculateRigs(bool aRunAll)
        {
            RenderFactory _rf = dynamicRenderFactory;
            if (_rf == null || modelInfo == null || !modelInfo.CanAnimate)
                return;

            SetLayer();
            SetPosition();
            _rf.FinishedCallback += RenderFactoryOnFinishedCallback;
            _rf.RunRecalculation(modelInfo.Animator, aRunAll);
        }

        private void RenderFactoryOnFinishedCallback(object aSender, EventArgs aE)
        {
            
            if(RenderingSettings.GetFor(transform).CenterModelOnRenderStartup)
                ResetPosition();
        }

        private void SetPosition()
        {
            GameObject _model = modelInfo != null ? modelInfo.gameObject : null;
            defaultPosition = transform.position;
            //Move the model to 0,0 if desired
            if(RenderingSettings.GetFor(transform).CenterModelOnRenderStartup)
                transform.position = Vector3.zero;
            //And center the camera on it too
            if (FactorySettings.GetOrCreateSettings().CenterCameraOnRenderStartup)
                GeneralUtilities.FocusSceneCamera(_model != null ? _model : gameObject);
            
        }

        private void ResetPosition()
        {
            GameObject _model = modelInfo != null ? modelInfo.gameObject : null;
            transform.position = defaultPosition;
            GetSelectedRenderFactory(runFactoryType, out var _factory);
            _factory.FinishedCallback -= RenderFactoryOnFinishedCallback;
            //Move the camera to the model if we moved it to begin with
            if(FactorySettings.GetOrCreateSettings().CenterCameraOnRenderStartup)
                GeneralUtilities.FocusSceneCamera(_model != null ? _model : gameObject);
        }


        private void SetLayer()
        {
            int _layer = RenderingSettings.GetFor(transform).RenderingLayer;
            gameObject.layer = _layer;
            if (modelInfo == null)
                return;
            modelInfo.SetLayer(_layer);
        }



        #region Inspector Conditionals


        public enum RenderFactoryType
        {
            Animated, Static
        }


        public bool CanAddRenderSettings => GetComponent<RenderingSettings>() == null;

        public bool CanAddFolderSettings => GetComponent<FolderSettings>() == null;

        public bool CanAddAdvancedAnimationSettings => GetComponent<AdvancedAnimationSettings>() == null;

        public bool CanAddRootMotionSettings => GetComponent<RootMotionSettings>() == null;

        public bool EnableRun
        {
            get
            {
                if (runFactoryType == RenderFactoryType.Animated)
                {
                    if (modelInfo == null || !modelInfo.CanAnimate)
                        return false;
                }
                
                bool _factoryAdded = GetSelectedRenderFactory(runFactoryType, out var _childRenderFactory);
                return _factoryAdded && !_childRenderFactory.Busy && _childRenderFactory.RigCount > 0 && enableStart;
            }
        }

        public string GetInvalidRunMessage()
        {
            var _factoryAdded = GetSelectedRenderFactory(runFactoryType, out var _childRenderFactory);
            if (!_factoryAdded)
                return "Can't run as selected factory type hasn't been added to the model.";
            if(runFactoryType == RenderFactoryType.Animated && (modelInfo == null || !modelInfo.CanAnimate))
            {
                return (modelInfo == null ? "Can't run animated render factory due to missing ModelInfo component on the model." : "Cant run animated render factory as the model can't animate.") + " Resetting the script should solve this if the model is correctly setup.";
            }
            if (!_childRenderFactory.Busy && !EnableRun)
            {
                return "Can't run as no Rig is enabled or added to the selected factory.";
            }

            return null;
        }

        private bool GetSelectedRenderFactory(RenderFactoryType aTypeSource, out RenderFactory aFactory)
        {
            switch (aTypeSource)
            {
                case RenderFactoryType.Animated when dynamicRenderFactory != null:
                    aFactory = dynamicRenderFactory;
                    return true;
                case RenderFactoryType.Static when staticRenderFactory != null:
                    aFactory = staticRenderFactory;
                    return true;
                default:
                    aFactory = null;
                    return false;
            }
            
        }

        #endregion

        #endregion

        #region Methods

        public void LinkComponents()
        {
            if (modelInfo == null)
                modelInfo = GetComponentInChildren<ModelInfo>();
        }

        private int defaultLayer;
        private Vector3 defaultPosition = Vector3.zero;

        private void Reset()
        {
            //Mega ugly
            dynamicRenderFactory = GetComponentsInChildren<RenderFactory>().FirstOrDefault(aFactory => aFactory.GetComponent<RigManager>() != null && !(aFactory.GetComponent<RigManager>() is StaticRigManager));
            staticRenderFactory  = GetComponentsInChildren<RenderFactory>().FirstOrDefault(aFactory => aFactory.GetComponent<StaticRigManager>() != null);
            modelInfo = GetComponentInChildren<ModelInfo>();
            LoadRigBaseSettings();
        }

        private void Start()
        {
            
            defaultLayer = modelInfo != null ? modelInfo.gameObject.layer : gameObject.layer; 
            //Turn off the ability to start rendering while we're running another renderer
            //Also, set the layer to the default layer so we won't interfere 
            RenderFactoryEvents.FactoryStarted += (aSender, aArgs) =>
            {
                if (!GeneralUtilities.CompareSenderToModelRoot(aSender, transform))
                {
                    RenderingSettings _settings = RenderingSettings.GetFor(transform);
                    enableStart = false;
                    if (!_settings.SetToLayer)
                    {
                        gameObject.layer = 0;
                        if(modelInfo != null)
                            modelInfo.SetLayer(0);
                    }
                    else
                    {
                        gameObject.layer = _settings.RenderingLayer;
                        if(modelInfo != null)
                            modelInfo.SetLayer(_settings.RenderingLayer);
                    }
                }
                
            };

            //make sure we're enabled again after rendering stops and reset back to our default layer
            RenderFactoryEvents.FactoryEnded += (aSender, aArgs) =>
            {
                enableStart = true;
                gameObject.layer = defaultLayer;
                if(modelInfo != null) 
                    modelInfo.SetLayer(defaultLayer);
            };
        }

        private void OnValidate()
        {
            if (numberOfCameras < 1)
                numberOfCameras = 1;
            if (isometricBaseSize <= 0.0000001f)
                isometricBaseSize = 0.0000001f;
        }

        #endregion
    }
}
