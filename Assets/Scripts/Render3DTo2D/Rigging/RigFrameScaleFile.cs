using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Logging;
using UnityEngine;

namespace Render3DTo2D.Rigging
{
    public class RigFrameScaleFile: ScriptableObject
    {
        [Serializable]
        public class AnimationCameraScales
        {
            public AnimationCameraScales(int aAnimationIndex, int aCameraIndex)
            {
                animationIndex = aAnimationIndex;
                cameraIndex = aCameraIndex;
                frames = new List<float>();
            }

            public AnimationCameraScales(AnimationCameraScales aCopyScales)
            {
                animationIndex = aCopyScales.animationIndex;
                cameraIndex = aCopyScales.cameraIndex;
                frames = new List<float>(aCopyScales.frames);
            }
            
            [SerializeField]
            private int cameraIndex;
            [SerializeField]
            private int animationIndex;
            [SerializeField]
            private List<float> frames;

            public int CameraIndex => cameraIndex;
            public int AnimationIndex => animationIndex;

            public List<float> Frames => frames;

            public int FrameCount => frames?.Count ?? 0;

            public void AddFrame(float aFrameScaleValue)
            {
                frames.Add(aFrameScaleValue);
            }

            public void AddFrames(IEnumerable<float> aFrameScaleValues)
            {
                frames.AddRange(aFrameScaleValues);
            }

            public float? GetFrame(int aFrameIndex)
            {
                if (aFrameIndex < 0 || aFrameIndex >= frames.Count)
                    return null;
                return frames[aFrameIndex];
            }
        }
        

        [SerializeField]
        private List<string> animations;
        
        // For each animation, each camera's every frame is added a
        [SerializeField]
        private List<AnimationCameraScales> framePackages;



        internal int? AdvancedSettingsHash => hasAdvancedAnimationSettings ? advancedSettingsValueHash : (int?) null; 
        [SerializeField, HideInInspector] 
        private bool hasAdvancedAnimationSettings = false;
        [SerializeField, HideInInspector]   
        private int advancedSettingsValueHash;

        internal int? RootMotionSettingsHash => hasRootMotionSettings ? rootMotionSettingsValueHash : (int?) null;
        
        [SerializeField, HideInInspector] 
        private bool hasRootMotionSettings = false;
        [SerializeField, HideInInspector]   
        private int rootMotionSettingsValueHash;
        

        internal int RenderingSettingsHash => renderingSettingsValueHash;
        [SerializeField, HideInInspector] 
        private int renderingSettingsValueHash;

        public void Init(List<AnimationCameraScales> aFrameScales, IEnumerable<string> aAnimations, int aRenderingSettingsHash, int? aAdvancedSettingsHash, int? aRootMotionSettingsHash)
        {
            RecordFramePackages(aFrameScales);
            
            
            animations = new List<string>(aAnimations);

            renderingSettingsValueHash = aRenderingSettingsHash;
            
            hasAdvancedAnimationSettings = aAdvancedSettingsHash.HasValue;
            if (aAdvancedSettingsHash != null)
                advancedSettingsValueHash = aAdvancedSettingsHash.Value;

            hasRootMotionSettings = aRootMotionSettingsHash.HasValue;
            if (aRootMotionSettingsHash != null)
                rootMotionSettingsValueHash = aRootMotionSettingsHash.Value;
        }

        private void RecordFramePackages(List<AnimationCameraScales> aScalePackages)
        {
            framePackages = new List<AnimationCameraScales>();
            if (aScalePackages == null || aScalePackages.Count == 0)
            {
                FLogger.LogMessage(this, FLogger.Severity.Debug, "Empty or null Camera Scales passed to file, list is not set.");
                return;
            }
            
            //Add all to our file list via the copy constructor
            foreach (var _scalePackage in aScalePackages)
            {
                framePackages.Add(new AnimationCameraScales(_scalePackage));
            }
        }
        
        public List<string> GetAnimationNames()
        {
            if(animations == null || animations.Count == 0)
                return null;

            return new List<string>(animations);
        }

        public IEnumerable<AnimationCameraScales> GetScalePackages()
        {
            return framePackages.OrderBy(aScales => aScales.AnimationIndex).ThenBy(aScales => aScales.CameraIndex);
        }
    }
}
