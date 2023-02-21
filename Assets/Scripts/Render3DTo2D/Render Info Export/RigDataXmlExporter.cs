using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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


namespace Render3DTo2D.Render_Info_Export
{
    
    public static class RigDataXmlExporter
    {
        private static ModelBaseManager _baseManager;
        #region Folder Name

        private const string RigDataFolderName = "Data";
        private const string FileName = "RenderInfo.xml";

        #endregion
        
        
        #region Public Methods
        public static void Export(RigRenderExportArgs aExportArgs)
        {
            //Side view / isometric write differences are handled by the different subfunctions
            using (XmlWriter _xmlWriter = WriteDocumentStart(aExportArgs))
            {
                //Report that we're exporting
                FLogger.LogMessage(null, FLogger.Severity.Priority, $"Writing Rig Rendering Data at {aExportArgs.LastOutputPath}", nameof(RigDataXmlExporter));
                
                //Write meta data
                WriteMetadata(aExportArgs, _xmlWriter);
                
                WriteCameraInfo(aExportArgs.CameraRig, _xmlWriter);
                
                //Write the animation lists if we're not in a static write
                if(aExportArgs.SmAnimatorInfo != null)
                {
                    WriteAnimations(aExportArgs.CameraRig, aExportArgs.SmAnimatorInfo, _xmlWriter);
                }

                //Write the ending tags
                XmlMethods.WriteDocumentEnd(_xmlWriter);
            }
        }
            
        #endregion
        
        #region Private Methods

        private static XmlWriter WriteDocumentStart(RigRenderExportArgs aExportArgs)
            {
                //Create a subfolder for the animation data
                FolderCreator.CreateFolder(aExportArgs.LastOutputPath, RigDataFolderName);

                //Get all the attributes we're interested in writing
                string _modelName = aExportArgs.CameraRig.GetModelName();
                string _rigTag = aExportArgs.CameraRig.RigTag;
                
                //Get the file name, path and start the writer
                (string _path, _) = ExportHelperMethods.GetFilePathInfo(aExportArgs, RigDataFolderName, FileName);

                XmlWriterSettings _settings = new XmlWriterSettings();
                _settings.Indent = true;
                _settings.IndentChars = "\t";
                
                XmlWriter _xmlWriter = XmlWriter.Create(_path, _settings);

                //Write the start of the document and the root node
                _xmlWriter.WriteStartDocument();
                _xmlWriter.WriteStartElement(XmlTags.RENDER_ROOT);
                
                //Write the model name
                XmlMethods.WriteStringElement(_xmlWriter, XmlTags.NAME, _modelName);
                //Write the tag of the rig
                XmlMethods.WriteStringElement(_xmlWriter, XmlTags.RIGTAG, _rigTag);

                return _xmlWriter;
            }



