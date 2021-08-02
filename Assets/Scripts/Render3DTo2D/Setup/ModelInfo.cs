using System;
using System.Linq;
using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using RootChecker;
using UnityEngine;

//Shortcut class to get some ModelRenderer inquiries regarding the model setup now that we've decoupled them from each other
namespace Render3DTo2D.Setup
{
    public class ModelInfo : MonoBehaviour
    {

        [SerializeField, HideInInspector] private string modelName;

        //TODO Make sure this also includes a check if we have 0 clips in the animator
        public bool CanAnimate => GetComponent<Animator>() != null;

        public Animator Animator => GetComponent<Animator>();

        public string ModelName => modelName;


        private void Start()
        {
            void OnRenderFactoryEventsOnFactoryStarted(object aSender, EventArgs aArgs)
            {
                var _settings = RenderingSettings.GetFor(transform);
                foreach (var _transform in _settings.RenderIgnoreList)
                {
                    var _renderer = _transform.GetComponent<Renderer>();
                    if (_renderer != null)
                        _renderer.enabled = false;
                }
            }

            RenderFactoryEvents.FactoryStarted += OnRenderFactoryEventsOnFactoryStarted;

            void OnRenderFactoryEventsOnFactoryEnded(object aSender, EventArgs aArgs)
            {
                var _settings = RenderingSettings.GetFor(transform);
                foreach (var _transform in _settings.RenderIgnoreList)
                {
                    if (_transform == null) continue;
                    var _renderer = _transform.GetComponent<Renderer>();
                    if (_renderer != null)
                        _renderer.enabled = true;
                }
            }

            RenderFactoryEvents.FactoryEnded += OnRenderFactoryEventsOnFactoryEnded;
        }

        public void RenameModel()
        {
            modelName = name;
            name = modelName + " - Model";
        }

        /// <summary>
        /// Retrieves and returns the ModelInfo for any given transform within the hierarchy
        /// </summary>
        public static ModelInfo GetFor(Transform aTransform)
        {
            Transform _root = RootFinder.FindHighestRoot(aTransform);
            return _root.GetComponentInChildren<ModelInfo>();
        }

        /// <summary>
        /// Helper function to make sure the animator has a controller and is set as disabled, otherwise our stop-motion animator won't work
        /// </summary>
        public void SetupAnimator()
        {
            if (Animator == null)
                return;
            
            Animator.enabled = false;
            if(Animator.runtimeAnimatorController == null)
                FLogger.LogMessage(this, FLogger.Severity.Priority, $"Can't find the Animator Controller on {gameObject.name} during setup, is it linked and valid?");
        }

        public void SetLayer(int aLayer)
        {
            gameObject.layer = aLayer;

            var _allChildren = GetComponentsInChildren<Transform>().Select(aTransform => aTransform.gameObject);

            foreach (var _child in _allChildren)
            {
                _child.layer = aLayer;
            }
        }
    }
}
