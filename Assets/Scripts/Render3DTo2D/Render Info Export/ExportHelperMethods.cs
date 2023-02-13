using System.IO;
using Render3DTo2D.Rigging;
using RootChecker;
using UnityEngine;

namespace Render3DTo2D.Render_Info_Export
{
    public static class ExportHelperMethods
    {
        public static (string Path, string FullFileName) GetFilePathInfo(RigRenderExportArgs aExportArgs, string aFolderName, string aFileName)
        {
            string _fileName = $"{aExportArgs.CameraRig.GetModelName()}_{aExportArgs.CameraRig.RigTag}_{aFileName}";
            string _path = Path.Combine(aExportArgs.LastOutputPath, aFolderName, _fileName);
            return (_path, _fileName);
        }
    }
    
    public static class CameraRigExtensions
    {
        public static string GetModelName(this CameraRig aRig)
        {
            return RootFinder.FindHighestRoot(aRig.transform).name;
        }
        
        
    
    }
}
