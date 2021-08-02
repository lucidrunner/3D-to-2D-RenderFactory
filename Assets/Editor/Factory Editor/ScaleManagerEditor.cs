using Render3DTo2D.Factory_Core;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(ScaleManager))]
    public class ScaleManagerEditor : Editor
    {
        private SerializedProperty calculatorsProp;
    
        protected virtual void OnEnable()
        {
            calculatorsProp = serializedObject.FindProperty("scaleCalculators");
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ScaleManager) target), typeof(ScaleManager), false);
            GUI.enabled = true;
            var _target = (ScaleManager) target;
        
            EditorGUILayout.LabelField($"Current Calculator Count: {_target.CalculatorCount}", new GUIStyle("BoldLabel"));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(calculatorsProp);
            GUI.enabled = true;
        }
    }
}