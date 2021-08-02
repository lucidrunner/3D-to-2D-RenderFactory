using System;
using Render3DTo2D.Model_Settings;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.RigCamera
{
    public class RenderTextureCreator : MonoBehaviour
    {
        #region Properties
        public RenderTexture WorkedRenderTexture => renderTexture;
        
        #endregion Properties

        #region References
        
        [SerializeField, HideInInspector]
        private Camera renderedCamera = null;
        
        #endregion References

        #region Private Fields
        
        private RenderTexture renderTexture;
        private static int _textureNumber = 0;
        
        #endregion Private Fields

        #region Public Methods

        private void Reset()
        {
            renderedCamera = GetComponent<Camera>();
        }

        public void CreateAndRenderNewRt(int aSize)
        {
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(transform);
            
            renderTexture = new RenderTexture(aSize, aSize, _renderingSettings.PresetRenderTextureDepth, _renderingSettings.PresetRenderTextureFormat,
                _renderingSettings.PresetRenderTextureReadWrite) {name = $"RT_{RootFinder.FindHighestRoot(transform).GetInstanceID()}_{_textureNumber}"};
            _textureNumber++;
            renderTexture.Create();
            //   if(originCamera.targetTexture != null) Honestly not sure if we need to release the current textures
            //      originCamera.targetTexture.Release(); This causes a slowdown & warning if we try to do it on single camera rendering or without any frames between rendering / creating a new texture
            renderedCamera.targetTexture = renderTexture;
            renderedCamera.Render();
        }
        

        #endregion Public Methods

    }
}
