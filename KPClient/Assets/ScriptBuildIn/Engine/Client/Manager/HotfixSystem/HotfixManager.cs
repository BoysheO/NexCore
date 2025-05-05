using System;
using System.Diagnostics;
using BoysheO.Extensions;
using BoysheO.Util;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem;
using UnityEngine;
using UnityEngine.Profiling;
using YooAsset;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ScriptFramework.BuildIn.ClientCode.Manager.HotfixSystem
{
    /// <summary>
    /// 热更步骤：初始化资源组件=》开始从资源组件加载代码=》重载DI=》开始播放（热更）主场景
    /// HotfixManager就不注入到容器内了
    /// </summary>
    public sealed class HotfixManager
    {
        public static HotfixManager Instance => new();

        public async UniTask LoadConfiguration()
        {
            //为了方便修改热更CDN地址不用重新出包，CDN地址要从SteamingAssets读取
            //因为SteamingAssets只能异步读取（其实是可以同步的，只是WebGL会有点不兼容），所以不能在Configuration构建时立即注入
            //所以要在容器构建完成后后面的流程中把配置补加载上
            await StreamingAssetsConfigurationSource.Instance.LoadAsync();
        }

        public async UniTask StartCodeUpdateWithUI()
        {
            var handle = Resources.LoadAsync<GameObject>("Prefab/UI/UIResourceLoadingCanvas");
            await handle;
            var go = (GameObject)handle.asset;
            _ = Object.Instantiate(go);
            //运行到这里，控制权就等于交给UIResourceLoadingCanvas了
        }

        public void ReloadDI()
        {
            DIContext.Rebuild();
        }

        public async UniTask StartUp(IProgress<float> progress)
        {
            //即使Configuration释放，这个是单例，不必二次加载
            // await StreamingAssetsConfigurationSource.Instance.LoadAsync();
            //在资源全部更新完成之后再加载YooAsset的Appsetting.json
            await YooAssetsConfigurationSource.Instance.LoadAsync();
            var startUpScene = DIContext.Configuration.GetValue<string>("StartUpScene").ThrowIfNullOrWhiteSpace();
            var handle = YooAssets.LoadSceneAsync(startUpScene);
            while (!handle.IsDone)
            {
                progress.Report(handle.Progress);
                await UniTask.Yield();
            }

            handle.Dispose();
        }
    }
}