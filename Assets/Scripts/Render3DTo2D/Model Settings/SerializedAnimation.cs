using System;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    [Serializable]
    public class SerializedAnimation
    {
        [SerializeField, HideInInspector] protected string animationName;

        [SerializeField, HideInInspector] protected AnimationClip animationClip;

        protected SerializedAnimation(AnimationClip aAnimationClip)
        {
            animationClip = aAnimationClip;
            animationName = aAnimationClip.name;
        }


        public string AnimationName => animationName;
        public AnimationClip Clip => animationClip;

        public virtual int GetValueHash()
        {
            unchecked
            {
                int _hashCode = animationName?.GetHashCode() ?? 0;
                _hashCode = (_hashCode * 397) ^ (animationClip != null ? animationClip.GetHashCode() : 0);
                return _hashCode;
            }
        }
    }
}