
using Render3DTo2D.RigCamera;
using Render3DTo2D.Rigging;

namespace Render3DTo2D.Isometric
{
    public class IsometricRigRenderer : RigRenderer
    {
        //There's not a lot we need to do in here, just two steps of matching y & scale that we don't want to do on an isometric render
        
        protected override void StartRenderingRoutine(CameraRenderer aCameraRenderer, CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            currentRenderStepState[aCameraRenderer] = false;
            StartCoroutine(aCameraRenderer.RunRenderer(aCameraFrameRenderInfo, () => currentRenderStepState[aCameraRenderer] = true));
        }


        protected override void IncrementAnimationFramesCounter(CameraFrameRenderInfo aCameraFrameRenderInfo)
        {
            if (RenderAnimationLengths.ContainsKey(aCameraFrameRenderInfo.AnimationNumber) == false)
            {
                GetComponent<ModelBaseManager>().OnNewAnimationRendering(aCameraFrameRenderInfo.AnimationNumber);
            }
            
            base.IncrementAnimationFramesCounter(aCameraFrameRenderInfo);
        }

        protected override void EndRenderStep()
        {
            //Do nothing
        }
    }
}