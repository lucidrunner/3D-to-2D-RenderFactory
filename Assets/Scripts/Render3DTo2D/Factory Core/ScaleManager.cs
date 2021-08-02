using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edelweiss.Coroutine;
using Render3DTo2D.Isometric;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.Setup;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using RootChecker;
using Unity.Collections;
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

        public virtual IEnumerator<int> CalculateScales(bool aRecalculateValid = false)
        {
            yield return CoroutineResultCodes.Working;
            List<RigScaleCalculator> _activeCalculators = GetCalculatorsForScaling(aRecalculateValid);

            //If we have nothing to recalculate, check if it's because of setup or not and end the iteration
            if(_activeCalculators.Count == 0)
            {
                yield return scaleCalculators.Count > 0 ? CoroutineResultCodes.Bypassed : CoroutineResultCodes.Failed;
                yield break;
            }
            
            PrintStartMessage(_activeCalculators);

            //Setup our animation runner that will step through all the frames of all animations on the smAnimator
            SMAnimatorRunner _animatorRunner = new SMAnimatorRunner(smAnimator);
            _animatorRunner.Start();

            //For each frame, let each rig calculate its own scale
            do
            {
                yield return CoroutineResultCodes.Working;
                RenderFactoryEvents.InvokePreFrameCalculator(transform, _animatorRunner.CurrentFrameInfo);
                
                if(_animatorRunner.OnSubStep)
                    continue;

                List<SafeCoroutine> _routines = new List<SafeCoroutine>();
                foreach(RigScaleCalculator _rigCalculator in _activeCalculators)
                {
                    if (Settings.UseEdgeCalculator)
                    {
                        SafeCoroutine _routine = this.StartSafeCoroutine(_rigCalculator.CalculateFrame(_animatorRunner.CurrentAnimationIndex, _animatorRunner.CurrentFrame));
                        //Wait for the bounds + edge calculations to finish
                        while (!_routine.HasFinished)
                            yield return CoroutineResultCodes.Working;
                    }
                    else
                    {
                        _routines.Add(this.StartSafeCoroutine(_rigCalculator.CalculateFrame(_animatorRunner.CurrentAnimationIndex, _animatorRunner.CurrentFrame)));
                    }
                }
                
                //If we don't run edge recalculations we simply need to wait for all the rigs to finish their much shorter bounds calculation
                //However, since they rely on frame lagging the project (by holding the update hostage until they finish) this is never actually reached
                //I'll still keep it though so I get a warning if we're starting to get overflows rather than lag
                while (!_routines.TrueForAll(routine => routine.HasFinished))
                {
                    FLogger.LogMessage(this, FLogger.Severity.LinkageError, "Entered the 'Wait for Scale Calculators' thing that we should never enter. Has the implementation changed?", RootFinder.FindHighestRoot(transform).name);
                    yield return CoroutineResultCodes.Working;
                }
            } while(_animatorRunner.Step(true));

            //Then export & save the results
            ExportCalculators(aRecalculateValid);
            var _renderingSettings = RenderingSettings.GetFor(transform);
            if(!_renderingSettings.BypassBoundsCalculator)
                ResetCameraSizes();

            yield return CoroutineResultCodes.Passed;
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

            _stringBuilder.Remove(_stringBuilder.Length - 2, 2);
            FLogger.LogMessage(this, FLogger.Severity.Status, _stringBuilder.ToString(), RootFinder.FindHighestRoot(transform).gameObject.name);
        }
        #endregion
    }
}
