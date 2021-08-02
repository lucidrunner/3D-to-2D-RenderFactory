using Render3DTo2D.Model_Settings;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(FolderSettings))]
    public class FolderSettingsEditor : Editor
    {
        private SerializedProperty baseFolderProp;
        private SerializedProperty timeStampedProp;
        private SerializedProperty subfolderProp;
        private SerializedProperty staticFolderProp;
        private SerializedProperty rootMotionFolderProp;
    
        protected virtual void OnEnable()
        {
            baseFolderProp = serializedObject.FindProperty("outputBaseFolder");
            timeStampedProp = serializedObject.FindProperty("createTimeStampedFolders");
            subfolderProp = serializedObject.FindProperty("createAnimationSubfolders");
            staticFolderProp = serializedObject.FindProperty("staticFolderName");
            rootMotionFolderProp = serializedObject.FindProperty("rootMotionDataFolderName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((FolderSettings) target), typeof(FolderSettings), false);
            GUI.enabled = true;
            var _target = (FolderSettings) target;
            
            EditorColors.OverrideTextColors();

            InspectorUtility.BeginBoxGroup("Setup", EditorColors.Header, EditorColors.Body);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(baseFolderProp);
            if (InspectorUtility.DrawButton(new GUIContent("Open"), EditorColors.ButtonAction, GUILayout.MaxWidth(50f), GUILayout.MinWidth(50f)))
            {
                string _folder = EditorUtility.OpenFolderPanel("Select Output Folder", baseFolderProp.stringValue, "");
                if (!string.IsNullOrWhiteSpace(_folder))
                    baseFolderProp.stringValue = _folder;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(timeStampedProp);
            EditorGUILayout.PropertyField(subfolderProp);
            EditorGUILayout.PropertyField(staticFolderProp);
            EditorGUILayout.PropertyField(rootMotionFolderProp);
            InspectorUtility.EndBoxGroup();
            
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }
        
        [MenuItem("CONTEXT/FolderSettings/Reset To Global Values")]
        static void ResetToGlobal(MenuCommand aCommand)
        {
            
            FolderSettings _settings = (FolderSettings) aCommand.context;
            _settings.ResetToGlobal();
        }

        [MenuItem("CONTEXT/FolderSettings/Reset To Global Values", true)]
        static bool CanResetToGlobal()
        {
            var _selectedObject = Selection.activeGameObject;
            if (_selectedObject == null) return false;
            var _global = _selectedObject.GetComponent<GlobalFolderSettings>();
            return _global == null;
        }
    }
}