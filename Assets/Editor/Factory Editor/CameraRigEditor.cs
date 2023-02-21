using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Render3DTo2D.Logging;
using Render3DTo2D.RigCamera;
using Render3DTo2D.Rigging;
using Render3DTo2D.Setup;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(CameraRig))]
    public class CameraRigEditor : Editor
    {
        private SerializedProperty customPlacementModeProp;
        private SerializedProperty rigTypeProp;
        private SerializedProperty cameraListsProp;
        
        [SerializeField] private bool camerasFoldoutState = false;
        [SerializeField] private AnimBool camerasFoldoutStateCurrent;
        
        private void OnEnable()
        {
            customPlacementModeProp = serializedObject.FindProperty("manualPlacementMode");
            rigTypeProp = serializedObject.FindProperty("rigType");
            cameraListsProp = serializedObject.FindProperty("cameras");
            camerasFoldoutStateCurrent = new AnimBool(camerasFoldoutState);
            camerasFoldoutStateCurrent.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            var _target = (CameraRig)target;
            //If we're not doing custom placement, draw the script as normal
            if(customPlacementModeProp.boolValue == false)
            {
                base.OnInspectorGUI();
                //With one hitch, we'll draw the Validate method here too
                DrawValidate(_target);
                return;
            }

            //Otherwise, we'll need to manually draw it to add camera adding / removing & validation
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((CameraRig)target), typeof(CameraRig), false);
            GUI.enabled = true;


            string _rigType = rigTypeProp.enumDisplayNames[rigTypeProp.enumValueIndex];
            InspectorUtility.BeginBoxGroup("Custom Camera Placement", EditorColors.Header, EditorColors.Body);
            //First, show the set mode for the rig since this will influence validation
            EditorGUILayout.HelpBox(new GUIContent($"Camera Placement Mode: {_rigType}"));
            
            //Draw the collapsible list of current cameras
            DrawCurrentCameras(_target);
            
            //Finally, draw the validate button
            DrawValidate(_target);

            InspectorUtility.EndBoxGroup();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCurrentCameras(CameraRig aRig)
        {
            bool _displayCameras = InspectorUtility.BeginSubFoldoutGroup("Current Cameras", ref camerasFoldoutState, ref camerasFoldoutStateCurrent, EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
            
            //For each camera, draw it and an option to remove it from
            if (_displayCameras)
            {
                DrawCameraList(aRig);
            }

            InspectorUtility.EndFoldoutGroup(_displayCameras);
        }

        private void DrawCameraList(CameraRig aRig)
        {
            if (cameraListsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox(new GUIContent("No Current Cameras on Rig."));
            }

            //Draw each camera with the option to remove them
            for (var _index = 0; _index < cameraListsProp.arraySize; _index++)
            {
                var _camera = cameraListsProp.GetArrayElementAtIndex(_index);
                DrawCamera(aRig,_camera, _index);
            }

            if (InspectorUtility.DrawButton(new GUIContent("Add Camera"), EditorColors.ButtonRun))
            {
                var _anchor = aRig.GetComponentInChildren<CameraAnchor>();
                if (_anchor == null)
                {
                    FLogger.LogMessage(this, FLogger.Severity.LinkageError, "No Camera anchor found on rig, aborting camera setup. Is the preset broken?");
                    return;
                }
                
                GameObject _addedCamera = SetupResources.Instance.AddRenderCameraToAnchor(SetupResources.Instance.GetRenderCameraPrefab((CameraRigger.SetupInfo.RigType)rigTypeProp.enumValueIndex), _anchor.gameObject);
                aRig.AddCamera(_addedCamera);
            }
            
        }

        private void DrawCamera(CameraRig aRig, SerializedProperty aCamera, int aIndex)
        {
            SerializedObject _cam = new SerializedObject(aCamera.objectReferenceValue);
            _cam.UpdateIfRequiredOrScript();
            var _currentCameraName = _cam.FindProperty("lastSetName");
            var _deletionToggle = _cam.FindProperty("deletionSafetyToggle");

            Color _titleColor = aIndex % 2 == 0 ? EditorColors.HeaderAlt1 : EditorColors.HeaderAlt2;
            Color _contentColor = aIndex % 2 == 0 ? EditorColors.BodyAlt1 : EditorColors.BodyAlt2;
            
            GUILayout.Space(3);
            InspectorUtility.BeginSubBoxGroup(_currentCameraName.stringValue, _titleColor, _contentColor);
            EditorGUILayout.BeginHorizontal();
            InspectorUtility.DrawToggleProperty(_deletionToggle, new GUIContent("Deletion Safety Switch"), true);
            GUI.enabled = _deletionToggle.boolValue;
            if (InspectorUtility.DrawButton(new GUIContent("Delete Camera"), EditorColors.ButtonAction))
            {
                aRig.RemoveCamera((CameraRenderer)aCamera.objectReferenceValue);
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            InspectorUtility.EndBoxGroup();
            if(_cam.targetObject != null)
                _cam.ApplyModifiedProperties();
        }

        private void DrawValidate(CameraRig aCameraRig)
        {
            if (InspectorUtility.DrawButton(new GUIContent("Validate Camera Setup"), EditorColors.ButtonRunAlt))
            {
                aCameraRig.ValidateCameraSetup();
            }
        }
    }

}