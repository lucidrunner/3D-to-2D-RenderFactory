using System;
using Render3DTo2D.Logging;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.SMAnimator;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Factory_Core
{
    public static class RenderFactoryEvents
    {
        #region Events
        /// <summary>
        /// Invoked when a factory is set to busy - ie it's starting a calculation or rendering run
        /// </summary>
        public static event EventHandler FactoryStarted;
        
        /// <summary>
        /// Invoked after a new animation / frame has been set on the animator but before the calculator has started calculating
        /// </summary>
        public static event EventHandler<FrameArgs> PreFrameCalculator;

        /// <summary>
        /// Invoked after a new animation / frame has been set on the animator but before the image has started rendering
        /// </summary>
        public static event EventHandler<FrameArgs> PreFrameRender;

        /// <summary>
        /// Invoked before any Calculator / Render has been performed and after any root motion recordings have been taken. Any model transform clamping should be applied on this.
        /// </summary>
        public static event EventHandler LatePreFrame;

        /// <summary>
        /// Invoked whenever the Model root motion has been forcibly reset. Any rig actions taken as a result of the model transform (Ie, if we follow the model) need to refresh on this.
        /// </summary>
        public static event EventHandler ModelTransformClamped;

        /// <summary>
        /// Invoked by the StopMotionAnimator just before the animation is changing
        /// </summary>
        public static event EventHandler<AnimationChangedArgs> PreAnimationChanged;

        /// <summary>
        /// Invoked by the StopMotionAnimator whenever the animation has changed
        /// </summary>
        public static event EventHandler<AnimationChangedArgs> AnimationChanged;

        /// <summary>
        /// Invoked when a factory is set to non-busy
        /// </summary>
        public static event EventHandler FactoryEnded;

        /// <summary>
        /// Invoked as a factory is ending to get the transform recorder to (possibly) export
        /// </summary>
        public static event EventHandler<ExportTransformArgs> TransformExport;

        #endregion

        #region Invokes
        internal static void InvokeFactoryStarted(Transform aFactoryTransform)
        {
            FactoryStarted?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), EventArgs.Empty);
        }

        internal static void InvokeFactoryEnded(Transform aFactoryTransform)
        {
            FactoryEnded?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), EventArgs.Empty);
        }

        internal static void InvokePreFrameCalculator(Transform aFactoryTransform, FrameArgs aCurrentFrameInfo)
        {
            PreFrameCalculator?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), aCurrentFrameInfo);
            InvokeLatePreFrame(aFactoryTransform);
        }

        internal static void InvokePreFrameRender(Transform aFactoryTransform, FrameArgs aCurrentFrameInfo)
        {
            PreFrameRender?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), aCurrentFrameInfo);
            InvokeLatePreFrame(aFactoryTransform);
        }

        internal static void InvokePreAnimationChanged(Transform aFactoryTransform, AnimationChangedArgs aArgs)
        {
            PreAnimationChanged?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), aArgs);
        }

        internal static void InvokeAnimationChanged(Transform aFactoryTransform, AnimationChangedArgs aArgs)
        {
            AnimationChanged?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), aArgs);
        }

        internal static void InvokeExportTransform(Transform aFactoryTransform, ExportTransformArgs aArgs)
        {
            TransformExport?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), aArgs);
        }

        public static void InvokeModelTransformClamped(Transform aModelTransform)
        {
            ModelTransformClamped?.Invoke(RootFinder.FindHighestRoot(aModelTransform), EventArgs.Empty);
        }

        private static void InvokeLatePreFrame(Transform aFactoryTransform)
        {
            LatePreFrame?.Invoke(RootFinder.FindHighestRoot(aFactoryTransform), EventArgs.Empty);
        }

        #endregion
        
        #region Custom Args
        public class AnimationChangedArgs
        {
            public int AnimationIndex { get; }
            public string AnimationName { get; }
            

            public AnimationChangedArgs(int aAnimationIndex, string aAnimationName)
            {
                AnimationIndex = aAnimationIndex;
                AnimationName = aAnimationName;
            }
        }
        
        public class ExportTransformArgs: EventArgs
        {
            public string OutputPath { get; }
            public DateTimeOffset TimeStamp { get; }
            
            public ExportTransformArgs(string aOutputPath, DateTimeOffset aRenderTimeStamp)
            {
                OutputPath = aOutputPath;
                TimeStamp = aRenderTimeStamp;
            }
        }
        #endregion

    }
}