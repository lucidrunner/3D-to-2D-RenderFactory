using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Render3DTo2D.Utility.Extensions
{
    public static class AnimatorExtensions
    {
        public static IEnumerable<string> GetAnimationNames(this Animator aAnimator)
        {
            return aAnimator.runtimeAnimatorController.animationClips.Select(aClip => aClip.name).ToArray();
        }
        
        public static IEnumerable<AnimationClip> GetClips(this Animator aAnimator)
        {
            return aAnimator.runtimeAnimatorController.animationClips;
        }

        public static IEnumerable<float> GetAnimationLengths(this Animator aAnimator)
        {
            return aAnimator.runtimeAnimatorController.animationClips.Select(aClip => aClip.length).ToArray();
        }
    }
}