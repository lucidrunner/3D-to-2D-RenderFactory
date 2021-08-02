using System;
using System.Collections.Generic;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility;

namespace Render3DTo2D.SMAnimator
{
    public class ClampedAnimation : AnimationSetting
    {

        #region  Properties

        public int AnimationIndex { get; }
        
        public int ClampedFrameCount { get; private set; }
        

        #endregion
        #region Private Fields

        private readonly Dictionary<int, float> overrideFrameLengths = new Dictionary<int, float>();

        #endregion
        
        #region Public Methods
        
        public ClampedAnimation(AnimationSetting aBaseSetting, int aAnimationIndex, int aFps) : base(aBaseSetting)
        {
            AnimationIndex = aAnimationIndex;
            SetClampedLength(aFps);
        }

        /// <summary>
        /// Returns a time larger than 0 if the render step time to the next frame is overriden, otherwise returns -1f
        /// </summary>
        /// <returns>
        /// Returns the time between aFrameIndex and the next frame if an overriden value has been added in the current clamp settings
        /// </returns>
        /// <param name="aFrameIndex">The index of the current frame in the animation</param>
        public float GetTimeToNextFrame(int aFrameIndex)
        {
            if (overrideFrameLengths.ContainsKey(aFrameIndex))
                return overrideFrameLengths[aFrameIndex];

            return -1f;
        }

        public override bool ShouldSkipFrame(int aCurrentFrame)
        {
            return  isLooping && ignoreLastFrame && aCurrentFrame == ClampedFrameCount - 1;
        }

        #endregion

        #region Private Methods

        private void SetClampedLength(int aFps)
        {
            //Floor to int + 1 since we'll always have the 0f time frame
            int _baseFrameLength = AnimationUtilities.GetAnimationFrameCount(Clip, aFps);
            //Set a default clamped frame count
            ClampedFrameCount = _baseFrameLength;
            
            switch(Clamping)
            {
                case ClampedMode.InsertEndFrame:
                    ForceLastFrame(_baseFrameLength, this , aFps);
                    break;
                case ClampedMode.StretchExistingFrames:
                    SmoothFrames(_baseFrameLength, this, aFps);
                    break;
                case ClampedMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void ForceLastFrame(int aBaseFrameCount, AnimationSetting aAnimationSetting, int aFps)
        {
            float _baseStepLength = 1f / aFps;
            //-1 since frames are at step length * frame index (-1 from frame count) and we're inspecting the current last frame
            float _timeAtLastIndex = (aBaseFrameCount - 1) * _baseStepLength;
            float _timeBetweenLastAndClamp = aAnimationSetting.Clip.length - _timeAtLastIndex;

            //if the current last frame should be removed then
            //replace the last frame with the clamping, add an overriden step time for the original second to last frame,
            //Animation frame count remains the same
            if (aAnimationSetting.InsertRemoveLastFrame)
            {
                float _timeAtSecondToLastIndex = (aBaseFrameCount - 2) * _baseStepLength;
                float _timeBetweenSecondLastAndClamp = aAnimationSetting.Clip.length - _timeAtSecondToLastIndex;
                overrideFrameLengths[aBaseFrameCount - 2] = _timeBetweenSecondLastAndClamp;
            }
            //Otherwise, append the clamped frame, add the time left as an overriden step time for the previously last frame
            //Animation frame count grows by 1
            else
            {
                ClampedFrameCount = aBaseFrameCount + 1;
                overrideFrameLengths[aBaseFrameCount - 1] = _timeBetweenLastAndClamp;
            }

            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Setup clamped animation with length {ClampedFrameCount}");
        }

        private void SmoothFrames(int aBaseFrameCount, AnimationSetting aAnimationSetting,
            int aFps)
        {
            //Edge case that we really shouldn't bother with
            if (aBaseFrameCount == 1)
            {
                ClampedFrameCount = 2;
                overrideFrameLengths[0] = 1f;
                return;
            }
            
            //Get the base step length
            float _baseStepLength = 1f / aFps;
            //Set the max index
            int _frameMaxIndex = aBaseFrameCount - 1;
            //Look at what index we should start smoothing
            int _smoothStartIndex = aAnimationSetting.StretchStartFrameIndex;
            //Calculate the time at our smooth start & the time left at that point
            float _smoothStartTime = _baseStepLength * _smoothStartIndex;
            float _timeLeftAtSmoothStart = aAnimationSetting.Clip.length - _smoothStartTime;
            
            //Split that into steps based on how many indexes will be moved
            float _smoothedStepTime = _timeLeftAtSmoothStart / (_frameMaxIndex - aAnimationSetting.StretchStartFrameIndex);
            
            //For all indexes starting at the smooth start and ending before the final index, change their step time
            for (int _index = _smoothStartIndex; _index <= _frameMaxIndex; _index++)
            {
                overrideFrameLengths[_index] = _smoothedStepTime;
            }
        }

        #endregion

        #region Enum

        public enum ClampedMode
        {
            None, InsertEndFrame, StretchExistingFrames
        }

        #endregion
    }
}