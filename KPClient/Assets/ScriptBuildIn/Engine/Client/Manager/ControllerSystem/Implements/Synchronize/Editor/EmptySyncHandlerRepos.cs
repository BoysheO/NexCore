using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NexCore.DI;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
using UnityEditor;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Implements.Synchronize.Editor
{
    public sealed class EmptySyncHandlerRepos : ISyncHandlerRepos
    {
        public bool TryGetHandler(string msgType, out object handle)
        {
            handle = default;
            return false;
        }

        private sealed class EmptySyncHandlerReposOnDiContextBuildingCallback : IOnDIContextBuildingCallback
        {
            public void OnCallback(IServiceCollection collection, IConfiguration configuration,
                IReadOnlyList<Type> allTypes)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    collection.AddSingleton<ISyncHandlerRepos, EmptySyncHandlerRepos>();
                }
            }
        }
    }
}