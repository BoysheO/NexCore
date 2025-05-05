using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoysheO.Extensions;
using BoysheO.TinyStateMachine;
using BoysheO.Util;
using Cysharp.Threading.Tasks;
using Framework.BuildIn.ClientCode.Loader;
using HybridCLR;
using Microsoft.Extensions.Configuration;
using ScriptBuildIn.Engine.Client.Configuration;
using ScriptEngine.BuildIn.ShareCode.Extensions;
using ScriptFramework.BuildIn.ClientCode.Manager.HotfixSystem;
using TMPro;
using UnityEngine;
using UpdateSystem.FixedLogic;
using UpdateSystem.Loader.Scripts;
using YooAsset;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;

namespace ScriptBuildIn.GamePlay.Client.UI
{
    /// <summary>
    /// 热更代码加载界面 写得垃圾，有空再改
    /// </summary>
    public class UIResourceLoadingCanvas : MonoBehaviour
    {
        //如果不热更加载代码则无需加载Patch
        private const bool IsSkipLoadPatch = BuildInConstConfig.IsSkipLoadPatch;
        [SerializeField] private UISlider01 barProgress;

        // [SerializeField] private Image img_back;

        [SerializeField] private TextMeshProUGUI txt_appVer;

        private UpdateStateMachine _sm;
        private UIMessageBox _uiMessageBox;
        private AssetHandle[] bgs;

        private enum State
        {
            NeverInit,
            UpdateAssets,
            LoadMetaAndDlls,
            EnterHotfix,
        }

        private enum Event
        {
            Start,
            UpdateCompeted,
            LoadMetaAndDllsCompeted,
        }

        private enum LoadScriptStep
        {
            PullUpdateInformation,
            DownloadingAssets, //empty
            LoadPatchesAssets,
            LoadPatchesInAsb,
            LoadDll,
            EnterHotfix,
        }

        private readonly Dictionary<LoadScriptStep, string> Step2Info = new()
        {
            { LoadScriptStep.PullUpdateInformation, "###Connecting server...".ToLan() },
            { LoadScriptStep.LoadPatchesAssets, $"###checking assets({(int)LoadScriptStep.LoadPatchesAssets})...".ToLan() },
            { LoadScriptStep.LoadPatchesInAsb, $"###checking assets({(int)LoadScriptStep.LoadPatchesInAsb})...".ToLan() },
            { LoadScriptStep.LoadDll, $"###checking assets({(int)LoadScriptStep.LoadDll})...".ToLan() },
            { LoadScriptStep.EnterHotfix, $"###entering the world...".ToLan() },
        };

        // private string[] SkipLoadMeta = new[] { "UniTask.patch" };
        private readonly List<string> SkipLoadMeta = new();

        //dll加载顺序
        // private readonly List<string> DLLLoadingOrder = new() { "Hotfix.dll", };

        private StateMachine<State, Event, UIResourceLoadingCanvas> _uiSm;

        private void Awake()
        {
            this._sm = new();

            var builder = StateMachineBuilder<State, Event, UIResourceLoadingCanvas>.Creat();

            builder.AddState(State.NeverInit)
                .AddTransition(Event.Start, State.UpdateAssets);

            builder.AddState(State.UpdateAssets)
                .AddTransition(Event.UpdateCompeted, State.LoadMetaAndDlls)
                .OnEnter((machine, _) =>
                {
                    machine.Context._sm.onStateChanged += machine.Context.On_Sm_OnonStateChanged;
                    machine.Context._sm.Start();
                });

            builder.AddState(State.LoadMetaAndDlls)
                .AddTransition(Event.LoadMetaAndDllsCompeted, State.EnterHotfix)
                .OnEnter((machine, _) => this.LoadMetaAndDlls().Forget());

            builder.AddState(State.EnterHotfix)
                .OnEnter((machine, __) => { this.OnEnterHotfix().Forget(); });

            this._uiSm = builder.Build(State.NeverInit, this);
#if DEBUG
            //可以删除
//            this._uiSm.Logger += v => { Debug.Log(v); };
#endif
        }

        private async UniTaskVoid OnEnterHotfix()
        {
            Debug.Log("OnEnterHotfix");
            var go = this.gameObject;
            UpdateProgress(LoadScriptStep.EnterHotfix, 0);
            
            //代码已经加载完了，接下来交给LoadScene加载其他东西，例如初始化表管理器
            var hotfixMgr = HotfixManager.Instance;
            hotfixMgr.ReloadDI();
            await hotfixMgr.StartUp(new Progress<float>(v =>
            {
                if (go == null) return;
                UpdateProgress(LoadScriptStep.EnterHotfix, v);
            }));
            //加载完LoadScene之后就是由场景中的脚本接管了
        }

