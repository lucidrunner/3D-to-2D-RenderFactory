using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using UnityEngine;

namespace Render3DTo2D.Isometric
{
    //TODO LAST ONE FOR UI
    public class ModelBaseManager : MonoBehaviour
    {
        #region Private Fields
        [SerializeField]
        private GameObject modelBase = null;

        #pragma warning disable 0414
        [SerializeField, UsedImplicitly] 
        private bool camerasNeedAlignment = false;
        #pragma warning restore 0414
        
        [SerializeField]
        private bool syncAutomatically = true;
        
        private Dictionary<int, float> perAnimationScales;

        private float baseReferenceOffset;
        
        
        #endregion
        
        #region Properties

        public float BaseReferenceOffset => baseReferenceOffset;

        public float BaseDeviation
        {
            get
            {
                float _baseline = RenderingSettings.GetFor(transform).IsometricBaseline;
                float _modelScale = modelBase.transform.localScale.x;
                return _modelScale / _baseline;
            }
        }
            
        #endregion

        #region Inspector

        [SerializeField] private float latestReferenceY;

        [SerializeField, Range(0f, 90f)]
        private float cameraAngle;
        
        [SerializeField, Min(0.00001f)]
        private float orthographicScale = 1;

        [SerializeField, Min(0.00001f)]
        private float referencePlateScale;

        //This is done automatically for alignment, however, one could set all cameras y to 0 and then simply match width to have centered isometric textures
        public void MatchBaseWidths()
        {
            List<Camera> _cameras = new List<Camera>(); 
            _cameras.AddRange(GetComponentsInChildren<Camera>());
            IsometricMethods.MatchCameraWidthToBasePlate(_cameras, modelBase);
        }
        
        public void AlignCamerasWithBase()
        {
            List<Camera> _cameras = new List<Camera>(); 
            _cameras.AddRange(GetComponentsInChildren<Camera>());
            latestReferenceY = IsometricMethods.AlignBasePlateWithViewportBottom(_cameras, modelBase);
            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Latest Reference Y image height for feet (base center) when aligning: {latestReferenceY * 100}%");
            camerasNeedAlignment = false;

            if (RenderingSettings.GetFor(transform).AutomaticDepthCalculation)
                IsometricMethods.SetCameraDepth(_cameras, modelBase);
        }

        #region Methods and Properties


        public void ApplyCameraAngle(float aNewAngle)
        {
            List<Camera> _cameras = new List<Camera>();
            _cameras.AddRange(GetComponentsInChildren<Camera>());

            IsometricMethods.SetCameraAngle(_cameras, aNewAngle);
            if (_cameras.Count <= 0) return;
            
            SyncCheck();
        }

        public void SetOrthographicSize(float aNewSize)
        {
            float _newPlateSize = aNewSize * Mathf.Sqrt(2);
            referencePlateScale = _newPlateSize;
            SetPlateSize(_newPlateSize);
        }

        public void SetPlateSize(float aNewSize)
        {
            Vector3 _newScale = new Vector3(aNewSize, aNewSize, 1);
            modelBase.transform.localScale = _newScale;
            SyncCheck();
            orthographicScale = GetComponentInChildren<Camera>().orthographicSize;
        }
        
        public float CameraAngle => GetComponentInChildren<Camera>().transform.localEulerAngles.x;

        public float CurrentOrthoScale => orthographicScale;

        public float CurrentPlateSize => referencePlateScale;
        
        
        #endregion

        #endregion
        
        #region Public Methods

        private void Reset()
        {
            modelBase = GetComponentInChildren<ModelBase>().gameObject;
            ApplyCameraAngle(GetComponentInParent<ModelRenderer>().IsometricSetupAngle);
            RefreshAlignment(GetComponentInParent<ModelRenderer>().IsometricSetupBaseSize);
        }

