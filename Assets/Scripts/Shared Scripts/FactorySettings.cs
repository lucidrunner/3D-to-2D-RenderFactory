using Render3DTo2D.Model_Settings;
using UnityEditor;
using UnityEngine;

namespace Shared_Scripts
{
    public class FactorySettings : ScriptableObject
    {
        public const string SettingsPath = "Assets/RenderFactory/Settings/FactorySettings.asset";

        [SerializeField] private EditorColors.EditorPalette editorPalette;
        [SerializeField] private EditorColors.ButtonPalette buttonPalette;
        [SerializeField] private GameObject overseerPrefab;

        
        [SerializeField, Tooltip(InspectorTooltips.FocusModelOnStartup)]
        private bool centerCameraOnRenderStartup = true;

        [SerializeField, Tooltip(InspectorTooltips.FollowModelOnRender)]
        private bool followCameraOnRender = true;

        public bool CenterCameraOnRenderStartup => centerCameraOnRenderStartup;

        public bool FollowCameraOnRender => followCameraOnRender;

        public GameObject OverseerPrefab => overseerPrefab;

        public EditorColors.ButtonPalette ButtonPalette => buttonPalette;

        public EditorColors.EditorPalette EditorPalette => editorPalette;

        public static FactorySettings GetOrCreateSettings()
        {
            var _settings = AssetDatabase.LoadAssetAtPath<FactorySettings>(SettingsPath);
            if (_settings != null) return _settings;

            _settings = ScriptableObject.CreateInstance<FactorySettings>();
            _settings.editorPalette = EditorColors.EditorPalette.BubbleGum;
            _settings.buttonPalette = EditorColors.ButtonPalette.Default;
            var _prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/RenderFactory/Prefabs" });
            GameObject _prefab = null;
            foreach (string _prefabGUID in _prefabGUIDs)
            {
                var _path = AssetDatabase.GUIDToAssetPath(_prefabGUID);
                var _checkedObjects = AssetDatabase.LoadAllAssetsAtPath(_path);
                foreach (var _object in _checkedObjects)
                {
                    var _gameObject = _object as GameObject;
                    if (_gameObject == null) continue;
                    //TODO Not magic string here
                    if (_gameObject.CompareTag("Overseer")) continue;
                    _prefab = _gameObject;
                    break;
                }

                if (_prefab != null)
                    break;
            }

            _settings.overseerPrefab = _prefab;
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