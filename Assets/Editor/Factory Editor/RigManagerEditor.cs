using Render3DTo2D.Factory_Core;
using Shared_Scripts;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(RigManager))]
    public class RigManagerEditor : Editor
    {
        private SerializedProperty rigs;

        private void OnEnable()
        {
            rigs = serializedObject.FindProperty("cameraRigs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
        
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RigManager) target), typeof(RigManager), false);
            GUI.enabled = true;
            var _target = (RigManager) target;
            Color _defaultColor = GUI.backgroundColor;
        
            if(rigs.arraySize == 0)
            {
                EditorGUILayout.LabelField($"No Rigs Added to Factory", new GUIStyle("BoldLabel"));
                return;
            }

            EditorColors.OverrideTextColors();
            InspectorUtility.BeginBoxGroup("Toggle Rigs To Render", EditorColors.Header, EditorColors.Body);

            for (var _index = 0; _index < rigs.arraySize; _index++)
            {
                var _toggleAbleRig = rigs.GetArrayElementAtIndex(_index);
                SerializedProperty _currentRigName = _toggleAbleRig.FindPropertyRelative("rigName");
                SerializedProperty _currentToggleProp = _toggleAbleRig.FindPropertyRelative("toggled");
                SerializedProperty _deletionToggle = _toggleAbleRig.FindPropertyRelative("deletionSafetyToggle");
            
                Color _varTitleColor = _index % 2 == 0 ?  EditorColors.HeaderAlt1 : EditorColors.HeaderAlt2;
                Color _varContentColor = _index % 2 == 0 ?  EditorColors.BodyAlt1 : EditorColors.BodyAlt2;
            
                GUILayout.Space(3);
                InspectorUtility.BeginSubBoxGroup(_currentRigName.stringValue, _varTitleColor, _varContentColor);

                GUI.backgroundColor = _currentToggleProp.boolValue ? EditorColors.ToggleOn : EditorColors.ToggleOff;
                string _toggleButtonText = _currentToggleProp.boolValue ? "Render" : "Don't Render";
            
                GUILayout.Space(3);
                if(GUILayout.Button(_toggleButtonText))
                {
                    _target.ToggleRig(_index);
                }

                GUI.backgroundColor = _defaultColor;
            
                GUILayout.Space(3);

                EditorGUILayout.BeginHorizontal();

                
                InspectorUtility.DrawToggleProperty(_deletionToggle, new GUIContent("Deletion Safety Toggle"), true);
               // _deletionToggle.boolValue = GUILayout.Toggle(_deletionToggle.boolValue, "Deletion Safety Toggle");

                GUI.enabled = _deletionToggle.boolValue;
                GUI.backgroundColor = EditorColors.ButtonAction;
                if(InspectorUtility.DrawButton(new GUIContent("Delete Rig"), EditorColors.ButtonAction))
                {
                    _target.RemoveRig(_index);
                }

                GUI.backgroundColor = _defaultColor;
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            
            

        
                InspectorUtility.EndSubBoxGroup();

            }
            InspectorUtility.EndBoxGroup();
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }
    
   
    }
}