using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using Shared_Scripts;
using UnityEngine;

namespace Render3DTo2D.RigCamera
{
    public class CameraRenderer: MonoBehaviour
    {
        #region Private Fields
        #region Serialized
        [SerializeField, HideInInspector] 
        private Camera renderCamera;

        [SerializeField, HideInInspector] private string defaultName = "";
        
        [SerializeField, HideInInspector] private int setCameraNumber;
        [SerializeField, HideInInspector] private string lastSetName = "";
        [SerializeField, HideInInspector] private bool deletionSafetyToggle = false;

        private Renderer optionalRenderer = null;

        #endregion

        private RenderTextureCreator renderTextureCreator;

        private string outputPath = "";
        private bool writingRender = false;
        private float startScale;
        private List<char> renderNameFormat;
        private bool includeFormatIdentifier = false;
        private int defaultLayerMask;

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Writes the current frame of the camera to the provided render texture
        /// Note that all cameras should be run with a .Render(), then a frame should be yielded and THEN this should be called for them.
        /// </summary>
        private void WriteFrame(CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            //Set the name of the rendered frame
            string _currentFrameImage = FormatRenderImageName(aCameraFrameRenderInfo);
                
            //Set the absolute base path
            string _currentFramePath = outputPath;

            //If we're supposed to write to a subfolder, do so
            if (aCameraFrameRenderInfo.SubFolder != null)
            {
                _currentFramePath += $"/{aCameraFrameRenderInfo.SubFolder}";
            }
            
            //Add the current image to the path
            _currentFramePath += $"/{_currentFrameImage}";
            

            //If we're not supposed to replace existing images, don't render if the path corresponds to an existing file
            if(aCameraFrameRenderInfo.Overwrite == false)
            {
                if(Directory.Exists(_currentFramePath))
                {
                    Debug.Log($"Not saving render {_currentFrameImage} due to overwrite set to false and existing image file detected.");
                    return;
                }
            }

            RenderTexture _rt = renderTextureCreator.WorkedRenderTexture;
            
            //Setup our texture based on the selected format and size of the render texture
            Texture2D _texture = new Texture2D(_rt.width, _rt.height, RenderingSettings.GetFor(transform).RenderingFormat, false);
            //Set the currently active rendertexture to our camera's texture so we can read the bytes
            RenderTexture.active = _rt;

            //Read the pixels
            _texture.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0, false);
            //and apply them so we can encode
            _texture.Apply();

            //Encode to bytes
            byte[] _bytes = _texture.EncodeToPNG();

            //and write to our provided path
            File.WriteAllBytes(_currentFramePath, _bytes);


            
            //Set our optional target renderer to our render texture so we can show the progress of the rendering
            //Traditionally this is a single mesh giving us a slideshow as we progress through the cameras on each rig pass
            if(optionalRenderer != null)
            {
                optionalRenderer.material.mainTexture = _rt;
            }

            //Reset so we can move on to the next camera
            writingRender = false;
        }

        private string FormatRenderImageName(CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            StringBuilder _builder = new StringBuilder();
            //Construct the name for the current rendered frame based on the set format
            //We could / should have more formalized flags for these rather than just sending some chars but w/e
            foreach (char _signifier in renderNameFormat)
            {
                switch (_signifier)
                {
                    //Model
                    case 'M':
                        _builder.Append(aCameraFrameRenderInfo.RenderName);
                        break;
                    //Rig Tag
                    case 'R':
                        _builder.Append(aCameraFrameRenderInfo.RigTag);
                        break;
                    //Animation
                    case 'A':
                        //Append either the number or the name depending on if the name is set or not
                        _builder.Append(!string.IsNullOrEmpty(aCameraFrameRenderInfo.AnimationName)
                            ? (includeFormatIdentifier ? "a" : "") + GeneralUtilities.CleanName(aCameraFrameRenderInfo.AnimationName)
                            : aCameraFrameRenderInfo.AnimationNumber.ToString());
                        break;
                    //Camera
                    case 'C':
                        _builder.Append((includeFormatIdentifier ? "c" : "") + aCameraFrameRenderInfo.CameraNumber);
                        break;
                    //Frame
                    case 'F':
                        _builder.Append((includeFormatIdentifier ? "f" : "") + aCameraFrameRenderInfo.FrameNumber);
                        break;
                    //Static Tag
                    case 'S':
                        _builder.Append("Static");
                        break;
                }

                _builder.Append("_");
            }

            _builder.Remove(_builder.Length - 1, 1);
            _builder.Append(".png");
            return _builder.ToString();
        }

