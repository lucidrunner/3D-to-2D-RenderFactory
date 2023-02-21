using Render3DTo2D.Model_Settings;
using Shared_Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Shared_Scripts
{
    public class FactorySettings : SettingsBase
    {
        public const string SettingsPath = SettingsBasePath +  "FactorySettings.asset";

        [SerializeField] private GameObject overseerPrefab;

        [SerializeField, Tooltip(InspectorTooltips.FocusModelOnStartup)]
        private bool centerCameraOnRenderStartup = true;

        [SerializeField, Tooltip(InspectorTooltips.FollowModelOnRender)]
        private bool followCameraOnRender = true;

        public bool CenterCameraOnRenderStartup => centerCameraOnRenderStartup;

        public bool FollowCameraOnRender => followCameraOnRender;

        public GameObject OverseerPrefab => overseerPrefab;


        public static FactorySettings GetOrCreateSettings()
        {
            //Get
            var _settings = AssetDatabase.LoadAssetAtPath<FactorySettings>(SettingsPath);
            if (_settings != null) return _settings;
            
            //Create if get fails
            _settings = ScriptableObject.CreateInstance<FactorySettings>();
            var _prefab = PrefabHelper.FindPrefabWithTag("Overseer");
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