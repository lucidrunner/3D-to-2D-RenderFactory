using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    [Serializable]
    public class LabeledToggle
    {
        [UsedImplicitly]
        public string Label => label;
        
        [SerializeField, HideInInspector]
        private string label;

        public bool State => toggle;
        
        [SerializeField]
        private bool toggle;

        public LabeledToggle(string aLabel, bool aInitialState)
        {
            label = aLabel;
            toggle = aInitialState;
        }
    }
}