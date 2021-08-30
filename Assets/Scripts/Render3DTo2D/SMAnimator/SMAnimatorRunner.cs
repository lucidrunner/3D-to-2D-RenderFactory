using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace Render3DTo2D.SMAnimator
{
    public class SMAnimatorRunner
    {
        public FrameArgs CurrentFrameInfo => new FrameArgs(CurrentFrame, CurrentAnimationIndex, LastStepTime, smAnimator.CurrentRealTime());
        
        public int CurrentFrame => smAnimator.CurrentFrame;
        public int CurrentAnimationIndex { get; private set; } = 0;
        public bool OnSubStep => currentSubStep > 0;

        public float LastStepTime => lastStepTime >= 0 ? lastStepTime : standardStepTime;

        private StopMotionAnimator smAnimator;
        private StopMotionAnimatorInfo smAnimatorInfo;
        private AdvancedAnimationSettings advancedAnimationSettings;
        private RootMotionSettings rootMotionSettings;
        private int currentAnimationFrameCount;
        private int subSteps = 1;
        private float standardStepTime = 1f;
        private float lastStepTime = 0;
        private int currentSubStep = 0;
        private ClampedAnimation currentClampedAnimation;

        public SMAnimatorRunner(StopMotionAnimator aSMAnimator)
        {
            smAnimator = aSMAnimator;
            smAnimatorInfo = smAnimator.GetAnimatorInfo();
            

            AdvancedAnimationSetup();
        }

        private void AdvancedAnimationSetup()
        {
            //Get Advanced animation settings
            advancedAnimationSettings = AdvancedAnimationSettings.GetFor(smAnimator.transform);

            var _renderingSettings = RenderingSettings.GetFor(smAnimator.transform);
            standardStepTime = (float)AnimationUtilities.GetStandardStepLength(_renderingSettings.AnimationFPS);
                
            //Check if we should use substeps
            var _rootSettings = RootMotionSettings.GetFor(smAnimator.transform);
            if(_rootSettings != null && _rootSettings.EnableRootMotionExport)
                subSteps = _renderingSettings.RecordingsPerFrame;
        }



        public void Start()
        {
            Reset();
            NewAnimationCheck();
        }


        //Steps through all animations until the last animation is reached

        public bool Step(bool aUseSubSteps = false)
        {
            float _stepOverride = -1f;
            if (currentClampedAnimation != null)
                _stepOverride = currentClampedAnimation.GetTimeToNextFrame(smAnimator.CurrentFrame);

            //If we're performing root motion substeps, perform some current substep tracking
            if (subSteps > 1 && aUseSubSteps)
            {
                currentSubStep++;

                if (currentSubStep >= subSteps)
                {
                    currentSubStep = 0;
                }
                
                
                //If we're not clamping, set the standard step time as our override
                if (_stepOverride < 0)
                    _stepOverride = standardStepTime;

                //split it based on the # of substeps we're at
                _stepOverride /= subSteps;
                
            }

            //Save our last step time so we can access it if needed
            lastStepTime = _stepOverride;
            
            //Run the next step with our possibly overriden step size, and tell the animator not to increment its frame counter 
            smAnimator.PlayStep(aOverrideStepSize: _stepOverride, !OnSubStep);
            
            

            //Keep stepping if we're supposed to for the current animation
            if (smAnimator.CurrentFrame < currentAnimationFrameCount) return true;

            //Otherwise increment animation
            return IncrementAnimation();
        }

        private bool IncrementAnimation()
        {
            //TODO In the future, this is what we need to change to be able to run specific animations (that is: load from a list of animations here instead)
            //Increment the animation index
            CurrentAnimationIndex++;
            
            //If the current index is out of bounds in the animator
            if (smAnimator.SetAnimationByIndex(CurrentAnimationIndex) == false)
            {
                //Reset and return that we can't step any more
                CurrentAnimationIndex = 0;
                smAnimator.ResetAnimator();
                return false;
            }
            
            //Otherwise, do our new animation check to load the frame count & possibly clamped animation
            NewAnimationCheck();
            
            return true;
        }

        private void NewAnimationCheck()
        {
            //Reset our last step time
            lastStepTime = 0f;
            currentSubStep = 0;
            
            //Get the frame count & the clamped animation module if the animation is in our clamping list
            currentAnimationFrameCount = smAnimator.GetAnimationFrameCount();
            currentClampedAnimation = smAnimatorInfo.GetClampedAnimation(smAnimator.CurrentlyPlayedAnimation);
            //Set the (possibly higher than usual) frame counter if we have a clamped animation
            if (currentClampedAnimation != null)
            {
                currentAnimationFrameCount = currentClampedAnimation.ClampedFrameCount;
            }
        }

        public void Reset()
        {
            smAnimator.ResetAnimator();
            CurrentAnimationIndex = 0;
        }

        public bool ShouldSkipFrame()
        {
            //If we're currently clamping, call the clamped animation to see if it should skip the frame
            if (currentClampedAnimation != null)
            {
                return currentClampedAnimation.ShouldSkipFrame(CurrentFrame);
            }

            //Otherwise, see if there's advanced animation settings set up at all
            if (advancedAnimationSettings == null) return false;
            
            //And if there is, check if the current animation should skip  the current frame
            AnimationSetting _animationSetting = advancedAnimationSettings.GetSettingsForAnimation(CurrentAnimationIndex);
            return _animationSetting != null && _animationSetting.ShouldSkipFrame(CurrentFrame);
        }
    }

    public class FrameArgs : EventArgs
    {
        public int CurrentAnimationIndex { get; }
        public int CurrentFrame { get; }
        public float LastStepTime { get; }
        public float FrameRealTime { get; }

        public FrameArgs(int aCurrentFrame, int aCurrentAnimationIndex, float aLastStepTime, float aCurrentFrameTime)
        {
            CurrentFrame = aCurrentFrame;
            CurrentAnimationIndex = aCurrentAnimationIndex;
            LastStepTime = aLastStepTime;
            FrameRealTime = aCurrentFrameTime;
        }
    }
}
