using Render3DTo2D.Factory_Core;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(RenderManager))]
    public class RenderManagerEditor : Editor
    {
        private SerializedProperty renderersProp;

        protected virtual void OnEnable()
        {
            renderersProp = serializedObject.FindProperty("rigRenderers");
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RenderManager) target), typeof(RenderManager), false);
            GUI.enabled = true;
            var _target = (RenderManager) target;
        
            EditorGUILayout.LabelField($"Current Renderer Count: {_target.RendererCount}", new GUIStyle("BoldLabel"));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(renderersProp);
            GUI.enabled = true;
        }
    }
}