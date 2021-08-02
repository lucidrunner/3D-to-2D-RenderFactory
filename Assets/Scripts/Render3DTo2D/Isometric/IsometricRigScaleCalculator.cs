using System.Collections;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;
using UnityEngine;

namespace Render3DTo2D.Isometric
{
    //Isometric scales aren't calculated, so we override the calculator to cancel any of the related scaling calculations, with the exception of the deviation since that's necessary to compare 
    //the output to other models
    public class IsometricRigScaleCalculator : RigScaleCalculator
    {
        public override void Startup(StopMotionAnimatorInfo aSmAnimatorInfo, bool aForceRecalculate)
        {
            settings = RenderingSettings.GetFor(transform);
            smAnimatorInfo = aSmAnimatorInfo;
            HasCalculatedFrames = true;
        }

        public override void SaveFrames()
        {
            //Do nothing when saving frames either
        }

        public override IEnumerator CalculateFrame(int aCurrentAnimation, int aCurrentFrame)
        {
            //Do nothing for calculation either
            yield return null;
        }

        public override void CalculateCameraDepths()
        {
            //Finally, do nothing when calculating depth either :7
        }

        public override void ResetCameraSizes()
        {
            //Do nothing
        }

        internal override float GetScaleForFrame(int aAnimationIndex, int aFrameIndex, int aCameraIndex, out float aScaleDeviation)
        {
            //Depending on our current settings, we return the current camera scale and deviate it with either the standard baseline scale as our 
            //reference scale or with the Isometric baseline via the ModelBaseManager
            RenderingSettings _settings = RenderingSettings.GetFor(transform);
            bool _preferPlateDeviation = _settings.PreferBasePlateDeviation;
            if (!_preferPlateDeviation)
            {
                var _cam = cameraScaleCalculators[aCameraIndex].GetComponent<Camera>();
                var _orthographicSize = _cam.orthographicSize;
                aScaleDeviation = _orthographicSize /  _settings.BaselineScale;
                return _orthographicSize;
            }
            else
            {
                var _cam = cameraScaleCalculators[aCameraIndex].GetComponent<Camera>();
                ModelBaseManager _baseManager = GetComponent<ModelBaseManager>();
                aScaleDeviation = _baseManager.BaseDeviation;
                return _cam.orthographicSize;
            }
        }
    }
}