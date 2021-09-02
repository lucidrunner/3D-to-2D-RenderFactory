using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility.Inspector;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    [CustomEditor(typeof(RenderingSettings))]
    public class RenderingSettingsEditor : Editor
    {
        //Admin
        private SerializedProperty settingsModeProp;
     
        //General
        private SerializedProperty fpsProp;
        private SerializedProperty centerModelProp;
 
        //Calculator
        private SerializedProperty boundsCalcEnabledProp;
        private SerializedProperty automaticDepthCalculationProp;
 
        //Bounds Calculator
        private SerializedProperty boundsCalcOverdrawProp;
        private SerializedProperty boundsCalcStepPercentageProp;
        private SerializedProperty boundsCalcMaxPaddingProp;
        private SerializedProperty boundsCalcMaxStepsProp;
        private SerializedProperty boundsIncludeRenderIgnoreProp;
        private SerializedProperty boundsIgnoreListProp;
 
        //Edge Calculator
        private SerializedProperty useEdgeCalcProp;
        private SerializedProperty edgeCalcOmniDirectionalProp;
        private SerializedProperty bypassStandardCalcProp;
        private SerializedProperty edgeCalcStepSizeProp;
        private SerializedProperty bottomOffsetProp;
     
        //Render output
        private SerializedProperty backgroundColorProp;
        private SerializedProperty renderingLayerProp;
        private SerializedProperty setToLayerProp;
        private SerializedProperty baseTextureSizeProp;
        private SerializedProperty nearestMultipleProp;
        private SerializedProperty overwriteFramesProp;
        private SerializedProperty baselineScaleProp;
        private SerializedProperty renderingFormatProp;
        private SerializedProperty renderTextureFormatProp;
        private SerializedProperty renderingTextureDepthProp;
        private SerializedProperty renderingTextureReadWriteProp;
        private SerializedProperty renderingIgnoreListProp;
     
     
        //Isometric
        private SerializedProperty isometricDefaultAngleProp;
        private SerializedProperty isometricBaselineProp;
        private SerializedProperty preferBasePlateDeviationProp;
        private SerializedProperty isometricStepSizeProp;
        private SerializedProperty isometricMaxStepsProp;
     
        //Root Motion
        private SerializedProperty recordingsPerFrameProp;
        private SerializedProperty applyBaselineDeviationProp;
        private SerializedProperty rootMotionToleranceProp;
 
        protected virtual void OnEnable()
        {
            //Admin
            settingsModeProp = serializedObject.FindProperty("settingsMode");
         
            //General
            fpsProp = serializedObject.FindProperty("animationFramesPerSecond");
            centerModelProp = serializedObject.FindProperty("centerModelOnRenderStartup");

            //Calculator
            boundsCalcEnabledProp = serializedObject.FindProperty("enableBoundsCalculator");
            automaticDepthCalculationProp = serializedObject.FindProperty("automaticDepthCalculation");
         
            //Bounds Calculator
            boundsCalcOverdrawProp = serializedObject.FindProperty("boundsCalculatorSizeModifier");
            boundsCalcStepPercentageProp = serializedObject.FindProperty("scaleCalculatorStepPercentage");
            boundsCalcMaxPaddingProp = serializedObject.FindProperty("scaleCalculatorMaxPadding");
            boundsCalcMaxStepsProp = serializedObject.FindProperty("scaleCalculatorMaxSteps");
            boundsIncludeRenderIgnoreProp = serializedObject.FindProperty("includeRenderIgnore");
            boundsIgnoreListProp = serializedObject.FindProperty("scaleCalculatorIgnoreMeshes");
         
            //Edge Calculator
            useEdgeCalcProp = serializedObject.FindProperty("useEdgeCalculator");
            edgeCalcOmniDirectionalProp = serializedObject.FindProperty("omnidirectionalEdgeCalculator");
            bypassStandardCalcProp = serializedObject.FindProperty("bypassStandardCalculator");
            edgeCalcStepSizeProp = serializedObject.FindProperty("edgeCalculatorStepSize");
            bottomOffsetProp = serializedObject.FindProperty("bottomOffset");

            //Render Output
            backgroundColorProp = serializedObject.FindProperty("renderBackgroundColor");
            renderingLayerProp = serializedObject.FindProperty("renderingLayer");
            setToLayerProp = serializedObject.FindProperty("setToLayerWhenNotRendering");
            baseTextureSizeProp = serializedObject.FindProperty("baseTextureSize");
            nearestMultipleProp = serializedObject.FindProperty("setSizeToNearestMultiple");
            overwriteFramesProp = serializedObject.FindProperty("overwriteExistingFrames");
            baselineScaleProp = serializedObject.FindProperty("baselineScale");
            renderingFormatProp = serializedObject.FindProperty("renderingFormat");
            renderTextureFormatProp = serializedObject.FindProperty("renderTextureFormat");         
            renderingTextureDepthProp = serializedObject.FindProperty("renderTextureDepth");
            renderingTextureReadWriteProp = serializedObject.FindProperty("renderTextureReadWrite");
            renderingIgnoreListProp = serializedObject.FindProperty("renderIgnoreList");

            //Isometric
            isometricDefaultAngleProp = serializedObject.FindProperty("isometricDefaultAngle");
            isometricBaselineProp = serializedObject.FindProperty("isometricBaseline");
            preferBasePlateDeviationProp = serializedObject.FindProperty("preferBasePlateDeviation");
            isometricStepSizeProp = serializedObject.FindProperty("isometricStepSize");
            isometricMaxStepsProp = serializedObject.FindProperty("isometricMaxSteps");
         
            //Root Motion
            recordingsPerFrameProp = serializedObject.FindProperty("recordingsPerFrame");
            applyBaselineDeviationProp = serializedObject.FindProperty("applyBaselineDeviation");
            rootMotionToleranceProp = serializedObject.FindProperty("rootMotionTolerance");

         
            showAdvancedFields = new AnimBool(ShowAdvanced());
            showAdvancedFields.valueChanged.AddListener(Repaint);
            calculatorFoldoutCurrent = new AnimBool(calculatorFoldoutTarget);
            calculatorFoldoutCurrent.valueChanged.AddListener(Repaint);
        }

        protected AnimBool showAdvancedFields;
        [SerializeField] private bool generalFoldoutState;
        [SerializeField] private bool calculatorFoldoutState;
        [SerializeField] private bool renderOutputFoldoutState;
        [SerializeField] private bool namingFoldoutState;
        [SerializeField] private bool isometricFoldoutState;
        [SerializeField] private bool rootMotionFoldoutState;

        [SerializeField] private bool calculatorFoldoutTarget;
        [SerializeField] private AnimBool calculatorFoldoutCurrent;


        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            //Draw script target
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RenderingSettings) target), typeof(RenderingSettings), false);
            GUI.enabled = true;
            var _target = (RenderingSettings) target;
            
            //Draw standard / advanced settings mode, and set the fade target to the correct toggle
            EditorGUILayout.PropertyField(settingsModeProp);
            showAdvancedFields.target = ShowAdvanced();
            EditorColors.OverrideTextColors();

            //General
            DrawGeneralSettings();

            //Calculator
            DrawCalculatorSettings();

            //Render Output
            DrawRenderingSettings(_target);

            //Isometric Settings
            DrawIsometricSettings();

            //Root Motion
            DrawRootMotionSettings();

            EditorColors.ResetTextColor();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings()
        {
            generalFoldoutState = InspectorUtility.BeginFoldoutGroup("General Settings", generalFoldoutState);
            if (generalFoldoutState)
            {
                EditorGUILayout.PropertyField(fpsProp, new GUIContent(fpsProp.displayName, InspectorTooltips.FPS));
                EditorGUILayout.PropertyField(centerModelProp, new GUIContent(centerModelProp.displayName, InspectorTooltips.MoveModelOnStartup));
            }

            InspectorUtility.EndFoldoutGroup(generalFoldoutState);
        }

        private void DrawCalculatorSettings()
        {
            bool _showCalculator = InspectorUtility.BeginFoldoutGroup("Frame Scale Calculator Settings", ref calculatorFoldoutTarget, ref calculatorFoldoutCurrent);
            if (_showCalculator)
            {
                if (IsGlobalSettings())
                    GUI.enabled = false;
                InspectorUtility.DrawToggleProperty(boundsCalcEnabledProp, new GUIContent("Enable Per Frame Scale Calculation", InspectorTooltips.EnableBoundsCalculator));
                GUI.enabled = true;

                if (ShowAdvanced())
                    InspectorUtility.DrawToggleProperty(automaticDepthCalculationProp, new GUIContent(automaticDepthCalculationProp.displayName, InspectorTooltips.AutomaticDepthCalculation));

                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    //Bounds Calculator
                    InspectorUtility.BeginSubBoxGroup("Standard Bounds Calculator Advanced Settings", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                    InspectorUtility.DrawProperty(boundsCalcOverdrawProp, new GUIContent("Bounds Size Modifier", InspectorTooltips.BoundsOverdraw));
                    InspectorUtility.DrawProperty(boundsCalcStepPercentageProp, new GUIContent(boundsCalcStepPercentageProp.displayName, InspectorTooltips.CalculatorStepPercentage));
                    InspectorUtility.DrawProperty(boundsCalcMaxPaddingProp, new GUIContent("Bounds Calculator Max Pixel Padding", InspectorTooltips.CalculatorMaxPadding));
                    InspectorUtility.DrawProperty(boundsCalcMaxStepsProp, new GUIContent("Bounds Calculator Max Steps", InspectorTooltips.CalculatorMaxSteps));
                    if(!IsGlobalSettings())
                    {
                        InspectorUtility.DrawToggleProperty(boundsIncludeRenderIgnoreProp, new GUIContent(boundsIncludeRenderIgnoreProp.displayName, InspectorTooltips.IncludeRenderIgnore));
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(boundsIgnoreListProp, new GUIContent("Mesh Ignore List", InspectorTooltips.BoundsIgnoreList), true);
                        EditorGUI.indentLevel--;
                    }

                    InspectorUtility.EndSubBoxGroup();
                }

                EditorGUILayout.EndFadeGroup();

                //Edge Calculator
                InspectorUtility.BeginSubBoxGroup("Close Edge Calculator Settings", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);

                if (IsGlobalSettings() || !boundsCalcEnabledProp.boolValue)
                    GUI.enabled = false;
                InspectorUtility.DrawToggleProperty(useEdgeCalcProp, new GUIContent("Use Close Edge Calculator", InspectorTooltips.UseEdgeCalculatorTooltip));
                GUI.enabled = useEdgeCalcProp.boolValue && boundsCalcEnabledProp.boolValue;
                InspectorUtility.DrawToggleProperty(bypassStandardCalcProp, new GUIContent(bypassStandardCalcProp.displayName, InspectorTooltips.BypassStandardCalculator));
                InspectorUtility.DrawToggleProperty(edgeCalcOmniDirectionalProp, new GUIContent(edgeCalcOmniDirectionalProp.displayName, InspectorTooltips.UseInverseEdgeCalculator));
                GUI.enabled = true;
                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    InspectorUtility.DrawProperty(edgeCalcStepSizeProp, new GUIContent(edgeCalcStepSizeProp.displayName, InspectorTooltips.EdgeCalculatorStepSize));
                    InspectorUtility.DrawProperty(bottomOffsetProp, new GUIContent(bottomOffsetProp.displayName, InspectorTooltips.EdgeCalculatorBottomOffset));
                }

                EditorGUILayout.EndFadeGroup();
                InspectorUtility.EndSubBoxGroup();
            }

            InspectorUtility.EndNewFoldoutGroup(_showCalculator);
            
        }

        private void DrawRenderingSettings(RenderingSettings _target)
        {
            renderOutputFoldoutState = InspectorUtility.BeginFoldoutGroup("Render Output Settings", renderOutputFoldoutState);

            if (renderOutputFoldoutState)
            {
                renderingLayerProp.intValue = InspectorUtility.DrawListPopup("Rendering Layer", _target.RenderingLayerOptions, renderingLayerProp.intValue, InspectorTooltips.RenderingLayer);
                EditorGUILayout.PropertyField(backgroundColorProp, new GUIContent(backgroundColorProp.displayName, InspectorTooltips.RenderBackgroundColor));


                EditorGUILayout.PropertyField(baseTextureSizeProp, new GUIContent(baseTextureSizeProp.displayName, InspectorTooltips.BaseTextureSize));
                
                
                EditorGUILayout.PropertyField(overwriteFramesProp, new GUIContent(overwriteFramesProp.displayName, InspectorTooltips.OverwriteExisting));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(baselineScaleProp, new GUIContent("Baseline Reference Scale", InspectorTooltips.BaselineTooltip));

                var _currentColor = GUI.backgroundColor;
                GUI.backgroundColor = EditorColors.ButtonAction;
                if (GUILayout.Button("?", GUILayout.Width(35)))
                {
                    GUI.backgroundColor = _currentColor;
                    ShowBaselineInfo();
                }

                GUI.backgroundColor = _currentColor;
                EditorGUILayout.EndHorizontal();


                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    InspectorUtility.BeginSubBoxGroup("Advanced Settings", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                    EditorGUILayout.PropertyField(setToLayerProp, new GUIContent("Visible during other Renders", InspectorTooltips.SetToLayer));
                    EditorGUILayout.PropertyField(nearestMultipleProp, new GUIContent("Set Output to nearest multiple of 4", InspectorTooltips.NearestMultiple));
                    EditorGUILayout.PropertyField(renderingFormatProp, new GUIContent(renderingFormatProp.displayName, InspectorTooltips.RenderingFormat));
                    EditorGUILayout.PropertyField(renderTextureFormatProp, new GUIContent(renderTextureFormatProp.displayName, InspectorTooltips.RenderTextureFormat));
                    renderingTextureDepthProp.intValue = InspectorUtility.DrawListPopup("Render Texture Depth", _target.RenderTextureDepthOptions, renderingTextureDepthProp.intValue, InspectorTooltips.RenderTextureDepth);
                    EditorGUILayout.PropertyField(renderingTextureReadWriteProp, new GUIContent(renderingTextureReadWriteProp.displayName, InspectorTooltips.RenderTextureReadWrite));
                    EditorGUI.indentLevel++;
                    if(!IsGlobalSettings()) EditorGUILayout.PropertyField(renderingIgnoreListProp, new GUIContent("Mesh Ignore List", InspectorTooltips.RenderingIgnoreList), true);
                    EditorGUI.indentLevel--;
                    InspectorUtility.EndSubBoxGroup();
                }

                EditorGUILayout.EndFadeGroup();
            }

            InspectorUtility.EndFoldoutGroup(renderOutputFoldoutState);
        }
        

        private void DrawIsometricSettings()
        {
            isometricFoldoutState = InspectorUtility.BeginFoldoutGroup("Isometric Setup Settings", isometricFoldoutState);

            if (isometricFoldoutState)
            {
                EditorGUILayout.PropertyField(isometricDefaultAngleProp, new GUIContent(isometricDefaultAngleProp.displayName, InspectorTooltips.IsometricDefaultAngle));
                EditorGUILayout.PropertyField(isometricBaselineProp, new GUIContent(isometricBaselineProp.displayName, InspectorTooltips.IsometricDefaultBaseline));
                EditorGUILayout.PropertyField(preferBasePlateDeviationProp, new GUIContent(preferBasePlateDeviationProp.displayName, InspectorTooltips.IsometricPreferBasePlate));
                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    EditorGUILayout.PropertyField(isometricStepSizeProp, new GUIContent(isometricStepSizeProp.displayName, InspectorTooltips.IsometricStepSize));
                    EditorGUILayout.PropertyField(isometricMaxStepsProp, new GUIContent(isometricMaxStepsProp.displayName, InspectorTooltips.IsometricMaxSteps));
                }

                EditorGUILayout.EndFadeGroup();
            }

            InspectorUtility.EndFoldoutGroup(isometricFoldoutState);
        }

        private void DrawRootMotionSettings()
        {
            rootMotionFoldoutState = InspectorUtility.BeginFoldoutGroup("Root Motion Export Settings", rootMotionFoldoutState);

            if (rootMotionFoldoutState)
            {
                EditorGUILayout.PropertyField(recordingsPerFrameProp, new GUIContent(recordingsPerFrameProp.displayName, InspectorTooltips.RootMotionRecordingsPerFrame));
                //    applyBaselineDeviationProp.boolValue = EditorGUILayout.ToggleLeft(new GUIContent(applyBaselineDeviationProp.displayName, InspectorTooltips.ApplyBaselineDeviation), applyBaselineDeviationProp.boolValue);
                EditorGUILayout.PropertyField(applyBaselineDeviationProp, new GUIContent(applyBaselineDeviationProp.displayName, InspectorTooltips.ApplyBaselineDeviation));
                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    EditorGUILayout.PropertyField(rootMotionToleranceProp, new GUIContent(rootMotionToleranceProp.displayName, InspectorTooltips.RootMotionTolerance));
                }

                EditorGUILayout.EndFadeGroup();
            }

            InspectorUtility.EndFoldoutGroup(rootMotionFoldoutState);
        }

        protected virtual bool IsGlobalSettings()
        {
            return false;
        }
     
        private void ShowBaselineInfo()
        {
            EditorPropertyInfoWindow _infoWindow = new EditorPropertyInfoWindow();
            Vector2 _popupSize = new Vector2(400, 205);

            _infoWindow.Init(_popupSize, InspectorTooltips.BaselinePopup);
            
            PopupWindow.Show(new Rect(Event.current.mousePosition - new Vector2(_popupSize.x,_popupSize.y), _popupSize), _infoWindow);
        }

        private bool ShowAdvanced() => settingsModeProp.enumValueIndex == (int)RenderingSettings.ShowMode.Advanced;
    }

    [CustomEditor(typeof(GlobalRenderingSettings))]
    public class GlobalRenderingSettingsEditor : RenderingSettingsEditor
    {
        [SerializeField] private bool clampingFoldoutState;
        private SerializedProperty clampInsertCutoffProp;
        private SerializedProperty clampDecimalTolerance;

        protected override void OnEnable()
        {
            base.OnEnable();
            clampInsertCutoffProp = serializedObject.FindProperty("defaultInsertClampCutoff");
            clampDecimalTolerance = serializedObject.FindProperty("clampingDecimalTolerance");
        
        }

        protected override bool IsGlobalSettings()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            clampingFoldoutState = InspectorUtility.BeginFoldoutGroup("Animation Clamping Setup Settings", clampingFoldoutState);
            if (clampingFoldoutState)
            {
                EditorGUILayout.PropertyField(clampInsertCutoffProp);
                EditorGUILayout.PropertyField(clampDecimalTolerance);
            }
            InspectorUtility.EndFoldoutGroup(clampingFoldoutState);
        }

        [MenuItem("CONTEXT/RenderingSettings/Reset To Global Values")]
        static void ResetToGlobal(MenuCommand aCommand)
        {
            
            RenderingSettings _settings = (RenderingSettings) aCommand.context;
            _settings.ResetToGlobal();
        }

        [MenuItem("CONTEXT/RenderingSettings/Reset To Global Values", true)]
        static bool CanResetToGlobal()
        {
            var _selectedObject = Selection.activeGameObject;
            if (_selectedObject == null) return false;
            var _global = _selectedObject.GetComponent<GlobalRenderingSettings>();
            return _global == null;
        }
    }
}