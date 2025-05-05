using System;
using System.Diagnostics;
using System.Threading;
using BoysheO.TinyStateMachine;
using Cysharp.Threading.Tasks;
using ScriptBuildIn.Engine.Client.Configuration;
using UnityEngine;
using UnityEngine.Profiling;
using UpdateSystem.FixedLogic;
using YooAsset;
using Debug = UnityEngine.Debug;

namespace UpdateSystem.Loader.Scripts
{
    //此类只负责加载DefaultPackage内的内容。对于多个Package的情况，请自行处理。
    public sealed class UpdateStateMachine : IDisposable
    {
        private const string DefaultPackage = "DefaultPackage";
        private const bool IsDebug = BuildInConstConfig.IsDebug;

        public enum State
        {
            NeverInit,

            //检查是否有最新的基础包，如果有，则不让登录（未完成）
            CheckNewBaseBundle,

            InitYoo,

            //拉取更新信息
            PullUpdateInformation,
            WaitingDownloadConfirm,
            DownloadingAssets,

            //检测到数据网络时默认暂停，直到网络恢复或用户确认使用流量数据
            DownloadingPause,
            Competed,
            UpdateFail,
        }

        private enum Event
        {
            Start,
            AnyFail,
            Retry,
            PullUpdateInfoOkAndNeedDownloading,
            PullUpdateInfoOkAndNoDownloadNeed,
            DownloadingPaused,
            DownloadCompeted,
            MobileNetworkDetected,
        }

        public enum ErrorType
        {
            None,
            InitFail,
            MobileNetworkDetected,
        }

        const int downloadingMaxNum = 10;
        const int failedTryAgain = 3;
        private readonly StateMachine<State, Event, UpdateStateMachine> _sm;
        private ResourcePackage _package;
        public ErrorType LastError { get; private set; }

        public string PackageVersion { get; private set; }

        //debug only
        public string ErrorMessage4Debug { get; private set; }

        private ResourceDownloaderOperation downloader;

        //PullUpdateInfomation成功后更新
        public int DownloadCount { get; private set; }

        //PullUpdateInfomation成功后更新
        public long TotalDownloadBytes { get; private set; }

        //DownloadingAssets时更新
        public int CurrentDownloadCount { get; private set; }

        //DownloadingAssets时更新
        public long CurrentDowloadBytes { get; private set; }

        //DownloadingAssets时更新
        public float Progress { get; private set; }

        //DownloadingAssets时更新，单位 bytes/s
        public long DownloadBytesSpeed { get; private set; }

        public bool IsMobileNetworkAllow { get; private set; }

        private readonly CancellationTokenSource _cts = new();

        public event Action<UpdateStateMachine, State> onStateChanged;
        public State MyState => this._sm.State;

        public UpdateStateMachine()
        {
            var builder = StateMachineBuilder<State, Event, UpdateStateMachine>.Creat();

            builder.AddState(State.NeverInit)
                .AddTransition(Event.Start, State.InitYoo);

            builder.AddState(State.InitYoo)
                .AddTransition(Event.Start, State.PullUpdateInformation)
                .OnEnter((_, _) => { OnInitYoo().Forget(); });

            builder.AddState(State.PullUpdateInformation)
                .AddTransition(Event.PullUpdateInfoOkAndNeedDownloading, State.WaitingDownloadConfirm)
                .AddTransition(Event.PullUpdateInfoOkAndNoDownloadNeed, State.Competed)
                .AddTransition(Event.AnyFail, State.UpdateFail)
                .OnEnter((machine, _) =>
                {
                    this.LastError = ErrorType.None;
                    this.PackageVersion = null;
                    this.ErrorMessage4Debug = null;
                    this.downloader = null;
                    this.DownloadCount = 0;
                    this.TotalDownloadBytes = 0;
                    this.CurrentDownloadCount = 0;
                    this.CurrentDowloadBytes = 0;
                    this.IsMobileNetworkAllow = false;
                    this.Progress = 0;
                    this.DownloadBytesSpeed = 0;
                })
                .OnEnter((machine, _) => this.OnEnterPullUpdateInformation().Forget());

            builder.AddState(State.UpdateFail)
                .AddTransition(Event.Retry, State.PullUpdateInformation);

            builder.AddState(State.WaitingDownloadConfirm)
                .AddTransition(Event.Start, State.DownloadingAssets);

            builder.AddState(State.DownloadingAssets)
                .AddTransition(Event.DownloadCompeted, State.Competed)
                .AddTransition(Event.MobileNetworkDetected, State.DownloadingPause)
                .OnEnter((machine, _) => this.OnEnterDownloadingAssets().Forget());

            builder.AddState(State.DownloadingPause)
                .AddTransition(Event.Start, State.DownloadingAssets)
                .OnEnter((machine, _) => this.OnEnterDownloadingPause().Forget());

            builder.AddState(State.Competed)
                .OnEnter((_, __) =>
                {
                    // 设置默认的资源包
                    var gamePackage = YooAssets.GetPackage(DefaultPackage);
                    YooAssets.SetDefaultPackage(gamePackage);
                });

            this._sm = builder.Build(State.NeverInit, this);
            this._sm.onStateChanged += (machine, v) => { this.onStateChanged?.Invoke(this, v); };

            //可以删除
            this._sm.Logger += v => { Debug.Log(v); };
        }

