using Factory_Editor;
using Render3DTo2D.Single_Frame;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StaticScaleManager))]
public class StaticScaleManagerEditor : ScaleManagerEditor
{
    private SerializedProperty toggleCalculatorProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        toggleCalculatorProp = serializedObject.FindProperty("runCalculator");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.UpdateIfRequiredOrScript();
        toggleCalculatorProp.boolValue = InspectorUtility.DrawToggleButton(toggleCalculatorProp.boolValue, new GUIContent("Run Size Calculator before Rendering"));
        serializedObject.ApplyModifiedProperties();
    }
}