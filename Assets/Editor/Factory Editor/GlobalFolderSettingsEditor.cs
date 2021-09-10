using Render3DTo2D.Model_Settings;
using Shared_Scripts;
using UnityEditor;

namespace Factory_Editor
{
    [CustomEditor(typeof(GlobalFolderSettings))]
    public class GlobalFolderSettingsEditor : FolderSettingsEditor
    {
        private SerializedProperty isometricTagProp;
        private SerializedProperty sideViewTagProp;
        protected override void OnEnable()
        {
            base.OnEnable();

            isometricTagProp = serializedObject.FindProperty("isometric");
            sideViewTagProp = serializedObject.FindProperty("sideRotational");
        
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.UpdateIfRequiredOrScript();
            EditorColors.OverrideTextColors();
            InspectorUtility.BeginBoxGroup("Camera Rig Tags", EditorColors.Header, EditorColors.Body);

            InspectorUtility.DrawProperty(isometricTagProp);
            InspectorUtility.DrawProperty(sideViewTagProp);
        
            InspectorUtility.EndBoxGroup();
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }
    }
}