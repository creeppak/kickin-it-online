using UnityEditor;

namespace Build
{
    public static class BuildApi
    {
        public static void BuildWebRelease()
        {
            var buildPath = "Builds/Release/Web";
            BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                buildPath, 
                BuildTarget.WebGL, 
                BuildOptions.None);
        }
    }
}