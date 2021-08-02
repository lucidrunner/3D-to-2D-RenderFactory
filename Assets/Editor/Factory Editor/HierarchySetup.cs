#if UNITY_EDITOR

using Render3DTo2D;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Root_Movement;
using Render3DTo2D.Setup;
using Render3DTo2D.Utility;
using RootChecker;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    public class HierarchySetup
    {
    
        [MenuItem("RenderFactory/Setup Rendering Hierarchy",false, 30)]
        public static void SetupModelHierarchy()
        {
            //Begin by spawning the overseer if we've not already done so
            if(Object.FindObjectOfType<Overseer>() == null)
                AddOverseer();
        
            GameObject _selected = Selection.activeGameObject;
        
            //Clean up the model name before creating the hierarchy
            _selected.name = GeneralUtilities.CleanName(_selected.name);
        
            Transform _rootTransform = RootFinder.FindHighestRoot(_selected.transform);
            if(_rootTransform.GetComponent<ModelRenderer>() != null)
            {
                FLogger.LogMessage(null, FLogger.Severity.Error, $"Found at least partial rendering setup on {_selected.name}. Remove root object with {nameof(ModelRenderer)} script and attempt hierarchy setup again.", nameof(HierarchySetup));
            }
            else
            {
                ModelSetupHelper.SetupHierarchyOnObject(_selected);
                GeneralUtilities.FocusSceneCamera(_selected);
            }
        }
        
        [MenuItem("RenderFactory/Setup Rendering Hierarchy",true)]
        public static bool IsSetupObject()
        {
            var _object = Selection.activeGameObject;
            return _object != null && _object.scene.IsValid();
        }

        [MenuItem("RenderFactory/Add Overseer to Scene", false, 25)]
        public static void AddOverseer()
        {
            if (Object.FindObjectOfType<Overseer>() != null) return;
            GameObject _overseer = FactorySettings.GetOrCreateSettings().OverseerPrefab;
            if(_overseer == null)
                _overseer = new GameObject("Overseer", typeof(Overseer), typeof(SetupResources), typeof(GlobalRenderingSettings), typeof(GlobalFolderSettings), typeof(TransformRecorder), typeof(LoggingSettings));
            else
            {
                PrefabUtility.InstantiatePrefab(_overseer);
            }
            _overseer.transform.SetAsFirstSibling();
        }

        [MenuItem("RenderFactory/Add Overseer to Scene", true)]
        public static bool EnableOverseer()
        {
            return Object.FindObjectOfType<Overseer>() == null;
        }
    }
}

#endif
