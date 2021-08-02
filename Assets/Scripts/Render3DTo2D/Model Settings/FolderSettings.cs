using System;
using System.IO;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    public class FolderSettings : MonoBehaviour
    {
        #region Inspector

        public string OutputBaseFolder => outputBaseFolder;

        [SerializeField]
        protected string outputBaseFolder;

        public bool CreateTimeStampedFolders => createTimeStampedFolders;                                                                                                           
        
        [SerializeField] 
        protected bool createTimeStampedFolders = true;

        public bool CreateAnimationSubfolders => createAnimationSubfolders;

        [SerializeField]
        protected bool createAnimationSubfolders = false;

        public string StaticFolderName => staticFolderName;

        [SerializeField]
        protected string staticFolderName = "Static";

        public string RootMotionDataFolderName => rootMotionDataFolderName;

        [SerializeField]
        protected string rootMotionDataFolderName = "Root Motion";
        

        #endregion

        #region Methods

        public static FolderSettings GetFor(Transform aTransform)
        {
            FolderSettings _localSettings = RootFinder.FindHighestRoot(aTransform).GetComponent<FolderSettings>();
            return _localSettings == null ? GlobalFolderSettings.Instance : _localSettings;
        }

        protected void Start()
        {
            //A bit of safeguarding to make sure we recreate the output folder if it's been deleted
            if (!outputBaseFolder.Equals("") && !Directory.Exists(outputBaseFolder))
            {
                Directory.CreateDirectory(outputBaseFolder);
            }
        }

        private void Reset()
        {
            if (string.IsNullOrWhiteSpace(outputBaseFolder) && GlobalFolderSettings.Instance != null)
                outputBaseFolder = string.Copy(GlobalFolderSettings.Instance.OutputBaseFolder);
        }

        public void ResetToGlobal()
        {
            var _global = GlobalFolderSettings.Instance;
            if (_global == null)
                return;
            
            outputBaseFolder = string.Copy(_global.OutputBaseFolder);
            createTimeStampedFolders = _global.createTimeStampedFolders;
            createAnimationSubfolders = _global.createAnimationSubfolders;
            staticFolderName = string.Copy(_global.staticFolderName);
            rootMotionDataFolderName = string.Copy(_global.rootMotionDataFolderName);
        }

        internal FolderSettings SetBaseFolder(string aOutputFolder)
        {
            outputBaseFolder = aOutputFolder;
            return this;
        }

        #endregion
    }
}