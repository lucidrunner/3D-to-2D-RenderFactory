#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Render3DTo2D.Utility
{
    [InitializeOnLoad]
    public static class PlayStateDelegater
    {
        public static event EventHandler OnExitingPlayMode;

        static PlayStateDelegater()
        {
            EditorApplication.playModeStateChanged += ModeChanged;
        }
        static void ModeChanged(PlayModeStateChange playModeState)
        {

            if(playModeState == PlayModeStateChange.ExitingPlayMode)
            {
                OnExitingPlayMode?.Invoke(null, EventArgs.Empty);
            }


        }
    }
}


#endif