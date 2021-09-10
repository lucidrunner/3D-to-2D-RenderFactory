using Render3DTo2D.Factory_Core;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(BoundsCalculator))]
    public class BoundsCalculatorEditor : Editor
    {
        [SerializeField] private bool foldoutTarget;
        [SerializeField] private AnimBool foldoutCurrent;

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
            foldoutCurrent = new AnimBool(foldoutTarget);
            foldoutCurrent.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((BoundsCalculator) target), typeof(BoundsCalculator), false);
            GUI.enabled = true;
            var _target = (BoundsCalculator) target;
            serializedObject.UpdateIfRequiredOrScript();


            EditorColors.OverrideTextColors();
            bool _showFoldout = InspectorUtility.BeginFoldoutGroup("Debug", ref foldoutTarget, ref foldoutCurrent);

            if (foldoutTarget)
            {
                toggleDrawProp.boolValue = InspectorUtility.DrawToggleButton(toggleDrawProp.boolValue, new GUIContent("Draw Bounds"));
                if (InspectorUtility.DrawButton(new GUIContent("Recalculate"), EditorColors.ButtonAction))
                {
                    _target.CalculateAndReturn();
                }

                InspectorUtility.DrawProperty(colorProp);
            
                GUI.enabled = false;
                EditorGUI.indentLevel++;
                InspectorUtility.DrawProperty(meshListProp);
                InspectorUtility.DrawProperty(boundListProp);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            
            }
        
        
            InspectorUtility.EndFoldoutGroup(_showFoldout);

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