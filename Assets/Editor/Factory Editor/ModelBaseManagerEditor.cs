using System;
using Render3DTo2D.Isometric;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(ModelBaseManager))]
    public class ModelBaseManagerEditor : Editor
    {
        private SerializedProperty cameraAngleProp;
        private SerializedProperty orthoScaleProp;
        private SerializedProperty referencePlateScaleProp;
        
        private SerializedProperty needAlignmentProp;
        private SerializedProperty autoSyncProp;
        private SerializedProperty modelBaseObjectProp;

        private void OnEnable()
        {
            cameraAngleProp = serializedObject.FindProperty("cameraAngle");
            orthoScaleProp = serializedObject.FindProperty("orthographicScale");
            referencePlateScaleProp = serializedObject.FindProperty("referencePlateScale");

            needAlignmentProp = serializedObject.FindProperty("camerasNeedAlignment");
            autoSyncProp = serializedObject.FindProperty("syncAutomatically");
            modelBaseObjectProp = serializedObject.FindProperty("modelBase");
        }

        private float currentCameraAngle;
        private float currentOrthoScale;
        private float currentReferencePlateScale;


        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ModelBaseManager) target), typeof(ModelBaseManager), false);
            EditorGUILayout.ObjectField(modelBaseObjectProp, typeof(GameObject));
            GUI.enabled = true;
            var _target = (ModelBaseManager) target;

            
            EditorColors.OverrideTextColors();
            EditorGUI.BeginChangeCheck();
            //Set Camera Angle Box
            InspectorUtility.BeginBoxGroup("Set Camera Angle", EditorColors.Header, EditorColors.Body);
            EditorGUILayout.PropertyField(cameraAngleProp);
            InspectorUtility.EndBoxGroup();
            
            //Set Size By Box
            InspectorUtility.BeginBoxGroup("Set Size by", EditorColors.Header, EditorColors.Body);
            EditorGUILayout.PropertyField(orthoScaleProp);
            EditorGUILayout.PropertyField(referencePlateScaleProp);
            InspectorUtility.EndBoxGroup();
            
            bool _hasChangedValues = EditorGUI.EndChangeCheck();
            
            //Sync box
            InspectorUtility.BeginBoxGroup("Sync Cameras to Model Base", EditorColors.Header, EditorColors.Body);
            if (needAlignmentProp.boolValue && !autoSyncProp.boolValue)
            {
                EditorGUILayout.HelpBox(new GUIContent("Cameras haven't been aligned after the angle or size changed. Please align manually unless this is intended."));
            }
            autoSyncProp.boolValue = InspectorUtility.DrawToggleButton(autoSyncProp.boolValue, new GUIContent("Sync Automatically"));
            if(autoSyncProp.boolValue == false)
            {
                if (InspectorUtility.DrawButton(new GUIContent("Force Center all Cameras"), EditorColors.ButtonRunAlt))
                {
                    _target.CenterCameras();
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (InspectorUtility.DrawButton(new GUIContent("Match Base Widths"), EditorColors.ButtonRunAlt, GUILayout.MaxWidth(Screen.width / 2f)))
            {
                _target.MatchBaseWidths();
            }

            if (InspectorUtility.DrawButton(new GUIContent("Align Cameras With Base"), EditorColors.ButtonRunAlt))
            {
                _target.AlignCamerasWithBase();
            }
            
            EditorGUILayout.EndHorizontal();
            InspectorUtility.EndBoxGroup();
            EditorColors.ResetTextColor();
            
            //We're only interested in letting people undo the manually changed properties, not the cascading values down below
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            if (_hasChangedValues)
            {
                if(!Mathf.Approximately(currentCameraAngle, cameraAngleProp.floatValue))
                {
                    currentCameraAngle = cameraAngleProp.floatValue;
                    _target.ApplyCameraAngle(currentCameraAngle);
                }

                if (!Mathf.Approximately(currentOrthoScale, orthoScaleProp.floatValue))
                {
                    currentOrthoScale = orthoScaleProp.floatValue;
                    _target.SetOrthographicSize(currentOrthoScale);
                    referencePlateScaleProp.floatValue = _target.CurrentPlateSize;
                    currentReferencePlateScale = _target.CurrentPlateSize;
                }

                if (!Mathf.Approximately(currentReferencePlateScale, referencePlateScaleProp.floatValue))
                {
                    currentReferencePlateScale = referencePlateScaleProp.floatValue;
                    _target.SetPlateSize(currentReferencePlateScale);
                    orthoScaleProp.floatValue = _target.CurrentOrthoScale;
                    currentOrthoScale = _target.CurrentOrthoScale;
                }
            }
            
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}