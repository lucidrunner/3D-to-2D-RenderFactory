using System;
using Render3DTo2D.Rigging;
using Shared_Scripts;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(CameraRig))]
    public class CameraRigEditor : Editor
    {
        private SerializedProperty customPlacementModeProp;
        private SerializedProperty rigTypeProp;
        
        private void OnEnable()
        {
            customPlacementModeProp = serializedObject.FindProperty("manualPlacementMode");
            rigTypeProp = serializedObject.FindProperty("rigType");
        }

        public override void OnInspectorGUI()
        {
            //If we're not doing custom placement, draw the script as normal
            if(customPlacementModeProp.boolValue == false)
            {
                base.OnInspectorGUI();
                return;
            }

            //Otherwise, we'll need to manually draw it to add camera adding / removing & validation
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((CameraRig)target), typeof(CameraRig), false);
            GUI.enabled = true;
            var _target = (CameraRig)target;


            string _rigType = rigTypeProp.enumDisplayNames[rigTypeProp.enumValueIndex];
            //First, show the set mode for the rig since this will influence validation
            EditorColors.OverrideTextColors();
            EditorGUILayout.LabelField($"Camera Placement Mode: {_rigType}");
            
            EditorColors.ResetTextColor();

        }
    }

}