        public void SetScaleAndTransform(float aScale)
        {
            startScale = renderCamera.orthographicSize;
            MatchOrthographic(aScale);
        }

        public void ResetScaleAndTransform()
        {
            MatchOrthographic(startScale);
        }
        
        
        public IEnumerator RunRenderer(CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            
            //Start our camera and give it a second to boot
            renderCamera.enabled = true;
            yield return null;
            //Create the deviated texture, this has the added effect of forcibly rendering the camera (this might not be true anymore but it was at one point)
            CreateDeviatedTexture(aCameraFrameRenderInfo.BaseTextureSize, aCameraFrameRenderInfo.ScaleDeviation);
            yield return null;
            //Start writing the frame
            writingRender = true;
            WriteFrame(aCameraFrameRenderInfo);
            
            while (writingRender)
            {
                //this is an overflow check that shouldn't be possible with the current lag based implementation so if this shows up we've either changed up to an async WriteRender or broken something
                FLogger.LogMessage(this, FLogger.Severity.Priority, "Waiting for frame rendering to finish on camera with id " + GetInstanceID());
                yield return null;
            }

            renderCamera.enabled = false;
        }

        public void Startup()
        {
            //Set the background color to the color provided in the settings
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(transform);
            renderCamera.backgroundColor = _renderingSettings.RenderBackgroundColor;
            
            //Get the list for the name format
            var _namingSettings = NamingSettings.GetOrCreateSettings();
            renderNameFormat = _namingSettings.RenderNameFormat;
            //Set if we should include a C / A / F identifier in the format
            includeFormatIdentifier = _namingSettings.IncludeFormatIdentifier;
            
            //Set our camera culling mask to only match our root object layer
            defaultLayerMask = renderCamera.cullingMask;
            int _rootLayer = AnimationUtilities.GetAnimatorTransform(transform).gameObject.layer;
            renderCamera.cullingMask |= (1 << _rootLayer);
            
            RenderFactoryEvents.FactoryEnded += RenderFactoryEventsOnFactoryEnded;
        }

        private void RenderFactoryEventsOnFactoryEnded(object aSender, EventArgs aE)
        {
            //Reset our culling mask to the default and unsubscribe
            renderCamera.cullingMask = defaultLayerMask;
            RenderFactoryEvents.FactoryEnded -= RenderFactoryEventsOnFactoryEnded;
        }

        public void RenderStartup(string aRigFolderName)
        {
            //Set our render path to the provided output path
            outputPath = aRigFolderName;
            //Get the optional renderer from the overseer
            optionalRenderer = Overseer.Instance.OptionalRenderer;
        }

        #endregion

        #region Internal Methods

        internal void SetCameraNumber(int aCameraIndex)
        {
            if(string.IsNullOrWhiteSpace(defaultName))
                defaultName = gameObject.name;
            setCameraNumber = aCameraIndex;
            lastSetName = defaultName + " " + setCameraNumber;
            gameObject.name = lastSetName;
        }
        

        #endregion

        #region Private Methods

        private void MatchOrthographic(float aScale)
        {
            renderCamera.orthographicSize = aScale;
            Transform _transform = transform;
            Vector3 _setPos = _transform.localPosition;
            _setPos.y = aScale;
            _transform.localPosition = _setPos;
        }

        private void CreateDeviatedTexture(int aBaseTextureSize, float aScaleDeviation)
        {
            int _size = (int) (aBaseTextureSize * aScaleDeviation);
            RenderingSettings _settings = RenderingSettings.GetFor(transform);
            if (_settings.SetToNearestMultiple)
            {
                _size = GeneralUtilities.RoundUp(_size, 4);
            }
            renderTextureCreator.CreateAndRenderNewRt(_size);
        }

        private void Reset()
        {
            renderCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            //Set our references
            renderTextureCreator = GetComponent<RenderTextureCreator>();
            renderCamera = GetComponent<Camera>();
            //turn off camera for performance reasons
            renderCamera.enabled = false;
            //Set the layer to nothing when entering play mode
            renderCamera.cullingMask = 0;
        }

        #endregion
    }
}
