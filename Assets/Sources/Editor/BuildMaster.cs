using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public static class BuildMaster
    {
        private class BuildTargetInfo
        {
            public BuildTarget BuildTarget { get; set; }
            public string BinaryName { get; set; }
        }
        
        private static readonly BuildTargetInfo [] BuildTargetInfos =
        {
            new()
            {
                BuildTarget = BuildTarget.StandaloneLinux64,
                BinaryName = "kicking-it.x86_64"
            },
            // new()
            // {
            //     BuildTarget = BuildTarget.StandaloneWindows64,
            //     BinaryName = "kicking-it.exe"
            // },
            // new()
            // {
            //     BuildTarget = BuildTarget.StandaloneOSX,
            //     BinaryName = "kicking-it.app"
            // },
            new()
            {
                BuildTarget = BuildTarget.WebGL,
                BinaryName = "kicking-it"
            }
        };
        
        [MenuItem("Kicking It/Build All Platforms")]
        public static void BuildAllPlatforms()
        {
            var initialActiveBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            
            for (var i = 0; i < BuildTargetInfos.Length; i++)
            {
                var buildTargetInfo = BuildTargetInfos[i];
                var buildTarget = buildTargetInfo.BuildTarget;
                var binaryName = buildTargetInfo.BinaryName;

                if (EditorUtility.DisplayCancelableProgressBar("Build all platforms", "Building " + buildTarget, (float)
                        (i + 1) / (BuildTargetInfos.Length + 1)))
                {
                    break;
                }

                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);

                var buildFolderPath = Path.Combine("Builds", "Release", buildTarget.ToString());
                Directory.CreateDirectory(buildFolderPath);

                var buildOptions = new BuildPlayerOptions
                {
                    scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray(),
                    locationPathName = Path.Combine(buildFolderPath, binaryName),
                    target = buildTarget,
                    options = BuildOptions.None
                };
                
                var report = BuildPipeline.BuildPlayer(buildOptions);

                if (report.summary.result != BuildResult.Succeeded)
                {
                    var shouldContinue = EditorUtility.DisplayDialog("Build Failed",
                        $"Build for {buildTarget} failed with error: {report.summary.result}", "Continue", "Terminate");

                    if (!shouldContinue)
                    {
                        break;
                    }
                }
                else
                {
                    Debug.Log($"Build for {buildTarget} succeeded: {report.summary.totalSize} bytes");
                }
            }
            
            EditorUtility.ClearProgressBar();
            
            var initialBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(initialActiveBuildTarget);
            EditorUserBuildSettings.SwitchActiveBuildTarget(initialBuildTargetGroup, initialActiveBuildTarget);
        }
    }
}