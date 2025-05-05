using System;
using System.Collections.Generic;
using NexCore.DI;
using BoysheO.Protobuf.Pooled;
using Google.Protobuf.WellKnownTypes;
using Hotfix.ContentSystems;
using Hotfix.ContentSystems.LocalDbSystem;
using Hotfix.LanMgr;
using HotScripts.Hotfix.GamePlay.FrameworkSystems.LanSystem;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
#if ENABLE_STEAMSYSTEM
using SteamSystem;
using SteamSystem.Abstractions;
#endif
using Type = System.Type;

namespace Hotfix
{
    public sealed class ExtraDIContext : IOnDIContextBuildingCallback
    {
        public void OnCallback(IServiceCollection collection, IConfiguration configuration,
            IReadOnlyList<Type> allTypes)
        {
            ProtobufFactoryContext.UsePooledFactoryProvider(); //池化pb；需要在DI初始化完成前完成
            collection.AddSingleton<Empty>(new Empty());
            // collection.AddSingleton<IGrpcChannelProvider, CoreGrpcChannelProvider>();
#if ENABLE_STEAMSYSTEM
            collection.AddSingleton<SimSteamManager>(new SimSteamManager(2998610));
            collection.AddSingleton<ISteamManager>(v=>v.GetRequiredService<SimSteamManager>());
#endif
        }
    }
}