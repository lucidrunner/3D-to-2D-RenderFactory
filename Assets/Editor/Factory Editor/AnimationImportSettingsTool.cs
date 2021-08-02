using System;
using System.Collections.Generic;
using System.IO;
using Render3DTo2D.Utility.Inspector;
using UnityEditor;
using UnityEngine;

namespace Factory_Editor
{
    public class AnimationImportSettingsTool : EditorWindow
    {
        /* ATTRIBUTION
     * The Mixamo renaming code is adapted from
     * https://forum.unity.com/threads/script-to-rename-mixamo-animations-clips-names-automatic.605281/
     * by Chocolade on the unity forums
     */
    
        private static AnimationImportSettingsTool Editor;
        private static int width = 500;
        private static int height = 300;
        private static int X;
        private static int Y;
        private static List<string> AllFiles = new List<string>();
        private static string LastSubFolderPath;
    
        //Inspector
        public Avatar targetAvatar;
        public bool setRootCurvesToOriginal = true;
    
        [SerializeField]
        public string subFolderPath;
   
        [MenuItem("RenderFactory/Open Imported Animations Setup")]
        static void ShowEditor()
        {
            Editor = GetWindow<AnimationImportSettingsTool>("Animation Import Settings Setup");
            Editor.subFolderPath = LastSubFolderPath;
            CenterWindow();
        }

        private void OnGUI()
        {
            InspectorUtility.BeginSubBoxGroup("Animation Setup", EditorColors.Header, EditorColors.Body);
            EditorGUILayout.BeginHorizontal();
            subFolderPath = EditorGUILayout.TextField("Animations Folder",subFolderPath, GUILayout.MaxWidth(Screen.width - 50f));
            if (InspectorUtility.DrawButton(new GUIContent("Open"), EditorColors.ButtonAction, GUILayout.MaxWidth(50f), GUILayout.MinWidth(50f)))
            {
                var _folder = EditorUtility.OpenFolderPanel("Imported Animations Folder", subFolderPath, "");
                if (_folder.GetProjectPathFromPath(out string _relativeFolder) && AssetDatabase.IsValidFolder(_relativeFolder))
                {
                    subFolderPath = _relativeFolder;
                    SetStaticSubfolderPath();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = AssetDatabase.IsValidFolder(subFolderPath);
            if (InspectorUtility.DrawButton(new GUIContent("Match Clip Names in Folder to File Name"), EditorColors.ButtonRun))
            {
                Rename();
            }
            GUI.enabled = true;
            InspectorUtility.EndSubBoxGroup();
        
            InspectorUtility.BeginSubBoxGroup("Root Motion Setup", EditorColors.Header, EditorColors.Body);
            targetAvatar = (Avatar) EditorGUILayout.ObjectField("Target Avatar", targetAvatar, typeof(Avatar), false);
            setRootCurvesToOriginal = EditorGUILayout.Toggle("Set Root Curves to Original", setRootCurvesToOriginal);
            GUI.enabled = targetAvatar != null && AssetDatabase.IsValidFolder(subFolderPath);
            if (InspectorUtility.DrawButton(new GUIContent("Setup Animations For Avatar"), EditorColors.ButtonRun))
            {
                PerformBaseSetupForAvatar();
            }
            GUI.enabled = true;
            InspectorUtility.EndSubBoxGroup();
        }

        public void Rename()
        {
            DirSearch();
 
            if (AllFiles.Count > 0)
            {
                for (int _index = 0; _index < AllFiles.Count; _index++)
                {
                    (string _fileName, var _modelImporter) = GetImporterAndFileNameFor(AllFiles[_index]);
                    RenameAndImport(_modelImporter, _fileName);
                }
            }
        }

        private (string aFileName, ModelImporter aModelImporter) GetImporterAndFileNameFor(string aAnimationPath)
        {
            int _idx = aAnimationPath.IndexOf("Assets", StringComparison.Ordinal);
            string _asset = aAnimationPath.Substring(_idx);
           
            var _fileName = Path.GetFileNameWithoutExtension(aAnimationPath);
            var _importer = (ModelImporter)AssetImporter.GetAtPath(_asset);
            return (_fileName, _importer);
        }


    

        public void PerformBaseSetupForAvatar()
        {
            if (targetAvatar == null)
                return;
        
            DirSearch();

            for (int _index = 0; _index < AllFiles.Count; _index++)
            {
                (string _fileName, var _modelImporter) = GetImporterAndFileNameFor(AllFiles[_index]);
                _modelImporter.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                _modelImporter.sourceAvatar = targetAvatar;
            
                _modelImporter.SaveAndReimport(); 

                ModelImporterClipAnimation[] _clipAnimations = _modelImporter.clipAnimations;
 
                foreach (var _clipAnimation in _clipAnimations)
                {
                    _clipAnimation.keepOriginalOrientation = setRootCurvesToOriginal;
                    _clipAnimation.keepOriginalPositionY = setRootCurvesToOriginal;
                    _clipAnimation.keepOriginalPositionXZ = setRootCurvesToOriginal;
                }
            
                _modelImporter.clipAnimations = _clipAnimations;
            
            
                _modelImporter.SaveAndReimport();
            }
        }

        private void RenameAndImport(ModelImporter aAsset, string aName)
        {
            ModelImporter _modelImporter = aAsset;
            ModelImporterClipAnimation[] _clipAnimations = _modelImporter.defaultClipAnimations;
            foreach (var _clipAnimation in _clipAnimations)
            {
                _clipAnimation.name = aName;
            }
       
            _modelImporter.clipAnimations = _clipAnimations;
            _modelImporter.SaveAndReimport();
        }
 
        private static void CenterWindow()
        {
            Editor = GetWindow<AnimationImportSettingsTool>();
            X = (Screen.currentResolution.width - width) / 2;
            Y = (Screen.currentResolution.height - height) / 2;
            Editor.position = new Rect(X, Y, width, height);
        }
    
        private static void SetStaticSubfolderPath()
        {
            if(Editor != null)
                LastSubFolderPath = Editor.subFolderPath;
        }

        private static void DirSearch()
        {
            AllFiles = new List<string>();
            string _info = string.IsNullOrWhiteSpace(Editor.subFolderPath) || !Directory.Exists(Editor.subFolderPath) ? Application.dataPath : Editor.subFolderPath;
            string[] _fileInfo = Directory.GetFiles(_info, "*.fbx", SearchOption.AllDirectories);
            foreach (string _file in _fileInfo)
            {
                if (_file.EndsWith(".fbx"))
                    AllFiles.Add(_file);
            }
        }
    }
}
 