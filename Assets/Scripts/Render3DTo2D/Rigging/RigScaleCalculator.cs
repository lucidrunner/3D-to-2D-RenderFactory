using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Coroutine;
using JetBrains.Annotations;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.IO;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Rigging
{
    //TODO BIG - Move from the Matrix to a List[AnimationIndex]Dictionary[CameraID, List of Frames]. We're never going to have jagged frames anyway.
    
    public class RigScaleCalculator : MonoBehaviour
    {
        #region Properties

        public bool HasCalculatedFrames { get; protected set; }

        #endregion


        #region Inspector

        #pragma warning disable 0414
        [SerializeField][UsedImplicitly] private float currentInspectorScale = 1f;
        [SerializeField][UsedImplicitly, Min(0f)] private float setClippingDepth = 1f;
        #pragma warning restore 0414
        
        [SerializeField] [HideInInspector]
        protected List<CameraScaleCalculator> cameraScaleCalculators = new List<CameraScaleCalculator>();

        [SerializeField] [HideInInspector]
        private List<CameraEdgeReCalculator> cameraEdgeReCalculators = new List<CameraEdgeReCalculator>();

        #endregion

        #region Private Fields

        private List<RigFrameScaleFile.AnimationCameraScales> animationFrameScalePackages;

        protected StopMotionAnimatorInfo smAnimatorInfo;
        protected RenderingSettings settings;

        #endregion

        #region Methods
        
        #region Setup

        public void AddCameraCalculator(CameraScaleCalculator aCameraCalculator)
        {
            cameraScaleCalculators ??= new List<CameraScaleCalculator>();
            cameraEdgeReCalculators ??= new List<CameraEdgeReCalculator>();

            //Warn if we have a broken prefab
            if (aCameraCalculator == null)
            {
                FLogger.LogMessage(this, FLogger.Severity.LinkageError, $"Couldn't find a {nameof(CameraScaleCalculator)} on added camera. This will most likely cause issues on run. Check if the camera prefab is correctly setup.");
            }

            if (cameraScaleCalculators.Contains(aCameraCalculator) == false)
            {
                cameraScaleCalculators.Add(aCameraCalculator);
                //Guard against future prefab changes
                var _edgeCalc = aCameraCalculator.GetComponent<CameraEdgeReCalculator>();
                if(_edgeCalc != null)
                    cameraEdgeReCalculators.Add(_edgeCalc);
            }
        }

        internal void RemoveCameraCalculator(CameraScaleCalculator aCameraCalculator)
        {
            if (aCameraCalculator == null)
                return;

            cameraScaleCalculators.Remove(aCameraCalculator);
            var _edgeCalc = aCameraCalculator.GetComponent<CameraEdgeReCalculator>();
            if (_edgeCalc != null)
                cameraEdgeReCalculators.Remove(_edgeCalc);
        }

        internal void ValidateSetup(IEnumerable<CameraRenderer> aCameras)
        {
            cameraScaleCalculators = new List<CameraScaleCalculator>();
            cameraEdgeReCalculators = new List<CameraEdgeReCalculator>();
            
            //Reload lists based on the provided collection
            foreach (var _camera in aCameras)
            {
                var _scaleCalculator = _camera.GetComponent<CameraScaleCalculator>();
                var _edgeCalc = _camera.GetComponent<CameraEdgeReCalculator>();
                if(_scaleCalculator != null)
                    cameraScaleCalculators.Add(_scaleCalculator);
                if(_edgeCalc != null)
                    cameraEdgeReCalculators.Add(_edgeCalc);
            }
        }
        
        private void RenderFactoryEventsOnFactoryEnded(object aSender, EventArgs aE)
        {
            RenderFactoryEvents.FactoryEnded -= RenderFactoryEventsOnFactoryEnded;
            ResetCalculator();
        }

        private void ResetCalculator()
        {
            smAnimatorInfo = null;
            settings = null;
            animationFrameScalePackages = null;
            HasCalculatedFrames = false;
        }

        private void Reset()
        {
            cameraScaleCalculators = new List<CameraScaleCalculator>();
            var _boundsCalculators = GetComponentsInChildren<CameraScaleCalculator>();
            foreach (var _cameraScaleCalculator in _boundsCalculators)
            {
                cameraScaleCalculators.Add(_cameraScaleCalculator);
                cameraEdgeReCalculators.Add(_cameraScaleCalculator.GetComponent<CameraEdgeReCalculator>());
            }
        }

        #endregion

        #region Standard Usage

        //Startup for animated rendering
        public virtual void Startup(StopMotionAnimatorInfo aSmAnimatorInfo, bool aForceRecalculate)
        {
            FLogger.LogMessage(this, FLogger.Severity.Priority, "Entered animated startup.", nameof(RigScaleCalculator));
            settings = RenderingSettings.GetFor(transform);
            smAnimatorInfo = aSmAnimatorInfo;
            animationFrameScalePackages = new List<RigFrameScaleFile.AnimationCameraScales>();

            if(!aForceRecalculate)
                LoadFrameScaleAsset();

            //On successful load, signal we might not need to recalculate this
            if (animationFrameScalePackages != null && animationFrameScalePackages.Count > 0) 
                HasCalculatedFrames = true;
            
            RenderFactoryEvents.FactoryEnded += RenderFactoryEventsOnFactoryEnded;
        }

        private void LoadFrameScaleAsset()
        {
            FLogger.LogMessage(this, FLogger.Severity.Debug, "Entering frame scale load now");
            //Attempt to load our serializable object from the asset database
            var _file = FrameScaleFileManager.LoadFrameScaleFromAsset(RootFinder.FindHighestRoot(transform).name, name);
            if (_file == null)
                return;

            //Compare the loaded animation settings to the current
            if (!CompareSettings(_file))
            {
                FLogger.LogMessage(this, FLogger.Severity.Status, "Saved animation settings did not match loaded, invalidating save.");
                //Null the file if we have a different clamping now from when we saved it
                return;
            }

            //Attempt to set the scale list via the loaded file
            try
            {
                FLogger.LogMessage(this, FLogger.Severity.Status, $"Setting scales by index now.");
                animationFrameScalePackages = new List<RigFrameScaleFile.AnimationCameraScales>(_file.GetScalePackages());
                FLogger.LogMessage(this, FLogger.Severity.Status, "Camera Scales successfully loaded from existing file.");
            }
            catch (Exception _e)
            {
                FLogger.LogMessage(this, FLogger.Severity.Error, "Failed with loading scale file " + _file.name +
                                                               " due to internal corrupted data. File will be overwritten during scaling.");
                FLogger.LogMessage(this, FLogger.Severity.Error, _e.Message);
                animationFrameScalePackages = new List<RigFrameScaleFile.AnimationCameraScales>();
            }
        }

        private bool CompareSettings(RigFrameScaleFile aFile)
        {
            //Get the current value hashes
            int _currentRenderSettings = RenderingSettings.GetFor(transform).GetRenderingValueHash();
            int? _currentAdvancedSettings = AdvancedAnimationSettings.GetFor(transform)?.GetValueHash();
            int? _currentRootMotionSettings = RootMotionSettings.GetFor(transform)?.GetValueHash();
            
            //Compare to the file hashes
            return _currentRenderSettings == aFile.RenderingSettingsHash 
                && _currentAdvancedSettings.GetValueOrDefault() == aFile.AdvancedSettingsHash.GetValueOrDefault()
                && _currentRootMotionSettings.GetValueOrDefault() == aFile.RootMotionSettingsHash.GetValueOrDefault();
        }

        //Secondary startup, needed because of some dependency gridlocking
        public void LateStartup(BoundsCalculator aBoundsCalculator)
        {
            foreach (var _cameraScaleCalculator in cameraScaleCalculators)
                _cameraScaleCalculator.Startup(aBoundsCalculator);
        }

        public virtual IEnumerator CalculateFrame(int aCurrentAnimation, int aCurrentFrame)
        {
            FLogger.LogMessage(this, FLogger.Severity.Status, $"Calculating frame for {aCurrentAnimation}:{aCurrentFrame}", "Rig Scale Calculator");
            for (int _index = 0; _index < cameraScaleCalculators.Count; _index++)
            {
                //If we're not doing any calculations at all, set the current scale as our saved scale 
                if (!settings.EnableBoundsCalculator)
                {
                    SaveToScalePackage(aCurrentAnimation, aCurrentFrame, _index, cameraScaleCalculators[_index].CurrentScale);
                    continue;
                }
                
                //Calculate the frame
                float _scale = 1f;
                //We're always doing the bounding box calculation on the 0-frame to save some time
                if(aCurrentFrame == 0 || !settings.BypassBoundsCalculator)
                     _scale = cameraScaleCalculators[_index].CalculateScale();
               
                //Second pass: Calculate the camera depth if enabled
                if(settings.AutomaticDepthCalculation)
                    cameraScaleCalculators[_index].CalculateDepth();
                //We do this after the first pass so we get the correct initial position even if we're bypassing the standard calculator
                
                //If we should use the edge re-calculator, then run it and wait for it to finish
                if (settings.UseEdgeCalculator)
                {
                    var _routine =
                        this.StartSafeCoroutine<float>(cameraEdgeReCalculators[_index].PerformEdgeCalculation());

                    while (!_routine.HasFinished)
                        yield return null;

                    _scale = _routine.Result;
                }

                SaveToScalePackage(aCurrentAnimation, aCurrentFrame, _index, _scale);
            }
        }

        private void SaveToScalePackage(int aCurrentAnimation, int aFrameIndex, int aCameraIndex, float aScale)
        {
            //We can enter negative values to bypass writing the values, which allows us to run the calculators for static rendering with a lot less disruptions to this class
            if (aCurrentAnimation < 0 || aFrameIndex < 0)
                return;
                
            var _package = animationFrameScalePackages.FirstOrDefault(aScales => aScales.AnimationIndex == aCurrentAnimation && aScales.CameraIndex == aCameraIndex);
            if (_package == null)
            {
                _package = new RigFrameScaleFile.AnimationCameraScales(aCurrentAnimation, aCameraIndex);
                animationFrameScalePackages.Add(_package);
            }
            
            _package.AddFrame(aScale);
        }


        public virtual void CalculateCameraDepths()
        {
            foreach (var _scaleCalculator in cameraScaleCalculators)
            {
                _scaleCalculator.CalculateDepth();
            }
        }

        public void SetCameraDepths(float aDepth)
        {
            if (aDepth < 0)
                return;
            
            foreach (var _scaleCalculator in cameraScaleCalculators)
            {
                _scaleCalculator.SetDepth(aDepth);
            }
        }


        public virtual void SaveFrames()
        {
            FrameScaleFileManager.CreateFrameScaleAsset(RootFinder.FindHighestRoot(transform).name, name,
                animationFrameScalePackages, smAnimatorInfo.AnimationNames, transform);
        }


        internal virtual float GetScaleForFrame(int aAnimationIndex, int aFrameIndex, int aCameraIndex,
            out float aScaleDeviation)
        {
            try
            {
                float _toReturn = 1;
                
                //If we've provided a faulty value, pass back the cameras current deviated scale - this helps a lot with single frame rendering
                if (aAnimationIndex < 0 || aFrameIndex < 0)
                {
                    _toReturn = cameraScaleCalculators[aCameraIndex].CurrentScale;
                }
                //otherwise, load the value from the saved frame data
                else
                {
                    var _package = animationFrameScalePackages.FirstOrDefault(aScales => aScales.AnimationIndex == aAnimationIndex && aScales.CameraIndex == aCameraIndex);
                    var _packageValue = _package?.GetFrame(aFrameIndex);
                    _toReturn = _packageValue ?? cameraScaleCalculators[aCameraIndex].CurrentScale;
                    
                    if(_packageValue == null)
                        FLogger.LogMessage(this, FLogger.Severity.Error, $"Couldn't find loaded scale for Animation / Camera / Frame: {aAnimationIndex}:{aCameraIndex}:{aFrameIndex}");
                }
                aScaleDeviation = _toReturn / RenderingSettings.GetFor(transform).BaselineScale;
                return _toReturn;
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"GetScaleForFrame: Failed with accessing animation / frame / camera indexes of {aAnimationIndex}/{aFrameIndex}/{aCameraIndex}.");
                Console.WriteLine(e);
                aScaleDeviation = 1f;
                return 1f;
            }
        }
        

        public virtual void ResetCameraSizes()
        {
            foreach (var _cameraScaleCalculator in cameraScaleCalculators) _cameraScaleCalculator.ResetScale();
        }

        #endregion

        #region Scale Editor

        #endregion

        #region Static Rendering

        //Generic startup for static rendering

        public void Startup()
        {
            settings = RenderingSettings.GetFor(transform);
            //A static render can still be on an animated object, however that is only important to the rendering rig
            smAnimatorInfo = null;
            HasCalculatedFrames = true;
            RenderFactoryEvents.FactoryEnded += RenderFactoryEventsOnFactoryEnded;
        }

        #endregion

        #region Inspector

        [UsedImplicitly]
        public void SetCameraScales(float aScaleToSet)
        {
            if (aScaleToSet < 0) aScaleToSet = 0.01f;

            foreach (var _cameraScaleCalculator in cameraScaleCalculators) _cameraScaleCalculator.SetScale(aScaleToSet);
        }

        #endregion

        #endregion
    }
}