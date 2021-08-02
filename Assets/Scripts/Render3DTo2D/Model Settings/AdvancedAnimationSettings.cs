using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Logging;
using Render3DTo2D.Setup;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility.Inspector;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    public class AdvancedAnimationSettings : MonoBehaviour
    {
       

        #region Private Fields
        [SerializeField, HideInInspector]
        private Animator animator;


        public List<AnimationSetting> GetAnimationSettings()
        {
            var _toReturn = new List<AnimationSetting>();

            for (var _index = 0; _index < animationSettings.Count; _index++)
            {
                var _animationSetting = animationSettings[_index];
                if (_animationSetting.Clamping == ClampedAnimation.ClampedMode.None)
                {
                    _toReturn.Add(_animationSetting);
                    FLogger.LogMessage(this, FLogger.Severity.Debug, $"Adding standard animation now. Count is {_toReturn.Count}");
                    continue;
                }

                _toReturn.Add(new ClampedAnimation(_animationSetting, _index, RenderingSettings.GetFor(transform).AnimationFPS));
                FLogger.LogMessage(this, FLogger.Severity.Debug, $"Adding clamped animation now. Count is {_toReturn.Count}");
            }

            return _toReturn;
        }

        /// <summary>
        /// Creates a list of ClampedAnimation's that can be read to get the clamped mode, clamped frame length & time until the next frame
        /// </summary>
        /// <returns></returns>
        public List<ClampedAnimation> GetClampedAnimations()
        {
            var _toReturn = new List<ClampedAnimation>();

            for (int _index = 0; _index < animationSettings.Count; _index++)
            {
                AnimationSetting _animationSetting = animationSettings[_index];
                if (_animationSetting.Clamping == ClampedAnimation.ClampedMode.None)
                    continue;

                _toReturn.Add(new ClampedAnimation(_animationSetting, _index, RenderingSettings.GetFor(transform).AnimationFPS));
                FLogger.LogMessage(this, FLogger.Severity.Debug, $"Adding clamped animation now. Count is {_toReturn.Count}");
            }
            
            return _toReturn;
        }

        #endregion

        #region Inspector
        

        [SerializeField] 
        private List<AnimationSetting> animationSettings;

        #endregion
        

        #region Methods

        #region Private
        

        public void SetupSettingsList()
        {
            animationSettings = new List<AnimationSetting>();

            List<AnimationClip> _clips = AnimatorEditorHelperMethods.GetClips(animator);
            //Create the list and link it to this
            foreach (AnimationClip _clip in _clips)
            {
                //create the animation setting & add it to the list
                animationSettings.Add(new AnimationSetting(this, _clip));
            }
        }

        #endregion

        #region Public

        public static AdvancedAnimationSettings GetFor(Transform aHierarchyTransform)
        {
            return RootFinder.FindHighestRoot(aHierarchyTransform).GetComponent<AdvancedAnimationSettings>();
        }

        private void Reset()
        {
            Setup(ModelInfo.GetFor(transform).Animator);
        }

        /// <summary>
        /// Links the animator & copies the setup values from the global settings depository
        /// </summary>
        public void Setup(Animator aAnimator)
        {
            animator = aAnimator;
            SetupSettingsList();
        }
        
        public AnimationSetting GetSettingsForAnimation(int aAnimationIndex)
        {
            //Return either the indexed animation or null if it's out of bounds
            return aAnimationIndex < animationSettings.Count ? animationSettings[aAnimationIndex] : null;
        }

        public AnimationSetting GetSettingsForAnimation(string aAnimationName)
        {
            return animationSettings.FirstOrDefault(aSetting =>
                aSetting.AnimationName.ToLower().Equals(aAnimationName.ToLower()));
        }

        internal int GetValueHash()
        {
            unchecked
            {
                int _hashCode = 500;
                if (animationSettings == null) return _hashCode;
                return animationSettings.Aggregate(_hashCode, (aCurrent, aAnimationSetting) => (aCurrent * 397) ^ aAnimationSetting.GetValueHash());
            }
            
        }

        private void OnValidate()
        {
            foreach (var _animationSetting in animationSettings)
            {
                _animationSetting.Refresh();
            }
        }

        #endregion

        #endregion

    }
}