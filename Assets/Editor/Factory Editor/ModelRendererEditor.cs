using Render3DTo2D;
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
        private SerializedProperty placementTypeProp;
        private SerializedProperty manualAngleProp;
        private SerializedProperty halfWrapProp;
    
        //Run
        private SerializedProperty runFactoryTypeProp;
    
        //Admin
        [SerializeField] private bool setupFoldoutState = false;
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
            placementTypeProp = serializedObject.FindProperty("placementType");
            manualAngleProp = serializedObject.FindProperty("manualAngle");
            halfWrapProp = serializedObject.FindProperty("halfWrap");

            var _target = (ModelRenderer) target;
            showRigAdd = new AnimBool(_target.ShowAdd);
            showFactoryAdd = new AnimBool(!_target.ShowAdd);
            showRigAdd.valueChanged.AddListener(Repaint);
            showFactoryAdd.valueChanged.AddListener(Repaint);
            showIsometricOptions  = new AnimBool(cameraRigProp.enumValueIndex ==  (int)CameraRigger.SetupInfo.RigType.Isometric);
            showIsometricOptions.valueChanged.AddListener(Repaint);
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
            
            setupFoldoutState = InspectorUtility.BeginFoldoutGroup(setupFoldoutState ? "Hide" : "Show", setupFoldoutState);
            
            if (setupFoldoutState)
            {
                EditorGUILayout.PropertyField(setupFactoryTypeProp);
                if (!Mathf.Approximately(showRigAdd.faded, 0f))
                {
                    DrawAddRigBox(aTarget);
                }
                else
                {
                    if(EditorGUILayout.BeginFadeGroup(showFactoryAdd.faded))
                        if (InspectorUtility.DrawButton(new GUIContent("Setup Factory"), EditorColors.ButtonAction))
                            aTarget.SetupRenderFactory();
                    EditorGUILayout.EndFadeGroup();
                }
            }

            InspectorUtility.EndFoldoutGroup(setupFoldoutState);
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

        private void DrawAddRigBox(ModelRenderer aTarget)
        {
            if (EditorGUILayout.BeginFadeGroup(showRigAdd.faded))
            {
                InspectorUtility.BeginSubBoxGroup("Rig Setup", EditorColors.Header, EditorColors.Body);
                InspectorUtility.BeginSubBoxGroup("Type", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                EditorGUILayout.PropertyField(cameraRigProp);
                if (ShowIsometric)
                {
                    EditorGUILayout.PropertyField(isometricAngleProp);
                    EditorGUILayout.PropertyField(isometricBaseSizeProp);
                }
                InspectorUtility.EndSubBoxGroup();
                InspectorUtility.BeginSubBoxGroup("Placement", EditorColors.HeaderAlt2, EditorColors.BodyAlt2);
                EditorGUILayout.PropertyField(numberOfCamerasProp);
                EditorGUILayout.PropertyField(initialCameraOffsetProp);
                EditorGUILayout.PropertyField(invertCameraRotationProp);
                EditorGUILayout.PropertyField(placementTypeProp);
                if (placementTypeProp.enumValueIndex == (int) CameraRigger.SetupInfo.PlacementType.AutoWrap)
                    EditorGUILayout.PropertyField(halfWrapProp, new GUIContent("Mirrored Half Wrap"));
                else if (placementTypeProp.enumValueIndex == (int) CameraRigger.SetupInfo.PlacementType.Manual)
                    EditorGUILayout.PropertyField(manualAngleProp, new GUIContent("Angle Between Cameras"));
                InspectorUtility.EndSubBoxGroup();
                if(InspectorUtility.DrawButton(new GUIContent("Add Rig To Factory"), EditorColors.ButtonRun, InspectorUtility.ButtonSize.Large))
                    aTarget.AddCameraRigToFactory();
                InspectorUtility.EndSubBoxGroup();
            }
            
            EditorGUILayout.EndFadeGroup();
            
        }

        private bool ShowIsometric => cameraRigProp.enumValueIndex == (int) CameraRigger.SetupInfo.RigType.Isometric;
    }
}
