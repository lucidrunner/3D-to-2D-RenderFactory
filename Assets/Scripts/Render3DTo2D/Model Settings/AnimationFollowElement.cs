using System;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    [Serializable]
    internal class AnimationFollowElement
    {
        public string AnimationName => animationName;

        [SerializeField, HideInInspector]
        private string animationName;

        public AnimationFollowElement(string aAnimationName)
        {
            animationName = aAnimationName;
        }
        
        public ToggleTransform Follow => followTransform;
        [SerializeField, BoxGroup("$AnimationName"), HideLabel]
        private ToggleTransform followTransform = new ToggleTransform("Camera Mimic Motion For", InspectorTooltips.FollowTransform, false, new bool[9]);
    }
}