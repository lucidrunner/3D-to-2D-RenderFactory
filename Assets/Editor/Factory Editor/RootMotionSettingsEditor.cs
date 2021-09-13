using System;
using System.Collections.Generic;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.Utility.Inspector;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(RootMotionSettings))]
    public class RootMotionSettingsEditor : Editor
    {
        [Serializable]
        internal class SettingState
        {
            [SerializeField] internal AnimBool displayExportOverrideButton;
            [SerializeField] internal AnimBool displayExportTransform;
            [SerializeField] internal AnimBool displayFollowOverrideButton;
            [SerializeField] internal AnimBool displayFollowTransform;
            [SerializeField] internal AnimBool displayClampingSettings;
            internal SerializedProperty AnimationNameProp;
            internal SerializedProperty EnableExportProp;
            internal SerializedProperty EnableFollowProp;
            internal SerializedProperty OverrideExportProp;
            internal SerializedProperty OverrideFollowProp;
            internal SerializedProperty ClampMovementProp;
            internal SerializedProperty ClampRotationProp;
            internal SerializedProperty ExportOverrideTransformProp;
            internal SerializedProperty FollowOverrideTransformProp;

            internal SettingState(SerializedProperty aSetting, RootMotionSettingsEditor aEditor)
            {
                Debug.Log("Creating new SettingState");
                AnimationNameProp = aSetting.FindPropertyRelative("animationName");
                EnableExportProp = aSetting.FindPropertyRelative("enableExport");
                EnableFollowProp = aSetting.FindPropertyRelative("enableFollow");
                OverrideExportProp = aSetting.FindPropertyRelative("overrideDefaultExport");
                OverrideFollowProp = aSetting.FindPropertyRelative("overrideDefaultFollow");
                ClampMovementProp = aSetting.FindPropertyRelative("forceMovementClamp");
                ClampRotationProp = aSetting.FindPropertyRelative("forceRotationClamp");
                ExportOverrideTransformProp = aSetting.FindPropertyRelative("overrideExportTransform");
                FollowOverrideTransformProp = aSetting.FindPropertyRelative("overrideFollowTransform");
                
                Debug.Log("AnimationNameProp is null: " + AnimationNameProp == null);
                
                displayExportOverrideButton = new AnimBool(EnableExportProp.boolValue);
                displayFollowOverrideButton = new AnimBool(EnableFollowProp.boolValue);
                displayExportOverrideButton.valueChanged.AddListener(aEditor.Repaint);
                displayFollowOverrideButton.valueChanged.AddListener(aEditor.Repaint);

                displayExportTransform = new AnimBool(OverrideExportProp.boolValue);
                displayFollowTransform = new AnimBool(OverrideFollowProp.boolValue);
                displayExportTransform.valueChanged.AddListener(aEditor.Repaint);
                displayFollowTransform.valueChanged.AddListener(aEditor.Repaint);

                displayClampingSettings = new AnimBool(!OverrideFollowProp.boolValue);
                displayClampingSettings.valueChanged.AddListener(aEditor.Repaint);
            
            }
        }

        [SerializeField] private AnimBool displayRootMotion;
        [SerializeField] private AnimBool displayDefault;
        [SerializeField] private AnimBool displayRootMotionList;
        [SerializeField] private AnimBool displayDefaultFollowSettings;
        [SerializeField] private AnimBool displayClampingSettings;
        [SerializeField] private List<SettingState> settingStates = new List<SettingState>();
        private SerializedProperty rootMotionExportProp;
        private SerializedProperty applyDefaultRootMovementProp;
        private SerializedProperty forceMovementClampProp;
        private SerializedProperty forceRotationClampProp;
        private SerializedProperty defaultExportProp;
        private SerializedProperty defaultFollowProp;
        private SerializedProperty settingListProp;
        [SerializeField] private bool currentRootMotionTab;
        [SerializeField] private bool rootMotionFoldoutTarget;
        [SerializeField] private AnimBool rootMotionFoldoutCurrent;


        private void OnEnable()
        {
            rootMotionExportProp = serializedObject.FindProperty("enableRootMotionExport");
            applyDefaultRootMovementProp = serializedObject.FindProperty("applyDefaultRootMovement");
            forceMovementClampProp = serializedObject.FindProperty("forceMovementClamp");
            forceRotationClampProp = serializedObject.FindProperty("forceRotationClamp");
            defaultExportProp = serializedObject.FindProperty("defaultExport");
            defaultFollowProp = serializedObject.FindProperty("defaultFollow");
            settingListProp = serializedObject.FindProperty("rootMotionSettings");
            displayRootMotion = new AnimBool(currentRootMotionTab);
            displayDefault = new AnimBool(!currentRootMotionTab);
            displayRootMotionList = new AnimBool(rootMotionExportProp.boolValue);
            displayDefaultFollowSettings = new AnimBool(!rootMotionExportProp.boolValue);
            displayClampingSettings = new AnimBool(applyDefaultRootMovementProp.boolValue);
            rootMotionFoldoutCurrent = new AnimBool(rootMotionFoldoutTarget);
            displayRootMotion.valueChanged.AddListener(Repaint);
            displayDefault.valueChanged.AddListener(Repaint);
            displayRootMotionList.valueChanged.AddListener(Repaint);
            displayDefaultFollowSettings.valueChanged.AddListener(Repaint);
            displayClampingSettings.valueChanged.AddListener(Repaint);
            rootMotionFoldoutCurrent.valueChanged.AddListener(Repaint);
            
            
            RefreshStateList();
        }


        private void RefreshStateList()
        {
            settingStates = new List<SettingState>();
            for (int _index = 0; _index < settingListProp.arraySize; _index++)
            {
                var _settingProp = settingListProp.GetArrayElementAtIndex(_index);
                settingStates.Add(new SettingState(_settingProp, this));
            }
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RootMotionSettings) target), typeof(RootMotionSettings), false);
            GUI.enabled = true;
            //Ugly check needed to sync our states on reset:ing the RootMotionSettings-script
            var _target = (RootMotionSettings) target;
            if(_target.HasReset)
            {
                RefreshStateList();
                _target.HasReset = false;
            }


            EditorGUILayout.BeginHorizontal();
            var _skin = new GUIStyle(GUI.skin.button);
            EditorColors.SetColorForStyle(_skin, EditorColors.TextColor);
            var _prevColor = GUI.backgroundColor;
            GUI.backgroundColor = currentRootMotionTab ? EditorColors.ActiveTab : EditorColors.InActiveTab;
            if (GUILayout.Button("Animation Root Motion Settings", new GUIStyle(currentRootMotionTab ? _skin : GUI.skin.button){margin = new RectOffset(0,-1,0,0)}, GUILayout.MaxWidth(Screen.width / 2f)))
            {
                displayDefault.target = false;
                displayRootMotion.target = true;
                currentRootMotionTab = true;
            }

            GUI.backgroundColor = currentRootMotionTab ? EditorColors.InActiveTab : EditorColors.ActiveTab;

            if (GUILayout.Button("Default RootMotionSettings", new GUIStyle(currentRootMotionTab ? GUI.skin.button : _skin){margin = new RectOffset(-1,0,0,0)}, GUILayout.MaxWidth(Screen.width / 2f)))
            {
                displayRootMotion.target = false;
                displayDefault.target = true;
                currentRootMotionTab = false;
            }
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = _prevColor;
        
        
            if(currentRootMotionTab)
            {
                if(EditorGUILayout.BeginFadeGroup(displayRootMotion.faded))
                    DrawRootMotionSettings();
            }
            else
            {
                if(EditorGUILayout.BeginFadeGroup(displayDefault.faded))
                    DrawDefault();
            }
        
            EditorGUILayout.EndFadeGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRootMotionSettings()
        {
            displayRootMotionList.target = rootMotionExportProp.boolValue;
            rootMotionExportProp.boolValue = InspectorUtility.DrawToggleButton(rootMotionExportProp.boolValue, new GUIContent(rootMotionExportProp.displayName), InspectorUtility.ButtonSize.Large);
            displayDefaultFollowSettings.target = !rootMotionExportProp.boolValue;
        
            EditorColors.OverrideTextColors();
            //Show List
            if (EditorGUILayout.BeginFadeGroup(displayRootMotionList.faded))
            {
                bool _displayFoldout = InspectorUtility.BeginSubFoldoutGroup("Individual Animation Settings", ref rootMotionFoldoutTarget, ref rootMotionFoldoutCurrent);
                if(_displayFoldout)
                {
                    foreach (var _settingState in settingStates)
                    {
                        InspectorUtility.BeginSubBoxGroup(_settingState.AnimationNameProp.stringValue, EditorColors.Header, EditorColors.Body);
                        DrawSetting(_settingState);
                        InspectorUtility.EndSubBoxGroup();
                    }
                }

                InspectorUtility.EndFoldoutGroup(_displayFoldout);
        
                if(InspectorUtility.DrawButton(new GUIContent("Reload Animation List"), EditorColors.ButtonRunAlt))
                {
                    var _target = (RootMotionSettings) target;
                    _target.Reset();
                    serializedObject.Update();
                    RefreshStateList();
                }
            }
            EditorGUILayout.EndFadeGroup();
            
            
        
            //Otherwise, show default follow buttons
            if (EditorGUILayout.BeginFadeGroup(displayDefaultFollowSettings.faded))
            {
                applyDefaultRootMovementProp.boolValue = InspectorUtility.DrawToggleButton(applyDefaultRootMovementProp.boolValue, new GUIContent(applyDefaultRootMovementProp.displayName));
                displayClampingSettings.target = !applyDefaultRootMovementProp.boolValue;
                if (EditorGUILayout.BeginFadeGroup(displayClampingSettings.faded))
                {
                    EditorGUILayout.BeginHorizontal();
                    forceMovementClampProp.boolValue = InspectorUtility.DrawToggleButton(forceMovementClampProp.boolValue, new GUIContent(forceMovementClampProp.displayName));
                    forceRotationClampProp.boolValue = InspectorUtility.DrawToggleButton(forceRotationClampProp.boolValue, new GUIContent(forceRotationClampProp.displayName));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();
        
            EditorColors.ResetTextColor();
        
        }

        private void DrawSetting(SettingState aSettingState)
        {
            //Export
            InspectorUtility.BeginSubBoxGroup("Export Settings", EditorColors.HeaderAlt1, EditorColors.BodyAlt2);
            aSettingState.EnableExportProp.boolValue = InspectorUtility.DrawToggleButton(aSettingState.EnableExportProp.boolValue, new GUIContent("Enable Motion Export"), InspectorUtility.ButtonSize.Large);
            aSettingState.displayExportOverrideButton.target = aSettingState.EnableExportProp.boolValue;
            if (EditorGUILayout.BeginFadeGroup(aSettingState.displayExportOverrideButton.faded))
            {
                aSettingState.OverrideExportProp.boolValue = InspectorUtility.DrawToggleButton(aSettingState.OverrideExportProp.boolValue, new GUIContent(aSettingState.OverrideExportProp.displayName));
                aSettingState.displayExportTransform.target = aSettingState.OverrideExportProp.boolValue;
                if (EditorGUILayout.BeginFadeGroup(aSettingState.displayExportTransform.faded))
                {
                    InspectorUtility.DrawToggleTransform(aSettingState.ExportOverrideTransformProp, false);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();
            InspectorUtility.EndSubBoxGroup();
        
        
            //Follow
            InspectorUtility.BeginSubBoxGroup("Camera Follow Settings", EditorColors.HeaderAlt1, EditorColors.BodyAlt2);
            aSettingState.EnableFollowProp.boolValue = InspectorUtility.DrawToggleButton(aSettingState.EnableFollowProp.boolValue, new GUIContent("Enable Camera Follow"), InspectorUtility.ButtonSize.Large);

            aSettingState.displayFollowOverrideButton.target = aSettingState.EnableFollowProp.boolValue;
            if (EditorGUILayout.BeginFadeGroup(aSettingState.displayFollowOverrideButton.faded))
            {
                aSettingState.OverrideFollowProp.boolValue = InspectorUtility.DrawToggleButton(aSettingState.OverrideFollowProp.boolValue, new GUIContent(aSettingState.OverrideFollowProp.displayName));
                aSettingState.displayFollowTransform.target = aSettingState.OverrideFollowProp.boolValue;
                if (EditorGUILayout.BeginFadeGroup(aSettingState.displayFollowTransform.faded))
                {
                    InspectorUtility.DrawToggleTransform(aSettingState.FollowOverrideTransformProp, false);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();
        
            //Follow Force Clamp
            aSettingState.displayClampingSettings.target = !aSettingState.EnableFollowProp.boolValue;
            if (EditorGUILayout.BeginFadeGroup(aSettingState.displayClampingSettings.faded))
            {
                EditorGUILayout.BeginHorizontal();
                aSettingState.ClampMovementProp.boolValue = InspectorUtility.DrawToggleButton(aSettingState.ClampMovementProp.boolValue, new GUIContent("Force Clamp Movement"));
                aSettingState.ClampRotationProp.boolValue = InspectorUtility.DrawToggleButton(aSettingState.ClampRotationProp.boolValue, new GUIContent("Force Clamp Rotation"));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFadeGroup();
            InspectorUtility.EndSubBoxGroup();

        }

        private void DrawDefault()
        {
            EditorColors.OverrideTextColors();
            InspectorUtility.DrawToggleTransform(defaultExportProp);
            InspectorUtility.DrawToggleTransform(defaultFollowProp);
            EditorColors.ResetTextColor();
        }
    }
}
 