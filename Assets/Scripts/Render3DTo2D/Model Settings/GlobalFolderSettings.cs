
using System;
using UnityEngine;

namespace Render3DTo2D.Model_Settings
{
    public class GlobalFolderSettings : FolderSettings
    {
        #region Inspector

        public string IsometricRigTag => isometric;

        [SerializeField]
        private string isometric = "ISO";


        public string SideViewRigTag => sideView;

        [SerializeField]
        private string sideView = "SIDE";

        public string TopDownRigTag => topDown;

        [SerializeField] private string topDown = "TOP";

        #endregion
        
        #region Singleton
        //Java-style logging tag for the class
        private const string Tag = nameof(GlobalFolderSettings);

        //Singleton implementation for a MonoBehaviour
        private static GlobalFolderSettings _instance = null;

        public static GlobalFolderSettings Instance
        {
            get
            {
                if (_instance != null) return _instance;
                //Note - In Editor mode this resets after play but should be fine? Not the most efficient method obv but since it's only accessed once it should be ok.
                _instance = FindObjectOfType(typeof(GlobalFolderSettings)) as GlobalFolderSettings;
                if(_instance == null)
                {
                    Debug.LogError($"{Tag}: Failed with finding {Tag} in the scene when it was accessed. Is it added to the Overseer object?");
                }
                return _instance;
            }
        }

        

        #endregion
    }
}