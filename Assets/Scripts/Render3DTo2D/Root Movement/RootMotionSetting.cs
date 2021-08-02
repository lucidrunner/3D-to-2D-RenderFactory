using System;
using JetBrains.Annotations;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using UnityEngine;

namespace Render3DTo2D.Root_Movement
{
    [Serializable]
    internal class RootMotionSetting : SerializedAnimation
    {
        [SerializeField, HideInInspector]
        private RootMotionSettings rootMotionSettings;

        [SerializeField, HideInInspector]
        private bool hasRootMotion;

        #region Constructor

        public RootMotionSetting(RootMotionSettings aRootMotionSettings, AnimationClip aAnimationClip) : base(aAnimationClip)
        {
            rootMotionSettings = aRootMotionSettings;
            //TODO See if this *always* is the correct way to query root motion in a test project
            hasRootMotion = AnimationUtilities.HasRootMotion(aAnimationClip);
        }

        #endregion

        #region Properties

        public bool HasRootMotion => hasRootMotion;

        public ToggleTransform Export
        {
            get
            {
                //If this specific or all animations disable export, return null
                if (ExportDisabled || !rootMotionSettings.EnableRootMotionExport) return null;

                //Return the override if it is set or otherwise the default export
                return overrideDefaultExport  ? overrideExportTransform : rootMotionSettings.DefaultExport;
            }
        }

        //If we're exporting and overriding, return that, otherwise return the default follow (which can be null)
        public ToggleTransform Follow
        {
            get
            {
                if (rootMotionSettings.EnableRootMotionExport && FollowDisabled)
                    return null;
                if (rootMotionSettings.EnableRootMotionExport && overrideDefaultFollow)
                    return overrideFollowTransform;

                return rootMotionSettings.DefaultFollow;
            }
        }

        #endregion

        #region Inspector

        #region Root Motion

        [SerializeField]
        private bool enableExport = true;
        
        [SerializeField]
        private bool enableFollow = true;

        [SerializeField]
        private bool forceMovementClamp = false;
        
        [SerializeField]
        private bool forceRotationClamp = false;

        [SerializeField]
        private bool overrideDefaultExport = false;
        
        [SerializeField]
        private bool overrideDefaultFollow = false;

        [SerializeField]
        private ToggleTransform overrideExportTransform = new ToggleTransform("Export Root Motion For", InspectorTooltips.ExportTransform);

        [SerializeField]
        private ToggleTransform overrideFollowTransform = new ToggleTransform("Camera Mimic Motion For", InspectorTooltips.FollowTransform, false);

        #endregion

        #region Conditionals

        [UsedImplicitly] internal bool ExportDisabled => !enableExport;

        [UsedImplicitly] internal bool FollowDisabled => !enableFollow;

        internal bool MovementForceClamped => FollowDisabled && forceMovementClamp;

        internal bool RotationForceClamped => FollowDisabled && forceRotationClamp;

        [UsedImplicitly] private bool ShowOverrideExport => !ExportDisabled && overrideDefaultExport;
        
        [UsedImplicitly]

        private bool ShowOverrideFollow => !FollowDisabled && overrideDefaultFollow;

        [UsedImplicitly] private string ExportGroupName => animationName + "/Export";
        [UsedImplicitly] private string FollowGroupName => animationName + "/Follow";

        #endregion

        #endregion

        #region Methods

        public override int GetValueHash()
        {
            unchecked
            {
                int _hashCode = base.GetValueHash();
                _hashCode = (_hashCode * 397) ^ hasRootMotion.GetHashCode();
                _hashCode = (_hashCode * 397) ^ (Export?.GetValueHash() ?? 0);
                _hashCode = (_hashCode * 397) ^ (Follow?.GetValueHash() ?? 0);
                return _hashCode;
            }
        }

        #endregion

    }
}