
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.SMAnimator
{
    //For a more comprehensive SMAnimator then check the one in 3dTo2dTest - this is the stripped down version without any experimentation
    public class StopMotionAnimator : MonoBehaviour
    {
        #region Inspector
        
        private StopMotionAnimatorInfo smInfo = null;
        
        [SerializeField, HideInInspector]
        private Animator animator;

        #endregion
        
        #region Properties
        
        public string CurrentlyPlayedAnimation => GetCurrentlyPlayedAnimationName();

        public int CurrentCycle { get; private set; } = 0;

        public int CurrentFrame { get; private set; } = 0;
        
        public float CurrentRealTime() =>  GetCurrentClipLength() * GetCurrentNormalizedTime();

        private float AnimationStepLength
        {
            get
            {
                RenderingSettings _settings = GetRenderingSettings();
                float _toReturn = 1f / _settings.AnimationFPS;
                return _toReturn;
            }
        }

        private RenderingSettings GetRenderingSettings()
        {
            if(renderingSettings == null)
                renderingSettings = RenderingSettings.GetFor(transform);

            return renderingSettings;
        }

        #endregion

        #region Private Fields

        private string currentAnimation;

        private float currentAnimationLength = -1f;
        private RenderingSettings renderingSettings;

        #endregion

        #region Public Methods
        public StopMotionAnimatorInfo GetAnimatorInfo()
        {
            if(smInfo != null)
                return smInfo;
            
            smInfo = StopMotionAnimatorInfo.CreateFor(transform);
            return smInfo;
        }
        
        public void PlayAtTime(float aNormalizedTime = 0f, bool aResetOnOverride = false)
        {
            //if we have passed a valid time to play, then play that
            if(aNormalizedTime >= 0f)
            {
                float _currentClipLength = GetCurrentClipLength();
                //We still set the tracked real animation time here so we can keep stepping from it if we go over to a standard step
                float _currentAnimTime = aNormalizedTime * _currentClipLength;
                animator.Play(currentAnimation, -1, 0f);
                animator.Update(_currentAnimTime);
                
                //If we're supposed to, center the scene camera on the model
                if(RenderingSettings.GetFor(transform).FollowCameraOnRender)
                    GeneralUtilities.FocusSceneCamera(RootFinder.FindHighestRoot(transform).gameObject);

                CurrentFrame++;
                
                if(aResetOnOverride)
                {
                    CurrentCycle = 0;
                    CurrentFrame = 0;
                }
                else if (aNormalizedTime > 1f)
                {
                    CurrentCycle = Mathf.FloorToInt(aNormalizedTime);
                }
                
            }
        }

        public void PlayStep(float aOverrideStepSize= -1f, bool aIncrementFrame = true)
        {
            //Update either with our normal step time or the override time
            animator.Update(aOverrideStepSize > 0f ? aOverrideStepSize : AnimationStepLength);
            float _stepTime = aOverrideStepSize > 0f ? aOverrideStepSize : AnimationStepLength;
            
            
            //If we're supposed to, center the scene camera on the model
            if(RenderingSettings.GetFor(transform).FollowCameraOnRender)
                GeneralUtilities.FocusSceneCamera(RootFinder.FindHighestRoot(transform).gameObject);

            
            
            //Increment the frame counter if we're supposed to
            if(aIncrementFrame)
            {
                CurrentFrame++;
            }

            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Standard step performed. Current frame is {currentAnimation ?? ""}:{CurrentFrame} and time {GetCurrentNormalizedTime()}. Step time was {_stepTime}", "Stop Motion Animator");

            //If we've after the update has moved into another cycle then increment the counter
            //We could also do this as a Math floor of the GetCurrentNormalizedTime but I'd rather have full control of the cycle counter since it's checked externally
            if (GetCurrentNormalizedTime() > 1f - CurrentCycle)
            {
                CurrentCycle++;
            }
        }

        public void Startup(Animator aAnimator)
        {
            animator = aAnimator;

            if (animator == null)
            {
                return;
            }
            
            animator.enabled = false;
            GetAnimatorInfo();
        }

        public bool SetAnimation(string aAnimationToSet)
        {
            return GetAnimatorInfo().AnimationNames.Contains(aAnimationToSet) && SetAnimationByIndex(GetAnimatorInfo().AnimationNames.IndexOf(aAnimationToSet));
        }


        public bool SetAnimationByIndex(int aIndex)
        {
            if (aIndex >= GetAnimatorInfo().Clips.Count || aIndex < 0) return false;
            
            
            //Reset cycle, frame & get the new animation name
            CurrentCycle = 0;
            CurrentFrame = 0;
            currentAnimation = GetAnimatorInfo().AnimationNames[aIndex];
            
            //Call the pre-changed event just before we play the 0 time for the animation
            RenderFactoryEvents.InvokePreAnimationChanged(transform, new RenderFactoryEvents.AnimationChangedArgs(aIndex, currentAnimation));
            
            currentAnimationLength = -1;
            animator.Play(currentAnimation, -1, 0f);
            //move from the current animation to the set animation
            animator.Update(0);
            
            //If we're supposed to, center the scene camera on the model
            if(RenderingSettings.GetFor(transform).FollowCameraOnRender)
                GeneralUtilities.FocusSceneCamera(RootFinder.FindHighestRoot(transform).gameObject);
            
            //Call the animation changed event and pass the animation number
            RenderFactoryEvents.InvokeAnimationChanged(transform, new RenderFactoryEvents.AnimationChangedArgs(aIndex, currentAnimation));
            
            return true;
        }

        public void ResetAnimator()
        {
            SetAnimationByIndex(0);
        }

        #endregion

        #region Private Methods



        private float GetCurrentNormalizedTime()
        {
            //TODO We do our play layer agnostic so we should probably look up a way to save a base layer of what we're playing too?
            //TODO Research state layers and why you'd have them (guessing it's body parts blending which we don't support anyway)
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        private float GetCurrentClipLength()
        {
            if (currentAnimationLength > 0)
                return currentAnimationLength;
            
            string _animToPlay = GetCurrentlyPlayedAnimationName();

            //Prep and check the current clip
            AnimationClip _currentClip = GetClip(_animToPlay);
            if (_currentClip == null)
            {
                FLogger.LogMessage(this, FLogger.Severity.Error, $"{this.GetType()}: Missing clip for {_animToPlay} during animation length check.", "Stop Motion Animator");
                
                return 0;
            }

            currentAnimationLength = _currentClip.length;
            return currentAnimationLength;
        }

        private AnimationClip GetClip(string aClipName)
        {
            return GetAnimatorInfo().Clips.FirstOrDefault(aClip => aClip.name == aClipName);
        }

        private string GetCurrentlyPlayedAnimationName() => currentAnimation;
        
        #endregion

        public int GetAnimationFrameCount()
        {
            return Mathf.FloorToInt(GetCurrentClipLength() * GetAnimatorInfo().FramesPerSecond) + 1; //To account for the 0 frame
            //Previously I calculated this as CeilToInt but that does not account for the 0 frame in the cases where we had a perfectly aligned animation
        }
    }
}
