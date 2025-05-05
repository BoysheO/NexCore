using System;
using System.IO;
using BoysheO.Extensions;
using Nett;
using NexCore.Extensions;
using UnityEngine;

namespace ScriptEngine.Editor.ClientCode.Manager.EnvironmentSystem
{
    //读取environmentconfig.toml应当用Helper而不是注入到Configuration
    //使用指南：
    public static class EnvironmentHelper
    {
        public static EnvironmentConfigModel GetEnvironment()
        {
            
            const string configFile = "environmentconfig.toml";

            // 获取Unity项目的根目录路径
            var projectRoot = Application.dataPath.AsPath().ToDirectoryInfo().Parent!.Parent!.FullName.ReplaceBackslash();
            // 拼接出config文件的绝对路径
            var configFilePath = Path.Combine(projectRoot, configFile).ReplaceBackslash();

            // 检查文件是否存在
            if (!File.Exists(configFilePath))
            {
                Debug.LogError("环境配置文件不存在：" + configFilePath);
                throw new NullReferenceException($"missing {configFilePath}");
            }

            var text = File.ReadAllText(configFilePath);
            var config = Toml.ReadString<EnvironmentConfigModel>(text);
            return config;
        }
    }
}