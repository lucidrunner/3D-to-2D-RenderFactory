using Render3DTo2D.Factory_Core;
using Render3DTo2D.Rigging;
using UnityEngine;

namespace Render3DTo2D.Setup
{
    public class ModelSetupHelper
    {
        
        public static RenderFactory AddAnimatedFactoryToObject(GameObject aGameObject)
        {
            return SetupResources.Instance.AddAnimatedRenderFactoryToGameObject(aGameObject);
        
        }
        public static RenderFactory AddStaticFactoryToObject(GameObject aGameObject)
        {
            return SetupResources.Instance.AddStaticRenderFactoryToGameObject(aGameObject);
        }

        public static void SetupCameraRig(RenderFactory aSetupFactory, GameObject aGameObject,
            CameraRigger.SetupInfo aSetupInfo)
        {
            //First get the Render Factory & its GameObject
            RenderFactory _renderFactory = aSetupFactory;
            if(_renderFactory == null)
            {
                //Abort setup since we have missed a step
                Debug.LogError($"Attempted to add Camera Rig without any Render Factory. Aborting setup.");
                return;
            }
            GameObject _rfGameObject = _renderFactory.gameObject;

            //Then attach a new Camera Rig to it by passing it along to the Camera Rigger with the setup info
            CameraRig _rig = CameraRigger.AddRigToModel(_rfGameObject, aSetupInfo);
            if (_rig == null)
                return;

            //Then pass the resulting Camera Rig to the Render factory so they can bind together
            _renderFactory.AddCameraRig(_rig);
        }

        public static void SetupHierarchyOnObject(GameObject aModelObject)
        {
            SetupResources.Instance.CreateRenderHierarchyOnGameObject(aModelObject);
        }

    }
}
