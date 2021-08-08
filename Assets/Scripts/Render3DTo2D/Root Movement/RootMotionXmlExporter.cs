﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility;
using Render3DTo2D.XML_Render_Info_Export;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Root_Movement
{
    internal static class RootMotionXmlExporter
    {
        #region Methods
        
        /// <summary>
        /// Exports the provided animation recordings to the path provided in the export transform args
        /// </summary>
        internal static void Export(RenderFactoryEvents.ExportTransformArgs aExportTransformArgs, Transform aFactoryTransform, List<TransformRecorder.AnimationRecording> aAnimationRecordings)
        {
            FLogger.LogMessage(null, FLogger.Severity.Debug, "Entering Root Motion export write now.", nameof(RootMotionXmlExporter));
            using (XmlWriter _xmlWriter = WriteDocumentStart(aExportTransformArgs, aFactoryTransform))
            {
                
                _xmlWriter.WriteStartElement(XmlTags.ANIMATION_LIST);
                
                var _renderingSettings = RenderingSettings.GetFor(aFactoryTransform);
                _xmlWriter.WriteAttributeString(XmlTags.SUBSTEPS, _renderingSettings.RecordingsPerFrame.ToString());
                //Iterate through our list and for each Frame Recording in the Animation recording, record all the ToStrings of the different values
                for (int _index = 0; _index < aAnimationRecordings.Count; _index++)
                {
                    WriteAnimationRecording(_xmlWriter, aAnimationRecordings[_index], aFactoryTransform);
                }
                _xmlWriter.WriteEndElement();
                XmlMethods.WriteDocumentEnd(_xmlWriter);
            }
        }

        
        
        private static void WriteAnimationRecording(XmlWriter aXMLWriter, TransformRecorder.AnimationRecording aAnimationRecording, Transform aFactoryTransform)
        {
            //Write the animation recording element start
            aXMLWriter.WriteStartElement(XmlTags.ANIMATION);
            
            //Write the animation info
            aXMLWriter.WriteAttributeString(XmlTags.ANIMATION_NAME, GeneralUtilities.CleanName(aAnimationRecording.AnimationName));
            aXMLWriter.WriteAttributeString(XmlTags.ANIMATION_INDEX, aAnimationRecording.AnimationIndex.ToString());

            FLogger.LogMessage(null, FLogger.Severity.Status, $"Writing frame recordings for animation {aAnimationRecording.AnimationName}, recorded Root Motion Frames {aAnimationRecording.FrameRecordings.Count}.", nameof(RootMotionXmlExporter));
            
            //Write all the frame recordings
            for (int _index = 0; _index < aAnimationRecording.FrameRecordings.Count; _index++)
            {
                var _frameRecording = aAnimationRecording.FrameRecordings[_index];
                WriteFrameRecording(aXMLWriter, _frameRecording, RenderingSettings.GetFor(aFactoryTransform), aAnimationRecording.AnimationSetting);
            }

            //Finish the animation recording element
            aXMLWriter.WriteEndElement();
        }

        private static void WriteFrameRecording(XmlWriter aXMLWriter, TransformRecorder.AnimationRecording.FrameRecording aFrameRecording, RenderingSettings aFactorySettings,
            RootMotionSetting aRootMotionSetting)
        {
            //Write the frame tag
            aXMLWriter.WriteStartElement(XmlTags.FRAME_RECORDING);
            //and add the frame # as an attribute
            aXMLWriter.WriteAttributeString(XmlTags.FRAME_INDEX, aFrameRecording.FrameIndex.ToString());
            aXMLWriter.WriteAttributeString(XmlTags.FRAME_STEPLENGTH, aFrameRecording.FrameStepTime.ToString(CultureInfo.InvariantCulture));

            //Only write the sub-nodes to a frame if there's any change to them
            if (!aFrameRecording.HasChanged)
            {
                aXMLWriter.WriteEndElement();
                return;
            }

            WriteFrameDelta(aXMLWriter, aFrameRecording, aRootMotionSetting, aFactorySettings);

            aXMLWriter.WriteEndElement();
        }

        private static void WriteFrameDelta(XmlWriter aXMLWriter, TransformRecorder.AnimationRecording.FrameRecording aFrameRecording, RootMotionSetting aRootMotionSetting, RenderingSettings aRenderingSettings)
        {
            if (aRootMotionSetting == null)
                return;

            var _transform = aFrameRecording.FrameTransform;
            
            //Basically, construct a vector for pos / rot / scale where each is either 0 or the delta depending on if it's toggled in the setting. Then only write those that have a magnitude.
            //POSITION
            var _posDelta = new Vector3(aRootMotionSetting.Export.PositionXToggled ? _transform.PositionDeltaX : 0, aRootMotionSetting.Export.PositionYToggled ? _transform.PositionDeltaY : 0,
                aRootMotionSetting.Export.PositionZToggled ? _transform.PositionDeltaZ : 0);
            if (_posDelta != Vector3.zero)
            {
                XmlMethods.WriteVector3(aXMLWriter, XmlTags.POSITION, aRenderingSettings.ApplyBaselineDeviation ? GeneralUtilities.DeviateVector(_posDelta, aRenderingSettings.BaselineScale, GlobalRenderingSettings.Instance.BaselineScale) : _posDelta, aRenderingSettings.RootMotionTolerance);
            }
            
            //ROTATION
            var _rotDelta = new Vector3(aRootMotionSetting.Export.RotationXToggled ? _transform.RotationDeltaX : 0, aRootMotionSetting.Export.RotationYToggled ? _transform.RotationDeltaY : 0,
                aRootMotionSetting.Export.RotationZToggled ? _transform.RotationDeltaZ : 0);
            if (_rotDelta != Vector3.zero)
            {
                XmlMethods.WriteVector3(aXMLWriter, XmlTags.ROTATION_EULER, _rotDelta, aRenderingSettings.RootMotionTolerance);
            }
            
            //SCALE
            var _scaleDelta = new Vector3(aRootMotionSetting.Export.ScaleXToggled ? _transform.ScaleDeltaX : 0, aRootMotionSetting.Export.ScaleYToggled ? _transform.ScaleDeltaY : 0,
                aRootMotionSetting.Export.ScaleZToggled ? _transform.ScaleDeltaZ : 0);
            if (_scaleDelta != Vector3.zero)
            {
                XmlMethods.WriteVector3(aXMLWriter, XmlTags.SCALE, _scaleDelta, aRenderingSettings.RootMotionTolerance);
            }
            
        }

        private static XmlWriter WriteDocumentStart(RenderFactoryEvents.ExportTransformArgs aExportTransformArgs, Transform aFactoryTransform)
        {
            //TODO Path is wrong here
            //The passed transform is already root by default but this is just some future proofing if we ever change that by accident
            string _modelName = RootFinder.FindHighestRoot(aFactoryTransform).name;
            string _fileName = $"{_modelName} - Root Motion Recording.xml";
            string _path = Path.Combine(aExportTransformArgs.OutputPath, _fileName);
            
            
            //Setup & Start the writer
            XmlWriterSettings _settings = new XmlWriterSettings {Indent = true, IndentChars = "\t"};

            XmlWriter _xmlWriter = XmlWriter.Create(_path, _settings);
            
            //Write the start of the document and the root node
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement(XmlTags.TRANSFORM_EXPORT_ROOT);
            
            //Write the model name
            XmlMethods.WriteStringElement(_xmlWriter, XmlTags.NAME, _modelName);
            //Write the timestamp
            XmlMethods.WriteStringElement(_xmlWriter, XmlTags.TIMESTAMP, aExportTransformArgs.TimeStamp.ToString());

            return _xmlWriter;
        }
        #endregion
    }
}
