using UnityEngine;

//This is nothing more than a singleton implemented RenderingSettings so we can use applied RenderingSettings on singular objects to override this based on the GetSettingsFor() method
namespace Render3DTo2D.Model_Settings
{
    public class GlobalRenderingSettings : RenderingSettings
    {
        #region Inspector
        #region Setup

        public int DefaultInsertClampCutoff => defaultInsertClampCutoff;
        [SerializeField] private int defaultInsertClampCutoff = 25;

        public int ClampingDecimalTolerance => clampingDecimalTolerance;
        [SerializeField] private int clampingDecimalTolerance = 3;
        #endregion
        #endregion
        
        #region Singleton
        //Java-style logging tag for the class
        private const string Tag = nameof(GlobalRenderingSettings);

        //Singleton implementation for a MonoBehaviour
        private static GlobalRenderingSettings _instance = null;

        public static GlobalRenderingSettings Instance
        {
            get
            {
                if(_instance == null)
                {
                    //Note - In Editor mode this resets after play but should be fine? Not the most efficient method obv but since it's only accessed once it should be ok.
                    _instance = FindObjectOfType(typeof(GlobalRenderingSettings)) as GlobalRenderingSettings;
                    if(_instance == null)
                    {
                        Debug.LogError($"{Tag}: Failed with finding {Tag} in the scene when it was accessed. Is it added to the Overseer object?");
                    }
                }

                return _instance;
            }
        }


        #endregion

    }
}
