using Render3DTo2D.Factory_Core;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;
using UnityEngine;

namespace Render3DTo2D.Single_Frame
{
    public class StaticRigManager : RigManager
    {
        public override void Startup(StopMotionAnimator aStopMotionAnimator, bool aForceRecalculate, bool aRunAll)
        {
            CurrentRunRigs = ToggledRigs;
            if (aRunAll)
                CurrentRunRigs = CameraRigs;
            
            foreach (CameraRig _cameraRig in CurrentRunRigs)
            {
                //Run the static startup
                _cameraRig.Startup();
            }
        }

        public override void RenderStartup(StopMotionAnimator aStopMotionAnimator)
        {
            foreach (CameraRig _cameraRig in CurrentRunRigs)
            {
                _cameraRig.RenderStartup();
            }
        }
    }
}