using System.Text;
using Render3DTo2D.Logging;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Utility
{
    public static class AnimationUtilities
    {
        public static double GetStandardStepLength(int aRenderingFPS)
        {
            return 1f / aRenderingFPS;
        }

        public static int GetAnimationFrameCount(AnimationClip aAnimationClip, int aAnimationFPS)
        {
            //Floor to int + 1 since we'll always have the 0f time frame
            return Mathf.FloorToInt(aAnimationFPS * aAnimationClip.length) + 1;
        }

        public static int GetAnimationMaxIndex(AnimationClip aAnimationClip, int aRenderingFPS)
        {
            //Count - 1 due indexing
            return GetAnimationFrameCount(aAnimationClip, aRenderingFPS) - 1;
        }

        public static double GetTimeToEnd(AnimationClip aAnimationClip, int aFrames, int aRenderingFPS)
        {
            return aAnimationClip.length - (aFrames * 1f / aRenderingFPS);
        }

        public static Transform GetAnimatorTransform(Transform aTransform)
        {
            if (aTransform == null)
                return null;
            
            var _root = RootFinder.FindHighestRoot(aTransform);
            return _root.GetComponent<Animator>() != null ? _root : _root.GetComponentInChildren<Animator>()?.transform;
        }

        public static bool HasRootMotion(AnimationClip aAnimationClip)
        {
            if (aAnimationClip == null) return false;
            return aAnimationClip.hasRootCurves;
        }
    }
}