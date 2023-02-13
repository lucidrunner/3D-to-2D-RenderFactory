using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Render3DTo2D.Isometric;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.IO;
using RootChecker;
using Shared_Scripts;
using UnityEngine;
using Formatting = Newtonsoft.Json.Formatting;

namespace Render3DTo2D.Render_Info_Export
{
    public static class RigDataJsonExporter
    {
        
        /* Write Order
         * TODO Update this
         * Document Start / Root Node
         *     Model Name
         *     Rig Tag
         * MetaData Node
         *     Baseline Scale
         *     Default Texture Size
         *     Isometric Angle (Iso)
         *     Use Sub Folder
         * Camera Setup Node
         *     Number of Cameras
         *     Y-Rotations: [x, x, x, etc times Number of Cameras]
         * Animation Setup Node (Non-static)
         *     Name Format: Attributes Include Prefix, Use Animation Name  
         *     FPS
         *     Animation Count
         * Animations Node (Non-static)
         *    X# of Animation Nodes
         *      Attributes: Animation Length, Clamped, (Iso) Isometric Y-offset
         *      Element: Animation Name
         */
        #region Folder Name

        private const string RigDataFolderName = "Data";
        private const string FileName = "RenderInfo.json";

        #endregion

        private static readonly JsonSerializerSettings options =
            new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Ignore};
        
        public static void Export(RigRenderExportArgs aExportArgs)
        {
            Output _output = BuildOutput(aExportArgs);
            string _jsonString = JsonConvert.SerializeObject(_output, Formatting.Indented, options);
            
            
            //Create a subfolder for the animation data
            FolderCreator.CreateFolder(aExportArgs.LastOutputPath, RigDataFolderName);

            //Write the json to the file
            (string _path, _) = ExportHelperMethods.GetFilePathInfo(aExportArgs, RigDataFolderName, FileName);
            File.WriteAllText(_path, _jsonString);
        }

        private static Output BuildOutput(RigRenderExportArgs aExportArgs)
        {
            CameraRig _cameraRig = aExportArgs.CameraRig;

            var _output = new Output()
            {
                ModelName = _cameraRig.GetModelName(),
                RigTag = _cameraRig.RigTag,
                MetaData = BuildMetaData(aExportArgs),
                CameraData = BuildCameraData(aExportArgs.CameraRig),
                AnimationData = BuildAnimationData(aExportArgs)
            };

            return _output;
        }

        private static Output.OutputMetadata BuildMetaData(RigRenderExportArgs aExportArgs)
        {
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(aExportArgs.CameraRig.transform);
            FolderSettings _folderSettings = FolderSettings.GetFor(aExportArgs.CameraRig.transform);
            bool _isometricWrite = aExportArgs.CameraRig.GetComponent<IsometricRigRenderer>() != null;
            ModelBaseManager _baseManager = null;
            if (_isometricWrite)
            {
                _baseManager = aExportArgs.CameraRig.GetComponent<ModelBaseManager>();
            }
            
            //Get the correct base line scale depending on if we're exporting isometric stuff or not
            float _baselineScale = _isometricWrite
                ? _renderingSettings.IsometricBaseline
                : _renderingSettings.BaselineScale;
            int _baseTextureSize = _renderingSettings.BaseTextureSize;
            
            //If we're exporting Root Motion, add a relative path
            string _rootPath = null;
            if (aExportArgs.RootMotionFilePath != null)
            {
                //Get the Root Path based on the LastOutputPath
                var _root = new DirectoryInfo(aExportArgs.LastOutputPath).Parent?.FullName;
                if (_root != null)
                {
                    //We have to do the Path.GetFullPath trick since the original FilePath can give us \ or / as a separation character, while the DirectoryInfo only gives us one of them
                    _rootPath = Path.GetFullPath(aExportArgs.RootMotionFilePath).Replace(_root, "ROOT");
                }
            }
            
            //Finally, package everything in our metadata inner class
            return new Output.OutputMetadata()
            {
                StartTimestamp = aExportArgs.TimeStamp,
                BaselineScale = _baselineScale,
                BaseTextureSize = _baseTextureSize,
                StaticFolderName = _folderSettings.StaticFolderName,
                AnimationsInSubFolders = _folderSettings.CreateAnimationSubfolders,
                RootMotionPath = _rootPath,
                IsometricInfo = _isometricWrite ? new Output.OutputMetadata.IsometricMetaData
                {
                    CameraAngle = (float)Math.Round(_baseManager.CameraAngle, 2),
                    ModelYOffset = (float)Math.Round(_baseManager.BaseReferenceOffset, 4)
                }: null
            };
        }

