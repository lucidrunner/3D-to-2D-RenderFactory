using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shared_Scripts
{
    /* Some colors
     * Palette 1 
     * #C4F0FF - pastel cyan
     * #62EBD4 - cyan
     * #99FFC2 - pastel green / cyan
     * #84E880 - green 
     * #CEFF8C - green / yellow
     */

    
    public static class EditorColors
    {
        private static readonly Color[] LabelDefault;
        private static readonly Color[] BoldLabelDefault;
        private static readonly Color[] FoldoutHeaderDefault;
        private static readonly Color[] SubFoldoutHeaderDefault;
        private static readonly Color[] ListHeaderDefault;
        private static readonly Color[] ButtonDefaultColor;
        private static readonly Color[] HelpBoxDefault;
        static EditorColors()
        {
            //Set the current default colors for all the different styles we use
            LabelDefault = SaveTextColorsForStyle(EditorStyles.label);
            FoldoutHeaderDefault = SaveTextColorsForStyle(FactoryStyles.FoldoutHeader);
            SubFoldoutHeaderDefault = SaveTextColorsForStyle(FactoryStyles.SubFoldoutHeader);
            BoldLabelDefault = SaveTextColorsForStyle(EditorStyles.boldLabel);
            ListHeaderDefault = SaveTextColorsForStyle(EditorStyles.foldout);
            ButtonDefaultColor = SaveTextColorsForStyle(GUI.skin.button);
            HelpBoxDefault = SaveTextColorsForStyle(EditorStyles.helpBox);
            //
        }

        private static Color[] SaveTextColorsForStyle(GUIStyle aLabel)
        {
            return new[]
            {
                aLabel.normal.textColor,
                aLabel.onNormal.textColor,
                aLabel.active.textColor,
                aLabel.onActive.textColor,
                aLabel.focused.textColor,
                aLabel.onFocused.textColor,
                aLabel.hover.textColor,
                aLabel.onHover.textColor
            };
        }


        #region Inspector Palette

        public enum EditorPalette
        {
            BubbleGum,
            LightPastel,
            Dark
        }

        private static EditorPalette CurrentPalette => FactorySettings.GetOrCreateSettings().EditorPalette;

        /// <summary>
        /// In Order:
        /// Header
        /// HeaderAlt1
        /// HeaderAlt2
        /// Body
        /// BodyAlt1
        /// BodyAlt2
        /// Active Tab
        /// Inactive Tab
        /// FoldoutHeader
        /// FoldoutBody
        /// Default Color
        /// </summary>
        private static readonly Color[,] InspectorPalettes = {
            //BubbleGum
            {
                ParseColor("#9FF5A4"),
                ParseColor("#99FFC2"),
                ParseColor("#E0D48C"),
                
                ParseColor("#D1F593"),
                ParseColor("#ABF5D9"),
                ParseColor("#EEE8C2"),
                
                ParseColor("#CEFF8C"),
                Color.grey,
                
                ParseColor("#62EBD4"),
                ParseColor("#C4F0FF"),
                
                Color.white
            },
            //Pastel
            {
            ParseColor("#CAFFBF"),
            ParseColor("#FDFFB6"),
            ParseColor("#FFADAD"),
            
            ParseColor("#DDFFD6"),
            ParseColor("#FEFFD6"),
            ParseColor("#FFD6D6"),
            
            ParseColor("#FFC6FF"),
            Color.grey,
            
            ParseColor("#9BF6FF"),
            ParseColor("#D6FCFF"),
            
            Color.white
            },
            //Dark
            {
                ParseColor("#212F45"),
                ParseColor("#3E1F47"),
                ParseColor("#272640"),
                
                ParseColor("#144552"),
                ParseColor("#312244"),
                ParseColor("#373659"),
                
                ParseColor("#5C1E5C"),
                Color.gray,
                
                ParseColor("#4D194D"),
                ParseColor("#4B2555"),
                
                Color.gray
            }
        };

        private const int HeaderColorID = 0;
        private const int HeaderAlt1ColorID = 1;
        private const int HeaderAlt2ColorID = 2;
        private const int BodyColorID = 3;
        private const int BodyAlt1ID = 4;
        private const int BodyAlt2ID = 5;
        private const int ActiveTabID = 6;
        private const int InActiveTabID = 7;
        private const int FoldoutHeaderID = 8;
        private const int FoldoutBodyID = 9;
        private const int DefaultColorID = 10;
        
        public static Color Header => GetInspectorColor(HeaderColorID);

        public static Color HeaderAlt1 => GetInspectorColor(HeaderAlt1ColorID);

        public static Color HeaderAlt2 => GetInspectorColor(HeaderAlt2ColorID);

        public static Color Body => GetInspectorColor(BodyColorID);

        public static Color BodyAlt1 => GetInspectorColor(BodyAlt1ID);
        
        public static Color BodyAlt2 => GetInspectorColor(BodyAlt2ID);
        
        public static Color ActiveTab => GetInspectorColor(ActiveTabID);

        public static Color InActiveTab => GetInspectorColor(InActiveTabID);

        public static Color FoldoutHeader => GetInspectorColor(FoldoutHeaderID);

        public static Color FoldoutBody => GetInspectorColor(FoldoutBodyID);

        public static Color DefaultColor => GetInspectorColor(DefaultColorID);
        
        #endregion

        #region ButtonPalette

        public enum ButtonPalette
        {
            Default,
            Pastel,
            Dark
        }

        private static ButtonPalette CurrentButtonPalette => FactorySettings.GetOrCreateSettings().ButtonPalette;

        /// <summary>
        /// In Order:
        /// Toggle On
        /// Toggle Off
        /// Run Button
        /// Run Alt Button
        /// Action Button
        /// Default Button
        /// </summary>
        private static readonly Color[,] ButtonPalettes =
        {
            //Default
            {
                Color.green,
                Color.red,
                new Color(0.467f, 0.867f, 0.667f),
                new Color(0.867f, 0.467f, 0.867f),
                new Color(1f, 0.70f, 0.278f),
                Color.white
            },
            //Pastel
            {
                ParseColor("#caffbf"),
                ParseColor("#ffadad"),
                ParseColor("#caffbf"),
                ParseColor("#ffc6ff"),
                ParseColor("#ffd6a5"),
                Color.white
            }
        };

        private const int ToggleOnColorID = 0;

        private const int ToggleOffColorID = 1;

        private const int RunButtonColorID = 2;

        private const int RunAltButtonColorID = 3;

        private const int ActionButtonColorID = 4;

        private const int DefaultButtonColorID = 5;

        public static Color ButtonRun => GetButtonColor(RunButtonColorID);
        public static Color ButtonRunAlt => GetButtonColor(RunAltButtonColorID);
        public static Color ToggleOff => GetButtonColor(ToggleOffColorID);
        public static Color ToggleOn => GetButtonColor(ToggleOnColorID);
        public static Color ButtonAction => GetButtonColor(ActionButtonColorID);

        public static Color DefaultButton => GetButtonColor(DefaultButtonColorID);
        
        #endregion

        public static void OverrideTextColors()
        {
            SetColorForStyle(EditorStyles.label, TextColor);
            SetColorForStyle(EditorStyles.boldLabel, TextColor);
            SetColorForStyle(FactoryStyles.FoldoutHeader, TextColor);
            SetColorForStyle(FactoryStyles.SubFoldoutHeader, TextColor);
            SetColorForStyle(EditorStyles.foldout, TextColor);
            SetColorForStyle(EditorStyles.helpBox, TextColor);
        }

        public static void ResetTextColor()
        {
            SetColorForStyle(EditorStyles.label, LabelDefault);
            SetColorForStyle(EditorStyles.boldLabel, BoldLabelDefault);
            SetColorForStyle(FactoryStyles.FoldoutHeader, FoldoutHeaderDefault);
            SetColorForStyle(FactoryStyles.SubFoldoutHeader, SubFoldoutHeaderDefault);
            SetColorForStyle(EditorStyles.foldout, ListHeaderDefault);
            SetColorForStyle(EditorStyles.helpBox, HelpBoxDefault);
        }
        
        public static void OverrideButtonColors()
        {
            SetColorForStyle(GUI.skin.button, ButtonTextColor);
        }

        public static void ResetButtonColors()
        {
            SetColorForStyle(GUI.skin.button, ButtonDefaultColor);
        }

        public static void SetColorForStyle(GUIStyle aStyle, Color aColor)
        {
            aStyle.normal.textColor = aColor;
            aStyle.onNormal.textColor = aColor;
            aStyle.active.textColor = aColor;
            aStyle.onActive.textColor = aColor;
            aStyle.focused.textColor = aColor;
            aStyle.onFocused.textColor = aColor;
            aStyle.hover.textColor = aColor;
            aStyle.onHover.textColor = aColor;
        }
        
        
        /// <summary>
        /// Sets textColor for the style in the order of normal, active, focus, hover (with every other being onX, eg normal, onNormal)
        /// </summary>
        /// <param name="aStyle"></param>
        /// <param name="aColor"></param>
        public static void SetColorForStyle(GUIStyle aStyle, Color[] aColor)
        {
            // if the color isn't saved correctly, back out
            if (aColor == null || aColor.Length < 8)
                return;
            
            
            aStyle.normal.textColor = aColor[0];
            aStyle.onNormal.textColor = aColor[1];
            aStyle.active.textColor = aColor[2];
            aStyle.onActive.textColor = aColor[3];
            aStyle.focused.textColor = aColor[4];
            aStyle.onFocused.textColor = aColor[5];
            aStyle.hover.textColor = aColor[6];
            aStyle.onHover.textColor = aColor[7];
        }


        public static Color TextColor
        {
            get {
                switch (CurrentPalette)
                {
                    case EditorPalette.Dark:
                        return Color.white;
                    default:
                        return Color.black;
                } 
            }
        }

        private static Color GetInspectorColor(int aColorID)
        {
            int _index;
            switch (CurrentPalette)
            {
                case EditorPalette.BubbleGum:
                    _index = 0;
                    break;
                case EditorPalette.LightPastel:
                    _index = 1;
                    break;
                case EditorPalette.Dark:
                    _index = 2;
                    break;
                default:
                    return InspectorPalettes[0, aColorID];
            }

            return InspectorPalettes[_index, aColorID];
        }

        private static Color GetButtonColor(int aButtonColorID)
        {
            //Obv we could just cast enum here but prefer to have this as a explicitly set value
            int _index;
            switch (CurrentButtonPalette)
            {
                case ButtonPalette.Default:
                    _index = 0;
                    break;
                case ButtonPalette.Pastel:
                    _index = 1;
                    break;
                default:
                    return ButtonPalettes[0, aButtonColorID];
            }

            return ButtonPalettes[_index, aButtonColorID];
        }

        private static Color ButtonTextColor
        {
            get
            {
                switch (CurrentButtonPalette)
                {
                    case ButtonPalette.Dark: return Color.white;
                    default: return Color.black;
                }
            }
        }

        private static Color ParseColor(string aHexString)
        {
            return ColorUtility.TryParseHtmlString(aHexString, out var _color) ? _color : Color.yellow;
        }

    }
}