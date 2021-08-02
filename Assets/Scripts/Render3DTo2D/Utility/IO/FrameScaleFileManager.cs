#if UNITY_EDITOR
using System.Collections.Generic;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.SMAnimator;
using UnityEditor;
using UnityEngine;

namespace Render3DTo2D.Utility.IO
{
    public static class FrameScaleFileManager
    {
        private const string FRAMESCALE_SUFFIX = "_FrameScales.asset";
        private const string SCALEDATA_FOLDERNAME = "ScaleData";
        
        public static void CreateFrameScaleAsset(string aFolderName,string aFileName, List<RigFrameScaleFile.AnimationCameraScales> aFrameScales, List<string> aAnimationNames, Transform aFactoryTransform)
        {
            RigFrameScaleFile _asset = ScriptableObject.CreateInstance<RigFrameScaleFile>();
            _asset.Init(aFrameScales, aAnimationNames, RenderingSettings.GetFor(aFactoryTransform).GetRenderingValueHash(), AdvancedAnimationSettings.GetFor(aFactoryTransform)?.GetValueHash(), RootMotionSettings.GetFor(aFactoryTransform)?.GetValueHash());

            string _pathName = $"Assets/{SCALEDATA_FOLDERNAME}/{aFolderName}";
            if (!AssetDatabase.IsValidFolder(_pathName))
            {
                AssetDatabase.CreateFolder($"Assets/{SCALEDATA_FOLDERNAME}", aFolderName);
            }
            
            string _outputName = $"{_pathName}/{aFileName}{FRAMESCALE_SUFFIX}";
            FLogger.LogMessage(null, FLogger.Severity.Status, $"Creating Scale File at {_outputName}", nameof(FrameScaleFileManager));
            AssetDatabase.CreateAsset(_asset, _outputName);
            AssetDatabase.SaveAssets();
        }

        public static void RemoveFrameScaleAsset(string aFolderName, string aFileName)
        {
            //See if the asset exists
            RigFrameScaleFile _asset = AssetDatabase.LoadAssetAtPath<RigFrameScaleFile>($"Assets/{SCALEDATA_FOLDERNAME}/{aFolderName}/{aFileName}{FRAMESCALE_SUFFIX}");
            if (_asset == null) return;
            
            //If it does, remove it
            AssetDatabase.DeleteAsset($"Assets/{SCALEDATA_FOLDERNAME}/{aFolderName}/{aFileName}{FRAMESCALE_SUFFIX}");
            AssetDatabase.SaveAssets();
        }

        public static RigFrameScaleFile LoadFrameScaleFromAsset(string aFolderName, string aFileName)
        {
            RigFrameScaleFile _asset = AssetDatabase.LoadAssetAtPath<RigFrameScaleFile>($"Assets/{SCALEDATA_FOLDERNAME}/{aFolderName}/{aFileName}{FRAMESCALE_SUFFIX}");
            return _asset;
        }
    }
}


#endif