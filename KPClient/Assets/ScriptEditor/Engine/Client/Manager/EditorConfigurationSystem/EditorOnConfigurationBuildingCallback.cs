using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BoysheO.Extensions;
using BoysheO.Extensions.Configuration.Toml;
using Microsoft.Extensions.Configuration;
using NexCore.DI;
using ScriptBuildIn.Engine.Client.Configuration;
using UnityEngine;
using Debug = UnityEngine.Debug;

// ReSharper disable InconsistentNaming

namespace SimEngineEditor.Manager.EditorUserConfigurationSystem
{

    [ConfigPriority(10)]
    public sealed class EditorOnConfigurationBuildingCallback : IOnConfigurationBuildingCallback
    {
#if UNITY_EDITOR_WIN
        //在本人电脑上，开启reload会在初始化FileWatch相关逻辑时发生长时间（约18秒）卡顿，因此关闭
        private const bool ReloadSupport = false;
#else
        //在mac上未出现此情况
        private const bool ReloadSupport = true;
#endif

        public void OnCallback(IConfigurationBuilder builder)
        {
            var workspaceRoot = Application.dataPath.AsPath().ToDirectoryInfo().Parent!.Parent!.FullName
                .ReplaceBackslash();
            builder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("WorkSpaceRoot", workspaceRoot)
            });
            //
            var clientRoot = Application.dataPath.AsPath().ToDirectoryInfo().Parent!.FullName.ReplaceBackslash();
            //在 Mac上，ReloadOnChange不会导致18秒的FileWatch时间;在我的windows机器上，ReloadOnChange会导致18秒的执行耗时，耗时在父类构造函数
            builder.AddTomlFile($"{workspaceRoot}/workspaceconfig.toml", false, ReloadSupport);

            if (BuildInConstConfig.IsUseGitBranchInfo)
            {
                var branch = GetCurrentGitBranch();
                Debug.Log($"[{nameof(EditorOnConfigurationBuildingCallback)}]当前分支：{branch}");
                builder.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Branch", branch)
                });
            }
            //
            // builder.Add<TomlConfigurationSource>(s =>
            // {
            //     s.FileProvider = null;
            //     s.Path = $"{workspaceRoot}/workspaceconfig.toml";
            //     s.Optional = false;
            //     s.ReloadOnChange = true;
            //     s.ResolveFileProvider();
            // });


            var editorJsonFile = clientRoot + "/ProjectSettings/Appsetting.json";
            builder.AddJsonFile(editorJsonFile, true, ReloadSupport);

            var editorTomlFile = clientRoot + "/ProjectSettings/Appsetting.toml";
            builder.AddTomlFile(editorTomlFile, true, ReloadSupport);

            var userJsonFile = clientRoot + "/UserSettings/Appsetting.json";
            builder.AddJsonFile(userJsonFile, true, ReloadSupport);

            var userTomlFile = clientRoot + "/UserSettings/Appsetting.toml";
            builder.AddTomlFile(userTomlFile, true, ReloadSupport);

            var args = Environment.GetCommandLineArgs();
            builder.AddCommandLine(args);
        }

        string GetCurrentGitBranch()
        {
            string branchName = "unknown";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("git");
                startInfo.Arguments = "rev-parse --abbrev-ref HEAD";
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        branchName = output.Trim();
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Git 错误: " + error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("获取 Git 分支时发生异常: " + ex.Message);
            }

            return branchName.ReplaceBackslash();
        }
    }
}