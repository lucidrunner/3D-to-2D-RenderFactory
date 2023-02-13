using System;
using Render3DTo2D.Rigging;
using Render3DTo2D.SMAnimator;

namespace Render3DTo2D.Render_Info_Export
{
    public class RigRenderExportArgs : EventArgs
    {
        public CameraRig CameraRig { get; }
        public string LastOutputPath { get; }
        public StopMotionAnimatorInfo SmAnimatorInfo { get; }
        public string TimeStamp { get; }
        public string RootMotionFilePath { get; }

        public RigRenderExportArgs(CameraRig aCameraRig, string aLastOutputPath, string aTimeStamp, StopMotionAnimatorInfo aSmAnimatorInfo = null, string aRootMotionFilePath = null)
        {
            CameraRig = aCameraRig;
            LastOutputPath = aLastOutputPath;
            SmAnimatorInfo = aSmAnimatorInfo;
            TimeStamp = aTimeStamp;
            RootMotionFilePath = aRootMotionFilePath;
        }
    }
}