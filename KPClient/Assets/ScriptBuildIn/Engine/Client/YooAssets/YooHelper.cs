using System;
using System.Diagnostics;
using BoysheO.Extensions;
using BoysheO.Util;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NexCore.Extensions;
using UnityEngine;
using UnityEngine.Profiling;
using YooAsset;
using YooAssetsArguments;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;
using Debug = UnityEngine.Debug;

namespace UpdateSystem.FixedLogic
{
    public static class YooHelper
    {
        private const bool IsDebug = BuildInConstConfig.IsDebug;
        private const string DefaultPackage = "DefaultPackage";

        public static async UniTask InitYoo()
        {
            if (YooAssets.Initialized)
            {
                Debug.Log($"[{nameof(YooHelper)}]YooAsset已初始化");
                return;
            }

            YooAssets.Initialize();

            var package = YooAssets.TryGetPackage(DefaultPackage);
            if (package == null) package = YooAssets.CreatePackage(DefaultPackage);
            YooAssets.SetDefaultPackage(package);

            // 编辑器下的模拟模式
            FileSystemParameters fileSystemParameters;
            InitializationOperation initializationOperation;
            if (BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_EDITOR)
            {
#if UNITY_EDITOR
                if (IsDebug)
                {
                    Debug.Log($"[{nameof(AppMain)}]加载资源使用编辑器模拟模式");
                }

                var buildResult = EditorSimulateModeHelper.SimulateBuild(DefaultPackage);
                var packageRoot = buildResult.PackageRootDirectory;
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters = fileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                initializationOperation = package.InitializeAsync(createParameters);
#else
                throw new NotSupportedException();
#endif
            }
            // 单机运行模式
            else if (BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_OFFLINE)
            {
                if (IsDebug)
                {
                    Debug.Log($"[{nameof(AppMain)}]加载资源使用单机模式");
                }

                var createParameters = new OfflinePlayModeParameters();
                createParameters.BuildinFileSystemParameters = fileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                initializationOperation = package.InitializeAsync(createParameters);
            }
            // 联机运行模式
            else if (BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_ONLINE)
            {
                var remoteServer = await GetResourceHostInfo();
                if (IsDebug)
                {
                    Debug.Log(
                        $"[{nameof(YooHelper)}]加载资源使用联机模式,更新URL：{remoteServer.ResourceHost},{remoteServer.ResourceHostFallback}");
                }
                
                string defaultHostServer = GetResourcePackageURL(remoteServer.ResourceHost);
                string fallbackHostServer = GetResourcePackageURL(remoteServer.ResourceHostFallback);
                IRemoteServices remoteServices = new SimpleCDNRemoteServices(defaultHostServer, fallbackHostServer);
                var createParameters = new HostPlayModeParameters();
                createParameters.BuildinFileSystemParameters = fileSystemParameters =
                    FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                createParameters.CacheFileSystemParameters =
                    FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                initializationOperation = package.InitializeAsync(createParameters);
            }else if (BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_ONLINE_WITH_REMOTE)
            {
                throw new NotImplementedException("未完工");
            }
            // WebGL运行模式
            else if (BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_WEBGL)
            {
                if (IsDebug)
                {
                    Debug.Log($"[{nameof(YooHelper)}]加载资源使用WebGL模式");
                }

#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
                #error 旧历史代码，未有时间维护
                var remoteServer = await GetResourceHostInfo();
                var createParameters = new WebPlayModeParameters();
			    string defaultHostServer = GetResourcePackageURL(remoteServer.YooUpdateURL);
                string fallbackHostServer = GetResourcePackageURL(remoteServer.YooUpdateURLFallback);
                string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE"; //注意：如果有子目录，请修改此处！
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                createParameters.WebServerFileSystemParameters =
 WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, remoteServices);
                initializationOperation = package.InitializeAsync(createParameters);
#else
                var createParameters = new WebPlayModeParameters();
                createParameters.WebServerFileSystemParameters =
                    FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                initializationOperation = package.InitializeAsync(createParameters);
#endif
            }
            else
            {
                throw new NotSupportedException(
                    $"[{nameof(YooHelper)}]yooMode = {BuildInConstConfig.YooMode} is not supported,please write code here to support");
            }

            await initializationOperation;
            // 如果初始化失败弹出提示界面
            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogWarning($"[{nameof(YooHelper)}]Yoo初始化失败{initializationOperation.Error}");
                throw new Exception(initializationOperation.Error);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        //这里使用async关键词是为了以后重构的话不用动其它逻辑；有可能HostServerIP会想要从远程服务器获取。
        //当然，建议就是不动。整个应用总得有一个地方写死网址，要么写死热更服务器，要么写死网关服务器，从网关服务器获得热更地址。
        //写死cdn网站的缺点是cdn更新没那么迅速，cdn不方便随意开关。写死网关服务器的做法会更灵活点，但是cdn一般会更稳定，承载量大
        //这里的做法是写死cdn网址
        private static async UniTask<RemoteServerModel> GetResourceHostInfo()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var model = DIContext.Configuration.Get<RemoteServerModel>().ThrowIfNull();
            model.ResourceHost.ThrowIfNullOrWhiteSpace();
            model.ResourceHostFallback.ThrowIfNullOrWhiteSpace();
            model.Branch = model.Branch.IsNullOrWhiteSpace() ? "" : model.Branch;
            return model;
        }
        
        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        private static string GetResourcePackageURL(string host)
        {
            host = host.TrimEnd('/');
            var platformName = GetPlatformName();
            return $"{host}/{platformName}";
        }

        private static string GetPlatformName()
        {
#if UNITY_EDITOR
            var target = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            return target switch
            {
                UnityEditor.BuildTarget.Android => "Android",
                UnityEditor.BuildTarget.iOS => "IPhone",
                UnityEditor.BuildTarget.WebGL => "WebGL",
                UnityEditor.BuildTarget.StandaloneWindows => "Win",
                UnityEditor.BuildTarget.StandaloneWindows64 => "Win",
                UnityEditor.BuildTarget.StandaloneOSX => "OSX",
                _ => throw new NotImplementedException(),
            };
#else
            return Application.platform switch
            {
                RuntimePlatform.Android => "Android",
                RuntimePlatform.IPhonePlayer => "IPhone",
                RuntimePlatform.WebGLPlayer => "WebGL",
                RuntimePlatform.WindowsPlayer => "Win",
                RuntimePlatform.OSXPlayer => "OSX",
                _ => throw new NotImplementedException(),
            };
#endif
        }

        private class RemoteServerModel
        {
            public string ResourceHost { get; set; }
            public string ResourceHostFallback { get; set; }
            //分支
            public string Branch { get; set; }
        }
    }
}