using System.Collections.Generic;
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
