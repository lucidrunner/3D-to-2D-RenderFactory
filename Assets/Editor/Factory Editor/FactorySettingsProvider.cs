using System.Collections.Generic;
using Render3DTo2D.Model_Settings;
using Shared_Scripts;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    static class FactorySettingsRegister
    {
        class Styles
        {
            public static GUIContent editorPalette = new GUIContent("Editor Palette");
            public static GUIContent buttonPalette = new GUIContent("Button Palette");
            public static GUIContent overseerPrefab = new GUIContent("Overseer Prefab");
        }

        [SettingsProvider]
        public static SettingsProvider CreateFactorySettingsProvider()
        {
            var _provider = new SettingsProvider("Preferences/RenderFactory", SettingsScope.User)
            {
                label = "Render Factory",
                guiHandler = (searchContext) =>
                {
                    var _settings = FactorySettings.GetSerializedSettings();
                    _settings.UpdateIfRequiredOrScript();
                    
                    //COLORS
                    DrawHeader("Factory Colors");
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(_settings.FindProperty("editorPalette"), Styles.editorPalette);
                    EditorGUILayout.PropertyField(_settings.FindProperty("buttonPalette"), Styles.buttonPalette);
                    var _changed = EditorGUI.EndChangeCheck();
                    
                    //PREFABS
                    DrawHeader("Prefabs");
                    EditorGUILayout.PropertyField(_settings.FindProperty("overseerPrefab"), Styles.overseerPrefab);
                    
                    //GENERAL SETTINGS
                    DrawHeader("Camera Follow Settings");
                    EditorGUILayout.PropertyField(_settings.FindProperty("followCameraOnRender"));
                    EditorGUILayout.PropertyField(_settings.FindProperty("centerCameraOnRenderStartup"));


                    _settings.ApplyModifiedProperties();
                    InspectorUtility.RepaintAll();
                },
                keywords = new HashSet<string>(new[] {Styles.editorPalette.text, Styles.buttonPalette.text, Styles.overseerPrefab.text})
                //TODO Add more keywords
            };
            return _provider;
        }

        [SettingsProvider]
        public static SettingsProvider CreateNamingSettingsProvider()
        {
            var _provider = new SettingsProvider("Preferences/RenderFactoryNaming", SettingsScope.User)
            {
                label = "Factory Naming Settings",
                guiHandler = (aSearchContext) =>
                {
                    //Get Settings
                    var _settings = NamingSettings.GetSerializedSettings();
                    _settings.UpdateIfRequiredOrScript();
                    
                    //Properties

                    var _useAnimationNameProp = _settings.FindProperty("useAnimationName");
                    var _includeRigTagProp = _settings.FindProperty("includeRigTag");
                    var _useFormatIdentifierProp = _settings.FindProperty("includeFormatIdentifier");
                    var _includeStaticTagProp = _settings.FindProperty("includeStaticTag");
                    var _renderNameFormatProp = _settings.FindProperty("renderNameFormat");

                    var _target = NamingSettings.GetOrCreateSettings();
                    
                    //NAMING
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Current Name Format Example", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField(_target.ExampleOutput, EditorStyles.boldLabel);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.PropertyField(_useAnimationNameProp, new GUIContent("Use Animation Name over Index", InspectorTooltips.UseAnimationName));
                    EditorGUILayout.PropertyField(_includeRigTagProp, new GUIContent(_includeRigTagProp.displayName, InspectorTooltips.IncludeRigTag));
                    EditorGUILayout.PropertyField(_useFormatIdentifierProp, new GUIContent(_useFormatIdentifierProp.displayName, InspectorTooltips.AddIdentifiers));
                    EditorGUILayout.PropertyField(_includeStaticTagProp, new GUIContent("Include Static Tag (If Applicable)", InspectorTooltips.IncludeStaticTag));
                    InspectorUtility.BeginSubBoxGroup("Format Order", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                    var _color = GUI.backgroundColor;
                    for (int _index = 0; _index < _renderNameFormatProp.arraySize; _index++)
                    {
                        var _prop = _renderNameFormatProp.GetArrayElementAtIndex(_index);
                        GUI.backgroundColor = EditorColors.BodyAlt2;
                        EditorGUILayout.BeginHorizontal(FactoryStyles.ClosedSubBoxGroup);
                        GUI.backgroundColor = _color;
                        EditorGUILayout.LabelField(_prop.stringValue, EditorStyles.boldLabel);
                        if (InspectorUtility.DrawButton(new GUIContent("▲"), EditorColors.DefaultButton))
                        {
                            _target.MoveFormatLeft(_index);
                        }

                        if (InspectorUtility.DrawButton(new GUIContent("▼"), EditorColors.DefaultButton))
                        {
                            _target.MoveFormatRight(_index);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (InspectorUtility.DrawButton(new GUIContent("Reset Default Order"), EditorColors.DefaultButton))
                        _target.ResetFormat();
                    InspectorUtility.EndBoxGroup();

                    _settings.ApplyModifiedProperties();
                    InspectorUtility.RepaintAll();
                },
                keywords = new HashSet<string>()
            };
            return _provider;
        }

        private static void DrawHeader(string aLabel)
        {
            GUILayout.Space(5f);
            GUILayout.Label(aLabel, new GUIStyle(EditorStyles.boldLabel));
            InspectorUtility.GuiLine(aSpaceBefore: -5, aSpaceAfter:1);
        }
        
    }
}
