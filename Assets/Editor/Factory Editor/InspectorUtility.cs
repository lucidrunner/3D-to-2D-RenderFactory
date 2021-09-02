﻿using System.Collections.Generic;
using System.Linq;
using Shared_Scripts;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Factory_Editor
{
    public static class InspectorUtility
    {

        #region Foldout

        public static bool BeginFoldoutGroup(string aFoldoutLabel, ref bool aFoldoutTarget, ref AnimBool aCurrentFoldoutState, Color? aHeaderHighlight = null, Color? aGroupHighlight = null)
        {
            return DrawNewFoldoutGroup(aFoldoutLabel, ref aFoldoutTarget, ref aCurrentFoldoutState, aHeaderHighlight ?? EditorColors.FoldoutHeader, aGroupHighlight ?? EditorColors.FoldoutBody, FactoryStyles.NewFoldoutHeader,
                FactoryStyles.NewFoldoutBody);
        }


        public static bool BeginSubFoldoutGroup(string aFoldoutLabel, ref bool aFoldoutTarget, ref AnimBool aCurrentFoldoutState, Color? aHeaderHighlight = null, Color? aGroupHighlight = null)
        {
            return DrawNewFoldoutGroup(aFoldoutLabel, ref aFoldoutTarget, ref aCurrentFoldoutState, aHeaderHighlight ?? EditorColors.FoldoutHeader, aGroupHighlight ?? EditorColors.FoldoutBody, FactoryStyles.NewSubFoldoutHeader,
                FactoryStyles.NewSubFoldoutBody);
        }

        private static bool DrawFoldoutGroup(string aFoldoutLabel, Color? aHeaderHighlight, Color? aGroupHighlight, bool aFoldoutState, GUIStyle aHeaderStyle, GUIStyle aBodyStyle)
        {
            var _prevColor = GUI.backgroundColor;
            if(aHeaderHighlight.HasValue)
            {
                GUI.backgroundColor = aHeaderHighlight.Value;
            }

            
            
            aFoldoutState = EditorGUILayout.BeginFoldoutHeaderGroup(aFoldoutState, aFoldoutLabel, aHeaderStyle);

            GUI.backgroundColor = _prevColor;

            if (aGroupHighlight.HasValue && aFoldoutState)
            {
                GUI.backgroundColor = aGroupHighlight.Value;
            }

            if (aFoldoutState)
            {
                var _foldoutBodyStyle = new GUIStyle(aBodyStyle)
                {
                    normal = {background = FactoryEditorTextures.FoldoutBodyBackground}
                };
                

                EditorGUILayout.BeginVertical(_foldoutBodyStyle);
            }

            GUI.backgroundColor = _prevColor;
                
            return aFoldoutState;
        }

        private static bool DrawNewFoldoutGroup(string aFoldoutLabel, ref bool aFoldoutTarget, ref AnimBool aCurrentFoldoutState, Color aHeaderHighlight, Color aGroupHighlight, GUIStyle aFoldoutHeader, GUIStyle aFoldoutBody)
        {
            
            var _prevColor = GUI.backgroundColor;
            GUI.backgroundColor = aHeaderHighlight;
            
            //HEADER
            //Begin by drawing the header box group
            EditorGUILayout.BeginVertical(aFoldoutHeader);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(aFoldoutLabel, "BoldLabel");
            GUILayout.FlexibleSpace();

            //Depending on if we're currently showing / hiding we want to display the opposite label on the button
            string _expandButtonLabel = aFoldoutTarget ? "Hide" : "Show";
            
            //Flip the current target if we press the button
            if (DrawButton(new GUIContent(_expandButtonLabel), EditorColors.ButtonAction, ButtonSize.Standard))
            {
                aFoldoutTarget = !aFoldoutTarget;
            }
            
            //And set that target to the anim bool so we can use our fade group 
            aCurrentFoldoutState.target = aFoldoutTarget;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = _prevColor;
            
            
            //BODY
            var _showFade = EditorGUILayout.BeginFadeGroup(aCurrentFoldoutState.faded);
            if (_showFade)
            {
                GUI.backgroundColor = aGroupHighlight;
                EditorGUILayout.BeginVertical(aFoldoutBody);
                GUI.backgroundColor = _prevColor;
            }

            //Return if we should show our variables atm
            return _showFade;
        }


        public static void EndNewFoldoutGroup(bool aFoldoutState)
        {
            //If we're currently drawing the group, end our vertical
            if (aFoldoutState)
            {
                EditorGUILayout.EndVertical();   
            }
            
            EditorGUILayout.EndFadeGroup();
        }

        public static void EndFoldoutGroup(bool aFoldoutState)
        {
            if(aFoldoutState)
            {
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        #endregion

        #region BoxGroup

        public static void BeginBoxGroup(string aBoxTitle, Color? aTitleBar, Color? aGroupHighlight)
        {
            var _prevColor = GUI.backgroundColor;
            
            if(aBoxTitle != null)
            {
                if(aTitleBar.HasValue)
                    GUI.backgroundColor = aTitleBar.Value;
                EditorGUILayout.BeginVertical(FactoryStyles.BoxGroupHeader);
                GUILayout.Label(aBoxTitle, "BoldLabel");
                EditorGUILayout.EndVertical();
            }
            
            
            GUI.backgroundColor = _prevColor;

            if (aGroupHighlight.HasValue)
                GUI.backgroundColor = aGroupHighlight.Value;

           
            EditorGUILayout.BeginVertical(aBoxTitle != null ? FactoryStyles.BoxGroup : FactoryStyles.ClosedBoxGroup);

            GUI.backgroundColor = _prevColor;
        }

        public static void EndBoxGroup()
        {
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region SubBoxGroup

        public static void BeginSubBoxGroup(string aListObjectTitle, Color? aTitleHighlight, Color? aContentHighlight)
        {
            var _prevColor = GUI.backgroundColor;

            if(aListObjectTitle != null)
            {
                if(aTitleHighlight.HasValue)
                    GUI.backgroundColor = aTitleHighlight.Value;
                EditorGUILayout.BeginVertical(FactoryStyles.BoxGroupHeader);
                GUILayout.Label(aListObjectTitle, new GUIStyle("BoldLabel"));
                EditorGUILayout.EndVertical();
            }
            GUI.backgroundColor = _prevColor;
            
            if(aContentHighlight.HasValue)
            {
                GUI.backgroundColor = aContentHighlight.Value;
            }

            EditorGUILayout.BeginVertical(aListObjectTitle != null ? FactoryStyles.SubBoxGroup : FactoryStyles.ClosedSubBoxGroup);
            
            GUI.backgroundColor = _prevColor;
        }

        public static void EndSubBoxGroup()
        {
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Other

        public static void GuiLine( int aHeight = 1, Color? aLineColor = null, int aSpaceBefore = 3, int aSpaceAfter = 3, float aWidthModifier = 1f )
        {

            Color _prevColor = GUI.backgroundColor;
            
            EditorGUILayout.Space(aSpaceBefore);
            
            Rect rect = EditorGUILayout.GetControlRect(false, aHeight );

            rect.width = rect.width * aWidthModifier;

            rect.height = aHeight;

            if (aLineColor.HasValue)
            {
                GUI.backgroundColor = aLineColor.Value;
            }
            
            EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
            EditorGUILayout.Space(aSpaceAfter);

            GUI.backgroundColor = _prevColor;
        }

        #endregion

        //echologin on unity forums https://forum.unity.com/threads/horizontal-line-in-editor-window.520812/
        public static void ToggleStates(bool aNewState, SerializedProperty[] aSerializedBoolProperties)
        {
            foreach (var _serializedProperty in aSerializedBoolProperties)
            {
                _serializedProperty.boolValue = aNewState;
            }
        }

        public static int DrawListPopup(string aPopupLabel, string[] aPopupOptions, int aCurrentIndex, string aToolTip = null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(aPopupLabel, aToolTip));
            aCurrentIndex = EditorGUILayout.Popup(aCurrentIndex, aPopupOptions);
            EditorGUILayout.EndHorizontal();

            return aCurrentIndex;
        }

        public static void DrawToggleTransform(SerializedProperty aToggleTransformProp, bool aIncludeLabel = true, Color? aHeaderColor = null, Color? aBodyColor = null)
        {
            //Then get the individual rows of that follow transform 
            var _positionRow = aToggleTransformProp.FindPropertyRelative("positionRow");
            var _rotationRow = aToggleTransformProp.FindPropertyRelative("rotationRow");

            var _drawScale = aToggleTransformProp.FindPropertyRelative("enableScaleRow");

            var _name = aToggleTransformProp.FindPropertyRelative("groupName");
        
            BeginSubBoxGroup(aIncludeLabel ? _name.stringValue : null, aHeaderColor ?? EditorColors.Header, aBodyColor ?? EditorColors.Body);
        
            //Now we draw the buttons
            EditorGUILayout.BeginHorizontal();
            DrawToggleRow(_positionRow);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            DrawToggleRow(_rotationRow);
            EditorGUILayout.EndHorizontal();
            if (_drawScale.boolValue)
            {
                var _scaleRow = aToggleTransformProp.FindPropertyRelative("scaleRow");
                EditorGUILayout.BeginHorizontal();
                DrawToggleRow(_scaleRow);
                EditorGUILayout.EndHorizontal();
            }
            EndSubBoxGroup();
        }

        private static void DrawToggleRow(SerializedProperty aToggleRow)
        {
            Color _prevColor = GUI.backgroundColor;
            EditorColors.OverrideButtonColors();
            SerializedProperty[] _positionLabels = {aToggleRow.FindPropertyRelative("label1"), aToggleRow.FindPropertyRelative("label2"), aToggleRow.FindPropertyRelative("label3")};
            SerializedProperty[] _positionStates = {aToggleRow.FindPropertyRelative("conditionalOne"), aToggleRow.FindPropertyRelative("conditionalTwo"), aToggleRow.FindPropertyRelative("conditionalThree")};
            bool _currentOverall = _positionStates.All(aProperty => aProperty.boolValue);


            GUI.backgroundColor = _currentOverall ? EditorColors.ToggleOn : EditorColors.ToggleOff;
            if (GUILayout.Button(aToggleRow.displayName.Replace(" Row", ""), GUILayout.MaxWidth(Screen.width / 3f)))
            {
                _currentOverall = !_currentOverall;
                ToggleStates(_currentOverall, _positionStates);
            }

            for (var _index = 0; _index < _positionStates.Length; _index++)
            {
                var _label = _positionLabels[_index];
                var _state = _positionStates[_index];

                GUI.backgroundColor = _state.boolValue ? EditorColors.ToggleOn : EditorColors.ToggleOff;
                if (GUILayout.Button(_label.stringValue))
                {
                    _state.boolValue = !_state.boolValue;
                }
            }
            
            EditorColors.ResetButtonColors();
            GUI.backgroundColor = _prevColor;
        }

        public static bool DrawToggleButton(bool aBoolProperty, GUIContent aButtonContent, ButtonSize aButtonSize = ButtonSize.Standard)
        {
            var _color = aBoolProperty ? EditorColors.ToggleOn : EditorColors.ToggleOff;
            if (DrawButton(aButtonContent, _color, aButtonSize))
            {
                aBoolProperty = !aBoolProperty;
            }
            return aBoolProperty;
        }


        //Allows us to pass the params without having to explicitly set the size
        public static bool DrawButton(GUIContent aGUIContent, Color aColor, params GUILayoutOption[] aOptions)
        {
            EditorColors.OverrideButtonColors();
            bool _result = DrawButton(aGUIContent, aColor, ButtonSize.Standard, aOptions);
            EditorColors.ResetButtonColors();
            return _result;
        }

        public static bool DrawButton(GUIContent aGUIContent, Color aColor, ButtonSize aButtonSize = ButtonSize.Standard, params GUILayoutOption[] aOptions)
        {
            var _prevColor = GUI.backgroundColor;
            EditorColors.OverrideButtonColors();
            float _height = 20f;
            switch (aButtonSize)
            {
                case ButtonSize.Large:
                    _height *= 1.5f;
                    break;
                case ButtonSize.Huge:
                    _height *= 2.2f;
                    break;
                case ButtonSize.Massive:
                    _height *= 3.5f;
                    break;
            }

            var _options = new List<GUILayoutOption>() {GUILayout.Height(_height)};
            _options.AddRange(aOptions);
            bool _result = false;
            GUI.backgroundColor = aColor;
            if (GUILayout.Button(aGUIContent, _options.ToArray()))
            {
                GUI.backgroundColor = _prevColor;
                _result = true;
            }
            GUI.backgroundColor = _prevColor;
            EditorColors.ResetButtonColors();
            return _result;
        }

        public enum ButtonSize
        {
            Standard,
            Large,
            Huge,
            Massive
        }

        public static void RepaintAll()
        {
            var _editors = Resources.FindObjectsOfTypeAll<Editor>();
            foreach (var _editor in _editors)
            {
                _editor.Repaint();
            }
        }
        
        public static void DrawProperty(SerializedProperty aProperty, float aLabelScreenWidth = 0.45f, params GUILayoutOption[] aOptions)
        {
            var _prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Screen.width * aLabelScreenWidth;
            EditorGUILayout.PropertyField(aProperty, aOptions);
            EditorGUIUtility.labelWidth = _prevLabelWidth;
        }
        
        public static void DrawProperty(SerializedProperty aProperty, GUIContent aPropertyLabel, float aLabelScreenWidth = 0.45f, params GUILayoutOption[] aOptions)
        {
            var _prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Screen.width * aLabelScreenWidth;
            EditorGUILayout.PropertyField(aProperty, aPropertyLabel, aOptions);
            EditorGUIUtility.labelWidth = _prevLabelWidth;
        }


        public static void DrawToggleProperty(SerializedProperty aProperty, bool aToggleLeft = false)
        {
            GUILayout.BeginHorizontal();
            if (aToggleLeft)
                EditorGUILayout.PropertyField(aProperty, GUIContent.none, true, GUILayout.Width(20));
            GUILayout.Label(aProperty.displayName);

            if(!aToggleLeft)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(aProperty, GUIContent.none, true, GUILayout.Width(20));
            }

            GUILayout.EndHorizontal();
        }
        
        public static void DrawToggleProperty(SerializedProperty aProperty, GUIContent aLabelContent, bool aToggleLeft = false)
        {
            GUILayout.BeginHorizontal();
            if (aToggleLeft)
                EditorGUILayout.PropertyField(aProperty, GUIContent.none, true, GUILayout.Width(20));
            GUILayout.Label(aLabelContent);

            if(!aToggleLeft)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(aProperty, GUIContent.none, true, GUILayout.Width(20));
            }

            GUILayout.EndHorizontal();
        }
    }

}
