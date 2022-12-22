using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Render3DTo2D.Isometric;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Factory_Core
{
    public class ScaleManager: MonoBehaviour
    {

        #region Inspector

        public int CalculatorCount => scaleCalculators?.Count ?? 0;
        
        [SerializeField]
        protected List<RigScaleCalculator> scaleCalculators = new List<RigScaleCalculator>();

        protected List<RigScaleCalculator> ActiveScaleCalculators = new List<RigScaleCalculator>();

        [SerializeField, HideInInspector]
        protected StopMotionAnimator smAnimator = null;

        [SerializeField, HideInInspector]
        protected BoundsCalculator boundsCalculator = null;
        
        #endregion

        #region Private Fields
        
        protected RenderingSettings Settings;

        #endregion



        #region Public Methods

        private void Reset()
        {
            scaleCalculators = new List<RigScaleCalculator>();
            var _calculators = GetComponentsInChildren<RigScaleCalculator>();
            if(_calculators != null)
                scaleCalculators.AddRange(_calculators);
        }

        internal virtual void Startup( bool aRecalculateAll)
        {
            Settings = RenderingSettings.GetFor(transform);
            smAnimator = GetComponent<StopMotionAnimator>();
            boundsCalculator = GetComponent<BoundsCalculator>();

            ActiveScaleCalculators = new List<RigScaleCalculator>();
            RigManager _rigManager = GetComponent<RigManager>();

            foreach(RigScaleCalculator _rigScaleCalculator in scaleCalculators)
            {
                if (_rigManager.IsRigActive(_rigScaleCalculator.GetComponent<CameraRig>()) || aRecalculateAll)
                {
                    ActiveScaleCalculators.Add(_rigScaleCalculator);
                    _rigScaleCalculator.LateStartup(boundsCalculator);
                }
            }
        }

        protected virtual IEnumerator CalculatorState(Action<int> aCallback, int aCode = CoroutineResultCodes.Working)
        {
            aCallback(aCode);
            yield return CoroutineResultCodes.Working;
        }

        
        
        public virtual IEnumerator CalculateScales(Action<int> aCalculatorCallback, bool aRecalculateValid = false)
        {
            
            yield return CalculatorState(aCalculatorCallback);
            //Get our toggled / all calculators based on our current settings
            List<RigScaleCalculator> _activeCalculators = GetCalculatorsForScaling(aRecalculateValid);
            
            
            //If we have nothing to recalculate, check if it's because of setup or not and end the iteration
            if(_activeCalculators.Count == 0)
            {
                yield return CalculatorState(aCalculatorCallback, scaleCalculators.Count > 0 ? CoroutineResultCodes.Bypassed : CoroutineResultCodes.Failed);
                yield break;
            }
            
            //Log our start message
            PrintStartMessage(_activeCalculators);
            
            //Setup our animation runner that will step through all the frames of all animations on the smAnimator
            SMAnimatorRunner _animatorRunner = new SMAnimatorRunner(smAnimator);
            _animatorRunner.Start();
            
            //For each frame, let each rig calculate its own scale
            do
            {
                yield return CalculatorState(aCalculatorCallback);
                
                RenderFactoryEvents.InvokePreFrameCalculator(transform, _animatorRunner.CurrentFrameInfo);
                
                //If our animation is setup in the substep mode, ignore the current frame for calculation
                //Subsets are only used for more precise positional data and will not be included in render & calculations
                if(_animatorRunner.OnSubStep)
                    continue;

                Dictionary<RigScaleCalculator, bool> _routineStates = new Dictionary<RigScaleCalculator, bool>();
                
                foreach(RigScaleCalculator _rigCalculator in _activeCalculators)
                {
                    if (Settings.UseEdgeCalculator)
                    {
                        bool _hasFinished = false;
                        StartCoroutine(_rigCalculator.CalculateFrame(_animatorRunner.CurrentAnimationIndex, _animatorRunner.CurrentFrame, () => _hasFinished = true));
                        
                        //Wait for the bounds + edge calculations to finish
                        while (!_hasFinished)
                            yield return CalculatorState(aCalculatorCallback);
                    }
                    else
                    {
                        _routineStates[_rigCalculator] = false;
                        StartCoroutine(_rigCalculator.CalculateFrame(_animatorRunner.CurrentAnimationIndex, _animatorRunner.CurrentFrame, () => _routineStates[_rigCalculator] = true));
                    }
                }
                
                //If we don't run edge recalculations we simply need to wait for all the rigs to finish their much shorter bounds calculation
                //However, since they rely on frame lagging the project (by holding the update hostage until they finish) this is never actually reached
                //I'll still keep it though so I get a warning if we're starting to get overflows rather than lag
                while (_routineStates.Values.Contains(false))
                {
                    FLogger.LogMessage(this, FLogger.Severity.LinkageError, "Entered the 'Wait for Scale Calculators' thing that we should never enter. Has the implementation changed?", RootFinder.FindHighestRoot(transform).name);
                    yield return CalculatorState(aCalculatorCallback);
                }
            } while(_animatorRunner.Step(true));
            
            
            //Then export & save the results
            ExportCalculators(aRecalculateValid);
            var _renderingSettings = RenderingSettings.GetFor(transform);
            if(!_renderingSettings.BypassBoundsCalculator)
                ResetCameraSizes();

            aCalculatorCallback(CoroutineResultCodes.Passed);
        }

        public void ResetCameraSizes()
        {
            foreach (RigScaleCalculator _rigScaleCalculator in scaleCalculators)
            {
                _rigScaleCalculator.ResetCameraSizes();
            }
        }


        public void AddRigCalculator(RigScaleCalculator aScaleCalculator)
        {

            if(scaleCalculators == null)
            {
                scaleCalculators = new List<RigScaleCalculator>();
            }

            scaleCalculators.Add(aScaleCalculator);
        }

        public void RemoveRigCalculator(GameObject aRigScaleCalculatorObject)
        {
            RigScaleCalculator _scaleCalculator = aRigScaleCalculatorObject.GetComponent<RigScaleCalculator>();
            if(scaleCalculators.Contains(_scaleCalculator))
            {
                scaleCalculators.Remove(_scaleCalculator);
            }
        }

        public void ValidateList()
        {
            for (int i = 0; i < scaleCalculators.Count; i++)
            {
                if (scaleCalculators[i] == null)
                {
                    scaleCalculators.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

        #region Private Methods

        private void ExportCalculators(bool aRecalculateValid)
        {
            foreach (RigScaleCalculator _rigCalculator in ActiveScaleCalculators)
            {
                if (aRecalculateValid || _rigCalculator.HasCalculatedFrames == false)
                {
                    _rigCalculator.SaveFrames();
                }
            }
        }

        protected List<RigScaleCalculator> GetCalculatorsForScaling(bool aRecalculateValid)
        {
            List<RigScaleCalculator> _activeCalculators = new List<RigScaleCalculator>();
            //Select all calculators if we're force recalculating
            if (aRecalculateValid)
            {
                //Force recalculate only on non-isometric rigs
                _activeCalculators.AddRange(ActiveScaleCalculators.Where(rigCalculator => !(rigCalculator is IsometricRigScaleCalculator)));
            }
            else //otherwise only recalculate those without loaded results
            {
                _activeCalculators.AddRange(ActiveScaleCalculators.Where(rigCalculator =>
                    rigCalculator.HasCalculatedFrames == false));
            }

            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Found {_activeCalculators.Count} calculators that need to be run.");

            return _activeCalculators;
        }
        
        private void PrintStartMessage(List<RigScaleCalculator> aActiveCalculators)
        {
            StringBuilder _stringBuilder = new StringBuilder($"Starting calculator on {RootFinder.FindHighestRoot(transform).gameObject.name} - {gameObject.name} for rigs ");
            
            foreach (RigScaleCalculator scaleCalculator in aActiveCalculators)
            {
                _stringBuilder.Append(scaleCalculator.gameObject.name + ", ");
            }

            //Remove the last ", " and log our full message
            _stringBuilder.Remove(_stringBuilder.Length - 2, 2);
            FLogger.LogMessage(this, FLogger.Severity.Status, _stringBuilder.ToString(), RootFinder.FindHighestRoot(transform).gameObject.name);
        }
        #endregion
    }
}
