using Render3DTo2D.Factory_Core;
using Render3DTo2D.Model_Settings;
using Shared_Scripts;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(RenderFactory))]
    public class RenderFactoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RenderFactory) target), typeof(RenderFactory), false);
            GUI.enabled = true;
            var _target = (RenderFactory) target;

            if (InspectorUtility.DrawButton(new GUIContent("Validate Current Rigs", InspectorTooltips.ValidateCurrentRigs), EditorColors.ButtonAction))
            {
                _target.ValidateRigs();
            }
            
        }
    }
}
