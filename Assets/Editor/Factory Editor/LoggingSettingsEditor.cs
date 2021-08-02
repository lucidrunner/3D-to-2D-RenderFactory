using Render3DTo2D.Logging;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(LoggingSettings))]
    public class LoggingSettingsEditor : Editor
    {
        private SerializedProperty loggedLevelsProp;
        private SerializedProperty minimumTimeStampedProp;
        private SerializedProperty timeStampFormat;
        private SerializedProperty includeOriginProp;
        private SerializedProperty includeSenderLabelProp;

        private void OnEnable()
        {
            loggedLevelsProp = serializedObject.FindProperty("loggedLevels");
            minimumTimeStampedProp = serializedObject.FindProperty("minimumTimeStampedLevel");
            timeStampFormat = serializedObject.FindProperty("timeStampFormat");
            includeOriginProp = serializedObject.FindProperty("includeOriginClass");
            includeSenderLabelProp = serializedObject.FindProperty("includeSenderLabel");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((LoggingSettings) target), typeof(LoggingSettings), false);
            GUI.enabled = true;
            var _target = (LoggingSettings) target;
            
            EditorColors.OverrideTextColors();
            InspectorUtility.BeginBoxGroup("Logged Levels", EditorColors.Header, EditorColors.Body);
            for (int _index = 0; _index < loggedLevelsProp.arraySize; _index++)
            {
                var _toggleButton = loggedLevelsProp.GetArrayElementAtIndex(_index);
                var _buttonLabel = _toggleButton.FindPropertyRelative("label");
                var _buttonState = _toggleButton.FindPropertyRelative("state");
                var _toolTip = _toggleButton.FindPropertyRelative("toolTip");
                _buttonState.boolValue = InspectorUtility.DrawToggleButton(_buttonState.boolValue, new GUIContent(_buttonLabel.stringValue, _toolTip.stringValue));
            }
            InspectorUtility.EndBoxGroup();
            EditorColors.ResetTextColor();
            EditorGUILayout.PropertyField(minimumTimeStampedProp);
            EditorGUILayout.PropertyField(timeStampFormat);
            EditorGUILayout.PropertyField(includeOriginProp);
            EditorGUILayout.PropertyField(includeSenderLabelProp);
            if (InspectorUtility.DrawButton(new GUIContent("Reset Logged Levels"), EditorColors.ButtonRunAlt))
            {
                _target.ResetLoggedLevels();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}