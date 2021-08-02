namespace Render3DTo2D.RigCamera
{
    //This really should've been done as a generic packer / unpacker with flags instead (see CameraRigger.SetupInfo) 
    public class CameraFrameRenderInfo
    {
        private CameraFrameRenderInfo()
        {}
        
        public string RenderName { get; private set;}
        public string RigTag { get; private set; }
        public string SubFolder { get; private set;}
        public int AnimationNumber { get; private set;}
        public string AnimationName { get; private set; }
        public int FrameNumber { get; private set;}
        public int CameraNumber { get; private set;}
        public int BaseTextureSize { get; private set;}
        public float FrameScale { get; private set;}
        public float ScaleDeviation { get; private set; }
        public bool Overwrite { get; private set; }
        public bool StaticRender { get; private set; } //Single image shot from a StaticRenderFactory rig
        public bool NonAnimatedModel { get; private set; }

        public class Builder
        {
            private string renderName;
            private string rigTag;
            private string subFolder;
            private int animationNumber;
            private string animationName;
            private int frameNumber;
            private int cameraNumber;
            private int baseTextureSize;
            private float frameScale;
            private float scaleDeviation;
            private bool overwrite;
            private bool staticRender;
            private bool nonAnimatedModel;

            public CameraFrameRenderInfo Build()
            {
                CameraFrameRenderInfo _info = new CameraFrameRenderInfo
                {
                    RenderName = renderName,
                    RigTag = rigTag,
                    SubFolder = subFolder,
                    AnimationNumber = animationNumber,
                    AnimationName =  animationName,
                    FrameNumber = frameNumber,
                    CameraNumber = cameraNumber,
                    BaseTextureSize = baseTextureSize,
                    FrameScale = frameScale,
                    Overwrite = overwrite,
                    ScaleDeviation = scaleDeviation,
                    StaticRender = staticRender,
                    NonAnimatedModel = nonAnimatedModel
                };
                return _info;
            }

            public Builder SetName(string aRenderName)
            {
                renderName = aRenderName;
                return this;
            }

            public Builder SetRigTag(string aRigTag)
            {
                rigTag = aRigTag;
                return this;
            }

            public Builder SetSubFolder(string aSubFolder)
            {
                subFolder = aSubFolder;
                return this;
            }

            public Builder SetAnimationNumber(int aAnimationNumber)
            {
                animationNumber = aAnimationNumber;
                return this;
            }

            public Builder SetAnimationName(string aAnimationName)
            {
                animationName = aAnimationName;
                return this;
            }

            public Builder SetFrameNumber(int aFrameNumber)
            {
                frameNumber = aFrameNumber;
                return this;
            }
            public Builder SetCameraNumber(int aCameraNumber)
            {
                cameraNumber = aCameraNumber;
                return this;
            }
            public Builder SetTextureSize(int aTextureSize)
            {
                baseTextureSize = aTextureSize;
                return this;
            }
            
            public Builder SetFrameScale(float aFrameScale)
            {
                frameScale = aFrameScale;
                return this;
            }

            public Builder SetScaleDeviation(float aScaleDeviation)
            {
                scaleDeviation = aScaleDeviation;
                return this;
            }
            
            public Builder SetOverwrite(bool aOverwrite)
            {
                overwrite = aOverwrite;
                return this;
            }

            public Builder SetStaticRender(bool aStaticRender)
            {
                staticRender = aStaticRender;
                return this;
            }
            
            public Builder SetNonAnimatedModel(bool aNonAnimated)
            {
                nonAnimatedModel = aNonAnimated;
                return this;
            }
            
        }
    }
}