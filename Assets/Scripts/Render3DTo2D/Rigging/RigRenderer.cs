using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Rigging
{
    /// <summary>
    /// Responsible for providing the correct
    /// </summary>
    public class RigRenderer : MonoBehaviour
    {
        private RigScaleCalculator scaleCalculator;
        private List<CameraRenderer> cameraRenderers = new List<CameraRenderer>();
        protected Dictionary<CameraRenderer, bool> currentRenderStepState = new Dictionary<CameraRenderer, bool>();
        private string renderTag = "";

        protected Dictionary<int, int> RenderAnimationLengths = new Dictionary<int, int>();
        public Dictionary<int, int> LatestAnimationLengths => RenderAnimationLengths;        

        public IEnumerator RenderStep(CameraFrameRenderInfo.Builder aRenderInfoBuilder, Action aFinishedCallback)
        {
            //Start the rendering routines
            StartRenderStep(aRenderInfoBuilder);

            //Wait for the routines to finish
            while (currentRenderStepState.Values.Contains(false))
            {
                yield return null;
            }
            
            EndRenderStep();
            aFinishedCallback();
        }

        private void StartRenderStep(CameraFrameRenderInfo.Builder aRenderInfoBuilder)
        {
            currentRenderStepState = new Dictionary<CameraRenderer, bool>();
            //Do a partial build of the rendering info to access frame & animation index for the scale deviation process as well as the current partial path
            CameraFrameRenderInfo _cameraFrameRenderInfo = aRenderInfoBuilder.Build();
            RenderingSettings _settings = RenderingSettings.GetFor(transform);
            
            //Start by incrementing our rendered frame counter
            IncrementAnimationFramesCounter(_cameraFrameRenderInfo);
            
            
            //Set the name and tag
            string _modelName = RootFinder.FindHighestRoot(transform).name;
            aRenderInfoBuilder.SetName(!_cameraFrameRenderInfo.StaticRender
                ? $"{_modelName}"
                : $"{_modelName}_Static")
                .SetRigTag(renderTag);
            
            //Finally, before rendering we need to calculate the camera depth
            if(RenderingSettings.GetFor(transform).AutomaticDepthCalculation)
                scaleCalculator.CalculateCameraDepths();

            //Start a rendering routine for each camera renderer simultaneously
            for (int _index = 0; _index < cameraRenderers.Count; _index++)
            {
                //Set our camera index in the builder
                aRenderInfoBuilder.SetCameraNumber(_index);
                //Get our scale deviation for the current cameraFolder
                float _scaleDeviation = 1f;
                float _frameScale = 1;
                
                //Get the scale deviation & frame size
                //For a static render - or if we're not doing bounds calculation, get the standard scale deviation via calling an invalid animation / frame index
                if (_cameraFrameRenderInfo.StaticRender || !_settings.EnableBoundsCalculator)
                {
                    //For static we don't pass any indexes and just get the current size & deviation based on the ortho camera size
                    //We could do this on the camera itself but i'd rather keep the calculation for deviation in a singular place
                    _frameScale = scaleCalculator.GetScaleForFrame(-1,-1,_index, out _scaleDeviation);
                }
                else
                {
                    //For an animated sequence, get the scale & deviation for the current frame
                    _frameScale = scaleCalculator.GetScaleForFrame(_cameraFrameRenderInfo.AnimationNumber, _cameraFrameRenderInfo.FrameNumber,
                        _index, out _scaleDeviation);
                }

                //Setup and build our now full info package
                aRenderInfoBuilder.SetFrameScale(_frameScale);
                aRenderInfoBuilder.SetScaleDeviation(_scaleDeviation);
                //At this point the aRenderInfoBuilder can be safely used by the next rig
                _cameraFrameRenderInfo = aRenderInfoBuilder.Build();

                //Start and the routine for the current renderer
                StartRenderingRoutine(cameraRenderers[_index], _cameraFrameRenderInfo);
            }
        }

        protected virtual void IncrementAnimationFramesCounter(CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            if (aCameraFrameRenderInfo.StaticRender == false)
            {
                RenderAnimationLengths[aCameraFrameRenderInfo.AnimationNumber] =
                    aCameraFrameRenderInfo.FrameNumber + 1; //0-16 = 17 in length
            }
        }

        protected virtual void EndRenderStep()
        {
            foreach (CameraRenderer _cameraRenderer in cameraRenderers)
            {
                _cameraRenderer.ResetScaleAndTransform();
            }
            currentRenderStepState.Clear();
        }

        protected virtual void StartRenderingRoutine(CameraRenderer aCameraRenderer, CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            //If we're static this would have already been matched?
            aCameraRenderer.SetScaleAndTransform(aCameraFrameRenderInfo.FrameScale);
            
            currentRenderStepState[aCameraRenderer] = false;
            StartCoroutine(aCameraRenderer.RunRenderer(aCameraFrameRenderInfo, () => currentRenderStepState[aCameraRenderer] = true));
        }


        public void Startup(RigScaleCalculator aRigScaleCalculator, string aTag)
        {
            cameraRenderers = new List<CameraRenderer>();
            foreach (CameraRenderer _cameraRenderer in GetComponentsInChildren<CameraRenderer>())
            {
                cameraRenderers.Add(_cameraRenderer);
                _cameraRenderer.Startup();
            }

            RenderAnimationLengths = new Dictionary<int, int>();

            scaleCalculator = aRigScaleCalculator;
            renderTag = aTag;
        }

        public void RenderStartup(string aRigFolderName)
        {
            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Setting Output Path for {renderTag} to {aRigFolderName}");
            foreach (var _cameraRenderer in cameraRenderers)
            {
                _cameraRenderer.RenderStartup(aRigFolderName);
            }
        }
    }
}