        /// <summary>
        /// Used during rig setup to make sure the values on this & the setup values match and aligns if they don't
        /// </summary>
        /// <param name="aModelBaseScale">The new scale of the model base</param>
        public void RefreshAlignment(float aModelBaseScale)
        {
            //Angles are set directly during rigging since we have access to all the cameras at that point
            cameraAngle = GetComponentInChildren<Camera>().transform.localRotation.eulerAngles.x;
            //The base is only known to this and needs to be set here though
            referencePlateScale = aModelBaseScale;
            SetPlateSize(aModelBaseScale);
            
            //If our prefab is not set to sync automatically, make sure we still sync after setting the plate size
            if(!syncAutomatically)
                AlignCamerasWithBase();
        }

        public void CenterCameras()
        {
            var _cameras = GetComponentsInChildren<Camera>();
            foreach (var _camera in _cameras)
            {
                var _transform = _camera.transform;
                var _position = _transform.localPosition;
                _position.y = 0;
                _transform.localPosition = _position;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if we should sync or not after one of the base / camera values have been changed
        /// </summary>
        private void SyncCheck()
        {
            if (syncAutomatically)
                AlignCamerasWithBase();
            else
                camerasNeedAlignment = true;
        }

        private void Start()
        {
            //If we're starting the factory and have the auto depth calculation on, set the depth
            void OnRenderFactoryEventsOnFactoryStarted(object aSender, EventArgs aArgs)
            {
                if (GeneralUtilities.CompareSenderToModelRoot(aSender, transform) && RenderingSettings.GetFor(transform).AutomaticDepthCalculation) 
                    IsometricMethods.SetCameraDepth(GetComponentsInChildren<Camera>(), modelBase);
            }

            RenderFactoryEvents.FactoryStarted += OnRenderFactoryEventsOnFactoryStarted;
        }

        #endregion

        //TODO Either remove these methods or implement the PerAnimationSettings in here
        //Implementation could be a "Use Per Animation Size" which when clicked loads all animations the same way our override does and have a settable float for size on that animation
        //Note that this might break the current baseReferenceOffset which is used by the data exporters at the moment
            
        /// <summary>
        /// Gets the Y-value offset between the base of the model and the bottom of the rendered frame
        /// </summary>
        /// <param name="aAnimationNumber">The index of the animation</param>
        /// <returns>The Y-offset between the bottom of the frame and the model feet</returns>
        public float GetOffsetForAnimation(int aAnimationNumber)
        {
            //TODO This only returns -1 atm
            if (!perAnimationScales.TryGetValue(aAnimationNumber, out float _toReturn))
                _toReturn = -1f;
            return _toReturn;
        }

        /// <summary>
        /// Checks if the new animation needs a specific model base scale, performs alignment and saves the y-offset to the offset dictionary
        /// </summary>
        /// <param name="aAnimationNumber">The index of the new animation</param>
        public void OnNewAnimationRendering(int aAnimationNumber)
        {
            FLogger.LogMessage(this, FLogger.Severity.Debug, "Entering OnNewAnimationRendering for the ModelBaseManager");
            //TODO This is seemingly never run
            if (perAnimationScales == null)
            {
                perAnimationScales = new Dictionary<int, float>();
                //TODO This shouldn't be set I think
                baseReferenceOffset = latestReferenceY;
            }

            //If we've already saved a frame for this animation 
            if (perAnimationScales.ContainsKey(aAnimationNumber))
            {
                Debug.Log("OnNewAnimationRendering called for ModelBaseManager with an already existing number. Have you changed the animation setup in any weird way?");
                return;
            }
            
            
            //TODO Check the PerAnimationSettings here
            //This whole deal might be deprecated?
            //float _valueFromPerAnimationsGoesHere = PerAnimationSettings.GetEtcEtc
            //Check if we need to realign due to last animation having set the scale to something else
            if (!Mathf.Approximately(referencePlateScale, modelBase.transform.localScale.x))
            {
                SetPlateSize(referencePlateScale);
                if(!syncAutomatically) //Force an align manually if we're not set to always sync
                    AlignCamerasWithBase();
                
                //Save the latest aligned value to the dictionary
                perAnimationScales[aAnimationNumber] = latestReferenceY;
            }
            else
            {
                FLogger.LogMessage(this, FLogger.Severity.Debug, $"Did not enter the approx check, the values were ${referencePlateScale}, {modelBase.transform.localScale.x}");
            }
            
        }
    }
}
