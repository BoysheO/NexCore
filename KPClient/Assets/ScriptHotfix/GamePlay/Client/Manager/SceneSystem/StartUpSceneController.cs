// #define ENABLENABLE_STEAMSYSTEM

using System;
using Cysharp.Threading.Tasks;
using Hotfix.FrameworkSystems.GameManagerSystem;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
using ScriptGamePlay.Hotfix.ClientCode.Script.Handler;
using UISystem;
using UnityEngine;

namespace ScriptGamePlay.Hotfix.ClientCode.Manager.SceneSystem
{
    public class StartUpSceneController : MonoBehaviour
    {
        private UIManager _uiManager;
        private GameManager _gameManager;

        private void Awake()
        {
            Debug.Log($"[{nameof(StartUpSceneController)}]hotfix run ok");
            //因为热更里面没有什么逻辑，这里特意触发一下gameManager的初始化，防止初学者忘记这一机制
            _gameManager = DIContext.ServiceProvider.GetRequiredService<GameManager>();
            _ = _gameManager.Root;
            _uiManager = DIContext.ServiceProvider.GetRequiredService<UIManager>();
            _ = _uiManager.UIManagerRoot;
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            //Do your things
            var panel = await _uiManager.LoadUIAsync<UIHelloWorldPanel>();
            panel.Show();
            //以上两句也可写成一句
            //await _uiManager.ShowUIAsync<UIHelloWorldPanel>();
        }
    }
}