        private async UniTask OnInitYoo()
        {
            if (YooAssets.Initialized)
            {
                UnityEngine.Debug.Log($"[{nameof(UpdateStateMachine)}]YooAsset已初始化");
                var package1 = YooAssets.GetPackage(DefaultPackage);
                if (package1 == null) package1 = YooAssets.CreatePackage(DefaultPackage);
                _package = package1;
                _sm.Fire(Event.Start);
                return;
            }

            try
            {
                await YooHelper.InitYoo();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogWarning($"[{nameof(UpdateStateMachine)}]Yoo初始化失败");
                ErrorMessage4Debug = ex.Message;
                LastError = ErrorType.InitFail;
                _sm.Fire(Event.AnyFail);
                return;
            }

            this._package = YooAssets.GetPackage(DefaultPackage);
            if (IsDebug)
            {
                this._sm.Fire(Event.Start);
            }
        }

        private async UniTask OnEnterDownloadingPause()
        {
            while (true)
            {
                if (_cts.IsCancellationRequested) return;
                if (this.IsMobileNetworkAllow)
                {
                    this._sm.Fire(Event.Start);
                    return;
                }

                if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    this._sm.Fire(Event.Start);
                    return;
                }

                await UniTask.Yield();
            }
        }

        public void Start()
        {
            this._sm.Fire(Event.Start);
        }

        public void ConfirmDownload()
        {
            this._sm.Fire(Event.Start);
        }

        public void Retry()
        {
            this._sm.Fire(Event.Retry);
        }

        public void AllowMobileNetwork()
        {
            this.IsMobileNetworkAllow = true;
        }

        private async UniTask OnEnterDownloadingAssets()
        {
            var token = this._cts.Token;
            this.downloader.ResumeDownload(); //直接resume
            this.downloader.BeginDownload();
            try
            {
                var lastDownloaded = 0L;
                var st = new Stopwatch();
                st.Start();
                while (!this.downloader.IsDone)
                {
                    token.ThrowIfCancellationRequested();
                    CurrentDownloadCount = this.downloader.CurrentDownloadCount;
                    CurrentDowloadBytes = this.downloader.CurrentDownloadBytes;
                    this.Progress = this.downloader.Progress;
                    if (st.ElapsedMilliseconds > 1000)
                    {
                        var delta = this.CurrentDowloadBytes - lastDownloaded;
                        lastDownloaded = this.CurrentDowloadBytes;
                        this.DownloadBytesSpeed = delta;
                        st.Restart();
                    }

                    if (!this.IsMobileNetworkAllow)
                    {
                        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        {
                            this.downloader.PauseDownload();
                            LastError = ErrorType.MobileNetworkDetected;
                            this._sm.Fire(Event.DownloadingPaused);
                            return;
                        }
                    }

                    await UniTask.Yield();
                }

                if (this.downloader.Status != EOperationStatus.Succeed)
                {
                    Debug.LogWarning($"[{nameof(UpdateStateMachine)}]下载失败:{downloader.Error}");
                    this._sm.Fire(Event.AnyFail);
                    return;
                }

                await _package.ClearCacheFilesAsync(EFileClearMode.ClearUnusedBundleFiles);
                Debug.Log($"[{nameof(UpdateStateMachine)}]更新完成");
                this._sm.Fire(Event.DownloadCompeted);
            }
            finally
            {
                this.downloader.CancelDownload();
            }
        }

        private async UniTask OnEnterPullUpdateInformation()
        {
            //空包会报错
            //Debug.Log($"[{nameof(UpdateStateMachine)}]本地包版本：{package.GetPackageVersion()}");

            var package = this._package ?? throw new NullReferenceException("missing package");
            var updatePackageVersionOperation = package.RequestPackageVersionAsync(true, 15);
            await updatePackageVersionOperation;

            if (updatePackageVersionOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogWarning($"[{nameof(UpdateStateMachine)}]更新包版本失败:{updatePackageVersionOperation.Error}");
                this.ErrorMessage4Debug = updatePackageVersionOperation.Error;
                this.LastError = ErrorType.InitFail;
                this._sm.Fire(Event.AnyFail);
                return;
            }

            Debug.Log($"[{nameof(UpdateStateMachine)}]更新包版本成功,远程包：{updatePackageVersionOperation.PackageVersion}");
            this.PackageVersion = updatePackageVersionOperation.PackageVersion;

            var updatePackageManifestOperation =
                package.UpdatePackageManifestAsync(updatePackageVersionOperation.PackageVersion, 15);
            await updatePackageManifestOperation;
            if (updatePackageManifestOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogWarning($"[{nameof(UpdateStateMachine)}]更新包清单失败：{updatePackageManifestOperation.Error}");
                this.ErrorMessage4Debug = updatePackageManifestOperation.Error;
                this.LastError = ErrorType.InitFail;
                this._sm.Fire(Event.AnyFail);
                return;
            }

#if DEBUG
            Debug.Log($"[{nameof(UpdateStateMachine)}]更新包清单成功");
#endif
            downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            if (downloader.TotalDownloadCount > 0)
            {
                this.DownloadCount = downloader.TotalDownloadCount;
                this.TotalDownloadBytes = downloader.TotalDownloadBytes;

                //todo 检查磁盘空间是否足够  --需要额外的权限，还要适配不同设备，测试很麻烦，这里留个注释，以后有空了再完善它。
                this._sm.Fire(Event.PullUpdateInfoOkAndNeedDownloading);
            }
            else
            {
                this.downloader = null;
                this._sm.Fire(Event.PullUpdateInfoOkAndNoDownloadNeed);
            }
        }

        public void Dispose()
        {
            if (this._cts.IsCancellationRequested) return;
            this._cts.Cancel();
            this._cts.Dispose();
            this._sm?.Dispose();
        }
    }
}