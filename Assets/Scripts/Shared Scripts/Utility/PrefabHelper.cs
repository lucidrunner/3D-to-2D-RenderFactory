using UnityEditor;
using UnityEngine;

namespace Shared_Scripts.Utility
{
    public static class PrefabHelper
    {
        public static GameObject FindPrefabWithTag(string aTag)
        {
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
                    if (_gameObject.CompareTag(aTag)) continue;
                    _prefab = _gameObject;
                    break;
                }

                if (_prefab != null)
                    break;
            }

            return _prefab;
        }
    }
}