using System.Collections.Generic;
using System.Linq;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Utility
{
    //TODO Make these into extensions instead
    public static class AnimatorHelperMethods
    {
        public static IEnumerable<string> GetAnimationNames(Animator aAnimator)
        {
            return aAnimator.runtimeAnimatorController.animationClips.Select(aClip => aClip.name).ToArray();
        }

        public static IEnumerable<AnimationClip> GetClips(Animator aAnimator)
        {
            return aAnimator.runtimeAnimatorController.animationClips;
        }

        public static IEnumerable<float> GetAnimationLengths(Animator aAnimator)
        {
            return aAnimator.runtimeAnimatorController.animationClips.Select(aClip => aClip.length).ToArray();
        }
    }
}