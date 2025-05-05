using NexCore.DI;
using NexCore.UnityEnvironment;
using Cysharp.Threading.Tasks;
using Framework.Hotfix.ClientCode.Configuration;
using Hotfix.ResourceMgr.Abstractions;
using HotScripts.Hotfix.GamePlay.FrameworkSystems.SoundManagerSystem;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
using ScriptGamePlay.Hotfix.ClientCode.Manager.MyLanSystem;
using ScriptGamePlay.Hotfix.ShareCode.Manager.ProfileSystem;
#if ENABLE_STEAMSYSTEM
using BoysheO.Util;
using Grpc.Net.Client;
using Steamworks;
#endif
using TableSystem.Abstractions;
using TableSystem.Implements;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;

namespace ScriptGamePlay.Hotfix.ClientCode.Script.Handler
{
    [SubscribeMessage(nameof(InitSystemHandler))][Preserve]
    public sealed class InitSystemHandler : ISyncHandler
    {
        private const bool IsDebug = HotfixConstConfig.IsDebug;
        private MyLanManager _myLanManager;
        private IResourceManager _resourceManager;
        private IProfileManager _profile;
        private IGameServiceProvider _gameServiceProvider;

        public InitSystemHandler(MyLanManager myLanManager,
            IResourceManager resourceManager, IProfileManager profile, IGameServiceProvider gameServiceProvider)
        {
            _myLanManager = myLanManager;
            _resourceManager = resourceManager;
            _profile = profile;
            _gameServiceProvider = gameServiceProvider;
        }

        private async UniTask ProcessAsync()
        {
            if (IsDebug)
            {
                var go = new GameObject("fpsCounter", typeof(FPSCounter));
                UnityEngine.Object.DontDestroyOnLoad(go);
                _profile.IsEnable = true;
            }

            MainThreadDispatcher.Initialize();

            // #region 测试grpc功能
            //
            // Debug.Log("grpcTest start");
            // {
            //     using var channel = GrpcChannel.ForAddress("http://localhost:5114", new GrpcChannelOptions()
            //     {
            //         HttpHandler = new YetAnotherHttpHandler()
            //         {
            //             Http2Only = true
            //         },
            //         DisposeHttpClient = true
            //     });
            //     var client = new Echo.EchoClient(channel);
            //     var ret = client.SayHello(new HelloRequest()
            //     {
            //         Message = "hello hclr"
            //     });
            //     var myUI = DIContext.ServiceProvider.GetRequiredService<MyUIManager>();
            //     myUI.FlowText(ret.ToString());
            //     Debug.Log(ret);
            // }
            // Debug.Log("grpcTest end");
            //
            // #endregion

            // Application.targetFrameRate = 60; //限制一下帧率防止手机发热,提供15、30、60三种选项
            Input.simulateMouseWithTouches = false; //触摸逻辑和鼠标逻辑完全不同，禁用模拟 *禁用模拟影响了finger组件

#if ENABLE_STEAMSYSTEM
            //初始化SteamSDK
            var stMgr = DIContext.ServiceProvider.GetRequiredService<SimSteamManager>();
            try
            {
                stMgr.Init();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(-1);
#endif
            }
#endif
            //初始化表格
            var tbManager = (PbTableDataManager)DIContext.ServiceProvider.GetRequiredService<ITableDataManager>();
            {
                var h = tbManager.Init();
                await h.Init(false);
            }

            #region 重新初始化语言

            _myLanManager.Reinit();

            #endregion

            var envir = DIContext.ServiceProvider.GetRequiredService<IUnityEnvironment>();
            var token = envir.CancellationToken;

            // Debug.Log("nothing");
            // var uiMgr = DIContext.ServiceProvider.GetRequiredService<MyUIManager>();
            // uiMgr.FlowText("do nothing");
            // await UIHelper.LoadAndShowFocusUIViewAsync<UIGameStartPanel>();

            //var resMgr = DIContext.ServiceProvider.GetRequiredService<IResourceManager>();

            // //ui系统测试代码，上线请移除
            // var uiMgr = DIContext.ServiceProvider.GetRequiredService<UIManager>();
            // var panel = await uiMgr.LoadUIAsync<UIExamplePanel>(token);
            // ((IView)panel).ShowFront();
            // panel.ShowText("今天天气真好！序号：1");

            // var mgr2 = DIContext.ServiceProvider.GetRequiredService<FlexalonManager>();
            // mgr2.Init();

            // var mgr = DIContext.ServiceProvider.GetRequiredService<CCSceneManager>();
            // await mgr.SwitchTitleSceneAsync();

            var soundManager = DIContext.ServiceProvider.GetRequiredService<SoundManager>();
            await soundManager.InitAsync();
            Debug.Log("done"); 
        }

        public void Process(object msg)
        {
            ProcessAsync().Forget();
        }
    }
}