using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BoysheO.Extensions;
using BoysheO.Util;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using Microsoft.Extensions.Configuration;
using UnityEditor;
using UnityEngine;

namespace ScriptEditor.Engine.Client.Menu
{
    //在这里的所有函数均以非dev build为标准生成。有需要可以自行补充
    public static class HCLRHelper
    {
        /// <summary>
        /// 将由HCLR编译好的Dll和Pdb复制到资源目录中
        /// </summary>
        [MenuItem("Engine/HCLRHelper/CompileHotDllBytes/Current")]
        public static void CompileHotDllBytesCurrent()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            EditorUserBuildSettings.development = false;
            CompileHotDllBytes(EditorUserBuildSettings.activeBuildTarget, false);
            AssetDatabase.Refresh();
            Debug.Log("done");
        }

        /// <summary>
        /// 将由HCLR编译好的Dll和Pdb复制到资源目录中
        /// </summary>
        [MenuItem("Engine/HCLRHelper/CompileHotDllBytes/All")]
        public static void CompileHotDllBytesAll()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            EditorUserBuildSettings.development = false;
            var targets = GetBuildTargets();
            foreach (var buildTarget in targets)
            {
                CompileHotDllBytes(buildTarget, false);
            }

            AssetDatabase.Refresh();
            Debug.Log("done");
        }

        private static void CompileHotDllBytes(BuildTarget target, bool devBuild)
        {
            CompileDllCommand.CompileDll(target, devBuild);
            var clientRoot = Application.dataPath.AsSpan().SkipTailCount("Assets/").ToString();
            var srcDir = $"{clientRoot}/HybridCLRData/HotUpdateDlls"; //这个地方存放的总是刚执行CompileDllCommand.CompileDll生成的文件
            var hotfixDlls = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Select(v => v.name).ToArray();
            var dllRelativeDir = DIContext.Configuration
                .GetValue<string>("client_setting:HotfixDllBytesRelativeSaveDir")
                .ThrowIfNullOrWhiteSpace();
            var dllDestDir = $"{clientRoot}/{dllRelativeDir}/{target}";
            const string dll_bytes_ext = "dll.bytes";
            CreatDir(dllDestDir);
            CleanDir(dllDestDir, $"*.{dll_bytes_ext}");
            WriteGitKeep(dllDestDir);
            WriteGitIgnore(dllDestDir, $"*.{dll_bytes_ext}\n*.{dll_bytes_ext}.meta");
            foreach (var s in hotfixDlls)
            {
                var src = $"{srcDir}/{EditorUserBuildSettings.activeBuildTarget}/{s}.dll";
                var dest = $"{dllDestDir}/{s}.{dll_bytes_ext}";
                FileCopy(src, dest);
            }

            var pdbRelativeDir = DIContext.Configuration
                .GetValue<string>("client_setting:HotfixPdbBytesRelativeSaveDir")
                .ThrowIfNullOrWhiteSpace();
            var pdbDestDir = $"{clientRoot}/{pdbRelativeDir}/{target}";
            const string pdb_bytes_ext = "pdb.bytes";
            CreatDir(pdbDestDir);
            CleanDir(pdbDestDir, $"*.{pdb_bytes_ext}");
            WriteGitKeep(pdbDestDir);
            WriteGitIgnore(pdbDestDir, $"*.{pdb_bytes_ext}\n*.{pdb_bytes_ext}.meta");
            foreach (var s in hotfixDlls)
            {
                var file = $"{srcDir}/{EditorUserBuildSettings.activeBuildTarget}/{s}.pdb";
                var dest = $"{pdbDestDir}/{s}.{pdb_bytes_ext}";
                FileCopy(file, dest);
            }
        }

        /// <summary>
        /// 将由HCLR编译好的Patch复制到资源目录中
        /// Patch只需要在程序集发生变化时生成一次，不需要太常用
        /// </summary>
        [MenuItem("Engine/HCLRHelper/GenPatchBytesAll")]
        public static void GenPatchBytesAll()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            var buildTargets = GetBuildTargets();
            foreach (var buildTarget in buildTargets)
            {
                GenAOTDlls(buildTarget, false);
            }

            CopyPatchBytes();
            AssetDatabase.Refresh();
            Debug.Log("done");
        }

        private static void GenAOTDlls(BuildTarget target, bool devBuild)
        {
            CompileDllCommand.CompileDll(target, devBuild);
            StripAOTDllCommand.GenerateStripedAOTDlls(target);
        }

        private static void CopyPatchBytes()
        {
            var lst = GetPatchedAOTAssemblyList();
            if (lst.Count > 0)
            {
                Debug.Log($"要复制的Patch为：\n{lst.JoinAsOneString("\n")}");
            }
            else
            {
                Debug.Log("无要复制的Patch");
                return;
            }

            var clientRoot = Application.dataPath.AsSpan().SkipTailCount("/Assets").ToString();
            var srcDir = $"{clientRoot}/HybridCLRData/AssembliesPostIl2CppStrip";
            var patchRelativeDir = DIContext.Configuration
                .GetValue<string>("client_setting:HotfixPatchBytesRelativeSaveDir")
                .ThrowIfNullOrWhiteSpace();
            const string patch_bytes_ext = "patch.bytes";
            foreach (var platformDir in Directory.GetDirectories(srcDir))
            {
                var files = Directory.GetFiles(platformDir);
                if (files.Length == 0) continue; //这个平台文件夹下没有文件，说明并没有为这个平台生成Patch，不需要复制到资源目录

                var platformName = platformDir.AsPath().GetFileName();
                var destDir = $"{clientRoot}/{patchRelativeDir}/{platformName}";
                CreatDir(destDir);
                CleanDir(destDir, $"*.{patch_bytes_ext}");
                WriteGitKeep(destDir);
                WriteGitIgnore(destDir, $"*.{patch_bytes_ext}\n*.{patch_bytes_ext}.meta");
                foreach (var srcFile in files)
                {
                    var fileName = srcFile.AsPath().GetFileNameWithoutExt();
                    var destFile = $"{destDir}/{fileName}.{patch_bytes_ext}";
                    //只有在PatchedAOTAssemblyList列表中的才需要被复制。多余的没有加载的必要
                    if (lst.Count > 0 && !lst.Contains(fileName)) continue;
                    FileCopy(srcFile, destFile);
                }
            }
        }

        private static void WriteGitKeep(string fullDir)
        {
            if (!File.Exists($"{fullDir}/.gitkeep"))
            {
                File.WriteAllText($"{fullDir}/.gitkeep", "");
            }
        }

        private static void WriteGitIgnore(string fullDir, string content)
        {
            var comment = $"# generate at {DebugUtil.GetCallerContext()}\n";
            content = comment + content;
            if (!File.Exists($"{fullDir}/.gitignore"))
            {
                File.WriteAllText($"{fullDir}/.gitignore", content);
            }
        }

        private static void CreatDir(string fullDir)
        {
            if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);
        }

        private static void CleanDir(string fullDir, string searchOption)
        {
            var files = Directory.GetFiles(fullDir, searchOption);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private static void FileCopy(string srcFile, string destFile)
        {
            if (File.Exists(srcFile))
            {
                File.Copy(srcFile, destFile, true);
                Debug.Log($"copy {srcFile} to {destFile} competed");
            }
            else Debug.LogWarning($"missing {srcFile}");
        }

        private static BuildTarget[] GetBuildTargets()
        {
            string configKey;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                configKey = "client_setting:BuildTargetsInMac";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                configKey = "client_setting:BuildTargetsInWin";
            }
            else throw new NotImplementedException();
            var targets = DIContext.Configuration.GetSection(configKey)
                .Get<BuildTarget[]>()
                .ThrowIfNull()!
                .Where(v => Enum.IsDefined(typeof(BuildTarget), v));
            return targets.ToArray();
        }

        private static IReadOnlyList<string> GetPatchedAOTAssemblyList()
        {
            //寻找由Hclr生成的AOTGenericReferences文件内容
            var propertyInfo = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(v => v.GetTypes())
                .Where(v => v.Name == "AOTGenericReferences")
                .Select(v =>
                    v.GetField("PatchedAOTAssemblyList", BindingFlags.Public | BindingFlags.Static))
                .FirstOrDefault(v => v != null);
            if (propertyInfo == null)
            {
                Debug.LogWarning("没找到AOTGenericReferences.PatchedAOTAssemblyList，可能是没有运行过HCLR的GenerateAll");
                return Array.Empty<string>();
            }

            var value = propertyInfo.GetValue(null) as IReadOnlyList<string>;
            if (value == null) throw new Exception("未知错误");
            return value.Select(v => v.AsSpan().SkipTailCount(".dll").ToString()).ToArray();
        }
    }
}