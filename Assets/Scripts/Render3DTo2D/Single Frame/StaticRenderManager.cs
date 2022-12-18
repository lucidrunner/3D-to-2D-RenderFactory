using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.RigCamera;
using Render3DTo2D.Rigging;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using UnityEngine;

namespace Render3DTo2D.Single_Frame
{
    //TODO Like all animator points this needs the new locomotion animator when that is added
    
    public class StaticRenderManager : RenderManager
    {
        #region Private Fields

        private static int renderedStaticFrames = 0;
        
        [SerializeField, HideInInspector] private ModelInfo modelInfo;

        #endregion

        #region Private Properties

        private ModelInfo GetModelInfo
        {
            get
            {
                if(modelInfo == null)
                    modelInfo = ModelInfo.GetFor(transform);
                

                return modelInfo;
            }
        }

        #endregion

        #region Inspector

        [SerializeField]
        private int selectedAnimation = 0;

        public string[] SelectedAnimationOptions => GetAnimations().ToArray();

        [SerializeField]
        private int frame = 0;

        #region Properties & Conditionals

        public int Frame => frame;

        private string SelectedAnimation
        {
            get
            {
                var _anims = GetAnimations();
                if(selectedAnimation >= 0 && selectedAnimation < _anims.Count)
                    return _anims[selectedAnimation];
                return "";
            }
        }

        public int MaxFrames
        {
            get
            {
                if (GetModelInfo == null)
                    return 0;
                RenderingSettings _settings = RenderingSettings.GetFor(transform);
                AnimationClip _clip = AnimatorEditorHelperMethods.GetClipByName(GetModelInfo.Animator, SelectedAnimation);
                return _clip != null ? AnimationUtilities.GetAnimationMaxIndex(_clip, _settings.AnimationFPS) : 0;
            }
        }

        private List<string> GetAnimations()
        {
            List<string> _toReturn = new List<string>();
            if (CanAnimate)
            {
                _toReturn.AddRange(AnimatorEditorHelperMethods.GetClipNames(GetModelInfo.Animator));
            }
            return _toReturn;
        }

        private void OnValidate()
        {
            if (CanAnimate && !string.IsNullOrEmpty(SelectedAnimation))
            {
                if (frame < 0)
                    frame = 0;
                if (frame >= MaxFrames)
                    frame = MaxFrames;
            }
        }

        public bool CanAnimate => GetModelInfo != null && GetModelInfo.CanAnimate;

        #endregion

        #endregion

        #region Public Functions

        public IEnumerator GoToFrame()
        {
            if (GetModelInfo == null || !GetModelInfo.CanAnimate || string.IsNullOrEmpty(SelectedAnimation))
            {
                yield return null;
                yield break;
            }
            
            RenderingSettings _settings = RenderingSettings.GetFor(transform);

            smAnimator.SetAnimation(SelectedAnimation);
            smAnimator.PlayAtTime(aNormalizedTime: (1f / _settings.AnimationFPS) * frame);
            
            yield return null;
        }

        internal override IEnumerator RenderAllActiveRigs(Action aFinishedCallback)
        {
            RenderingSettings _renderingSettings = RenderingSettings.GetFor(transform);
            FolderSettings _folderSettings = FolderSettings.GetFor(transform);
            //start setting up the rendering info builder we'll be using to transfer data downwards
            CameraFrameRenderInfo.Builder _renderInfoBuilder = new CameraFrameRenderInfo.Builder()
                .SetSubFolder(null)
                .SetTextureSize(_renderingSettings.BaseTextureSize)
                .SetOverwrite(_renderingSettings.OverwriteExistingFrames)
                .SetStaticRender(true);

            
            
            
            if (CanAnimate)
            {
                _renderInfoBuilder.SetAnimationNumber(selectedAnimation)
                .SetFrameNumber(frame);

                //If we didn't run the calculator we'll need to go to the correct static frame now
                if (!GetComponent<StaticScaleManager>().RunCalculator)
                {
                    //Go to the frame if necessary
                    SafeCoroutine _routine = this.StartSafeCoroutine(GoToFrame());
                    do
                    {
                        yield return null;
                    } while (!_routine.HasFinished);
                }
            }
            else
            {
                _renderInfoBuilder.SetNonAnimatedModel(true);
                _renderInfoBuilder.SetFrameNumber(renderedStaticFrames);
                renderedStaticFrames++;
            }

            //Print a message of which rigs we are rendering
            PrintStartMessage();

            //Run our rendering process
            yield return null;
            
            //Call the pre-frame render event
            RenderFactoryEvents.InvokePreFrameRender(transform, null);
            

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

            aFinishedCallback();
        } 

        #endregion

    }
}