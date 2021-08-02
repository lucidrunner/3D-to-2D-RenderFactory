using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    public static class AnimatorEditorHelperMethods
    {
        public static AnimatorController GetController(Animator aAnimator)
        {
            if (aAnimator == null)
                return null;
            
            AnimatorController _controller = aAnimator.runtimeAnimatorController as AnimatorController;
            return _controller;
        }

        public static List<AnimationClip> GetClips(Animator aAnimator)
        {
            AnimatorController _controller = GetController(aAnimator);
            return _controller == null ? new List<AnimationClip>() : _controller.animationClips.ToList();
        }
        
        public static List<string> GetClipNames(Animator aAnimator)
        {
            AnimatorController _controller = GetController(aAnimator);
            return _controller == null
                ? new List<string>()
                : _controller.animationClips.Select(aClip => aClip.name).ToList();
        }

        public static AnimationClip GetClipByName(Animator aAnimator, string aName)
        {
            AnimatorController _controller = GetController(aAnimator);
            if (_controller == null)
                return null;

            return _controller.animationClips.FirstOrDefault(clip => clip.name == aName);
        }
    }
}