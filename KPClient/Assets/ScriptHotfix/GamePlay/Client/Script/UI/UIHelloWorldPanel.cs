using System;
using System.Linq;
using BoysheO.Extensions;
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Engine.Client.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Hotfix.UIScripts.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptHotfix.GamePlay.Share.Manager.ExampleSystem;
using TMPro;
using UISystem.Abstractions;
using UnityEngine;

[UILayer(UILayer.Panel)]
public sealed class UIHelloWorldPanel : UIBase
{
    [SerializeField] private TextMeshProUGUI txt_data;
    private HelloWorldManager helloWorldManager;

    private void Awake()
    {
        helloWorldManager = DIContext.ServiceProvider.GetRequiredService<HelloWorldManager>();
    }

    // ReSharper disable once Unity.IncorrectMethodSignature
    async UniTaskVoid Start()
    {
        Debug.Log("Hello world!");
        ShowTimeRegular().Forget();
        ShowRandomNumber().Forget();
    }

    private async UniTaskVoid ShowTimeRegular()
    {
        var go = gameObject;
        do
        {
            var data = DateTimeOffset.Now.ToString();
            txt_data.text = data;
            await 1f; //等1秒
        } while (go);
    }

    private async UniTaskVoid ShowRandomNumber()
    {
        var asm = typeof(YetAnotherHttpHandler).Assembly.FullName;
        Debug.Log(asm);
        var go = gameObject;
        var url = DIContext.Configuration.GetValue<string>("HelloWorldService").ThrowIfNullOrWhiteSpace();
        Debug.Log($"HelloWorldService's url = {url}");
        using var channel = GrpcChannel.ForAddress(url,new GrpcChannelOptions()
        {
            HttpHandler = new YetAnotherHttpHandler()
            {
                Http2Only = true
            },
            DisposeHttpClient = true,
        });
        var client = new HelloWorld.HelloWorld.HelloWorldClient(channel);
        var data = await client.GetInitDataAsync(new Empty());
        helloWorldManager.Init(data.InitData.ToArray());
        while (go)
        {
            var localNext = helloWorldManager.GetNext();
            bool isSuccess;
            do
            {
                try
                {
                    var remoteRet = await client.GetRandomAsync(new Empty());
                    var remoteNext = remoteRet.RandomVal;
                    Debug.Log($"local next={localNext},remote next = {remoteNext}");
                    isSuccess = true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    isSuccess = false;
                }

                await 1f;
            } while (go && !isSuccess);
        }
    }
}