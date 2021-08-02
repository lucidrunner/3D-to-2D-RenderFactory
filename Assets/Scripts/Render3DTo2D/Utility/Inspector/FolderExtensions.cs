using System;
using System.IO;
using UnityEditor;

namespace Render3DTo2D.Utility.Inspector
{
    public static class FolderExtensions
    {
        /// <summary>
        /// Returns a relative path that's usable by the AssetDataBase from a full file path.
        /// </summary>
        /// <param name="aFullPath">The full path of a folder / file.</param>
        /// <param name="aAssetPath">The relative path in the folder structure.</param>
        /// <returns>True if the path was valid.</returns>
        public static bool GetProjectPathFromPath(this string aFullPath, out string aAssetPath)
        {
            aAssetPath = null;
            if (string.IsNullOrWhiteSpace(aFullPath) || !Directory.Exists(aFullPath))
            {
                return false;
            }

            int _index = aFullPath.IndexOf("Assets/", StringComparison.Ordinal);
            if (_index < 0)
                return false;

            aAssetPath = aFullPath.Substring(_index);
            return true;
        }
    }
}