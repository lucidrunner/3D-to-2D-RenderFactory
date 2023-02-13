using Render3DTo2D.Factory_Core;
using Render3DTo2D.Logging;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.Render_Info_Export;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Utility.IO
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FactoryFolderCreator : FolderCreator
    {
        private FactoryFolderCreator(){}

        public static string CreateTransformExportFolderForFactory(Transform aFactoryTransform)
        {
            FolderSettings _settings = FolderSettings.GetFor(aFactoryTransform);
            string _modelName = RootFinder.FindHighestRoot(aFactoryTransform).name;
            
            //Create the base folder if it hasn't been created yet
            string _path = CreateBaseFolder(aFactoryTransform, _settings.CreateTimeStampedFolders);

            //Create the RootMotion folder for the run
            CreateFolder(_path, _settings.RootMotionDataFolderName);
            
            //And append it to the returned path
            _path += $"/{_settings.RootMotionDataFolderName}";
            return _path;
        }
        
        public static string CreateFolderForRig(CameraRig aRig)
        {
            FolderSettings _settings = FolderSettings.GetFor(aRig.transform);
            
            //Get or create the base folder path
            string _path = CreateBaseFolder(aRig.GetComponentInParent<RenderFactory>().transform, _settings.CreateTimeStampedFolders);

            //Create the folder for the rig
            CreateFolder(_path, aRig.name);
            _path += $"/{aRig.name}";

            //Return the path
            return _path;
        }

        public static string CreateStaticFolderForRig(CameraRig aRig)
        {
            FolderSettings _settings = FolderSettings.GetFor(aRig.transform);
            
            //Get the base path
            string _path = CreateBaseFolder(aRig.GetComponentInParent<RenderFactory>().transform, _settings.CreateTimeStampedFolders);

            //Create the static folder 
            CreateFolder(_path, _settings.StaticFolderName);
            _path += $"/{_settings.StaticFolderName}";

            //Create the folder for the rig
            CreateFolder(_path, aRig.name);
            _path += $"/{aRig.name}";


            return _path;
        }


        public static void CreateAnimationFolderForRig(CameraRig aRig, string aAnimationName)
        {
            FolderSettings _settings = FolderSettings.GetFor(aRig.transform);
            string _path = CreateFolderForRig(aRig);
            CreateFolder(_path, aAnimationName);
        }


        private static string CreateBaseFolder(Transform aFactoryTransform, bool aIncludeTimeStamp = false)
        {
            if(aFactoryTransform == null)
                FLogger.LogMessage(null, FLogger.Severity.LinkageError, "Failed with creating base folder due to factory transform being null.", nameof(FactoryFolderCreator));
            
            FolderSettings _settings = FolderSettings.GetFor(aFactoryTransform);
            
            //Create the base folder for the model if it doesn't exist yet
            string _modelName = RootFinder.FindHighestRoot(aFactoryTransform).name;
            CreateFolder(_settings.OutputBaseFolder, _modelName);

            string _path = $"{_settings.OutputBaseFolder}/{_modelName}";

            if (aIncludeTimeStamp)
            {
                _path = AppendTimestamp(aFactoryTransform, _path);
            }    
            
            //Return the folder name
            return _path;
        }

        private static string AppendTimestamp(Transform aFactoryTransform, string aToReturn)
        {
            string _timeStamp = aFactoryTransform.GetComponent<RenderFactory>()?.GetRenderTimestamp();

            if (!string.IsNullOrEmpty(_timeStamp))
            {
                CreateFolder(aToReturn, _timeStamp);
                aToReturn += $"/{_timeStamp}";
            }

            return aToReturn;
        }
    }
}