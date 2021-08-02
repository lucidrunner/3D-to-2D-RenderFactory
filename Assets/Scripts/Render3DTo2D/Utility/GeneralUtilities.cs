using System;
using System.Text;
using Render3DTo2D.Model_Settings;
using RootChecker;
using UnityEditor;
using UnityEngine;

namespace Render3DTo2D.Utility
{
    public static class GeneralUtilities 
    {
        public static Vector3 ConditionalVectorCopy(Vector3 aOrigin, (bool, bool, bool) aConditional, Vector3 aBaseVector = default)
        {
            Vector3 _toReturn = aBaseVector;
            if (aConditional.Item1)
                _toReturn.x = aOrigin.x;
            if (aConditional.Item2)
                _toReturn.y = aOrigin.y;
            if (aConditional.Item3)
                _toReturn.z = aOrigin.z;
            return _toReturn;
        }

        public static bool CompareSenderToModelRoot(object aSender, Transform aTransform)
        {
            Transform _root = RootFinder.FindHighestRoot(aTransform);
            Transform _senderRoot = aSender as Transform;
            return _senderRoot != null && RootFinder.FindHighestRoot(_senderRoot) == _root;
        }

        public static void SetBoolArray(ref bool[] aBoolArray, bool aState)
        {
            for (int _index = 0; _index < aBoolArray.Length; _index++)
            {
                aBoolArray[_index] = aState;
            }
        }
        
        public static string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string CleanName(string aName)
        {
            //Since we're using _ as a divider between the parseable parts of the render output it really can't be apart of the animation / model name
            return aName.Replace("_", "");
        }

        public static Vector3 DeviateVector(Vector3 aVector3, float aFactoryBaselineScale, float aGlobalBaselineScale)
        {
            if (Mathf.Approximately(aFactoryBaselineScale, aGlobalBaselineScale))
                return aVector3;
            float _deviation = aGlobalBaselineScale / aFactoryBaselineScale;
            
            return new Vector3(aVector3.x * _deviation, aVector3.y * _deviation, aVector3.z * _deviation);
        }
        
        
        public static int RoundUp(int aValue, int aMultiple)
        {
            int _remaining = aValue % aMultiple;
            int _result = aValue + (_remaining > 0 ? (aMultiple - _remaining) : 0);
            return _result;
        }

        public static void FocusSceneCamera(GameObject aGameObject)
        {
            var _prevObject = Selection.activeGameObject;
            Selection.activeGameObject = aGameObject;
            SceneView.lastActiveSceneView.FrameSelected();
            Selection.activeGameObject = _prevObject;
        }
    }
}
