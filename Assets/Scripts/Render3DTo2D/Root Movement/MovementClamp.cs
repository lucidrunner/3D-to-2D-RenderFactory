using System;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Root_Movement
{
    public class MovementClamp : MonoBehaviour
    {
        private Transform targetTransform;
        private StoredTransform originalTransform;
        private RootMotionSettings motionSettings;
        private RootMotionSetting currentAnimationSetting;
        private bool forceClamp = false;

        private void Start()
        {
            targetTransform = transform;
            RenderFactoryEvents.FactoryStarted += RenderFactoryEventsOnFactoryStarted;
            RenderFactoryEvents.FactoryEnded += RenderFactoryEventsOnFactoryEnded;
        }

        private void RenderFactoryEventsOnFactoryEnded(object aSender, EventArgs aE)
        {
            if (!GeneralUtilities.CompareSenderToModelRoot(aSender, transform))
            {
                return;
            }

            forceClamp = false;
            motionSettings = null;
            RenderFactoryEvents.AnimationChanged -= RenderFactoryEventsOnAnimationChanged;
            RenderFactoryEvents.LatePreFrame -= RenderFactoryEvents_OnPreFrame;
        }

        private void RenderFactoryEventsOnFactoryStarted(object aSender, EventArgs aE)
        {
            if (!GeneralUtilities.CompareSenderToModelRoot(aSender, transform))
            {
                return;
            }

            originalTransform = new StoredTransform(targetTransform);
            
            
            motionSettings = RootMotionSettings.GetFor(transform);
            forceClamp = motionSettings != null && (motionSettings.ClampMovement || motionSettings.ClampRotation);
            if (forceClamp)
            {
                ModelInfo.GetFor(transform).Animator.applyRootMotion = true;
            }
            
            //If we have root motion settings, or if we should forcibly clamp, we need to check if we're clamping on each animation
            if(motionSettings != null || forceClamp)
                RenderFactoryEvents.AnimationChanged += RenderFactoryEventsOnAnimationChanged;
        }

        private void RenderFactoryEventsOnAnimationChanged(object aSender, RenderFactoryEvents.AnimationChangedArgs aE)
        {
            //if we've lost our motion settings connection for some reason, return
            if (motionSettings == null)
                return;
            
            //If we're top level force clamping, ignore any further options
            if(forceClamp)
            {
                RenderFactoryEvents.LatePreFrame += RenderFactoryEvents_OnPreFrame;
                return;
            }

            //If the specific animation has clamping on, check that
            currentAnimationSetting = motionSettings.GetSettingsForAnimation(aE.AnimationName);
            if (currentAnimationSetting == null) return;
            if(currentAnimationSetting.MovementForceClamped ||  currentAnimationSetting.RotationForceClamped)
                RenderFactoryEvents.LatePreFrame += RenderFactoryEvents_OnPreFrame;
            
        }


        private void RenderFactoryEvents_OnPreFrame(object aSender, EventArgs aE)
        {
            if (motionSettings == null)
                return;
            
            bool _clampPosition = false;
            bool _clampRotation = false;
            
            if (forceClamp)
            {
                _clampPosition = motionSettings.ClampMovement;
                _clampRotation = motionSettings.ClampRotation;
            }
            else if (currentAnimationSetting != null)
            {
                _clampPosition = currentAnimationSetting.MovementForceClamped;
                _clampRotation = currentAnimationSetting.RotationForceClamped;
            }

            if (_clampPosition)
            {
                targetTransform.localPosition = originalTransform.Position;
            }

            if (_clampRotation)
            {
                targetTransform.localEulerAngles = originalTransform.RotationEuler;
            }
            

            if(_clampPosition || _clampRotation)
                RenderFactoryEvents.InvokeModelTransformClamped(targetTransform);
        }

    }
}
