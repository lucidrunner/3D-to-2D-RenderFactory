using Render3DTo2D.Model_Settings;
using Render3DTo2D.Utility.Inspector;
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
        private SerializedProperty focusOnStartProp;
        private SerializedProperty focusOnRenderProp;
        private SerializedProperty centerModelOnStartProp;
 
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
     
        //Naming
        private SerializedProperty useAnimationNameProp;
        private SerializedProperty includeRigTagProp;
        private SerializedProperty useFormatIdentifierProp;
        private SerializedProperty includeStaticTagProp;
        private SerializedProperty renderNameFormatProp;
     
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
            focusOnRenderProp = serializedObject.FindProperty("followCameraOnRender");
            focusOnStartProp = serializedObject.FindProperty("centerCameraOnRenderStartup");
            centerModelOnStartProp = serializedObject.FindProperty("centerModelOnRenderStartup");

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

            //Naming
            useAnimationNameProp = serializedObject.FindProperty("useAnimationName");
            includeRigTagProp = serializedObject.FindProperty("includeRigTag");
            useFormatIdentifierProp = serializedObject.FindProperty("includeFormatIdentifier");
            includeStaticTagProp = serializedObject.FindProperty("includeStaticTag");
            renderNameFormatProp = serializedObject.FindProperty("renderNameFormat");
         
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
        }

        protected AnimBool showAdvancedFields;
        [SerializeField] private bool generalFoldoutState;
        [SerializeField] private bool calculatorFoldoutState;
        [SerializeField] private bool renderOutputFoldoutState;
        [SerializeField] private bool namingFoldoutState;
        [SerializeField] private bool isometricFoldoutState;
        [SerializeField] private bool rootMotionFoldoutState;


        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((RenderingSettings) target), typeof(RenderingSettings), false);
            GUI.enabled = true;
            var _target = (RenderingSettings) target;
            EditorGUILayout.PropertyField(settingsModeProp);
            showAdvancedFields.target = ShowAdvanced();
            EditorColors.OverrideTextColors();

            //General
            DrawGeneralSettings();

            //Calculator
            DrawCalculatorSettings();


            //Render Output
            DrawRenderingSettings(_target);

            //Naming Settings
            DrawNamingSettings(_target);


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
                EditorGUILayout.PropertyField(centerModelOnStartProp, new GUIContent(centerModelOnStartProp.displayName, InspectorTooltips.MoveModelOnStartup));
                EditorGUILayout.PropertyField(focusOnStartProp, new GUIContent(focusOnStartProp.displayName, InspectorTooltips.FocusModelOnStartup));
                EditorGUILayout.PropertyField(focusOnRenderProp, new GUIContent(focusOnRenderProp.displayName, InspectorTooltips.FollowModelOnRender));
            }

            InspectorUtility.EndFoldoutGroup(generalFoldoutState);
        }

        private void DrawCalculatorSettings()
        {
            calculatorFoldoutState = InspectorUtility.BeginFoldoutGroup("Frame Scale Calculator Settings", calculatorFoldoutState);
            if (calculatorFoldoutState)
            {
                if (IsGlobalSettings())
                    GUI.enabled = false;
                EditorGUILayout.PropertyField(boundsCalcEnabledProp, new GUIContent("Enable Per Frame Scale Calculation", InspectorTooltips.EnableBoundsCalculator));
                GUI.enabled = true;

                if (ShowAdvanced())
                    EditorGUILayout.PropertyField(automaticDepthCalculationProp, new GUIContent(automaticDepthCalculationProp.displayName, InspectorTooltips.AutomaticDepthCalculation));

                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    //Bounds Calculator
                    InspectorUtility.BeginSubBoxGroup("Standard Bounds Calculator Advanced Settings", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                    EditorGUILayout.PropertyField(boundsCalcOverdrawProp, new GUIContent("Bounds Size Modifier", InspectorTooltips.BoundsOverdraw));
                    EditorGUILayout.PropertyField(boundsCalcStepPercentageProp, new GUIContent(boundsCalcStepPercentageProp.displayName, InspectorTooltips.CalculatorStepPercentage));
                    EditorGUILayout.PropertyField(boundsCalcMaxPaddingProp, new GUIContent("Bounds Calculator Max Pixel Padding", InspectorTooltips.CalculatorMaxPadding));
                    EditorGUILayout.PropertyField(boundsCalcMaxStepsProp, new GUIContent("Bounds Calculator Max Steps", InspectorTooltips.CalculatorMaxSteps));
                    if(!IsGlobalSettings())
                    {
                        EditorGUILayout.PropertyField(boundsIncludeRenderIgnoreProp, new GUIContent(boundsIncludeRenderIgnoreProp.displayName, InspectorTooltips.IncludeRenderIgnore));
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
                EditorGUILayout.PropertyField(useEdgeCalcProp, new GUIContent("Use Close Edge Calculator", InspectorTooltips.UseEdgeCalculatorTooltip));
                GUI.enabled = useEdgeCalcProp.boolValue && boundsCalcEnabledProp.boolValue;
                EditorGUILayout.PropertyField(bypassStandardCalcProp, new GUIContent(bypassStandardCalcProp.displayName, InspectorTooltips.BypassStandardCalculator));
                EditorGUILayout.PropertyField(edgeCalcOmniDirectionalProp, new GUIContent(edgeCalcOmniDirectionalProp.displayName, InspectorTooltips.UseInverseEdgeCalculator));
                GUI.enabled = true;
                if (EditorGUILayout.BeginFadeGroup(showAdvancedFields.faded))
                {
                    EditorGUILayout.PropertyField(edgeCalcStepSizeProp, new GUIContent(edgeCalcStepSizeProp.displayName, InspectorTooltips.EdgeCalculatorStepSize));
                    EditorGUILayout.PropertyField(bottomOffsetProp, new GUIContent(bottomOffsetProp.displayName, InspectorTooltips.EdgeCalculatorBottomOffset));
                }

                EditorGUILayout.EndFadeGroup();
                InspectorUtility.EndSubBoxGroup();
            }

            InspectorUtility.EndFoldoutGroup(calculatorFoldoutState);
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

        private void DrawNamingSettings(RenderingSettings aTarget)
        {
            namingFoldoutState = InspectorUtility.BeginFoldoutGroup("Output Naming", namingFoldoutState);
            if (namingFoldoutState)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current Name Format Example", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(aTarget.ExampleOutput, EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(useAnimationNameProp, new GUIContent("Use Animation Name over Index", InspectorTooltips.UseAnimationName));
                EditorGUILayout.PropertyField(includeRigTagProp, new GUIContent(includeRigTagProp.displayName, InspectorTooltips.IncludeRigTag));
                EditorGUILayout.PropertyField(useFormatIdentifierProp, new GUIContent(useFormatIdentifierProp.displayName, InspectorTooltips.AddIdentifiers));
                EditorGUILayout.PropertyField(includeStaticTagProp, new GUIContent("Include Static Tag (If Applicable)", InspectorTooltips.IncludeStaticTag));

                InspectorUtility.BeginSubBoxGroup("Format Order", EditorColors.HeaderAlt1, EditorColors.BodyAlt1);
                var _color = GUI.backgroundColor;
                for (int _index = 0; _index < renderNameFormatProp.arraySize; _index++)
                {
                    var _prop = renderNameFormatProp.GetArrayElementAtIndex(_index);
                    GUI.backgroundColor = EditorColors.BodyAlt2;
                    EditorGUILayout.BeginHorizontal(InspectorUtility.FactoryStyles.ClosedSubBoxGroup);
                    GUI.backgroundColor = _color;
                    EditorGUILayout.LabelField(_prop.stringValue, EditorStyles.boldLabel);
                    if (InspectorUtility.DrawButton(new GUIContent("▲"), EditorColors.DefaultButton))
                    {
                        aTarget.MoveFormatLeft(_index);
                    }

                    if (InspectorUtility.DrawButton(new GUIContent("▼"), EditorColors.DefaultButton))
                    {
                        aTarget.MoveFormatRight(_index);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (InspectorUtility.DrawButton(new GUIContent("Reset Default Order"), EditorColors.DefaultButton))
                    aTarget.ResetFormat();
                InspectorUtility.EndBoxGroup();
            }

            InspectorUtility.EndFoldoutGroup(namingFoldoutState);
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