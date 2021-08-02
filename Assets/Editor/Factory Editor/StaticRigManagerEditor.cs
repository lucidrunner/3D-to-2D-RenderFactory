using Render3DTo2D.Single_Frame;
using Render3DTo2D.Utility.Inspector;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(StaticRigManager))]
    public class StaticRigManagerEditor : RigManagerEditor
    {
    
    }

    [CustomEditor(typeof(StaticRenderManager))]
    public class StaticRenderManagerEditor : RenderManagerEditor
    {
        private SerializedProperty selectedAnimationProp;
        private SerializedProperty targetFrameProp;
        protected override void OnEnable()
        {
            base.OnEnable();
            selectedAnimationProp = serializedObject.FindProperty("selectedAnimation");
            targetFrameProp = serializedObject.FindProperty("frame");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.UpdateIfRequiredOrScript();
            var _target = (StaticRenderManager)target;
            if (_target.CanAnimate)
            {
                EditorColors.OverrideTextColors();
                InspectorUtility.BeginBoxGroup("Animated Frame Target", EditorColors.Header, EditorColors.Body);
                selectedAnimationProp.intValue = InspectorUtility.DrawListPopup("Target Animation", _target.SelectedAnimationOptions, selectedAnimationProp.intValue);
                targetFrameProp.intValue = EditorGUILayout.IntSlider("Target Frame Index",targetFrameProp.intValue, 0, _target.MaxFrames);
                InspectorUtility.EndBoxGroup();
                EditorColors.ResetTextColor();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
