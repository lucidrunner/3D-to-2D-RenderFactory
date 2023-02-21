using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility.Extensions;
using UnityEngine;

namespace Render3DTo2D.SMAnimator
{
    [Serializable]
    public class StopMotionAnimatorInfo
    {
        #region Public Fields

        public int FramesPerSecond { get; private set; }

        public List<AnimationClip> Clips { get; }

        public List<float> AnimationLengths => Clips.Select(aClip => aClip.length).ToList();

        public List<string> AnimationNames => Clips.Select(aClip => aClip.name).ToList();
        
        public List<AnimationSetting> AnimationSettings { get; }

        public List<ClampedAnimation> ClampedAnimations { get; }


        public bool Valid { get; }

        #endregion Public Fields
        
        #region Methods
        
        public static StopMotionAnimatorInfo CreateFor(Transform aTransform)
        {
            ModelInfo _modelInfo = ModelInfo.GetFor(aTransform);

            if (!_modelInfo.CanAnimate)
                return new StopMotionAnimatorInfo();

            return new StopMotionAnimatorInfo(_modelInfo.Animator, aTransform);
        }
        
        #endregion

        #region Constructors

        private StopMotionAnimatorInfo()
        {
            Valid = false;
            FramesPerSecond = 30;
            Clips = new List<AnimationClip>();
            ClampedAnimations = new List<ClampedAnimation>();
        }

        private StopMotionAnimatorInfo(Animator aAnimator, Transform aHierarchyTransform)
        {
            Valid = true;
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(aHierarchyTransform);
            FramesPerSecond = _renderingSettings.AnimationFPS;
            
            //If we have an advanced animation settings present in the hierarchy, get them and package their clamped animation settings
            AdvancedAnimationSettings _advancedAnimationSettings = AdvancedAnimationSettings.GetFor(aHierarchyTransform);
            AnimationSettings = _advancedAnimationSettings != null ? _advancedAnimationSettings.GetAnimationSettings() : new List<AnimationSetting>();
            ClampedAnimations = _advancedAnimationSettings != null ? _advancedAnimationSettings.GetClampedAnimations() : new List<ClampedAnimation>();
            

            Clips = new List<AnimationClip>(aAnimator.GetClips());
        }

        /// <summary>
        /// Returns the ClampedAnimation info module of an animation if the animation should be clamped, otherwise returns null
        /// </summary>
        /// <param name="aAnimationName">The name of the animation</param>
        /// <returns>A ClampedAnimation matching from the ClampedAnimation list, null otherwise.</returns>
        public ClampedAnimation GetClampedAnimation(string aAnimationName)
        {
            return ClampedAnimations.FirstOrDefault(aClampedAnimation => string.Equals(aClampedAnimation.AnimationName,
                aAnimationName, StringComparison.OrdinalIgnoreCase));
        }


        #endregion Constructors
    }
}
