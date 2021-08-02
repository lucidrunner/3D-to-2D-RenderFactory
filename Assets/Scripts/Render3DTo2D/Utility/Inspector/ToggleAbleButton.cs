using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    [Serializable]
    public class ToggleAbleButton
    {
        
        [SerializeField]
        private string label;

        public bool State => state;
        
        [SerializeField]
        private bool state;

        
        [SerializeField, UsedImplicitly]
        private string toolTip;

        public ToggleAbleButton(string aLabel, bool aState, string aPropertyToolTip = null)
        {
            label = aLabel;
            state = aState;
            toolTip = aPropertyToolTip ?? "";
        }
        
    }
}