using System;
using System.Collections.Generic;
using System.IO;
using BoysheO.Extensions;
using BoysheO.Util;
using UnityEditor;
using UnityEngine;

namespace ScriptEditor.Engine.Client.Menu
{
    internal class Storage
    {
        [MenuItem("Tools/Storage/OpenPersistentDataPath")]
        static void Open()
        {
            EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
        }
    
        [MenuItem("Tools/Storage/DeleteAllPlayerPrefs")]
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("done");
        }

        [MenuItem("Tools/Storage/DeleteEditorPlayerPrefs")]
        public static void DeleteAllEditorPlayerPrefs()
        {
            EditorPrefs.DeleteAll();
            Debug.Log("done");
        }
    
        // [MenuItem("Tools/" + nameof(ClearBuild))]
        //BuildLocation容易出现误选的情况，造成意外删除正常文件
        private static void ClearBuild()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            var location = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);
            var dir = location.AsSpan().SkipTailCount(location.AsPath().GetFileName()).ToNewString();
            if (EditorUtility.DisplayDialog("", $"即将清空文件夹{dir}", "sure", "no"))
            {
                Directory.Delete(dir, true);
                Directory.CreateDirectory(dir);
                Debug.Log("done");
            }
        }

        [MenuItem("Tools/Storage/" + nameof(ClearBuildCache))]
        private static void ClearBuildCache()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            Caching.ClearCache();
            Debug.Log("done");
        }
    
        [MenuItem("Tools/Storage/ClearUPMCache")]
        private static void ClearUPMCache()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            //文件夹路径来源：https://docs.unity3d.com/cn/2020.1/Manual/upm-cache.html
            List<string> dirs = new();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                {
                    var envir = System.Environment.GetEnvironmentVariable("LOCALAPPDATA");
                    if (envir != null)
                    {
                        var path = Path.Combine(envir, @"Unity\cache");
                        dirs.Add(path);
                    }
                }
                {
                    var envir = System.Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
                    if (envir != null)
                    {
                        var path = Path.Combine(envir, @"Unity\cache");
                        dirs.Add(path);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                var envir = System.Environment.GetEnvironmentVariable("HOME");
                if (envir != null)
                {
                    var path = Path.Combine(envir, @"Library/Unity/cache");
                    dirs.Add(path);
                }
            }
            else throw new NotImplementedException();

            Debug.Log($"即将清空文件夹{string.Join("\n", dirs)}");
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
                Directory.CreateDirectory(dir);
            }

            Debug.Log("done");
        }

    }
}
