using System;
using System.Text;
using UnityEngine;
using Ywz;

namespace Render3DTo2D.Logging
{
    public static class FLogger
    {
        public static void LogMessage(object aOriginClass, Severity aSeverity, string aMessage, string aSenderLabel = "")
        {
            LogMessage(aOriginClass, new LogMessage(aSeverity, aMessage, aSenderLabel));
        }
        
        // ReSharper disable once CognitiveComplexity
        public static void LogMessage(object aOriginClass, LogMessage aMessage)
        { 
            var _settings = LoggingSettings.Instance;;
            var _loggedLevels = _settings.LoggedSeverityLevels();
            
            //If we're not supposed to log, instantly return
            if (!_loggedLevels.Contains(aMessage.Severity))
            {
                return;
            }

            var _sb = new StringBuilder();

            //Begin by timestamping if we should
            if(aMessage.Severity >= _settings.MinimumTimeStampedLevel)
                _sb.Append($"{DateTime.Now.ToString(_settings.TimeStampFormat)} - ");

            bool _labelAdded = false;
            bool _originAdded = false;
            //Then add the origin class if requested
            if (_settings.IncludeOriginClass && aOriginClass != null)
            {
                _sb.Append($"{aOriginClass.GetType().Name} ");
                _originAdded = true;
            }
            
            //If origin is null (ie static or otherwise non-instanced), check if we can use the label instead
            else if (aOriginClass == null && !string.IsNullOrWhiteSpace(aMessage.SenderLabel))
            {
                _sb.Append($"{aMessage.SenderLabel} ");
                _labelAdded = true;
            }
            
            
            //Include the label if requested and we haven't already replaced the class with label for a static class
            if (_settings.IncludeSenderLabel && !_labelAdded)
            {
                //If we've already added the origin class include the ( ) around the label, otherwise print it as it is
                if (_originAdded)
                    _sb.Append("(");
                _sb.Append(aMessage.SenderLabel);
                _sb.Append($"{(_originAdded ? ") " : " ")}");
            }


            //Replace the last char (if we've set anything so far) with a ": " instead
            if (_sb.Length > 0)
                _sb.Remove(_sb.Length - 1, 1).Append(": ");

            //Append the actual message
            _sb.Append(aMessage.Message);

            //Depending on if we're an error or status message, send it to the correct unity logging channel
            if (aMessage.Severity < Severity.Error)
            {
                Debug.Log(_sb.ToString());
            }
            else
            {
                Debug.LogError(_sb.ToString());
            }
        }

        //It should be noted that these need to be in order due to some enum / int comparisons
        public enum Severity
        {
            Debug, //Generally unnecessary debug messages (Eg "Setting up blank matrix with length xx and total time yy")
            Status, //Standard status logging (Eg "Performing step x / 1000 now!")
            Priority, //Priority messages (Eg "Rendering starting at 20xx:xx")
            Error, //Standard Error message (Eg "Missing frame data for frame xx:xx; using default size")
            FatalError, //Priority Error (Eg "Incorrect scale data loaded, aborting rendering.")
            LinkageError //Internal linkage null errors, eg when a subcomponent isn't found. This means some manual repairs are needed and should always be logged.
        }
    }


    public readonly struct LogMessage
    {
        /// <summary>
        /// The logged message
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// The severity level of the message. Can be used to filter both messages and time stamping for messages in the settings class
        /// </summary>
        public FLogger.Severity Severity { get;}
        /// <summary>
        /// An optional label for easier grouping. If set for a message logged from a static class, this will be printed if either print label or print origin class is set as true
        /// </summary>
        public string SenderLabel { get; }


        public LogMessage(FLogger.Severity aSeverity, string aMessage, string aSenderLabel)
        {
            Message = aMessage;
            SenderLabel = aSenderLabel;
            Severity = aSeverity;
        }
    }
}