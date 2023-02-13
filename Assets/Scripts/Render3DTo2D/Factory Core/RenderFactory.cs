﻿using System;
using Render3DTo2D.Logging;
using Render3DTo2D.Rigging;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.Single_Frame;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.IO;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Factory_Core
{
    public class RenderFactory : MonoBehaviour
    {
        
        #region Serialized Fields
        [SerializeField, HideInInspector]
        private int rigsAdded = 0;

        [SerializeField, HideInInspector] 
        private RigManager factoryRigManager = null;

        [SerializeField, HideInInspector]
        private ScaleManager factoryScaleManager = null;

        [SerializeField, HideInInspector]
        private StopMotionAnimator stopMotionAnimator = null;

        [SerializeField, HideInInspector] 
        private RenderManager factoryRenderManager = null;
        #endregion
        
        #region Private Fields
        private Coroutine calculatorCoroutine;
        private Coroutine renderingCoroutine;
        private DateTimeOffset renderTimeStamp;
        [SerializeField, HideInInspector] private bool busy = false;

        #endregion
        
        #region Properties

        public event EventHandler FinishedCallback;
        
        public int RigCount => factoryRigManager != null ? factoryRigManager.ToggledRigCount : 0;

        public bool Busy
        {
            get => busy;
            private set
            {
                busy = value;
                if (busy)
                {
                    RenderFactoryEvents.InvokeFactoryStarted(transform);
                }
                else
                {
                    RenderFactoryEvents.InvokeFactoryEnded(transform);
                    InvokeFinishedCallback();
                }
            }
        }

        private void InvokeFinishedCallback()
        {
            FinishedCallback?.Invoke(this, EventArgs.Empty);
        }


        public bool CreateFolders { get; private set; }

        #endregion
        
        #region Inspector
    
        public void ValidateRigs()
        {
            GetReferencedComponents();

            factoryRigManager.ValidateLists();
            factoryScaleManager.ValidateList();
            factoryRenderManager.ValidateList();
        }
        #endregion

        #region Public Methods
        
        public void RunRecalculation(Animator aAnimator, bool aRecalculateAll)
        {
            //Lock down the interface
            Busy = true;
            
            //Make sure we don't attempt to create render folders during this force-rerender
            CreateFolders = false;
            //Depending on our settings we want to force recalculate all or only the currently toggled active rigs
            Startup(aAnimator, true, aRecalculateAll);
            
            FLogger.LogMessage(this, FLogger.Severity.Priority, "Calculator starting.", RootFinder.FindHighestRoot(transform).gameObject.name);
            calculatorCoroutine = this.StartCoroutine(factoryScaleManager.CalculateScales(RecalculationCallback, true));
            
            //Local method for post-calculation cleanup
            void RecalculationCallback(int aState)
            {
                if (aState == CoroutineResultCodes.Working)
                {
                    return;
                }
        
                //Report finished & reset the factory
                FLogger.LogMessage(this, FLogger.Severity.Priority, "Calculator finished", RootFinder.FindHighestRoot(transform).gameObject.name);
                Busy = false;
                calculatorCoroutine = null;
            }

        }
        
        public void RunFactory(Animator aAnimator)
        {
            //Pre-render startup settings
            Busy = true;
            CreateFolders = true;
            Startup(aAnimator);
            
            //We run the calculator here, but it won't recalculate by default if all of our current rigs have valid data set
            FLogger.LogMessage(this, FLogger.Severity.Priority, "Calculator starting.", RootFinder.FindHighestRoot(transform).gameObject.name);
            calculatorCoroutine = StartCoroutine(factoryScaleManager.CalculateScales(CalculatorCallback));
            
            //Setup so we can run the rendering via callback after the calculator finishes
            void CalculatorCallback(int aState)
            {
                // FLogger.LogMessage(this, FLogger.Severity.Debug, "Entered calculator callback with state " + CoroutineResultCodes.AsString(aState));
                //Wait for the calculation to actually finish before proceeding
                if (aState == CoroutineResultCodes.Working)
                {
                    return;
                }

                //Based on the finished working state, log & continue or abort
                switch (aState)
                {
                    case CoroutineResultCodes.Passed:
                        FLogger.LogMessage(this, FLogger.Severity.Status, "Calculator finished with creating new frames.", RootFinder.FindHighestRoot(transform).gameObject.name);
                        break;
                    case CoroutineResultCodes.Bypassed:
                        FLogger.LogMessage(this, FLogger.Severity.Status, "Calculator bypassed - Frames were either found & loaded or all toggled rigs are isometric.", RootFinder.FindHighestRoot(transform).gameObject.name);
                        break;
                    case CoroutineResultCodes.Failed:
                        FLogger.LogMessage(this, FLogger.Severity.FatalError, "No existing scales or scale calculators for rigs found so rendering can't be run. Is the setup prefab broken?", RootFinder.FindHighestRoot(transform).gameObject.name);
                        return;
                }
                
                FLogger.LogMessage(this, FLogger.Severity.Priority, "Rendering starting.", "Factory" );

                RenderStartup();

                //Start the rendering routine and another listener so we get a callback for when rendering is done
                renderingCoroutine = StartCoroutine(factoryRenderManager.RenderAllActiveRigs(OnRenderingFinished));
                
                //Clear the routine for future use
                calculatorCoroutine = null;
            }

        }

    public string GetRenderTimestamp(bool aFormat = true)
    {
        //Return the time stamp as a printable format
        return aFormat ? renderTimeStamp.ToString("yyyy-M-dd HH-mm-ss") : renderTimeStamp.ToString();
    }

    #endregion

    #region Internal Methods

    internal void AddCameraRig(CameraRig aRigToAdd)
        {
            GetReferencedComponents();

            //Give the rig a guaranteed unique name by appending the static _rigsAdded
            rigsAdded++;
            aRigToAdd.SetupName(rigsAdded);
            factoryRigManager.AddRig(aRigToAdd);
            factoryScaleManager.AddRigCalculator(aRigToAdd.GetComponent<RigScaleCalculator>());
            factoryRenderManager.AddRigRenderer(aRigToAdd.GetComponent<RigRenderer>());
        }

    #endregion

    #region Private Methods

    private void Startup(Animator aAnimator, bool aForceRecalculate = false, bool aRecalculateAll = false)
        {
            //Reset our routines
            calculatorCoroutine = null;
            renderingCoroutine = null;

            //Get our internal references if needed
            GetReferencedComponents();

            //Save the current timestamp
            renderTimeStamp = DateTimeOffset.Now;

            //Propagate the startup down the hierarchy
            if(aAnimator != null)
                stopMotionAnimator.Startup(aAnimator);
            factoryRenderManager.Startup(aRecalculateAll);
            factoryRigManager.Startup(stopMotionAnimator, aForceRecalculate, aRecalculateAll);
            //TODO Fix this startup sequence since this must be run after the rig manager to avoid possible bad data
            factoryScaleManager.Startup(aRecalculateAll);
        }

    private void RenderStartup()
    {
        factoryRigManager.RenderStartup(stopMotionAnimator);
    }


    private void OnRenderingFinished()
    {
        //If we aren't rendering and have entered this somehow, return
        if (renderingCoroutine == null) return;
        
        //Report finished an reset the factory
        FLogger.LogMessage(this, FLogger.Severity.Priority, $"Rendering Finished", RootFinder.FindHighestRoot(transform).gameObject.name);
            
        bool? _applyRootMotion = RootMotionSettings.GetFor(transform)?.EnableRootMotionExport;
        string _rootMotionFilePath = null;
        if (_applyRootMotion.HasValue && _applyRootMotion.Value == true)
        {
            //Export the Root Data to an XML file
            string _outputPath = FactoryFolderCreator.CreateTransformExportFolderForFactory(transform);
            var _exportArgs = new RenderFactoryEvents.ExportTransformArgs(_outputPath, renderTimeStamp);
            RenderFactoryEvents.InvokeExportTransform(transform, _exportArgs);
            _rootMotionFilePath = _exportArgs.FullFilePath;
        }
            
        Busy = false;
        renderingCoroutine = null;

        //TODO Add the XML / JSON check here
        //Export the animation data to XML if we're a non-static factory
        if (GetComponent<StaticRenderManager>() == null)
            factoryRigManager.ExportToXML(stopMotionAnimator, _rootMotionFilePath);
        else
            factoryRigManager.ExportToXML(_rootMotionFilePath);

        //Finally, reset the camera sizes 
        factoryScaleManager.ResetCameraSizes();
    }

    private void GetReferencedComponents()
    {
        if(factoryScaleManager == null)
            factoryScaleManager = GetComponent<ScaleManager>();
        if(factoryRigManager == null)
            factoryRigManager = GetComponent<RigManager>();
        if(factoryRenderManager == null)
            factoryRenderManager = GetComponent<RenderManager>();
        if(stopMotionAnimator == null)
            stopMotionAnimator = GetComponent<StopMotionAnimator>();
    }

    private void Reset()
    {
        GetReferencedComponents();
    }

    #endregion

    }
}
