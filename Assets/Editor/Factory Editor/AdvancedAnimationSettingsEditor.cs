using System;
using System.Collections.Generic;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.SMAnimator;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(AdvancedAnimationSettings))]
    public class AdvancedAnimationSettingsEditor : Editor
    {
        private SerializedProperty animationSettingsProp;
        [SerializeField] private bool settingsFoldoutState;

        private List<(AnimBool aShowLooping, AnimBool aShowClampSettings)> settingAnimBool = new List<(AnimBool aShowLooping, AnimBool aShowClampSettings)>();

        private void OnEnable()
        {
            animationSettingsProp = serializedObject.FindProperty("animationSettings");
            ReloadAnimBoolList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((AdvancedAnimationSettings) target), typeof(AdvancedAnimationSettings), false);
            GUI.enabled = true;
            var _target = (AdvancedAnimationSettings) target;

            EditorColors.OverrideTextColors();
        
            settingsFoldoutState = InspectorUtility.BeginFoldoutGroup("Individual Animation Settings", settingsFoldoutState);
            if (settingsFoldoutState)
            {
                for (int _index = 0; _index < animationSettingsProp.arraySize; _index++)
                {
                    var _setting = animationSettingsProp.GetArrayElementAtIndex(_index);
                    DrawAnimationSetting(_setting, _index);
                }    
            }

            InspectorUtility.EndFoldoutGroup(settingsFoldoutState);
            EditorColors.ResetTextColor();

            var _prevColor = GUI.backgroundColor;
            GUI.backgroundColor = EditorColors.ButtonRunAlt;
            if (GUILayout.Button("Reload Animation List"))
            {
                _target.SetupSettingsList();
                ReloadAnimBoolList();
            }

            GUI.backgroundColor = _prevColor;

            serializedObject.ApplyModifiedProperties();
        }

        private void ReloadAnimBoolList()
        {
            settingAnimBool.Clear();
            for (int _index = 0; _index < animationSettingsProp.arraySize; _index++)
            {
                var _loopProp = animationSettingsProp.GetArrayElementAtIndex(_index).FindPropertyRelative("isLooping");
                var _clampProp = animationSettingsProp.GetArrayElementAtIndex(_index).FindPropertyRelative("clampedMode");
                AnimBool _loopBool = new AnimBool(_loopProp.boolValue);
                AnimBool _clampBool = new AnimBool(_clampProp.boolValue);
                _loopBool.valueChanged.AddListener(Repaint);
                _clampBool.valueChanged.AddListener(Repaint);
                settingAnimBool.Add((_loopBool, _clampBool));
            }  
        }

        private void DrawAnimationSetting(SerializedProperty aSetting, int aIndex)
        {
            var _loopProp = aSetting.FindPropertyRelative("isLooping");
            var _showClampingProp = aSetting.FindPropertyRelative("showClamping");
            var _clampedModeProp = aSetting.FindPropertyRelative("clampedMode");
            var (_showLooping, _showClampSettings) = settingAnimBool[aIndex];
            _showLooping.target = _loopProp.boolValue;
            _showClampSettings.target = _clampedModeProp.enumValueIndex > 0;
            InspectorUtility.BeginSubBoxGroup(aSetting.FindPropertyRelative("animationName").stringValue, EditorColors.Header, EditorColors.HeaderAlt1);
        
            //Looping
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_loopProp, new GUIContent(_loopProp.displayName, InspectorTooltips.LoopedAnimation));
            if (EditorGUILayout.BeginFadeGroup(_showLooping.faded))
            {
                var _ignoreProp = aSetting.FindPropertyRelative("ignoreLastFrame");
                EditorGUILayout.PropertyField(_ignoreProp, new GUIContent(_ignoreProp.displayName, InspectorTooltips.IgnoreFrameForLooping));
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndHorizontal();
        
            //Clamp Mode Selection
            if (_showClampingProp.boolValue)
            {
                EditorGUILayout.PropertyField(_clampedModeProp, new GUIContent(_clampedModeProp.displayName, InspectorTooltips.ClampedDetailedMessage));
                switch (_clampedModeProp.enumValueIndex)
                {
                    case (int)ClampedAnimation.ClampedMode.InsertEndFrame:
                        DrawEndFrameClamping(aSetting, _showClampSettings.faded);
                        break;
                    case (int) ClampedAnimation.ClampedMode.StretchExistingFrames:
                        DrawStretchExistingClamping(aSetting, _showClampSettings.faded);
                        break;
                }
            
            }
        
        
            InspectorUtility.EndBoxGroup();
        }

        private void DrawEndFrameClamping(SerializedProperty aSetting, float aFaded)
        {
            if (EditorGUILayout.BeginFadeGroup(aFaded))
            {
                var _endFrameMessageProp = aSetting.FindPropertyRelative("clampedFramesInfoMessage");
                EditorGUILayout.HelpBox(_endFrameMessageProp.stringValue, MessageType.None);
                EditorGUILayout.PropertyField(aSetting.FindPropertyRelative("insertRemoveLastFrame"), new GUIContent("Remove Previous Last Frame", InspectorTooltips.ClampedInsertRemoveLast));
            }
            EditorGUILayout.EndFadeGroup();
          
        }

        private void DrawStretchExistingClamping(SerializedProperty aSetting, float aFaded)
        {
            if (EditorGUILayout.BeginFadeGroup(aFaded))
            {
                var _endFrameMessageProp = aSetting.FindPropertyRelative("stretchFramesInfoMessage");
                EditorGUILayout.HelpBox(_endFrameMessageProp.stringValue, MessageType.None);
                var _indexProp = aSetting.FindPropertyRelative("stretchStartFrameIndex");
                EditorGUILayout.PropertyField(_indexProp, new GUIContent(_indexProp.displayName, InspectorTooltips.ClampedStretchFrames));
            }
            EditorGUILayout.EndFadeGroup();
        }
    
    
    }
}