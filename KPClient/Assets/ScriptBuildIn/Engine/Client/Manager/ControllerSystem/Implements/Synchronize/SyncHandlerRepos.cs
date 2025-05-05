using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NexCore.DI;
using BoysheO.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Implements.Synchronize
{
    public sealed class SyncHandlerRepos : IOnDIContextBuildingCallback, ISyncHandlerRepos
    {
        private const bool LogAllTypesOnBuilding = false;
        private readonly Dictionary<string, Type> msgType2handlerTypes;
        private readonly IServiceProvider _provider;

        //作为IOnDIContextBuilding出现
        public SyncHandlerRepos()
        {
        }

        private SyncHandlerRepos(Dictionary<string, Type> msgType2HandlerTypes, IServiceProvider provider)
        {
            this.msgType2handlerTypes = msgType2HandlerTypes;
            _provider = provider;
        }


        void IOnDIContextBuildingCallback.OnCallback(IServiceCollection collection, IConfiguration configuration,
            IReadOnlyList<Type> allTypes)
        {
#if UNITY_EDITOR
            //Editor使用空白Repos以避免拖慢Editor刷新，由Editor那边的脚本负责注入
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif
            var dic = new Dictionary<string, Type>();
            for (int i = 0, count = allTypes.Count; i < count; i++)
            {
                var type = allTypes[i];
                if (!type.IsSealedAndImplement(typeof(ISyncHandler)))
                {
                    continue;
                }

                var atb = type.GetCustomAttribute<SubscribeMessageAttribute>();
                if (atb == null)
                {
                    Debug.Log(
                        $"The handle named {type.Name} is missing {nameof(SubscribeMessageAttribute)}.This handle will be ignore.");
                    continue;
                }

                if (!dic.TryAdd(atb.Message, type))
                {
                    // @formatter:off
                    throw new Exception($"Message global handler should exist only one.But multiple handlers are detected.Message type={atb.Message}");
                    // @formatter:on
                }

                collection.AddSingleton(type);
            }

            dic.TrimExcess();
            if (LogAllTypesOnBuilding)
            {
                var sb = new StringBuilder();
                foreach (var (key, value) in dic)
                {
                    sb.AppendLine($"Discover handle named {value.Name} observe {key}.");
                }

                Debug.Log($"[{nameof(SyncHandlerRepos)}]{sb}");
            }

            collection.AddSingleton<ISyncHandlerRepos>(v => new SyncHandlerRepos(dic, v));
        }

        public bool TryGetHandler(string msgType, out object handle)
        {
            var exist = msgType2handlerTypes.TryGetValue(msgType, out var type);
            if (!exist)
            {
                handle = default;
                return exist;
            }

            handle = _provider.GetRequiredService(type!);
            return true;
        }
    }
}