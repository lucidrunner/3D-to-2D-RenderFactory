using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Root_Movement
{
    public class RootMotionSettings : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void Start()
        {
            RenderFactoryEvents.FactoryStarted += RenderFactoryEventsOnFactoryStarted;
        }

        #region Properties
        
        /// <summary>
        /// The default export settings, null if export is disabled
        /// </summary>
        public ToggleTransform DefaultExport => enableRootMotionExport ? defaultExport : null;
        /// <summary>
        /// Whether we should export root motion or not
        /// </summary>
        public bool EnableRootMotionExport => enableRootMotionExport;
        /// <summary>
        /// The Default follow settings, null if export is disabled and apply default root movement isn't toggled
        /// </summary>
        public ToggleTransform DefaultFollow => enableRootMotionExport || applyDefaultRootMovement ? defaultFollow : null;

        /// <summary>
        /// Whether or not we should forcibly clamp the movement. Necessary as some animation sources still end up applying movement / rotation even with the root movement set to off.
        /// </summary>
        public bool ClampMovement => ShowClampingButtons && forceMovementClamp;
        
        /// <summary>
        /// Whether or not we should forcibly clamp the rotation. Necessary as some animation sources still end up applying movement / rotation even with the root movement set to off.
        /// </summary>
        public bool ClampRotation => ShowClampingButtons && forceRotationClamp;

        #endregion

        #region Inspector

        [SerializeField] private List<RootMotionSetting> rootMotionSettings;

        [SerializeField] private bool enableRootMotionExport = false;

        [SerializeField] private bool applyDefaultRootMovement = true;
        
        [SerializeField] private bool forceMovementClamp = false;

        [SerializeField] private bool forceRotationClamp = false;

        public bool HasReset = false;

        [SerializeField]
        private ToggleTransform defaultExport = new ToggleTransform("Default Export", InspectorTooltips.ExportTransform);

        [SerializeField]
        private ToggleTransform defaultFollow = new ToggleTransform("Default Follow Behaviour", InspectorTooltips.FollowTransform, false, DefaultFollowState);
        
        
        #region Conditionals

        private bool ShowClampingButtons => !enableRootMotionExport && !applyDefaultRootMovement; 
        
        private static readonly bool[] DefaultFollowState = {true, false, true, false, false, false};

        #endregion

        #endregion

        #region Methods

        public void Setup(Animator aAnimator)
        {
            animator = aAnimator;
            SetupSettingsList();
        }

        public void Reset()
        {
            animator = AnimationUtilities.GetAnimatorTransform(transform).GetComponent<Animator>();
            SetupSettingsList();
            HasReset = true;
        }

        private void SetupSettingsList()
        {
            rootMotionSettings = new List<RootMotionSetting>();

            var _clips = AnimatorEditorHelperMethods.GetClips(animator);
            //Create the list and link it to this
            foreach (var _clip in _clips)
                //create the animation setting & add it to the list
                rootMotionSettings.Add(new RootMotionSetting(this, _clip));
        }

        private void RenderFactoryEventsOnFactoryStarted(object aSender, EventArgs aE)
        {
            if (!GeneralUtilities.CompareSenderToModelRoot(aSender, transform) || animator == null) return;

            RenderFactoryEvents.AnimationChanged += OnRenderFactoryEventsOnAnimationChanged;
        }

        private void OnRenderFactoryEventsOnAnimationChanged(object aO, RenderFactoryEvents.AnimationChangedArgs aArgs)
        {
            FLogger.LogMessage(this, FLogger.Severity.Debug, "Checking apply root motion now");
            if (enableRootMotionExport)
                animator.applyRootMotion = GetSettingsForAnimation(aArgs.AnimationName)?.HasRootMotion ?? false;
            else if (applyDefaultRootMovement) 
                animator.applyRootMotion = true;

            RenderFactoryEvents.AnimationChanged -= OnRenderFactoryEventsOnAnimationChanged;
        }

        internal RootMotionSetting GetSettingsForAnimation(int aAnimationIndex)
        {
            //Return either the indexed animation or null if it's out of bounds
            return aAnimationIndex < rootMotionSettings.Count ? rootMotionSettings[aAnimationIndex] : null;
        }

        internal RootMotionSetting GetSettingsForAnimation(string aAnimationName)
        {
            return rootMotionSettings.FirstOrDefault(aSetting =>
                aSetting.AnimationName.ToLower().Equals(aAnimationName.ToLower()));
        }

        public static RootMotionSettings GetFor(Transform aHierarchyTransform)
        {
            return RootFinder.FindHighestRoot(aHierarchyTransform).GetComponent<RootMotionSettings>();
        }


        public int GetValueHash()
        {
            unchecked
            {
                int _hashCode = enableRootMotionExport.GetHashCode();
                _hashCode = (_hashCode * 397) ^ DefaultFollow?.GetValueHash() ?? 0;
                _hashCode = (_hashCode * 397) ^ ClampMovement.GetHashCode();
                _hashCode = (_hashCode * 397) ^ ClampRotation.GetHashCode();
                if (rootMotionSettings == null)
                    return _hashCode;
                _hashCode = rootMotionSettings.Aggregate(_hashCode, (aCurrent, aRootMotionSetting) => (aCurrent * 397) ^ aRootMotionSetting.GetValueHash());

                return _hashCode;
            }
        }

        #endregion
    }
}