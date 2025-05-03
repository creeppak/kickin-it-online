using System.IO;
using UnityEditor;

namespace Editor
{
    public static class RunDevBuildMenuItem
    {
        private const string DevBuildPath = @"Builds\Dev\Kickin_It_Online_URP.exe";
        
        [MenuItem("Kicking It/Run Dev Build _%#&d")]
        public static void RunDevBuild()
        {
            var fullPath = Path.GetFullPath(DevBuildPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Unable to find the Dev Build executable.");
            }
            
            System.Diagnostics.Process.Start(fullPath);
        }
    }
}