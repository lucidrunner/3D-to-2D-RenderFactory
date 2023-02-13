namespace Render3DTo2D.Render_Info_Export
{
    public static class XmlTags
    {
        public static string RENDER_ROOT = "RenderRunInfo";
        public static string NAME = "ModelName";
        public static string NAME_FORMAT = "NameFormat";
        public static string RIGTAG = "RigTag";
        public static string TIMESTAMP = "StartTimestamp";
        public static string CAMERA_SETUP = "CameraSetupData";
        public static string CAMERA_COUNT = "CameraCount";
        public static string CAMERA_ANGLES = "CameraAngles";
        public static string ANIMATION_SETUP = "AnimationData";
        public static string ANIMATION_LIST = "Animations";
        public static string ANIMATION = "Animation";
        public static string ANIMATION_LOOPED = "Loop";
        public static string ANIMATION_LENGTH = "Length";
        public static string CLAMPED = "Clamped";
        public static string CLAMPED_MODE = "ClampedMode";
        public static string METADATA = "MetaData";
        public static string FPS = "FramesPerSecond";
        public static string BASELINE_SCALE = "BaselineScale";
        public static string DEFAULT_TEXTURE_SIZE = "BaseTextureSize";
        public static string ISOMETRIC_ANGLE = "CameraAngle";
        public static string ISOMETRIC_OFFSET = "ModelYOffset";
        public static string ANIMATION_COUNT = "AnimationCount";
        public static string NAME_PREFIX = "UsePrefix";
        public static string PREFER_ANIMATION_NAME = "UseAnimationNameOverIndex";
        public static string STATIC_FOLDER = "StaticFolderName";
        public static string SUBFOLDERS = "AnimationsInSubFolders";
        
        
        #region Root Movement Transform Export
        public static string TRANSFORM_EXPORT_ROOT = "RenderTransformInfo";
        public static string SOURCETYPE = "SourceType";
        public static string ANIMATION_INDEX = "AnimationIndex";
        public static string ANIMATION_NAME = "AnimationName";
        public static string FRAME_RECORDING = "FrameRecording";
        public static string POSITION_DELTA = "PositionDelta";
        public static string ROTATION = "Rotation";
        public static string ROTATION_EULER_DELTA = "RotationDelta";
        public static string SCALE_DELTA = "ScaleDelta";
        public static string FRAME_INDEX = "FrameIndex";
        public static string FRAME_STEPLENGTH = "StepElapsedTime";
        public static string FRAME_REALTIME = "ElapsedRealTime";
        public static string SUBSTEPS = "RecordingsPerFrame";
        public static string ROOT_FILEPATH = "RootMotionPath";

        #endregion

    }
}