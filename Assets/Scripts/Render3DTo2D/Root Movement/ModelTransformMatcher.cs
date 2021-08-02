using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


namespace Render3DTo2D.Root_Movement
{
    public class ModelTransformMatcher : MonoBehaviour
    {
        [SerializeField]
        private Transform modelTransform;
        //The source that we read if we should follow position / rotation on
        //Can either be locally provided or sourced from the advanced animation settings
        [SerializeField]
        private ToggleTransform overrideSource;
        
        [SerializeField]
        private bool toggleOverride;

        #region Inspector

        public void SetupFollowList()
        {
            ModelInfo _modelInfo = ModelInfo.GetFor(transform);
            List<string> _clipNames = AnimatorEditorHelperMethods.GetClipNames(_modelInfo.Animator);
            animationFollowElements = new List<AnimationFollowElement>();
            for (var _index = 0; _index < _clipNames.Count; _index++)
            {
                string _clipName = _clipNames[_index];
                animationFollowElements.Add(new AnimationFollowElement(_clipName));
            }
        }

        [SerializeField]
        private List<AnimationFollowElement> animationFollowElements;
        
        #endregion

        #region  Methods

        // Start is called before the first frame update

        private void Reset()
        {
            SetupFollowList();
        }

        void Start()
        {
            RenderFactoryEvents.FactoryStarted += RenderFactoryEventsOnFactoryStarted;
        }

        private void RenderFactoryEventsOnFactoryStarted(object aSender, EventArgs aE)
        {
            if (!GeneralUtilities.CompareSenderToModelRoot(aSender, transform))
            {
                return;
            }
            

            //Set the targeted transform
            modelTransform = AnimationUtilities.GetAnimatorTransform(transform);
            
            //Reset the rig if it's our factory that's animating
            RenderFactoryEvents.PreAnimationChanged += RenderFactoryEventsOnPreAnimationChanged;

            RenderFactoryEvents.AnimationChanged += RenderFactoryEventsOnAnimationChanged;
            
            //And sign us up to the follow check
            RenderFactoryEvents.PreFrameRender += RenderFactoryEventsOnPreFrame;
            RenderFactoryEvents.PreFrameCalculator += RenderFactoryEventsOnPreFrame;
            RenderFactoryEvents.ModelTransformClamped += RenderFactoryEventsOnPreFrame;

            //Unsubscribe on factory end
            RenderFactoryEvents.FactoryEnded += RenderFactoryEventsOnFactoryEnded;
        }

        private void RenderFactoryEventsOnFactoryEnded(object aSender, EventArgs aE)
        {
            RenderFactoryEvents.PreAnimationChanged -= RenderFactoryEventsOnPreAnimationChanged;
            RenderFactoryEvents.AnimationChanged -= RenderFactoryEventsOnAnimationChanged;
            RenderFactoryEvents.PreFrameRender -= RenderFactoryEventsOnPreFrame;
            RenderFactoryEvents.PreFrameCalculator -= RenderFactoryEventsOnPreFrame;
            RenderFactoryEvents.ModelTransformClamped -= RenderFactoryEventsOnPreFrame;
        }

        private void RenderFactoryEventsOnPreAnimationChanged(object aSender, RenderFactoryEvents.AnimationChangedArgs aArgs)
        {
            //TODO Use initial values for these instead
            //Reset the transform of the model
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localScale = Vector3.one;
            modelTransform.localEulerAngles = Vector3.zero;
            
            //Reset the transform of the rig
            var _transform = transform;
            _transform.localPosition = Vector3.zero;
            _transform.localScale = Vector3.one;
            _transform.localEulerAngles = Vector3.zero;
            
        }

        private void RenderFactoryEventsOnAnimationChanged(object aSender, RenderFactoryEvents.AnimationChangedArgs aArgs)
        {
            //Load the override source for the animation
            LoadOverrideSource(aArgs.AnimationName);
        }


        private void RenderFactoryEventsOnPreFrame(object aSender, EventArgs aE)
        {
            if (overrideSource == null)
                return;

            //Conditionally copy position & rotation depending on if we have sat the it to follow
            Vector3 _localPos = GeneralUtilities.ConditionalVectorCopy(modelTransform.localPosition,
                (overrideSource.PositionXToggled, overrideSource.PositionYToggled, overrideSource.PositionZToggled));
            Vector3 _localRot = GeneralUtilities.ConditionalVectorCopy(modelTransform.localEulerAngles,
                (overrideSource.RotationXToggled, overrideSource.RotationYToggled, overrideSource.RotationZToggled));

            
            var _transform = transform;
            _transform.localPosition = _localPos;
            _transform.localEulerAngles = _localRot;
        }

        private void LoadOverrideSource(string aAnimationName)
        {
            overrideSource = GetOverrideSource(aAnimationName);
        }

        private ToggleTransform GetOverrideSource(string aAnimationName)
        {
            ToggleTransform _source;
            if (!toggleOverride)
            {
                //Using the .? is a bit unsafe with monobehaviours so breaking this up insteps
                RootMotionSettings _animationSettings = RootMotionSettings.GetFor(transform);
                if (_animationSettings == null)
                    return null;
                
                _source = _animationSettings.GetSettingsForAnimation(aAnimationName)?.Follow;
            }
            else
            {
                _source = animationFollowElements.FirstOrDefault(aElement => aElement.AnimationName == aAnimationName)
                    ?.Follow;
            }

            return _source;
        }

        #endregion
    }
}
