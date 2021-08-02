using System;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Rigging;
using Render3DTo2D.Root_Movement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Render3DTo2D.Setup
{
    public class SetupResources : MonoBehaviour
    {
        
        #region Private Fields


        //The prefab for the animated Render Factory 
        [FormerlySerializedAs("renderFactoryPrefab")] [SerializeField]
        private GameObject animatedRenderFactoryPrefab = null;
        //The prefab for the static render factory
        [SerializeField]
        private GameObject staticRenderFactoryPrefab = null;
        [SerializeField]
        private GameObject cameraRigPrefab = null;
        [SerializeField] 
        private GameObject isometricCameraRigPrefab = null;
        //The prefab for the OrtoRotational Render Camera (Used for standard 1-X sided rotational rendering)
        [SerializeField]
        private GameObject sideViewCameraPrefab = null;
        [SerializeField]
        private GameObject isometricViewCameraPrefab = null;
        //The prefab for the parent object we put the model renderer on
        [SerializeField] 
        private GameObject renderParentObjectPrefab = null;


        //Java-style logging tag for the class
        private static readonly string Tag = nameof(SetupResources);

        #endregion

        #region Singleton

        //Singleton implementation for a MonoBehaviour
        private static SetupResources _instance = null;

        public static SetupResources Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                //Note - In Editor mode this resets after play but should be fine? Not the most efficient method obv but since it's only accessed once it should be ok.
                _instance = FindObjectOfType(typeof(SetupResources)) as SetupResources;
                if(_instance == null)
                {
                    Debug.LogError($"{Tag}: Failed with finding {Tag} in the scene when it was accessed. Is it added to the Overseer object?");
                }

                return _instance;
            }
        }
        
        #endregion
    
        #region Public Methods

        //Need to do this in here since it's a Mono->Mono operation

        public RenderFactory AddAnimatedRenderFactoryToGameObject(GameObject aGameObject)
        {
            string _name = animatedRenderFactoryPrefab.name;
            GameObject _go = Instantiate(animatedRenderFactoryPrefab, aGameObject.transform, false);
            _go.name = _name;
            return _go.GetComponent<RenderFactory>();
            /*
         * Leaving these prefab spawns in to make sure I don't use them in the future
         * The problem with them is that unless data in lists is added into the inspector manually it's not recognized as changed
         * which leads to them resetting automatically generated lists (of things like rigs etc) to empty on startup unless they're manually copy+pasted
         * This is obviously not something we want to spend time doing (+ really annoying when missed since new setup is necessary) so we're just gonna have to be happy with
         * instantiating "normal" GameObjects rather than prefabs.
         */
            //PrefabUtility.InstantiatePrefab(animatedRenderFactoryPrefab, aGameObject.transform);
        }

        public RenderFactory AddStaticRenderFactoryToGameObject(GameObject aGameObject)
        {
            string _name = staticRenderFactoryPrefab.name;
            GameObject _go = Instantiate(staticRenderFactoryPrefab, aGameObject.transform, false);
            _go.name = _name;
            return _go.GetComponent<RenderFactory>();
        }

        public GameObject GetRenderCameraPrefab(CameraRigger.SetupInfo.RigType aRenderCameraType)
        {
            switch(aRenderCameraType)
            {
                case CameraRigger.SetupInfo.RigType.SideView:
                    return sideViewCameraPrefab;
                case CameraRigger.SetupInfo.RigType.Isometric:
                    return isometricViewCameraPrefab;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aRenderCameraType), aRenderCameraType, null);
            }
        }


        public GameObject AddCameraRigToGameObject(GameObject aGameObject, CameraRigger.SetupInfo.RigType aSetupInfoRig)
        {
            //Since Instantiation creates a Name (Copy) we save and apply the names instead
            string _name = cameraRigPrefab.name;
            GameObject _toReturn = null;
            switch(aSetupInfoRig)
            {
                case CameraRigger.SetupInfo.RigType.SideView:
                    _toReturn = Instantiate(cameraRigPrefab, aGameObject.transform, false);
                    break;
                case CameraRigger.SetupInfo.RigType.Isometric:
                    _toReturn = Instantiate(isometricCameraRigPrefab, aGameObject.transform, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(aSetupInfoRig), aSetupInfoRig, null);
            }

            _toReturn.name = _name;
            return _toReturn;

            //return PrefabUtility.InstantiatePrefab(cameraRigPrefab, aGameObject.transform) as GameObject;
        }

        public GameObject AddRenderCameraToAnchor(GameObject aRenderCamera, GameObject aAnchor)
        {
            string _name = aRenderCamera.name;
            GameObject _toReturn = Instantiate(aRenderCamera, aAnchor.transform, false);
            _toReturn.name = _name;
            return _toReturn;

            // return PrefabUtility.InstantiatePrefab(aRenderCamera, aAnchor.transform) as GameObject;
        }
        

        public void CreateRenderHierarchyOnGameObject(GameObject aModelObject)
        {
            //Remove the old model info if needed and set the model position
            if(aModelObject.GetComponent<ModelInfo>() != null)
                Destroy(aModelObject.GetComponent<ModelInfo>());
            var _position = Overseer.GetUniqueScenePosition();
            //Don't set a position if we're lacking a overseer or have turned off the 
            
            //Create the parent object, set the name to match our model and put our model as a child object of it
            string _name = aModelObject.name;
            GameObject _parent = Instantiate(renderParentObjectPrefab, null, false);
            if (_position != null)
                _parent.transform.position = new Vector3(_position.Value.x, aModelObject.transform.localPosition.y, _position.Value.y);
            _parent.name = _name;
            aModelObject.transform.SetParent(_parent.transform);
            
            //add the model info and run our setup methods
            aModelObject.AddComponent<ModelInfo>();
            aModelObject.GetComponent<ModelInfo>().RenameModel();
            aModelObject.GetComponent<ModelInfo>().SetupAnimator();
            
            //Setup the clamp if needed
            if (aModelObject.GetComponent<MovementClamp>() == null)
                aModelObject.AddComponent<MovementClamp>();
            
            
            
            //Reset the model local positionals
            aModelObject.transform.localPosition = Vector3.zero;
            aModelObject.transform.localEulerAngles = Vector3.zero;
            aModelObject.transform.localScale = Vector3.one;
            
        }


        #endregion

    }
}
