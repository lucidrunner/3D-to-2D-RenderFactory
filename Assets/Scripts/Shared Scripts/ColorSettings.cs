using UnityEditor;
using UnityEngine;

namespace Shared_Scripts
{
    public class ColorSettings : SettingsBase
    {
        public const string SettingsPath = SettingsBasePath + "ColorSettings.asset";
        
        [SerializeField] private EditorColors.EditorPalette editorPalette;
        [SerializeField] private EditorColors.ButtonPalette buttonPalette;
        public EditorColors.ButtonPalette ButtonPalette => buttonPalette;
        public EditorColors.EditorPalette EditorPalette => editorPalette;

        public static ColorSettings GetOrCreateSettings()
        {
            var _settings = AssetDatabase.LoadAssetAtPath<ColorSettings>(SettingsPath);
            if (_settings != null) return _settings;

            _settings = ScriptableObject.CreateInstance<ColorSettings>();
            _settings.editorPalette = EditorColors.EditorPalette.BubbleGum;
            _settings.buttonPalette = EditorColors.ButtonPalette.Default;
            
            AssetDatabase.CreateAsset(_settings, SettingsPath);
            AssetDatabase.SaveAssets();
            return _settings;
        }
        
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}