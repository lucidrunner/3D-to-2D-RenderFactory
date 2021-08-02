
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(RootFinderDebugTest))]
    public class RootFinderDebugTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RootFinderDebugTest) target), typeof(RootFinderDebugTest), false);
            GUI.enabled = true;
            var _target = (RootFinderDebugTest) target;
        
            if (InspectorUtility.DrawButton(new GUIContent("Debug Test"), EditorColors.ButtonRunAlt))
            {
                _target.DebugRoot();
            }
        }
    }
}
