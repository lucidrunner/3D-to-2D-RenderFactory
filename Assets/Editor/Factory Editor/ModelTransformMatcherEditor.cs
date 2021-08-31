using Render3DTo2D.Root_Movement;
using Shared_Scripts;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(ModelTransformMatcher))]
    public class ModelTransformMatcherEditor : Editor
    {
        private SerializedProperty toggleProp;
        private SerializedProperty animationFollows;
        

        private void OnEnable()
        {
            toggleProp = serializedObject.FindProperty("toggleOverride");
            animationFollows = serializedObject.FindProperty("animationFollowElements");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ModelTransformMatcher) target), typeof(ModelTransformMatcher), false);
            GUI.enabled = true;
            var _target = (ModelTransformMatcher) target;


            bool _prevToggleState = toggleProp.boolValue;

            toggleProp.boolValue = InspectorUtility.DrawToggleButton(toggleProp.boolValue, new GUIContent("Override Animation Follow Settings"));
            
            if(toggleProp.boolValue != _prevToggleState)
            {
                if(animationFollows.arraySize == 0)
                    _target.SetupFollowList();
            }

            if (!toggleProp.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorColors.OverrideTextColors();

            InspectorUtility.BeginBoxGroup("Override Movement For", EditorColors.Header, EditorColors.Body);

        
        
            for (int _index = 0; _index < animationFollows.arraySize; _index++)
            {
                //Get our indexed property (aka our AnimationFollowElement)
                var _prop = animationFollows.GetArrayElementAtIndex(_index);
            
                //In turn, get the current toggle transform for it
                var _followTransformProp = _prop.FindPropertyRelative("followTransform");

                //And the current animation name
                var _animNameProp = _prop.FindPropertyRelative("animationName");
            
                InspectorUtility.BeginSubBoxGroup(_animNameProp.stringValue, EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                InspectorUtility.DrawToggleTransform(_followTransformProp, true, EditorColors.HeaderAlt2, EditorColors.BodyAlt2);
                InspectorUtility.EndSubBoxGroup();
            }
        
            InspectorUtility.EndBoxGroup();
            EditorColors.ResetTextColor();
        
        
            if(InspectorUtility.DrawButton(new GUIContent("Reload Animation list"), EditorColors.ButtonRunAlt))
            {
                _target.SetupFollowList();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

