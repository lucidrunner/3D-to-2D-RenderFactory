using System;
using System.Collections;
using Render3DTo2D.Model_Settings;
using UnityEngine;

namespace Render3DTo2D.RigCamera
{
    public class CameraEdgeReCalculator : MonoBehaviour
    {
        #region Private Fields
        private Camera ortoCam;
        private RenderTextureCreator rtCreator;
        private double bottomOffset = 0.02f;
        
        #endregion

        #region Public Methods

        public IEnumerator PerformEdgeCalculation(Action<float> aScaleCallback, Action aFinishCallback)
        {
            //If we're not running the edge calculations standard + inverse this needs to be run directly after the bounds calculations
            
            ortoCam.enabled = true;
            //Set a clear background color for the renders so we only have to check alpha
            ortoCam.backgroundColor = Color.clear;
            
            //Setup some variables for the run
            float _initialSize = ortoCam.orthographicSize;
            float _lastSize = _initialSize;
            int _currentStep = 0;
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(transform);
            bottomOffset = _renderingSettings.BottomOffset;
            bool _passed = false;
            bool _firstRun = true;
            bool _isRunningInverse = false;

            while (!_passed)
            {
                //We perform all steps of the DeviatedScaling calculation in here, treating the _currentSize as we would our FrameScale
                float _currentSize = ortoCam.orthographicSize;
                //Create a render texture and write it to a texture
                int _deviatedSize = (int) (_renderingSettings.BaseTextureSize * (_currentSize / _renderingSettings.BaselineScale));
                rtCreator.CreateAndRenderNewRt(_deviatedSize);
                //Wait for render
                aScaleCallback(-1f);
                yield return null;
                //Create a new render texture based on the current size
                RenderTexture _rt = rtCreator.WorkedRenderTexture;
                //Setup our texture based on the selected format and size of the render texture
                Texture2D _texture = new Texture2D(_rt.width, _rt.height, _renderingSettings.RenderingFormat, false);
                //Set the currently active rendertexture to our camera's texture so we can read the bytes
                RenderTexture.active = _rt;
                //Read the pixels
                _texture.ReadPixels(new Rect(0, 0, _texture.width, _texture.height), 0, 0, false);
                //and apply them so we can encode
                _texture.Apply(false);
                
                //Wait for GPU & CPU sync - we return current if we're edge calculating upwards and last if we're counting down (since current can be clipping)
                aScaleCallback(_isRunningInverse ? _lastSize : _currentSize);
                yield return null;
                
                //Check if we're currently clipping the model with our render
                bool _clips = EdgeAlphaClipping(_texture);

                //Perform our initial state check
                if (_firstRun)
                {
                    //Make sure we don't do this again
                    _firstRun = false;
                    
                    //We're running inverse mode if we're supposed to and our initial state is that we're not clipping
                    _isRunningInverse = _renderingSettings.UseInverseEdgeCalculator && !_clips;  
                }

                //Perform our exit check
                _passed = ExitCheck(_clips, _isRunningInverse);
                if (_passed)
                {
                    continue;
                }

                
                //Before changing the size, save the current so we can yield that when performing inverse calculations
                _lastSize = _currentSize;

                //If we don't pass then increment / decrement size depending on current direction and try again
                if (!_clips)
                {
                    _currentSize *= 1f - _renderingSettings.EdgeStepSize;
                    
                    //If we've reached an invalid size, reset to our initial and force pass
                    if (_currentSize <= 0)
                    {
                        _currentSize = _initialSize;
                        _passed = true;
                    }
                }
                else
                    _currentSize *= 1f + _renderingSettings.EdgeStepSize;

                ortoCam.orthographicSize = _currentSize;
                var _transform = transform;
                Vector3 _toSet = _transform.localPosition;
                _toSet.y = _currentSize;
                _transform.localPosition = _toSet;
                
                
                //Increment our step counter so we can exit it needed
                _currentStep++;
                //Force exit if needed
                if (_currentStep > _renderingSettings.ScaleCalculatorMaxSteps)
                    _passed = true;
            }
            
            //Reset the camera colour
            ortoCam.backgroundColor = _renderingSettings.RenderBackgroundColor;;
            ortoCam.enabled = false;
            aFinishCallback();
        }

        private static bool ExitCheck(bool aClips, bool aIsRunningInverse)
        {
            //If we're clipping
            if (aClips)
            {
                //And we're running backwards, return true
                return aIsRunningInverse;
            }

            //Otherwise, return true if we're running forward
            return !aIsRunningInverse;
        }

        #endregion

        #region Private Methods



        private bool EdgeAlphaClipping(Texture2D aTexture)
        {
            //Read Top
            for (int _topPx = 0; _topPx < aTexture.width; _topPx++)
            {
                if (aTexture.GetPixel(_topPx, aTexture.height - 1).a > 0)
                {
                    return true;
                }
            }
            //Read left
            for (int _leftPx = (int) (aTexture.height * bottomOffset); _leftPx < aTexture.height; _leftPx++)
            {
                if (aTexture.GetPixel(0, _leftPx).a > 0)
                    return true;
            }
            
            //Read Right
            for (int _rightPx = (int) (aTexture.height * bottomOffset); _rightPx < aTexture.height; _rightPx++)
            {
                if (aTexture.GetPixel(aTexture.width - 1, _rightPx).a > 0)
                    return true;
            }
            return false;
        }

        private void Start()
        {
            ortoCam = GetComponent<Camera>();
            rtCreator = GetComponent<RenderTextureCreator>();
        }

        #endregion
    }
}