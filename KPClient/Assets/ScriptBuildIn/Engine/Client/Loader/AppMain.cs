using System;
using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using IngameDebugConsole;
using Microsoft.Extensions.DependencyInjection;
using ScriptBuildIn.GamePlay.Client.UI;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
using ScriptFramework.BuildIn.ClientCode;
using ScriptFramework.BuildIn.ClientCode.Manager.HotfixSystem;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Scripting;
using UpdateSystem.FixedLogic;
using UpdateSystem.Loader.Scripts;
using YooAsset;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
#pragma warning disable CS0162 // Unreachable code detected

[Preserve]
internal class AppMain : MonoBehaviour
{
    private const bool IsSkipLogoStep = BuildInConstConfig.IsSkipLogoStep;

    private const bool IsSkipCodeLauncher = BuildInConstConfig.LoadMode == BuildInConstConfig.LOADMODE_INTERNAL &&
                                            BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_EDITOR;

    private const bool IsShowLogConsole = BuildInConstConfig.IsShowLogConsole;

    [SerializeField] private UILogoPanel _panel;

    // ReSharper disable once Unity.IncorrectMethodSignature
    private async UniTaskVoid Start()
    {
        Debug.Log($"[{nameof(BuildInConstConfig.FrameworkBuildInVer)}]{BuildInConstConfig.FrameworkBuildInVer}");
        //等待一帧是因为这一帧AppMain刚Start，其他组件还没有Start。等待一帧让其他组件Start完
        await UniTask.Yield();
        if (!IsShowLogConsole)
        {
            DebugLogManager.Instance.gameObject.Hide();
        }

        if (!IsSkipLogoStep)
        {
            await _panel.FadeIn();
        }

        await HotfixManager.Instance.LoadConfiguration();

        if (!IsSkipCodeLauncher)
        {
            await HotfixManager.Instance.StartCodeUpdateWithUI();
            Destroy(gameObject); //销毁本节点和LogoUI，释放内存
            //运行到这里，控制权就等于交给UICodeLoadingPanel了
            //在后面的流程中，会切换场景，自动销毁本场景的LogoPanel
        }
        else
        {
            Profiler.BeginSample("UpdateStateMachine");
            var updateMachine = new UpdateStateMachine();
            updateMachine.Start();
            Profiler.EndSample();
            //在满足 LOADMODE_INTERNAL + YOOMODE_EDITOR的情况下，直接等待初始化完成即可
            await UniTask.WaitUntil(() =>
            {
                Profiler.BeginSample("Wait competed");
                return updateMachine.MyState == UpdateStateMachine.State.Competed;
                Profiler.EndSample();
            });
            //跳过热更直接加载
            await HotfixManager.Instance.StartUp(new Progress<float>());
            //场景加载完成后会自动销毁本场景资源
        }
    }
}