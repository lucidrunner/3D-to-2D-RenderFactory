using UnityEditor;
using UnityEngine;

namespace Shared_Scripts
{
    public static class FactoryEditorTextures
    {
        public static Texture2D BoxBackground
        {
            get
            {
                if (boxBackground == null)
                    boxBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/GUI Textures/foldout_border_lightgray.png");
    
                return boxBackground;
            }
        }
            
        public static Texture2D FoldoutBodyBackground
        {
            get
            {
                if(foldoutBodyBackground == null)
                    foldoutBodyBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/GUI Textures/foldout_border_lightgray.png");
                return foldoutBodyBackground;
            }
        }

        public static Texture2D BoxHeaderBackground
        {
            get
            {
                if (boxheaderBackground == null)
                    boxheaderBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/GUI Textures/border_lightgray_alt2.png");

                return boxheaderBackground;
            }
        }

        private static Texture2D boxheaderBackground;

        public static Texture2D ClosedBoxBackground
        {
            get
            {
                if (closedBoxBackground == null)
                    closedBoxBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/GUI Textures/border_lightgray_alt.png");

                return closedBoxBackground;
            }
        }

        private static Texture2D closedBoxBackground;

        private static Texture2D foldoutBodyBackground;
    
        private static Texture2D boxBackground;
    }
}