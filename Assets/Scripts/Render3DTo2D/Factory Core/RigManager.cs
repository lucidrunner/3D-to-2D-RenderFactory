using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.RigCamera;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;
using Render3DTo2D.Utility.Inspector;
using UnityEngine;

namespace Render3DTo2D.Factory_Core
{
    public class RigManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField] 
        protected List<ToggleableRig> cameraRigs;
        
        
        public void ToggleRig(int aIndex)
        {
            cameraRigs[aIndex].Toggle();
        }
        #endregion
        
        #region Private & Protected Fields

        protected List<CameraRig> CurrentRunRigs;


        #endregion

        #region Properties

        public List<CameraRig> CameraRigs => cameraRigs.Select(aRig => aRig.Rig).ToList();

        public List<CameraRig> ToggledRigs =>
            cameraRigs.Where(aRigState => aRigState.Toggled).Select(aRig => aRig.Rig).ToList();

        internal int ToggledRigCount => ToggledRigs?.Count ?? 0;

        #endregion

        #region Methods
        #region Public

        public void AddRig(CameraRig aRigToAdd)
        {
            if (cameraRigs == null)
                cameraRigs = new List<ToggleableRig>();
            cameraRigs.Add(new ToggleableRig(this ,aRigToAdd, true));
        }

        public void RemoveRig(int aRigIndex)
        {
            if (aRigIndex < 0 || aRigIndex >= cameraRigs.Count)
                return;
            
            RemoveRig(cameraRigs[aRigIndex]);
        }

        public void RemoveRig(ToggleableRig aRigToRemove)
        {
            if (cameraRigs == null || aRigToRemove == null)
                return;

            //Remove the rig from the different lists
            GetComponent<ScaleManager>().RemoveRigCalculator(aRigToRemove.Rig.gameObject);
            GetComponent<RenderManager>().RemoveRigRenderer(aRigToRemove.Rig.gameObject);
            cameraRigs.Remove(aRigToRemove);
            
            //Delete the rig
            cameraRigs.Remove(aRigToRemove);
            DestroyImmediate(aRigToRemove.Rig.gameObject);
        }


        public virtual void Startup(StopMotionAnimator aStopMotionAnimator, bool aForceRecalculate, bool aRunAll)
        {
            CurrentRunRigs = ToggledRigs;
            if (aRunAll)
                CurrentRunRigs = CameraRigs;
            
            foreach (CameraRig _cameraRig in CurrentRunRigs)
            {
                _cameraRig.Startup(aStopMotionAnimator.GetAnimatorInfo(), aForceRecalculate);
            }
        }

        public virtual void RenderStartup(StopMotionAnimator aStopMotionAnimator)
        {
            foreach (CameraRig _cameraRig in CurrentRunRigs)
            {
                _cameraRig.RenderStartup(aStopMotionAnimator.GetAnimatorInfo());
            }
        }
        
        /// <summary>
        /// Export only the non-animated info of the last render
        /// </summary>
        /// <param name="aRootMotionFilePath">The optional file path to the accompanying root motion file</param>
        public void ExportDataForRigs(string aRootMotionFilePath = null)
        {
            foreach (CameraRig _cameraRig in CurrentRunRigs)
            {
                _cameraRig.ExportAnimationData(null, aRootMotionFilePath);
            }
        }

        /// <summary>
        /// Export the full info from the last render
        /// </summary>
        /// <param name="aStopMotionAnimator">The stop motion animator used during the render</param>
        /// <param name="aRootMotionFilePath">The optional file path to the accompanying root motion file</param>
        public void ExportDataForRigs(StopMotionAnimator aStopMotionAnimator, string aRootMotionFilePath = null)
        {
            foreach (CameraRig _cameraRig in CurrentRunRigs)
            {
                _cameraRig.ExportAnimationData(aStopMotionAnimator.GetAnimatorInfo(), aRootMotionFilePath);
            }
        }


        public void ValidateLists()
        {
            for(int i = 0; i < cameraRigs.Count; i++)
            {
                if(cameraRigs[i].Rig == null)
                {
                    cameraRigs.RemoveAt(i);
                    i--;
                }
            }
        }

        public bool IsRigActive(CameraRig aCameraRig)
        {
            return cameraRigs.DefaultIfEmpty(null).First(rig => rig.Rig == aCameraRig)?.Toggled ?? false;
        }

        private void Reset()
        {
            cameraRigs = new List<ToggleableRig>();
            var _rigs = GetComponentsInChildren<CameraRig>();
            foreach (var _camera in _rigs)
            {
                cameraRigs.Add(new ToggleableRig(this, _camera, true));
            }
        }

        #endregion

        #endregion

    }
}