        private void Start()
        {
            txt_appVer.text = $"Ver:{BuildInConstConfig.FrameworkBuildInVer}";
            SetupBackgroundUpdateRegularly();
            this._uiSm.Fire(Event.Start);
        }

        private void SetupBackgroundUpdateRegularly()
        {
            // this.bgs = new AssetHandle[3];
            //
            // this.bgs[0] = YooAssets.LoadAssetAsync<Sprite>("Loading screen1.png");
            // this.bgs[1] = YooAssets.LoadAssetAsync<Sprite>("Loading screen2.png");
            // this.bgs[2] = YooAssets.LoadAssetAsync<Sprite>("Loading screen3.png");
            // UpdateBackgroundRegularly().Forget();
        }

        private void On_Sm_OnonStateChanged(UpdateStateMachine machine, UpdateStateMachine.State state)
        {
            if (state != machine.MyState) return;
            switch (state)
            {
                case UpdateStateMachine.State.NeverInit:
                    break;
                case UpdateStateMachine.State.PullUpdateInformation:
                    UpdateProgress(LoadScriptStep.PullUpdateInformation, 0);
                    break;
                case UpdateStateMachine.State.WaitingDownloadConfirm:
                {
                    var totalBytes = this._sm.TotalDownloadBytes;
                    (double size, StringUtil.CapacityUnit capacityUnit) = StringUtil.BytesToReadableSize(totalBytes);
                    OpenMsgBox($"You need to download an update package that is {size:F1}{capacityUnit} in size.", "Ok",
                            "Exit")
                        .ContinueWith(v =>
                        {
                            if (v == UIMessageBox.Result.Ok)
                            {
                                this._sm.ConfirmDownload();
                            }
                            else
                            {
                                Application.Quit();
#if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
#endif
                            }
                        }).Forget();
                    break;
                }

                case UpdateStateMachine.State.DownloadingAssets:
                {
                    UpdateRegualry().Forget();

                    async UniTask UpdateRegualry()
                    {
                        var go = this.gameObject;
                        while (this._sm.MyState == UpdateStateMachine.State.DownloadingAssets)
                        {
                            var total = this._sm.TotalDownloadBytes;
                            (double totalSize, StringUtil.CapacityUnit totalUnit) =
                                StringUtil.BytesToReadableSize(total);
                            var inProgress = this._sm.CurrentDowloadBytes;
                            (double inProgressSize, StringUtil.CapacityUnit inPUnit) =
                                StringUtil.BytesToReadableSize(inProgress);
                            var speed = this._sm.DownloadBytesSpeed;
                            (double speed2, StringUtil.CapacityUnit speedUnit) = StringUtil.BytesToReadableSize(speed);
                            barProgress.Title =
                                $"downloading [{inProgressSize:F1}{inPUnit}/{totalSize:F1}{totalUnit}]{speed2:F1}{speedUnit}/s";
                            this.barProgress.Progress = this._sm.Progress;
                            await UniTask.Yield();
                            if (go == null) return;
                        }
                    }

                    break;
                }
                case UpdateStateMachine.State.DownloadingPause:
                {
                    if (this._sm.LastError == UpdateStateMachine.ErrorType.MobileNetworkDetected &&
                        !this._sm.IsMobileNetworkAllow)
                    {
                        OpenMsgBox("Currently not on a Wi-Fi network. Do you want to continue downloading?", "yes",
                                "no")
                            .ContinueWith(v =>
                            {
                                if (v == UIMessageBox.Result.No)
                                {
                                    Application.Quit();
#if UNITY_EDITOR
                                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                                }
                                else
                                {
                                    this._sm.AllowMobileNetwork();
                                    this._sm.ConfirmDownload();
                                }
                            }).Forget();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    break;
                }
                case UpdateStateMachine.State.Competed:
                {
                    Debug.Log("更新完成！");
                    this._uiSm.Fire(Event.UpdateCompeted);
                    break;
                }
                case UpdateStateMachine.State.UpdateFail:
                {
                    OpenMsgBox("Update fail", "Retry", "Exit")
                        .ContinueWith(v =>
                        {
                            if (v == UIMessageBox.Result.Ok)
                            {
                                this._sm.Retry();
                            }
                            else
                            {
                                Application.Quit();
#if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
#endif
                            }
                        }).Forget();
                    break;
                }
                case UpdateStateMachine.State.InitYoo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private async UniTask<HotfixListModel> LoadHotfixList()
        {
            _ = DIContext.Configuration;
            Debug.Log(DIContext.GetConfigDebugView());
            var model = DIContext.Configuration.Get<HotfixListModel>().ThrowIfNull();
            if (model.HotfixList == null || model.HotfixList.Length == 0) throw new Exception("no hotfix list");
            return model;
        }

        async UniTask LoadMetaAndDlls()
        {
            if (BuildInConstConfig.LoadMode ==
                BuildInConstConfig.LOADMODE_ASSET)
            {
                if (!YooAssets.Initialized) throw new Exception("YooAsset没初始化。");
                await this.LoadPatches();

                #region 加载dll

                {
                    UpdateProgress(LoadScriptStep.LoadDll, 0);
                    var dllInfos = YooAssets.GetAssetInfos("Dll");
                    if (dllInfos.Length == 0) throw new Exception("YooAsset没配置");

                    var pdbInfos = YooAssets.GetAssetInfos("Pdb");
                    var lst = await LoadHotfixList();
                    var totalJobs = lst.HotfixList.Length;
                    var jobsDone = 0;
                    foreach (var dll in lst.HotfixList)
                    {
                        var dllInfo = dllInfos.FirstOrDefault(v =>
                            v.AssetPath.AsPath().GetFileNameWithoutExt().AsPath().GetFileNameWithoutExt() == dll);
                        if (dllInfo == null) throw new Exception($"missing dll {dll}");
                        using var ass = YooAssets.LoadAssetAsync<TextAsset>(dllInfo.AssetPath);
                        await ass;
                        var asemblyBytes = ass.GetAssetObject<TextAsset>().bytes;

                        byte[]? pdbBytes = null;
                        {
                            var pdbInfo = pdbInfos.FirstOrDefault(v =>
                                v.AssetPath.AsPath().GetFileNameWithoutExt().AsPath().GetFileNameWithoutExt() == dll);
                            if (pdbInfo != null)
                            {
                                using var pdbhandle = YooAssets.LoadAssetAsync<TextAsset>(pdbInfo.AssetPath);
                                await pdbhandle;
                                pdbBytes = pdbhandle.GetAssetObject<TextAsset>().bytes;
                            }
                        }

                        if (BuildInConstConfig.AssemblyLoad == BuildInConstConfig.ASSEMBLYLOAD_SKIP)
                        {
                            //skip
                            //do nothing
                            Debug.Log($"由于 {nameof(BuildInConstConfig.ASSEMBLYLOAD_SKIP)} ，跳过Assembly.Load({dll})");
                        }
                        else if (BuildInConstConfig.AssemblyLoad == BuildInConstConfig.ASSEMBLYLOAD_NOSKIP)
                        {
                            if (pdbBytes == null)
                            {
                                Assembly.Load(asemblyBytes);
                            }
                            else
                            {
                                Assembly.Load(asemblyBytes, pdbBytes);
                            }
                        }

                        jobsDone++;
                        UpdateProgress(LoadScriptStep.LoadDll, jobsDone * 1f / totalJobs);
                    }
                }

                #endregion
            }
            else if (BuildInConstConfig.LoadMode == BuildInConstConfig.LOADMODE_INTERNAL)
            {
                //do nothing
            }

            else if (BuildInConstConfig.LoadMode == BuildInConstConfig.LOADMODE_IO)
            {
                //patch还是要加载的
                await this.LoadPatches();
                UpdateProgress(LoadScriptStep.LoadDll, 0);
                var lst = await LoadHotfixList();
                foreach (string s in lst.HotfixList)
                {
                    var appSetting = DIContext.Configuration.Get<AppSettingModel>()!;
                    var loadPath = appSetting.PatchDir.ThrowIfNullOrWhiteSpace();
                    if (loadPath.IsNullOrWhiteSpace()) throw new NullReferenceException("missing appsetting PatchDir");

                    var dllPath = appSetting.DllDir;
                    if (dllPath.IsNullOrWhiteSpace()) throw new NullReferenceException("missing appsetting DllDir");
                    var dllBytes = await DownloadHelper.Download($"{dllPath}/{s}.dll");
                    byte[]? pdb = null;
                    try
                    {
                        pdb = await DownloadHelper.Download($"{dllPath}/{s}.pdb");
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                    if (BuildInConstConfig.AssemblyLoad == BuildInConstConfig.ASSEMBLYLOAD_NOSKIP)
                    {
                        if (pdb != null) Assembly.Load(dllBytes, pdb);
                        else Assembly.Load(dllBytes);
                    }
                    else if (BuildInConstConfig.AssemblyLoad == BuildInConstConfig.ASSEMBLYLOAD_SKIP)
                    {
                        //do nothing
                        Debug.Log($"由于 ASSEMBLYLOAD_SKIP ，跳过Assembly.Load({s})");
                    }
                }

                UpdateProgress(LoadScriptStep.LoadDll, 1);
            }
            else throw new NotImplementedException($"load mode = {BuildInConstConfig.LoadMode}");

            this._uiSm.Fire(Event.LoadMetaAndDllsCompeted);
        }

        //下载资源时的ui显示是由函数直接操作的，不使用此api
        private void UpdateProgress(LoadScriptStep step, float progress)
        {
            this.barProgress.Title = Step2Info[step];
            this.barProgress.Progress = progress;
        }

        private async UniTask LoadPatches()
        {
            if (IsSkipLoadPatch) return;
            UpdateProgress(LoadScriptStep.LoadPatchesAssets, 0);
            var patchesInfos = YooAssets.GetAssetInfos("Patch");
            if (patchesInfos.Length == 0)
            {
                throw new Exception("YooAsset没配置Patch或没有Patch(至少有一个patch");
            }

            using var patchesHandle = YooAssets.LoadAllAssetsAsync<TextAsset>(patchesInfos[0].AssetPath);
            while (!patchesHandle.IsDone)
            {
                UpdateProgress(LoadScriptStep.LoadPatchesAssets, patchesHandle.Progress);
                await UniTask.Yield();
            }

            var totalJobs = patchesHandle.AllAssetObjects.Count;
            var jobsDone = 0;
            foreach (var ass in patchesHandle.AllAssetObjects.Cast<TextAsset>())
            {
                if (this.SkipLoadMeta.Contains(ass.name))
                {
                    //skip
                    Debug.Log($"skip meta:{ass.name}");
                }
                else
                {
                    var er = RuntimeApi.LoadMetadataForAOTAssembly(ass.bytes, HomologousImageMode.SuperSet);
                    if (er != LoadImageErrorCode.OK)
                    {
                        Debug.LogError($"patch with er:{er}");
                    }
                }

                jobsDone++;
                UpdateProgress(LoadScriptStep.LoadPatchesAssets, jobsDone * 1f / totalJobs);
                await UniTask.Yield();
            }
        }

        private UniTask<UIMessageBox.Result> OpenMsgBox(string content, string yes, string no)
        {
            var prefab = Resources.Load<GameObject>("Prefab/UI/UIMessageBoxPanel");
            // var handle = YooAssets.LoadAssetAsync<GameObject>("Assets/UpdateSystem/Loader/PackedAssets/UIMessageBox/UIMessageBox.prefab");
            // handle.WaitForAsyncComplete();
            var uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null) throw new Exception("misisng uicanvas");
            var go = Instantiate(prefab, uiCanvas.transform);
            // var go = handle.InstantiateSync(uiCanvas.transform);
            var box = go.GetComponent<UIMessageBox>();
            return box.Go(content, yes, no);
        }

        private async UniTask UpdateBackgroundRegularly()
        {
            //这里先阉割掉
            return;
            // var go = this.gameObject;
            // int bgIdx = 0;
            // while (true)
            // {
            //     await UniTask.Delay(3000);
            //     if (go == null) return;
            //     this.img_back.CrossFadeColor(Color.black, 0.5f, false, false);
            //     await UniTask.Delay(500);
            //     if (go == null) return;
            //     bgIdx++;
            //     if (bgIdx >= this.bgs.Length)
            //     {
            //         bgIdx = 0;
            //     }
            //
            //     this.img_back.sprite = this.bgs[bgIdx].GetAssetObject<Sprite>();
            //     this.img_back.CrossFadeColor(Color.white, 0.5f, false, false);
            //     await UniTask.Delay(500);
            //     if (go == null) return;
            // }
        }

        private void OnDestroy()
        {
            this._sm.Dispose();
            this._uiSm.Dispose();
            if (this.bgs != null)
            {
                // foreach (Sprite sprite in this.bgs)
                // {
                //     if (sprite != null) Resources.UnloadAsset(sprite);
                // }
                foreach (var assetHandle in this.bgs)
                {
                    assetHandle.Dispose();
                }
            }
        }
    }
}