using System.IO;
using Render3DTo2D.Logging;
using UnityEditor;
using UnityEngine;

namespace Render3DTo2D.Utility.IO
{
    public class FolderCreator
    {
        //We can't have this static since we wanna provide the opportunity to inherit it so instead we have to hide the ctor
        protected FolderCreator(){}
        
        

        /// <summary>
        /// Creates a folder using System.IO
        /// </summary>
        public static void CreateFolder(string aPath, string aFolderName)
        {
            if(!Directory.Exists(aPath + @"\" + aFolderName))
            {
                FLogger.LogMessage(null, FLogger.Severity.Priority, $"Creating folder {aFolderName} at path {aPath}", "Folder Creator");
                Directory.CreateDirectory(aPath + @"\" + aFolderName);
            }
        }



        /// <summary>
        /// Creates a folder using the editor mode only AssetDatabase
        /// Contains folder validation via the parent path
        /// </summary>
        public static void CreateFolderViaAdb(string aPath, string aFolderName)
        {

            if(!AssetDatabase.IsValidFolder(aPath + @"\" + aFolderName))
            {
                Debug.Log($"Creating folder {aFolderName} at path {aPath}");
                AssetDatabase.CreateFolder(aPath, aFolderName);
                AssetDatabase.SaveAssets();
            }
            AssetDatabase.Refresh();
        }

        // DEPRECATED - easier to just nuke folder on the HDD than provide this as a risky click in the editor
        // public static void RemoveFolder(bool aRemoveIfUsed, string aPath, string aFolderName)
        // {
        //     FLogger.LogMessage(null, FLogger.Severity.Priority, $"Attempting to remove folder at path {aPath} with name {aFolderName}", "Folder Creator");
        //     //If it doesn't exist we're not gonna try removing it
        //     if (!Directory.Exists(aPath + "/" + aFolderName))
        //         return;
        //     
        //     FLogger.LogMessage(null, FLogger.Severity.Debug, "Passed Exist checked", "Folder Creator");
        //     
        //     //If we're supposed to force remove it we don't even bother looking for files
        //     if(aRemoveIfUsed)
        //     {
        //         Directory.Delete(aPath + "/" + aFolderName, true);
        //         FLogger.LogMessage(null, FLogger.Severity.Debug, "Should have been force deleted now.", "Folder Creator");
        //         return;
        //     }
        //
        //     //Otherwise, query for files & remove only if it's empty (we specifically look for files since we might have created animation folders)
        //     DirectoryInfo _info = new DirectoryInfo(aPath + "/" + aFolderName);
        //     FileInfo[] _files = _info.GetFiles("*.png", SearchOption.AllDirectories);
        //     if (_files.Length == 0)
        //         Directory.Delete(aPath + "/" + aFolderName, true);
        // }
    }
}