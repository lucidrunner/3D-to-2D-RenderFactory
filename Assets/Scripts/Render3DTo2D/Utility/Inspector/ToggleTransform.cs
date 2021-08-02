using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    /// <summary>
    /// Odin inspector class to mimic a transform and let us toggle buttons for position / rotation / scale 
    /// </summary>
    [Serializable]
    public class ToggleTransform
    {
        [UsedImplicitly, SerializeField, HideInInspector]
        private string groupName;

        [UsedImplicitly, SerializeField, HideInInspector]
        private bool enableScaleRow;

        public ToggleTransform(string aGroupName, string aButtonTooltip, bool aEnableScaleRow = true, bool[] aTogglingStartState = null)
        {
            groupName = aGroupName;
            enableScaleRow = aEnableScaleRow;

            positionRow = new ToggleAbleRow("Position", "X", "Y", "Z", aButtonTooltip);
            rotationRow = new ToggleAbleRow("Rotation", "X", "Y", "Z", aButtonTooltip);
            scaleRow = new ToggleAbleRow("Scale", "X", "Y", "Z", aButtonTooltip);
            
                
            if (aTogglingStartState != null && (aEnableScaleRow && aTogglingStartState.Length == 9 || !aEnableScaleRow && aTogglingStartState.Length == 6))
            {
                positionRow.ConditionalOne = aTogglingStartState[0];
                positionRow.ConditionalTwo = aTogglingStartState[1];
                positionRow.ConditionalThree = aTogglingStartState[2];
                
                rotationRow.ConditionalOne = aTogglingStartState[3];
                rotationRow.ConditionalTwo = aTogglingStartState[4];
                rotationRow.ConditionalThree = aTogglingStartState[5];

                if (aEnableScaleRow)
                {
                    scaleRow.ConditionalOne = aTogglingStartState[6];
                    scaleRow.ConditionalTwo = aTogglingStartState[7];
                    scaleRow.ConditionalThree = aTogglingStartState[8];
                }
            }
        }


        public int GetValueHash()
        {
            unchecked
            {
                int _hashCode = positionRow?.GetValueHash() ?? 0;
                _hashCode = (_hashCode * 397) ^ (rotationRow?.GetValueHash() ?? 0);
                _hashCode = (_hashCode * 397) ^ (scaleRow?.GetValueHash() ?? 0);
                return _hashCode;
            }
        }




        #region Inspector

        [SerializeField]
        private ToggleAbleRow positionRow;
        [SerializeField]
        private ToggleAbleRow rotationRow;
        [SerializeField]
        private ToggleAbleRow scaleRow;



        #region Inspector Values & Conditionals


        public bool PositionYToggled => positionRow.ConditionalOne;
        public bool PositionXToggled => positionRow.ConditionalTwo;
        public bool PositionZToggled => positionRow.ConditionalThree;

        public bool RotationYToggled => rotationRow.ConditionalOne;
        public bool RotationZToggled => rotationRow.ConditionalTwo;
        public bool RotationXToggled => rotationRow.ConditionalThree;

        public bool ScaleXToggled => enableScaleRow && scaleRow.ConditionalOne;
        public bool ScaleYToggled => enableScaleRow && scaleRow.ConditionalTwo;
        public bool ScaleZToggled => enableScaleRow && scaleRow.ConditionalThree;

        #endregion

        #endregion

    }

}