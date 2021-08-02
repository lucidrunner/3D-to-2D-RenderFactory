using System;
using System.Collections.Generic;
using System.Linq;
using Render3DTo2D.Utility;
using Render3DTo2D.Utility.Inspector;
using UnityEngine;

namespace Render3DTo2D.Logging
{
    public class LoggingSettings : MonoBehaviour
    {
        private void Reset()
        {
            ResetLoggedLevels();
        }

        public void ResetLoggedLevels()
        {
            loggedLevels = new List<ToggleAbleButton>();
            string[] _names = Enum.GetNames(typeof(FLogger.Severity));
            foreach (string _name in _names)
            {
                loggedLevels.Add(new ToggleAbleButton( GeneralUtilities.AddSpacesToSentence(_name), true));
            }
        }

        [SerializeField]
        private List<ToggleAbleButton> loggedLevels;

        [SerializeField] private FLogger.Severity minimumTimeStampedLevel = FLogger.Severity.Priority;

        [SerializeField] private string timeStampFormat = "HH.mm:ss:fff";
            
        [SerializeField] private bool includeOriginClass = false;

        [SerializeField] private bool includeSenderLabel = false;
        
        public FLogger.Severity[] LoggedSeverityLevels()
        {
            var _levels = new List<FLogger.Severity>();
            for (int _index = 0; _index < loggedLevels.Count(); _index++)
            {
                if(loggedLevels[_index].State == true)
                    _levels.Add((FLogger.Severity)_index);
            }
            return _levels.ToArray();
        }

        public FLogger.Severity MinimumTimeStampedLevel => minimumTimeStampedLevel;
        
        public string TimeStampFormat => timeStampFormat;

        public bool IncludeOriginClass => includeOriginClass;

        public bool IncludeSenderLabel => includeSenderLabel;

        #region Singleton

        //Java-style logging tag for the class
        private const string Tag = nameof(LoggingSettings);

        //Singleton implementation for a MonoBehaviour
        private static LoggingSettings _instance = null;

        public static LoggingSettings Instance
        {
            get
            {
                if (_instance != null) return _instance;
                //Note - In Editor mode this resets after play but should be fine? Not the most efficient method obv but since it's only accessed once it should be ok.
                _instance = FindObjectOfType(typeof(LoggingSettings)) as LoggingSettings;
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