        private static Output.OutputCameraData BuildCameraData(CameraRig aCameraRig)
        {
            //Get the camera transforms from the rig
            List<Transform> _cameraTransforms = new List<Transform>();
            for (int _index = 0; _index < aCameraRig.CameraAnchor.transform.childCount; _index++)
            {
                _cameraTransforms.Add(aCameraRig.CameraAnchor.transform.GetChild(_index).transform);
            }

            return new Output.OutputCameraData()
            {
                CameraCount = _cameraTransforms.Count,
                CameraAngles = _cameraTransforms.Select(t => t.localEulerAngles.y).ToArray()
            };
        }

        private static Output.OutputAnimationData BuildAnimationData(RigRenderExportArgs aExportArgs)
        {
            var _cameraRig = aExportArgs.CameraRig;
            var _smAnimatorInfo = aExportArgs.SmAnimatorInfo;
            //Get the data we want to write
            RenderingSettings _settings = RenderingSettings.GetFor(_cameraRig.transform);
            List<string> _animationNames = _smAnimatorInfo.AnimationNames.Select(GeneralUtilities.CleanName).ToList();
            Dictionary<int, int> _animationLengths = _cameraRig.GetComponent<RigRenderer>().LatestAnimationLengths;
            
            //Get the render name format
            var _namingSettings = NamingSettings.GetOrCreateSettings();
            string _format = string.Join("", _namingSettings.RenderNameFormat);

            List<Output.OutputAnimationData.OutputAnimation> _animations = new List<Output.OutputAnimationData.OutputAnimation>();
            
            for (int _index = 0; _index < _animationNames.Count; _index++)
            {
                var _animationSetting = _smAnimatorInfo.AnimationSettings.Find(aSetting => aSetting.AnimationName.Equals(_animationNames[_index]));
                ClampedAnimation _clampedAnimation = _smAnimatorInfo.GetClampedAnimation(_animationNames[_index]);
                ModelBaseManager _isometricBaseManager = _cameraRig.GetComponent<ModelBaseManager>();

                _animations.Add(new Output.OutputAnimationData.OutputAnimation()
                {
                    AnimationName = _animationNames[_index],
                    AnimationIndex = _index,
                    Length = _animationLengths.ContainsKey(_index) ? _animationLengths[_index] : (int?)null,
                    Loop = _animationSetting?.IsLooping,
                    LastFrameIgnored = _animationSetting?.IgnoreLastFrame,
                    Clamped = _clampedAnimation != null,
                    ClampedMode = _clampedAnimation?.Clamping.ToString(),
                    AnimationYOffset = _isometricBaseManager != null ? _isometricBaseManager.GetOffsetForAnimation(_index) >= 0 ? _isometricBaseManager.GetOffsetForAnimation(_index) : (float?) null : null,
                });
            }

            return new Output.OutputAnimationData()
            {
                NameFormat = _format,
                UsePrefix = _namingSettings.IncludeFormatIdentifier,
                UseAnimationNameOverIndex = _namingSettings.UseAnimationName,
                FramesPerSecond = _settings.AnimationFPS,
                AnimationCount = _animationNames.Count,
                Animations = _animations.ToArray()
            };
        }

        private class Output
        {
            public string ModelName { get; set; }
            public string RigTag { get; set; }

            public OutputMetadata MetaData { get; set; }
            
            public OutputCameraData CameraData { get; set; }
            
            public OutputAnimationData AnimationData { get; set; }
            
            public class OutputMetadata
            {
                public string StartTimestamp { get; set; }
                public float BaselineScale { get; set; }
                public int BaseTextureSize { get; set; }
                public string StaticFolderName { get; set; }
                public bool AnimationsInSubFolders { get; set; }
                public string RootMotionPath { get; set; }
                public IsometricMetaData IsometricInfo { get; set; }
                
                public class IsometricMetaData
                {
                    public float CameraAngle { get; set; }
                    public float ModelYOffset { get; set; }
                }
            }

            public class OutputCameraData
            {
                public int CameraCount { get; set; }
                public float[] CameraAngles { get; set; }
            }

            public class OutputAnimationData
            {
                public string NameFormat { get; set; }
                public bool UsePrefix { get; set; }
                public bool UseAnimationNameOverIndex { get; set; }
                public int FramesPerSecond { get; set; }
                public int AnimationCount { get; set; }

                public OutputAnimation[] Animations { get; set; }
                
                public class OutputAnimation
                {
                    public string AnimationName { get; set; }
                    public int AnimationIndex { get; set; }
                    public int? Length { get; set; }
                    public bool? Loop { get; set; }
                    public bool? LastFrameIgnored { get; set; }
                    public bool Clamped { get; set; }
                    public string ClampedMode { get; set; }
                    public float? AnimationYOffset { get; set; }
                }
            }
        }
    }
}
