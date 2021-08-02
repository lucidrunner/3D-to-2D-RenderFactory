using System;
using JetBrains.Annotations;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    [Serializable]
    public class AnimationSetting : SerializedAnimation
    {
        [SerializeField, HideInInspector] 
        int frameCount;
        
        public AdvancedAnimationSettings AnimationSettings => animationSettings;
        
        [SerializeField, HideInInspector] 
        private AdvancedAnimationSettings animationSettings;

        //Gnarly copy constructor, used when creating clamped animations
        public AnimationSetting(AnimationSetting aCopySetting) : base(aCopySetting.animationClip)
        {
            animationSettings = aCopySetting.animationSettings; 
            frameCount = aCopySetting.frameCount;
            showClamping = aCopySetting.showClamping;
            maxStretchFrames = aCopySetting.maxStretchFrames;
            insertRemoveLastFrame = aCopySetting.insertRemoveLastFrame;
            isLooping = aCopySetting.isLooping;
            ignoreLastFrame = aCopySetting.ignoreLastFrame;
            clampedMode = aCopySetting.clampedMode;
            stretchStartFrameIndex = aCopySetting.stretchStartFrameIndex;
            RefreshStretchMessage();
            RefreshInsertMessage();
        }

        /// <summary>
        /// Creates an new AnimationSetting based on the provided advanced animation setting
        /// </summary>
        public AnimationSetting([NotNull] AdvancedAnimationSettings aAdvancedAnimationSettings, AnimationClip aAnimationClip) : base(aAnimationClip)
        {
            animationSettings = aAdvancedAnimationSettings;
            isLooping = aAnimationClip.isLooping;
            
            //Get the rendering settings for the model we're attaching to
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(aAdvancedAnimationSettings.transform);
            frameCount = AnimationUtilities.GetAnimationFrameCount(aAnimationClip, _renderingSettings.AnimationFPS);
            //Calculate the max % of start frames we can offer during smooth clamping
            //We need to subtract 3 partly because we can't start smoothing on the second to last or last frame & indexes go 0->max index while the FloorToInt gives 1->x # of frames
            maxStretchFrames = frameCount - 3;
            if (maxStretchFrames < 1)
                maxStretchFrames = 1;
            //Get the default clamping percentage
            var _globalRenderingSettings = GlobalRenderingSettings.Instance;

            //Determine if we need to show clamping or not
            showClamping = AnimationAnalyser.ShowClamping(aAnimationClip, _renderingSettings.AnimationFPS, _globalRenderingSettings.ClampingDecimalTolerance);

            //If we offer clamping, setup the base messages and check if we should remove by default on Insert-clamping
            if (!showClamping) return;
            
            InsertRemoveLastFrameCheck(_renderingSettings.AnimationFPS);
            RefreshStretchMessage();
            RefreshInsertMessage();
        }

        private void InsertRemoveLastFrameCheck(int aRenderingSettingsAnimationFPS)
        {
            double _insertStepDeviation = AnimationAnalyser.GetInsertNewLastFrameStep(animationClip,aRenderingSettingsAnimationFPS);

            if (_insertStepDeviation * 100 >= GlobalRenderingSettings.Instance.DefaultInsertClampCutoff)
                insertRemoveLastFrame = true;
        }


        #region Inspector

        public bool IsLooping => isLooping;
        [SerializeField] protected bool isLooping;
        
        [SerializeField] protected bool ignoreLastFrame = true;
        
            
        #region Clamped Settings
        
        public ClampedAnimation.ClampedMode Clamping => clampedMode;
        [SerializeField] private ClampedAnimation.ClampedMode clampedMode = ClampedAnimation.ClampedMode.None;

        public bool InsertRemoveLastFrame => insertRemoveLastFrame;
        [SerializeField] private bool insertRemoveLastFrame = false;
        
        
        public int StretchStartFrameIndex => stretchStartFrameIndex;
        [SerializeField] private int stretchStartFrameIndex = 0;

        #endregion
        


        #region Conditionals



        [SerializeField, HideInInspector]
        private bool showClamping;

        [UsedImplicitly, SerializeField, HideInInspector]
        private int maxStretchFrames;


        [UsedImplicitly, SerializeField, HideInInspector] 
        private string stretchFramesInfoMessage = "";
        
        [UsedImplicitly, SerializeField, HideInInspector]
        private string clampedFramesInfoMessage = "";
        
        #endregion

        
        private void RefreshInsertMessage()
        {
            if (animationSettings == null || clampedMode != ClampedAnimation.ClampedMode.InsertEndFrame)
                return;
            
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(animationSettings.transform);
            clampedFramesInfoMessage =
                AnimationAnalyser.GetInsertFrameMessage(animationClip, _renderingSettings.AnimationFPS,
                    insertRemoveLastFrame, isLooping && ignoreLastFrame);
        }

        private void RefreshStretchMessage()
        {
            if (animationSettings == null || clampedMode != ClampedAnimation.ClampedMode.StretchExistingFrames)
                return;
            
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(animationSettings.transform);
            stretchFramesInfoMessage =
                AnimationAnalyser.GetStretchMessage(animationClip, _renderingSettings.AnimationFPS, stretchStartFrameIndex);
        }

        
        #endregion
        
        #region Methods

        public virtual bool ShouldSkipFrame(int aCurrentFrame)
        {
            return isLooping && ignoreLastFrame && aCurrentFrame == frameCount - 1;
        }

        public new virtual int GetValueHash()
        {
            unchecked
            {
                int _hashCode = base.GetValueHash();
                _hashCode = (_hashCode * 397) ^ frameCount;
                _hashCode = (_hashCode * 397) ^ isLooping.GetHashCode();
                _hashCode = (_hashCode * 397) ^ ignoreLastFrame.GetHashCode();
                _hashCode = (_hashCode * 397) ^ (int) clampedMode;
                _hashCode = (_hashCode * 397) ^ insertRemoveLastFrame.GetHashCode();
                _hashCode = (_hashCode * 397) ^ stretchStartFrameIndex;
                 _hashCode = (_hashCode * 397) ^ maxStretchFrames;
                return _hashCode;
            }
        }


        public void Refresh()
        {
            if (stretchStartFrameIndex > maxStretchFrames)
                stretchStartFrameIndex = maxStretchFrames;
            if (stretchStartFrameIndex < 0)
                stretchStartFrameIndex = 0;
            RefreshInsertMessage();
            RefreshStretchMessage();
        }

        #endregion
    }
}