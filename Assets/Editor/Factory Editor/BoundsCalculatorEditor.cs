using Render3DTo2D.Factory_Core;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(BoundsCalculator))]
    public class BoundsCalculatorEditor : Editor
    {
        private bool headerGroupExpanded ;

        private SerializedProperty meshListProp;
        private SerializedProperty boundListProp;
        private SerializedProperty colorProp;
        private SerializedProperty toggleDrawProp;
        private void OnEnable()
        {
            boundListProp = serializedObject.FindProperty("meshBounds");
            meshListProp = serializedObject.FindProperty("meshesInHierarchy");
            colorProp = serializedObject.FindProperty("boundsColor");
            toggleDrawProp = serializedObject.FindProperty("drawCalculatedBounds");
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BoundsCalculator) target), typeof(BoundsCalculator), false);
            GUI.enabled = true;
            var _target = (BoundsCalculator) target;
            serializedObject.UpdateIfRequiredOrScript();


            EditorColors.OverrideTextColors();
            headerGroupExpanded = InspectorUtility.BeginFoldoutGroup("Debug", headerGroupExpanded);

            if (headerGroupExpanded)
            {
                toggleDrawProp.boolValue = InspectorUtility.DrawToggleButton(toggleDrawProp.boolValue, new GUIContent("Draw Bounds"));
                if (InspectorUtility.DrawButton(new GUIContent("Recalculate"), EditorColors.ButtonAction))
                {
                    _target.CalculateAndReturn();
                }

                EditorGUILayout.PropertyField(colorProp);
            
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(meshListProp, true);
                EditorGUILayout.PropertyField(boundListProp, true);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            
            }
        
        
            InspectorUtility.EndFoldoutGroup(headerGroupExpanded);

            bool _modified = serializedObject.hasModifiedProperties;
        
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();

            if (_modified)
            {
                _target.CalculateAndReturn();
            }
        }
    }
}