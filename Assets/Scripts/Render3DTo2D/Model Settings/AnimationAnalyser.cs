
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Render3DTo2D.Utility;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    public static class AnimationAnalyser
    {
        public static bool ShowClamping(AnimationClip aAnimationClip, int aRenderingFPS, int aDecimalTolerance = 4)
        {
            //We clamp if the distance to the end from the last frame isn't approximately 0
            int _frames = Mathf.FloorToInt(aAnimationClip.length * aRenderingFPS);
            //Check the distance between the actual end of animation and the last index
            double _distanceToEnd = aAnimationClip.length - (_frames * 1f / aRenderingFPS);
            double _round = Math.Round(_distanceToEnd, aDecimalTolerance);
            return _round % 1 > double.Epsilon * 10; //If the rounded diff is large enough, we should show clamping
        }
        

        public static string GetInsertFrameMessage(AnimationClip aAnimationClip, int aRenderingFPS, bool aInsertRemoveLastFrame, bool aLoopingIgnoreLastFrame)
        {
            //Calculate the max possible frame index
            int _frames = AnimationUtilities.GetAnimationMaxIndex(aAnimationClip, aRenderingFPS);
            //..and the standard length of a step
            double _standardStepLength = AnimationUtilities.GetStandardStepLength(aRenderingFPS);
            //Check the distance between the actual end of animation and the last index
            double _distanceToEnd = AnimationUtilities.GetTimeToEnd(aAnimationClip, _frames, aRenderingFPS);
            //Calculate the deviation between this step and a standard step
            double _stepDeviation = _distanceToEnd / _standardStepLength;
            double _stepDeviationPercentage = Math.Round(_stepDeviation * 100, 3);
            double _secondToLastDeviationPercentage = Math.Round((1 + _stepDeviation) * 100, 3);

            //Display the correct message depending on the current clamp percentage
            string _toReturn =
                !aInsertRemoveLastFrame
                    ? $"The time between the newly inserted frame and the old last frame is {_stepDeviationPercentage:N1}% ({_distanceToEnd:N4}s) of a standard step ({_standardStepLength:N4}s). The previous last frame will not be removed."
                    : $"The previous last frame will be removed. The last frame transition will cover {_secondToLastDeviationPercentage:N1}% ({(_standardStepLength + _standardStepLength * _stepDeviation):N4}s) of a standard step ({_standardStepLength:N4}s).";

            if (aLoopingIgnoreLastFrame)
            {
                _toReturn += "\nNote that due to the current loop settings the newly inserted last frame will be removed.";
            }
            
            return _toReturn;
        }

        public static double GetInsertNewLastFrameStep(AnimationClip aAnimationClip, int aRenderingFPS)
        {
            //Calculate the max possible frame index
            int _frames = AnimationUtilities.GetAnimationMaxIndex(aAnimationClip, aRenderingFPS);
            //..and the standard length of a step
            double _standardStepLength = AnimationUtilities.GetStandardStepLength(aRenderingFPS);
            //Check the distance between the actual end of animation and the last index
            double _distanceToEnd = AnimationUtilities.GetTimeToEnd(aAnimationClip, _frames, aRenderingFPS);
            //Calculate the deviation between this step and a standard step and return it
            double _stepDeviation = _distanceToEnd / _standardStepLength;

            return _stepDeviation;
        }

        public static string GetStretchMessage(AnimationClip aAnimationClip, int aRenderingFPS, int aSmoothStartFrameIndex)
        {
            //Calculate the max possible frame index
            int _frames = AnimationUtilities.GetAnimationMaxIndex(aAnimationClip, aRenderingFPS);
            //..and the standard length of a step
            double _standardStepLength = AnimationUtilities.GetStandardStepLength(aRenderingFPS);
            //Check the distance between the selected starting index of the smooth and the end of the animation
            double _distanceToEnd = AnimationUtilities.GetTimeToEnd(aAnimationClip, aSmoothStartFrameIndex, aRenderingFPS);
            //Calculate the needed length of each smoothed step to end up at the 1f on the last frame index
            double _smoothedStepLength = _distanceToEnd / (_frames - aSmoothStartFrameIndex);
            //Calculate the deviation between the new step and the standard step length
            double _smoothedStepDeviation = _smoothedStepLength / _standardStepLength;
            //And convert it to a more presentable % format
            double _deviationPercentage =  Math.Round(_smoothedStepDeviation * 100, 3);
            //Finally, return a message for the UI to display with the calculated information
            return $"Starting at index {aSmoothStartFrameIndex} gives {(_frames + 1) - aSmoothStartFrameIndex} frames to be rendered with a stretched step time of {Math.Round(_smoothedStepLength, 5)}, which is {_deviationPercentage:N2}% of the standard speed.";
        }
    }
}