        private static void WriteMetadata(RigRenderExportArgs aExportArgs, XmlWriter aXmlWriter)
        {
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(aExportArgs.CameraRig.transform);
            FolderSettings _folderSettings = FolderSettings.GetFor(aExportArgs.CameraRig.transform);
            bool _isometricWrite = aExportArgs.CameraRig.GetComponent<IsometricRigRenderer>() != null;

            //Get the correct base line scale depending on if we're exporting isometric stuff or not
            float _baselineScale = _isometricWrite
                ? _renderingSettings.IsometricBaseline
                : _renderingSettings.BaselineScale;
            int _baseTextureSize = _renderingSettings.BaseTextureSize;

            //Write the setup info
            aXmlWriter.WriteStartElement(XmlTags.METADATA);
            //Write the render start timestamp
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.TIMESTAMP, aExportArgs.TimeStamp);
            //Write the baseline stuff here
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.BASELINE_SCALE, _baselineScale.ToString(CultureInfo.InvariantCulture));
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.DEFAULT_TEXTURE_SIZE, _baseTextureSize.ToString());
            //Write Folder stuff
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.STATIC_FOLDER, _folderSettings.StaticFolderName);
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.SUBFOLDERS, _folderSettings.CreateAnimationSubfolders.ToString());
            
            //If we're exporting Root Motion, add a relative path
            if (aExportArgs.RootMotionFilePath != null)
            {
                //Get the Root Path based on the LastOutputPath
                var _root = new DirectoryInfo(aExportArgs.LastOutputPath).Parent?.FullName;
                if (_root != null)
                {
                    //We have to do the Path.GetFullPath trick since the original FilePath can give us \ or / as a separation character, while the DirectoryInfo only gives us one of them
                    XmlMethods.WriteStringElement(aXmlWriter, XmlTags.ROOT_FILEPATH, Path.GetFullPath(aExportArgs.RootMotionFilePath).Replace(_root, "ROOT"));
                }
            }
            
            if (_isometricWrite)
            {
                //Get the angle and write it for an isometric rig
                _baseManager = aExportArgs.CameraRig.GetComponent<ModelBaseManager>();
                float _isometricAngle = _baseManager.CameraAngle;
                float _isometricOffset = _baseManager.BaseReferenceOffset; 
                XmlMethods.WriteStringElement(aXmlWriter, XmlTags.ISOMETRIC_ANGLE, _isometricAngle.ToString(CultureInfo.InvariantCulture));
                XmlMethods.WriteStringElement(aXmlWriter, XmlTags.ISOMETRIC_OFFSET, _isometricOffset.ToString(CultureInfo.InvariantCulture));
            }
            
            aXmlWriter.WriteEndElement();

        }

        private static void WriteCameraInfo(CameraRig aCameraRig, XmlWriter aXmlWriter)
        {
            //Get the camera transforms from the rig
            List<Transform> _cameraTransforms = new List<Transform>();
            for (int _index = 0; _index < aCameraRig.CameraAnchor.transform.childCount; _index++)
            {
                _cameraTransforms.Add(aCameraRig.CameraAnchor.transform.GetChild(_index).transform);
            }
            
            //Write the camera info header
            aXmlWriter.WriteStartElement(XmlTags.CAMERA_SETUP);
            //Write our camera count
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.CAMERA_COUNT, _cameraTransforms.Count.ToString(CultureInfo.InvariantCulture));
            //And write our angles as a comma separated list
            XmlMethods.WriteStringElement(aXmlWriter, XmlTags.CAMERA_ANGLES, string.Join(",", _cameraTransforms.Select(t => t.localEulerAngles.y.ToString(CultureInfo.InvariantCulture))));
            aXmlWriter.WriteEndElement();
        }

        private static void WriteAnimations(CameraRig aCameraRig, StopMotionAnimatorInfo aSmAnimatorInfo, XmlWriter aXMLWriter)
        {
            //Get the data we want to write
            RenderingSettings _settings = RenderingSettings.GetFor(aCameraRig.transform);
            int _fps = _settings.AnimationFPS;
            List<string> _animationNames = aSmAnimatorInfo.AnimationNames.Select(GeneralUtilities.CleanName).ToList();
            Dictionary<int, int> _animationLengths = aCameraRig.GetComponent<RigRenderer>().LatestAnimationLengths;
            
            //Write the animation meta data
            aXMLWriter.WriteStartElement(XmlTags.ANIMATION_SETUP);
            
            //Write the render name format
            var _namingSettings = NamingSettings.GetOrCreateSettings();
            StringBuilder _format = new StringBuilder();
            foreach (char _signifier in _namingSettings.RenderNameFormat)
            {
                _format.Append(_signifier);
            }

            //Add some extra info on the name format so we can use it more easily
            List<(string attributeTag, string attributeString)> _nameAttributes = new List<(string attributeTag, string attributeString)>
            {
                (XmlTags.NAME_PREFIX, _namingSettings.IncludeFormatIdentifier.ToString()),
                (XmlTags.PREFER_ANIMATION_NAME, _namingSettings.UseAnimationName.ToString())
            };
            XmlMethods.WriteStringElement(aXMLWriter, XmlTags.NAME_FORMAT, _format.ToString(), _nameAttributes.ToArray());
            //Write the FPS for the animations
            XmlMethods.WriteStringElement(aXMLWriter, XmlTags.FPS, _fps.ToString());
            //Write the number of animations
            XmlMethods.WriteStringElement(aXMLWriter, XmlTags.ANIMATION_COUNT, _animationNames.Count.ToString());
            aXMLWriter.WriteEndElement();

            //Create our attributes list
            List<(string, string)[]> _attributes = new List<(string, string)[]>();
            for (int _index = 0; _index < _animationNames.Count; _index++)
            {
                
                List<(string AttributeTag, string AttributeContent)> _elementAttributes = new List<(string, string)>();
                
                //Add the realtime animation length
                if(_animationLengths.ContainsKey(_index))
                {
                    _elementAttributes.Add((XmlTags.ANIMATION_LENGTH, _animationLengths[_index].ToString()));
                }

                var _animationSetting = aSmAnimatorInfo.AnimationSettings.Find(aSetting => aSetting.AnimationName.Equals(_animationNames[_index]));
                if (_animationSetting != null)
                {
                    //Add if it's looped or not
                    _elementAttributes.Add((XmlTags.ANIMATION_LOOPED, _animationSetting.IsLooping.ToString()));
                    
                }
                

                //Add if it's clamped or not
                ClampedAnimation _clampedAnimation = aSmAnimatorInfo.GetClampedAnimation(_animationNames[_index]);
                _elementAttributes.Add((XmlTags.CLAMPED, (_clampedAnimation != null).ToString()));
                //if it is clamped, add the clamped mode
                if(_clampedAnimation != null)
                    _elementAttributes.Add((XmlTags.CLAMPED_MODE, _clampedAnimation.Clamping.ToString()));
                
                //If it's isometric and the y-offset is overloaded, add the y-offset 
                ModelBaseManager _isometricBaseManager = aCameraRig.GetComponent<ModelBaseManager>();
                if (_isometricBaseManager != null)
                {
                    float _isometricOffset = _isometricBaseManager.GetOffsetForAnimation(_index);
                    if(_isometricOffset > 0)
                        _elementAttributes.Add((XmlTags.ISOMETRIC_OFFSET, _isometricOffset.ToString(CultureInfo.InvariantCulture)));
                }

                
                //Add the animation index TODO For the future: This is one of the places where we'll have to check what index the clip has
                _elementAttributes.Add((XmlTags.ANIMATION_INDEX, _index.ToString()));
                
                //Add the attributes to the larger attributes list as an array
                _attributes.Add(_elementAttributes.ToArray());
            }
            
            //Write the animation lists
            XmlMethods.WriteStringList(aXMLWriter, XmlTags.ANIMATION_LIST, XmlTags.ANIMATION, _animationNames, _attributes);
        }

        #endregion
       
        
        #region Args

        //Creating these as event args so we can do this as an event in the future

        #endregion
    }
}