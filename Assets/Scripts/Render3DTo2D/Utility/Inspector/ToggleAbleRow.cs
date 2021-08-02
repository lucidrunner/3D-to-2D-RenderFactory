using System;
using UnityEngine;

namespace Render3DTo2D.Utility.Inspector
{
    [Serializable]
    public class ToggleAbleRow
    {
        public string RowName => rowName;
        [SerializeField]
        private string rowName;
        
        [SerializeField]
        private string buttonTooltip;
        
        [SerializeField]
        private string label1, label2, label3;

        [SerializeField] 
        private bool conditionalOne = true;
        [SerializeField] 
        private bool conditionalTwo = true;
        [SerializeField] 
        private bool conditionalThree= true;

        public bool ConditionalOne
        {
            get => conditionalOne;
            set => conditionalOne = value;
        }

        public bool ConditionalTwo
        {
            get => conditionalTwo;
            set => conditionalTwo = value;
        }

        public bool ConditionalThree
        {
            get => conditionalThree;
            set => conditionalThree = value;
        }

        public ToggleAbleRow(string aRowName, string aFirstLabel, string aSecondLabel, string aThirdLabel,
            string aButtonTooltip)
        {
            rowName = aRowName;
            label1 = aFirstLabel;
            label2 = aSecondLabel;
            label3 = aThirdLabel;
            buttonTooltip = aButtonTooltip;
        }
        

        public int GetValueHash()
        {
            unchecked
            {
                int _hashCode = rowName?.GetHashCode() ?? 0;
                _hashCode = (_hashCode * 397) ^ (label1?.GetHashCode() ?? 0);
                _hashCode = (_hashCode * 397) ^ (label2?.GetHashCode() ?? 0);
                _hashCode = (_hashCode * 397) ^ (label3?.GetHashCode() ?? 0);
                _hashCode = (_hashCode * 397) ^ conditionalOne.GetHashCode();
                _hashCode = (_hashCode * 397) ^ conditionalTwo.GetHashCode();
                _hashCode = (_hashCode * 397) ^ conditionalThree.GetHashCode();
                return _hashCode;
            }
        }


        #region Inspector
        public void ToggleAll()
        {
            if (ConditionalOne && ConditionalTwo && ConditionalThree)
            {
                ConditionalOne = false; ConditionalTwo = false; ConditionalThree = false;
            }
            else
            {
                ConditionalOne = true; ConditionalTwo = true; ConditionalThree = true;
            }
        }

        public void ToggleFirst()
        {
            ConditionalOne = !ConditionalOne;
        }
        public void ToggleSecond()
        {
            ConditionalTwo = !ConditionalTwo;
        }
        public void ToggleThird()
        {
            ConditionalThree = !ConditionalThree;
        }

        #endregion
    }
}