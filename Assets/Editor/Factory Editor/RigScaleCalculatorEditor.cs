using Render3DTo2D.Isometric;
using Render3DTo2D.Rigging;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(IsometricRigScaleCalculator))]
    public class IsometricRigScaleCalculatorEditor : RigScaleCalculatorEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((IsometricRigScaleCalculator) target), typeof(IsometricRigScaleCalculator), false);
            GUI.enabled = true;
            var _target = (IsometricRigScaleCalculator) target;
            EditorColors.OverrideTextColors();
            DrawSetClipping(_target);
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }
        
        
    }
    
    [CustomEditor(typeof(RigScaleCalculator))]
    public class RigScaleCalculatorEditor : Editor
    {
        private SerializedProperty scaleProp;
        private SerializedProperty setClippingDepthProp;

        protected virtual void OnEnable()
        {
            scaleProp = serializedObject.FindProperty("currentInspectorScale");
            setClippingDepthProp = serializedObject.FindProperty("setClippingDepth");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RigScaleCalculator) target), typeof(RigScaleCalculator), false);
            GUI.enabled = true;

            var _target = (RigScaleCalculator) target;

            EditorColors.OverrideTextColors();
            DrawSetSize(_target);

            DrawSetClipping(_target);
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawSetSize(RigScaleCalculator aTarget)
        {
            InspectorUtility.BeginBoxGroup("Set Current Camera Size", EditorColors.Header, EditorColors.Body);
            EditorGUILayout.BeginHorizontal();
            scaleProp.floatValue = EditorGUILayout.FloatField("Orthographic Scale ", scaleProp.floatValue);

            if (InspectorUtility.DrawButton(new GUIContent("Apply"), EditorColors.ButtonAction))
            {
                aTarget.SetCameraScales(scaleProp.floatValue);
            }

            EditorGUILayout.EndHorizontal();
            InspectorUtility.EndBoxGroup();
        }

        protected void DrawSetClipping(RigScaleCalculator aTarget)
        {
            InspectorUtility.BeginBoxGroup("Set Current Clipping Depth", EditorColors.Header, EditorColors.Body);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(setClippingDepthProp);
            if (InspectorUtility.DrawButton(new GUIContent("Apply"), EditorColors.ButtonAction))
            {
                aTarget.SetCameraDepths(setClippingDepthProp.floatValue);
            }

            EditorGUILayout.EndHorizontal();
            InspectorUtility.EndBoxGroup();
        }
    }
}
