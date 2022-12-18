using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;
using RootChecker;
using UnityEngine;
using Render3DTo2D.Logging;
using Render3DTo2D.Root_Movement;
using Shared_Scripts;

namespace Render3DTo2D.Factory_Core
{
    public class RenderManager : MonoBehaviour
    {
        
        #region Private Fields
        [SerializeField]
        private List<RigRenderer> rigRenderers;

        protected List<RigRenderer> ActiveRigRenderers;

        [SerializeField, HideInInspector] protected StopMotionAnimator smAnimator;
        public object RendererCount => rigRenderers?.Count ?? 0;

        #endregion
        
        #region Internal Methods
        internal virtual void Startup(bool aRunAll)
        {
            smAnimator = GetComponent<StopMotionAnimator>();
            ActiveRigRenderers = new List<RigRenderer>();
            RigManager _rigManager = GetComponent<RigManager>();
            foreach(RigRenderer _rigRenderer in rigRenderers)
            {
                if(_rigManager.IsRigActive(_rigRenderer.GetComponent<CameraRig>()) || aRunAll)
                    ActiveRigRenderers.Add(_rigRenderer);
            }
        }

        internal virtual IEnumerator RenderAllActiveRigs(Action aFinishedCallback)
        {
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(transform);
            FolderSettings _folderSettings = FolderSettings.GetFor(transform);
            //start setting up the rendering info builder we'll be using to transfer data downwards
            CameraFrameRenderInfo.Builder _renderInfoBuilder = new CameraFrameRenderInfo.Builder()
                .SetSubFolder(null)
                .SetTextureSize(_renderingSettings.BaseTextureSize)
                .SetOverwrite(_renderingSettings.OverwriteExistingFrames);
                     
            bool _useSubFolders = _folderSettings.CreateAnimationSubfolders;

            //Set up the runner
            SMAnimatorRunner _smAnimatorRunner = new SMAnimatorRunner(smAnimator);
            _smAnimatorRunner.Start();
            
            //And check if we should perform our partial steps
            bool _useSubSteps = TransformRecorder.Instance != null && !TransformRecorder.Instance.HasFactoryRecorded(transform);
            
            //Print a message of which rigs we are rendering
            PrintStartMessage();

            //Run our rendering process
            yield return null;
            do
            {
                //Call the pre-frame render event
                RenderFactoryEvents.InvokePreFrameRender(transform, _smAnimatorRunner.CurrentFrameInfo);
                
                
                //Check if we're supposed to skip rendering the current frame
                if (_smAnimatorRunner.ShouldSkipFrame() || _smAnimatorRunner.OnSubStep)
                    continue;
                
                //Set our current frame & animation index & possible animation name
                _renderInfoBuilder.SetAnimationNumber(_smAnimatorRunner.CurrentAnimationIndex)
                    .SetFrameNumber(_smAnimatorRunner.CurrentFrame)
                    .SetAnimationName(NamingSettings.GetOrCreateSettings().UseAnimationName ? smAnimator.CurrentlyPlayedAnimation : "");

                FLogger.LogMessage(this, FLogger.Severity.Status, $"Rendering for animation / frame {_smAnimatorRunner.CurrentAnimationIndex} / {_smAnimatorRunner.CurrentFrame}");
                
                //Check if we need to set a subfolder
                if (_useSubFolders)
                {
                    _renderInfoBuilder.SetSubFolder(smAnimator.CurrentlyPlayedAnimation);
                }

                //Start each RigRenderer rendering routine in turn
                foreach (RigRenderer _rigRenderer in ActiveRigRenderers)
                {
                    bool _finished = false;
                    StartCoroutine(_rigRenderer.RenderStep(_renderInfoBuilder, () => _finished = true));
                    while (!_finished)
                    {
                        yield return null;
                    }
                }
            } while (_smAnimatorRunner.Step(_useSubSteps)); //For ease of access we're only setting the step mode to use substeps, if they're actually applied is down to the settings the runner reads

            aFinishedCallback();
        }

        protected void PrintStartMessage()
        {
            StringBuilder stringBuilder = new StringBuilder($"Starting render on {RootFinder.FindHighestRoot(transform).gameObject.name} - {gameObject.name} for rigs ");
            
            foreach (RigRenderer rigRenderer in ActiveRigRenderers)
            {
                stringBuilder.Append(rigRenderer.gameObject.name + ", ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            Debug.Log(stringBuilder.ToString());
        }

        #endregion
        
        #region Public Methods
        public void AddRigRenderer(RigRenderer aRigRenderer)
        {
            if (rigRenderers == null)
            {
                rigRenderers = new List<RigRenderer>();
            }
            
            rigRenderers.Add(aRigRenderer);
        }

        public void RemoveRigRenderer(GameObject aRigRendererObject)
        {
            RigRenderer _renderer = aRigRendererObject.GetComponent<RigRenderer>();
            if (rigRenderers.Contains(_renderer))
            {
                rigRenderers.Remove(_renderer);
            }
        }

        public void ValidateList()
        {
            for (int i = 0; i < rigRenderers.Count; i++)
            {
                if (rigRenderers[i] == null)
                {
                    rigRenderers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Reset()
        {
            rigRenderers = new List<RigRenderer>();
            var _renderers = GetComponentsInChildren<RigRenderer>();
            if(_renderers != null)
                rigRenderers.AddRange(_renderers);
        }

        #endregion

    }
}
