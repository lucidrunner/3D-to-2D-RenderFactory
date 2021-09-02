using UnityEditor;
using UnityEngine;

namespace Shared_Scripts
{
    public static class FactoryStyles
    {
            
        public static readonly GUIStyle FoldoutHeader = new GUIStyle(EditorStyles.foldoutHeader)
        {
            margin =  new RectOffset(33, 9, 5, 0),
            fontSize = 13,
        };
            
        public static readonly GUIStyle SubFoldoutHeader = new GUIStyle(EditorStyles.foldoutHeader)
        {
            margin =  new RectOffset(18, 9, 5, 0),
            fontSize = 13
        };

        public static readonly GUIStyle NewFoldoutHeader = new GUIStyle(GUI.skin.box)
        {
            margin =  new RectOffset(0,0,5,0),
            padding = new RectOffset(5,5,5,5),
            normal = {background = FactoryEditorTextures.BoxHeaderBackground}
        };

        public static readonly GUIStyle NewFoldoutBody = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5,5,5,5),
            margin =  new RectOffset(19,5,0,5),
            normal = {background = FactoryEditorTextures.BoxBackground}
        };
        public static readonly GUIStyle NewSubFoldoutHeader = new GUIStyle(GUI.skin.box)
        {
            margin =  new RectOffset(5, 9, 5, 0),
            normal = {background = FactoryEditorTextures.BoxHeaderBackground}
        };

        public static readonly GUIStyle NewSubFoldoutBody = new GUIStyle(GUI.skin.box)
        {
            margin =  new RectOffset(5, 9, 0, 0),
            normal = {background = FactoryEditorTextures.BoxBackground}
        };


        public static readonly GUIStyle BoxGroup = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5,5,5,5),
            margin =  new RectOffset(19,5,0,5),
            normal = {background = FactoryEditorTextures.BoxBackground}
        };
            
        public static readonly GUIStyle ClosedBoxGroup = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5,5,5,5),
            margin =  new RectOffset(19,5,0,5),
            normal = {background = FactoryEditorTextures.ClosedBoxBackground}
        };


        public static readonly GUIStyle BoxGroupHeader = new GUIStyle(GUI.skin.box)
        {
            margin =  new RectOffset(0,0,5,0),
            padding = new RectOffset(5,5,5,5),
            normal = {background = FactoryEditorTextures.BoxHeaderBackground}

        };
            
        public static readonly GUIStyle SubBoxGroup = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5,5,5,5),
            margin = new RectOffset(0,0,0,0),
            normal =  {background = FactoryEditorTextures.BoxBackground}
        };
            
        public static readonly GUIStyle ClosedSubBoxGroup = new GUIStyle(GUI.skin.box)
        {
            padding = new RectOffset(5,5,5,5),
            margin = new RectOffset(0,0,0,0),
            normal =  {background = FactoryEditorTextures.ClosedBoxBackground}
        };


        public static readonly GUIStyle AlignedButton = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(5, 0 ,0 ,0)
        };
    }
}
