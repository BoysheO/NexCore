using System;
using System.IO;
using BoysheO.Extensions.Unity3D.Extensions;
using ScriptBuildIn.Engine.Client.Configuration;
using UnityEngine;
using System.Linq;
using BoysheO.Util;
using UnityEditor;

namespace ScriptEditor.GamePlay.Client.BuildInConstConfigSystem
{
    public static class BuildInConstConfigHelper
    {
        private static string[] AllowLoadModes = {
            nameof(BuildInConstConfig.LOADMODE_IO),
            nameof(BuildInConstConfig.LOADMODE_ASSET),
            nameof(BuildInConstConfig.LOADMODE_INTERNAL),
            nameof(BuildInConstConfig.LOADMODE_ASSET_BUT_EDITOR)
        };

        private static string[] AllowYooMode = {
            nameof(BuildInConstConfig.YOOMODE_WEBGL),
            nameof(BuildInConstConfig.YOOMODE_EDITOR),
            nameof(BuildInConstConfig.YOOMODE_ONLINE),
            nameof(BuildInConstConfig.YOOMODE_OFFLINE)
        };
        
        private static string FilePath => $"{Application.dataPath}/ScriptBuildIn/Engine/Client/Configuration/BuildInConstConfig.g.cs";

        private static string GenCode(string loadMode, string yooMode)
        {
            if (!AllowLoadModes.Contains(loadMode)) throw new ArgumentOutOfRangeException();
            if (!AllowYooMode.Contains(yooMode)) throw new ArgumentOutOfRangeException();
            var template = Resources.Load<TextAsset>("BuildInConstConfig.cs");//BuildInConstConfig.cs.txt
            template.ThrowIfNullOrFakeNull();
            var templateText = template.text;
            return templateText.Replace("{LOADMODE}", loadMode)
                .Replace("{YOOMODE}", yooMode)
                .Replace("{Generator}",DebugUtil.GetCallerContext());
        }
        
        public static void Set(string loadMode, string yooMode)
        {
            var code = GenCode(loadMode, yooMode);
            File.WriteAllText(FilePath,code);
            AssetDatabase.Refresh();
            Debug.Log($"Write string {code.Length} length to {FilePath} done");
        }

        [MenuItem("Engine/GenBuildInConfig/Example")]
        private static void Example()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            Debug.Log("这只是一个代码生成示例，具体生成的代码由开发者自己维护");
            Set(nameof(BuildInConstConfig.LOADMODE_ASSET), nameof(BuildInConstConfig.YOOMODE_ONLINE));
            Debug.Log("Done");
        }
    }
}