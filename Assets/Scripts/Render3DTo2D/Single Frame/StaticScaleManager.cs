using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Rigging;
using Render3DTo2D.Utility;
using UnityEngine;

namespace Render3DTo2D.Single_Frame
{
    public class StaticScaleManager : ScaleManager
    {
        private StaticRenderManager renderManager;
        
        [SerializeField]
        private bool runCalculator = true;

        public bool RunCalculator => runCalculator;

        
        public override IEnumerator CalculateScales(Action<int> aCalculatorCallback ,bool aRecalculateValid = false)
        {
            yield return CalculatorState(aCalculatorCallback);
            List<RigScaleCalculator> _activeCalculators = GetCalculatorsForScaling(true);

            //If we have nothing to recalculate, check if it's because of setup or not and end the iteration
            if(_activeCalculators.Count == 0)
            {
                
                yield return CalculatorState(aCalculatorCallback, scaleCalculators.Count > 0 ? CoroutineResultCodes.Bypassed : CoroutineResultCodes.Failed);
                yield break;
            }
            
            if (!runCalculator)
            {
                yield return CalculatorState(aCalculatorCallback, CoroutineResultCodes.Passed);
                yield break;
            }

            
            //Go to our selected frame if necessary
            bool _finished = false;
            StartCoroutine(renderManager.GoToFrame(() => _finished = true));
            do
            {
                yield return CalculatorState(aCalculatorCallback);
            } while (!_finished);
            
            RenderFactoryEvents.InvokePreFrameCalculator(transform, null);
            
            //Then, run all the calculators
            yield return CoroutineResultCodes.Working;
            Dictionary<RigScaleCalculator, bool> _routineStates = new Dictionary<RigScaleCalculator, bool>();
            foreach (RigScaleCalculator _rigScaleCalculator in _activeCalculators)
            {
                if (Settings.UseEdgeCalculator)
                {
                    //Whilst we could piggyback onto the whole animated system with saved frame scale data this comes with its own sets of risks (since we're using different rigs & factories which might lead to incorrect syncing)
                    //Instead we're simply gonna force a recalculation for each camera and use the resulting camera size instantly to render
                    //This means a larger static setup can / will always take a bit longer to render than a single animated frame
                    bool _finishedEdgeCalc = false;
                    StartCoroutine(_rigScaleCalculator.CalculateFrame(-1, -1, () => _finishedEdgeCalc = true));
                    while (!_finishedEdgeCalc)
                        yield return CalculatorState(aCalculatorCallback);
                }
                else
                {
                    _routineStates[_rigScaleCalculator] = false;
                    StartCoroutine(_rigScaleCalculator.CalculateFrame(-1,-1, () => _routineStates[_rigScaleCalculator] = true));
                }

                //As in the normal scale manager, have this failsafe to show somethings wrong if we're not using the edge calculator and getting routines over several frames
                while (_routineStates.Values.Contains(false))
                {
                    Debug.Log("Entered the 'Wait for Scale Calculators' thing that we should never enter. Has the implementation changed?");
                    yield return CalculatorState(aCalculatorCallback);
                }
            }

            //We're not doing any exporting so just tell the factory to start rendering our frame
            yield return CalculatorState(aCalculatorCallback,  CoroutineResultCodes.Passed);
        }

        internal override void Startup(bool aRecalculateAll)
        {
            base.Startup(aRecalculateAll);
            renderManager = GetComponent<StaticRenderManager>();
        }
    }
}