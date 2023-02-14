using Render3DTo2D;
using Render3DTo2D.Model_Settings;
using Render3DTo2D.Rigging;
using Render3DTo2D.Utility.Inspector;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(ModelRenderer))]
    public class ModelRendererEditor : Editor
    {

        //Setup
        private SerializedProperty setupFactoryTypeProp;
        private SerializedProperty cameraRigProp;
        private SerializedProperty isometricAngleProp;
        private SerializedProperty isometricBaseSizeProp;
        private SerializedProperty numberOfCamerasProp;
        private SerializedProperty initialCameraOffsetProp;
        private SerializedProperty invertCameraRotationProp;
        private SerializedProperty rotationModeProp;
        private SerializedProperty manualAngleProp;
        private SerializedProperty halfWrapProp;
        private SerializedProperty prefabProp;
        private SerializedProperty placementModeProp;
    
        //Run
        private SerializedProperty runFactoryTypeProp;
    
        //Admin
        [SerializeField] private bool setupFoldoutTarget = false;
        [SerializeField] private AnimBool setupFoldoutCurrent;
        private AnimBool showIsometricOptions;
        private AnimBool showRigAdd;
        private AnimBool showFactoryAdd;


        private void OnEnable()
        {
            setupFactoryTypeProp = serializedObject.FindProperty("setupFactoryType");
            runFactoryTypeProp = serializedObject.FindProperty("runFactoryType");
            cameraRigProp = serializedObject.FindProperty("cameraRig");
            isometricAngleProp = serializedObject.FindProperty("isometricAngle");
            isometricBaseSizeProp = serializedObject.FindProperty("isometricBaseSize");
            numberOfCamerasProp = serializedObject.FindProperty("numberOfCameras");
            initialCameraOffsetProp = serializedObject.FindProperty("initialCameraOffset");
            invertCameraRotationProp = serializedObject.FindProperty("invertCameraRotation");
            rotationModeProp = serializedObject.FindProperty("rotationMode");
            manualAngleProp = serializedObject.FindProperty("manualAngle");
            halfWrapProp = serializedObject.FindProperty("halfWrap");
            prefabProp = serializedObject.FindProperty("prefabRig");
            placementModeProp = serializedObject.FindProperty("placementMode");
            

            var _target = (ModelRenderer) target;
            showRigAdd = new AnimBool(_target.ShowAdd);
            showFactoryAdd = new AnimBool(!_target.ShowAdd);
            showRigAdd.valueChanged.AddListener(Repaint);
            showFactoryAdd.valueChanged.AddListener(Repaint);
            showIsometricOptions  = new AnimBool(cameraRigProp.enumValueIndex ==  (int)CameraRigger.SetupInfo.RigType.Isometric);
            showIsometricOptions.valueChanged.AddListener(Repaint);
            setupFoldoutCurrent = new AnimBool(setupFoldoutTarget);
            setupFoldoutCurrent.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ModelRenderer) target), typeof(ModelRenderer), false);
            GUI.enabled = true;
            var _target = (ModelRenderer) target;
            showRigAdd.target = _target.ShowAdd;
            showFactoryAdd.target = Mathf.Approximately(showRigAdd.faded, 0f) && !_target.ShowAdd;
            showIsometricOptions.target = cameraRigProp.enumValueIndex == (int) CameraRigger.SetupInfo.RigType.Isometric;

            EditorColors.OverrideTextColors();
            DrawSetup(_target);
            DrawSettingsOverrides(_target);
            DrawRun(_target);
            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSetup(ModelRenderer aTarget)
        {
            GUILayout.Label("Setup", new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold});
            InspectorUtility.GuiLine(aHeight: 2, aSpaceBefore: -5, aSpaceAfter: -2);
            
            
            bool _showFoldout = InspectorUtility.BeginFoldoutGroup("Details", ref setupFoldoutTarget, ref setupFoldoutCurrent);
            
            if (_showFoldout)
            {
                InspectorUtility.DrawProperty(setupFactoryTypeProp, new GUIContent(setupFactoryTypeProp.displayName, InspectorTooltips.SetupFactoryType));
                if (!Mathf.Approximately(showRigAdd.faded, 0f))
                {
                    DrawRigSetup(aTarget);
                }
                else
                {
                    if(EditorGUILayout.BeginFadeGroup(showFactoryAdd.faded))
                        if (InspectorUtility.DrawButton(new GUIContent("Setup Factory"), EditorColors.ButtonAction))
                            aTarget.SetupRenderFactory();
                    EditorGUILayout.EndFadeGroup();
                }
            }

            InspectorUtility.EndFoldoutGroup(_showFoldout);
        }

        private static void DrawSettingsOverrides(ModelRenderer aTarget)
        {
            EditorGUILayout.Space(10f);
            GUILayout.Label("Settings", new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold});
            InspectorUtility.GuiLine(aHeight: 2, aSpaceBefore: -5);
            GUI.enabled = aTarget.CanAddRenderSettings;
            if (InspectorUtility.DrawButton(new GUIContent("Add Render Settings Override"), EditorColors.ButtonAction))
            {
                aTarget.AddRenderSettingsOverride();
            }

            GUI.enabled = aTarget.CanAddFolderSettings;
            if (InspectorUtility.DrawButton(new GUIContent("Add Folder Settings Override"), EditorColors.ButtonAction))
            {
                aTarget.AddFolderSettingsOverride();
            }

            GUI.enabled = aTarget.CanAddAdvancedAnimationSettings;
            if (InspectorUtility.DrawButton(new GUIContent("Add Advanced Animation Settings"), EditorColors.ButtonAction))
            {
                aTarget.AddAdvancedAnimationSettings();
            }

            GUI.enabled = aTarget.CanAddRootMotionSettings;
            if (InspectorUtility.DrawButton(new GUIContent("Add Root Motion Settings"), EditorColors.ButtonAction))
            {
                aTarget.AddRootMotionSettings();
            }

            GUI.enabled = true;
        }

        private void DrawRun(ModelRenderer aTarget)
        {
            EditorGUILayout.Space(10f);
            GUILayout.Label("Run", new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Bold});
            InspectorUtility.GuiLine(aHeight: 2, aSpaceBefore: -5);

            EditorGUILayout.PropertyField(runFactoryTypeProp);
            string _warningBoxString = aTarget.GetInvalidRunMessage();
            if (_warningBoxString != null)
            {
                EditorGUILayout.HelpBox(_warningBoxString, MessageType.Warning);
            }
            GUI.enabled = aTarget.EnableRun && EditorApplication.isPlaying;
            if (InspectorUtility.DrawButton(new GUIContent("Render Model"), EditorColors.ButtonRun, InspectorUtility.ButtonSize.Massive))
            {
                aTarget.RenderModel();
            }

            if (runFactoryTypeProp.enumValueIndex == (int) ModelRenderer.RenderFactoryType.Animated)
            {
                GUI.enabled = true;
                InspectorUtility.BeginBoxGroup("Force Recalculate Frame Data", EditorColors.Header, EditorColors.Body);
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = aTarget.EnableRun && EditorApplication.isPlaying;
                if (InspectorUtility.DrawButton(new GUIContent("On Toggled Rigs"), EditorColors.ButtonRunAlt))
                {
                    aTarget.CalculateToggledRigs();
                }

                if (InspectorUtility.DrawButton(new GUIContent("On All Rigs"), EditorColors.ButtonRunAlt))
                {
                    aTarget.CalculateAllRigs();
                }

                EditorGUILayout.EndHorizontal();
                InspectorUtility.EndBoxGroup();
            }

            GUI.enabled = true;
        }

        private void DrawRigSetup(ModelRenderer aTarget)
        {
            if (EditorGUILayout.BeginFadeGroup(showRigAdd.faded))
            {
                InspectorUtility.BeginSubBoxGroup("Rig Setup", EditorColors.Header, EditorColors.Body);
                InspectorUtility.BeginSubBoxGroup("Type", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                InspectorUtility.DrawProperty(cameraRigProp, new GUIContent(cameraRigProp.displayName, InspectorTooltips.CameraRig));
                if (ShowPrefab)
                {
                    prefabProp.objectReferenceValue = InspectorUtility.DrawObjectPicker(new GUIContent(prefabProp.displayName), prefabProp.objectReferenceValue, typeof(CameraRig));
                }
                InspectorUtility.EndSubBoxGroup();
                if (!ShowPrefab)
                {
                    InspectorUtility.BeginSubBoxGroup("Placement", EditorColors.HeaderAlt2, EditorColors.BodyAlt2);
                    InspectorUtility.DrawProperty(placementModeProp, new GUIContent(placementModeProp.displayName, InspectorTooltips.PlacementMode));
                    if (ShowPlacementOptions)
                    {
                        InspectorUtility.DrawProperty(numberOfCamerasProp, new GUIContent(numberOfCamerasProp.displayName, InspectorTooltips.NumberOfCameras));
                        InspectorUtility.DrawProperty(initialCameraOffsetProp, new GUIContent(initialCameraOffsetProp.displayName, InspectorTooltips.InitialCameraOffset));
                        InspectorUtility.DrawProperty(invertCameraRotationProp, new GUIContent(invertCameraRotationProp.displayName, InspectorTooltips.InvertCameraRotation));
                        InspectorUtility.DrawProperty(rotationModeProp, new GUIContent(rotationModeProp.displayName, InspectorTooltips.RotationMode));
                        if (rotationModeProp.enumValueIndex == (int) CameraRigger.SetupInfo.RotationMode.AutoWrap)
                            InspectorUtility.DrawProperty(halfWrapProp, new GUIContent("Mirrored Half Wrap", InspectorTooltips.HalfWrap));
                        else if (rotationModeProp.enumValueIndex == (int) CameraRigger.SetupInfo.RotationMode.Manual)
                            InspectorUtility.DrawProperty(manualAngleProp, new GUIContent("Angle Between Cameras"));
                        if (ShowIsometric)
                        {
                            InspectorUtility.DrawProperty(isometricAngleProp, new GUIContent(isometricAngleProp.displayName, InspectorTooltips.IsometricAngle));
                            InspectorUtility.DrawProperty(isometricBaseSizeProp, new GUIContent(isometricBaseSizeProp.displayName, InspectorTooltips.IsometricBaseline));
                        }
                    }
                    InspectorUtility.EndSubBoxGroup();
                }
                if(InspectorUtility.DrawButton(new GUIContent("Add Rig To Factory"), EditorColors.ButtonRun, InspectorUtility.ButtonSize.Large))
                    aTarget.AddCameraRigToFactory();
                InspectorUtility.EndSubBoxGroup();
            }
            
            EditorGUILayout.EndFadeGroup();
            
        }

        private bool ShowIsometric => cameraRigProp.enumValueIndex == (int) CameraRigger.SetupInfo.RigType.Isometric;
        private bool ShowPrefab => cameraRigProp.enumValueIndex == (int)CameraRigger.SetupInfo.RigType.Prefab;
        private bool ShowPlacementOptions => placementModeProp.enumValueIndex != (int)CameraRigger.SetupInfo.PlacementMode.CustomPlacement;
    }
}
