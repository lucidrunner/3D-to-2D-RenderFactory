using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Root_Movement
{
    public class TransformRecorder : MonoBehaviour
    {
        internal class AnimationRecording
        {
            public int AnimationIndex { get; }
            public string AnimationName { get; }
            internal RootMotionSetting AnimationSetting { get; }
            internal List<FrameRecording> FrameRecordings { get; }

            private StoredTransform lastTransform;
            
            public AnimationRecording(RootMotionSetting aAnimationSetting,
                int aAnimationIndex, string aAnimationName)
            {
                AnimationSetting = aAnimationSetting;
                AnimationIndex = aAnimationIndex;
                AnimationName = aAnimationName;
                FrameRecordings = new List<FrameRecording>();
            }

            public void AddFrameRecording(Transform aTransform, FrameArgs aFrameArgs)
            {
                FLogger.LogMessage(this, FLogger.Severity.Debug, $"Adding Frame Recording for frame {aFrameArgs.CurrentFrame}.");
                //Do a delta calculation on the stored transform since the last recorded frame
                StoredTransform _deltaTransform = new StoredTransform(aTransform);
                if (FrameRecordings.Count > 0)
                {
                    _deltaTransform.SetDelta(lastTransform);
                }
                else
                {
                    //We're only interested in scale changes after the first frame 
                    _deltaTransform.Scale -= Vector3.one;
                }
                lastTransform = new StoredTransform(aTransform);    
                
                FrameRecordings.Add(new FrameRecording(_deltaTransform, aFrameArgs));
            }


            internal class FrameRecording
            {
                public int AnimationIndex { get; }
                public int FrameIndex { get; }
                public float FrameStepTime { get; }
                public float FrameRealTime { get; }
                
                public StoredTransform FrameTransform { get; }

                public bool HasChanged => FrameTransform?.HasDeltaChange ?? false;


                public FrameRecording(StoredTransform aTransform,
                    FrameArgs aFrameArgs)
                {
                    FrameIndex = aFrameArgs.CurrentFrame;
                    AnimationIndex = aFrameArgs.CurrentAnimationIndex;
                    FrameStepTime = aFrameArgs.LastStepTime;
                    FrameRealTime = aFrameArgs.FrameRealTime;
                    FrameTransform = aTransform;
                }
            }
        }

        
        
        #region Fields
        public static TransformRecorder Instance;
        
        private object currentRecordedObject = null;
        private bool hasRecorded = false;
        private Transform currentModelTransform = null;
        private List<AnimationRecording> animationRecordings;
        private RootMotionSettings linkedSettings;
        #endregion
        
        #region Methods
        public void Start()
        {
            //Subscribe to the static render factory event
            RenderFactoryEvents.FactoryStarted += RenderFactoryEventsOnFactoryStarted;
            RenderFactoryEvents.FactoryEnded += RenderFactoryEventsOnFactoryEnded;
            //Set our singleton link
            Instance = this;
        }
        
        public bool HasFactoryRecorded(Transform aFactoryTransform)
        {
            return currentModelTransform == AnimationUtilities.GetAnimatorTransform(aFactoryTransform) && hasRecorded;
        }

        
        #region Event Listeners
        private void RenderFactoryEventsOnFactoryStarted(object aSender, EventArgs aE)
        {
            //To begin with, always record if we don't have a recorded object
            Transform _root = aSender as Transform;
            if (currentRecordedObject != null || _root == null)
            {
                return;
            }

            //Check the senders toplevel object for if we should record
            _root = RootFinder.FindHighestRoot(_root);
            linkedSettings = RootMotionSettings.GetFor(_root);
            if (linkedSettings == null || !linkedSettings.EnableRootMotionExport)
                return;
            
            //Set the currently recorded object
            currentRecordedObject = _root;
            animationRecordings = new List<AnimationRecording>();

            //Subscribe to the recording events
            RenderFactoryEvents.AnimationChanged += RenderFactoryEventsOnAnimationChanged;
            RenderFactoryEvents.PreFrameRender += OnPreFrameCalculatedOrRendered;
            RenderFactoryEvents.PreFrameCalculator += OnPreFrameCalculatedOrRendered; //We record either when we're doing the calculation or when we're doing the rendering
            RenderFactoryEvents.TransformExport += RenderFactoryEventsOnTransformExport;
        }

        private void RenderFactoryEventsOnFactoryEnded(object aSender, EventArgs aE)
        {
            if(currentRecordedObject != aSender || currentRecordedObject == null)
                return;

            //Reset the recorded object so we can record the next
            currentRecordedObject = null;
            currentModelTransform = null;
            hasRecorded = false;
            
            //Unsub to the recording events
            RenderFactoryEvents.AnimationChanged -= RenderFactoryEventsOnAnimationChanged;
            RenderFactoryEvents.PreFrameCalculator -= OnPreFrameCalculatedOrRendered;
            RenderFactoryEvents.PreFrameRender -= OnPreFrameCalculatedOrRendered;
        }
        
        private void RenderFactoryEventsOnAnimationChanged(object aSender, RenderFactoryEvents.AnimationChangedArgs aArgs)
        {

            //Check if the list currently contains a frame recording for that animation (Reset Called most likely)
            //TODO This will almost deffo have to change if we do blends buuuut we're not doing that now so w/e
            //For blends in the future we'll have to keep index of the clips and index / id's of the blended states as different
            if (animationRecordings.Count(aRecording => aRecording.AnimationIndex == aArgs.AnimationIndex) == 0)
            {
                //Record the animation only if it has a root motion to record
                var _animationSetting = linkedSettings.GetSettingsForAnimation(aArgs.AnimationName);
                if(_animationSetting?.Export != null)
                {
                    currentModelTransform = AnimationUtilities.GetAnimatorTransform(aSender as Transform);
                    animationRecordings.Add(new AnimationRecording(_animationSetting ,aArgs.AnimationIndex, aArgs.AnimationName));
                    FLogger.LogMessage(this, FLogger.Severity.Debug, $"AnimationSetting {aArgs.AnimationName} is supposed to record");
                }
                else
                {
                    FLogger.LogMessage(this, FLogger.Severity.Debug, $"AnimationSetting {aArgs.AnimationName} is not supposed to record");
                    currentModelTransform = null;
                }
            }
        }

        private void OnPreFrameCalculatedOrRendered(object aSender, FrameArgs aFrameArgs)
        {
            //Make sure we have a recording we can record to
            if (aSender != currentRecordedObject || (animationRecordings == null || animationRecordings.Count == 0) || currentModelTransform == null)
                return;
            
            //Don't record if we're not attached atm
            if (currentModelTransform == null)
                return;

            //Set a note that we have recorded already
            hasRecorded = true;
            
            //Record 
            animationRecordings.Last().AddFrameRecording(currentModelTransform, aFrameArgs);
        }

        private void RenderFactoryEventsOnTransformExport(object aSender, RenderFactoryEvents.ExportTransformArgs aArgs)
        {
            FLogger.LogMessage(this, FLogger.Severity.Debug, "Entering Root Export.");
            if (aSender != currentRecordedObject || (animationRecordings == null || animationRecordings.Count == 0))
                return;

            FLogger.LogMessage(this, FLogger.Severity.Debug, $"Found recorded object {aSender}");
            
            Transform _transform = aSender as Transform;
            if (_transform == null)
                return;
            
            RootMotionXmlExporter.Export(aArgs, _transform, animationRecordings);
            
            //Unsub to the export event
            RenderFactoryEvents.TransformExport -= RenderFactoryEventsOnTransformExport;
        }

        #endregion
        #endregion
    }
}
