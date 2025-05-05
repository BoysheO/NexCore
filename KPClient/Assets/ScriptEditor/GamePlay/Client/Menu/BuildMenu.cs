// using System;
// using System.IO;
// using BoysheO.Extensions;
// using BoysheO.Util;
// using PreserveModule.Editor;
// using ScriptBuildIn.Engine.Client.Configuration;
// using ScriptEditor.GamePlay.Client.BuildInConstConfigSystem;
// using UnityEditor;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace ScriptEditor.GamePlay.Client.Menu
// {
//     public class BuildMenu
//     {
//         [MenuItem("Engine/Build/QuickBuild")]
//         public static void QuickBuild1()
//         {
//             Debug.Log(DebugUtil.GetCallerContext());
//             Debug.Log("只出基础包");
//             var target = EditorUserBuildSettings.activeBuildTarget;
//             BuildInConstConfigHelper.Set(nameof(BuildInConstConfig.LOADMODE_ASSET), nameof(BuildInConstConfig.YOOMODE_ONLINE));
//             //要出python大招来完成Editor的异步逻辑
//             throw new NotImplementedException();
//             //
//             // var saveDir = Application.dataPath.AsPath().ToDirectoryInfo().Parent!.FullName + "/BuildResult";
//             // var ignoreFile = $"{saveDir}/.gitignore";
//             // IOUtil.ValidateAndPreparePath(ignoreFile, true, false);
//             // if(!File.Exists(ignoreFile)) File.WriteAllText(ignoreFile,"# 忽略所有文件夹下的所有内容\n**/*");
//             // IncludeMenu.IncludePreserve();
//             // BuildPipeline.BuildPlayer(new BuildPlayerOptions()
//             // {
//             //     target = target,
//             //     locationPathName = saveDir,
//             //     extraScriptingDefines = new[]
//             //     {
//             //         IncludeMenu.INCLUDE_PRESERVETYPE,
//             //     }
//             // });
//             // BuildInConstConfigHelper.Set(nameof(BuildInConstConfig.LOADMODE_INTERNAL), nameof(BuildInConstConfig.YOOMODE_EDITOR));
//             // Debug.Log("done");
//         }
//
//         public static void QuickBuild()
//         {
//             var config = DIContext.GetConfigDebugView();
//             Debug.Log(config);
//             var isSuccess = Random.value > 0.5f;
//             if (isSuccess)
//             {
//                 Environment.Exit(0);
//             }
//             else
//             {
//                 Environment.Exit(1);
//             }
//         }
//     }
// }