using System;
using System.Collections.Generic;
using System.Linq;
using NexCore.DI;
using Hotfix.ResourceMgr.Abstractions;
using Hotfix.ResourceMgr.Implements.Editor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnityEditor;


namespace Hotfix.ResourceMgr.Implements
{
    public sealed class EditorBaseResourceManagerOnDiContextBuildingCallback : IOnDIContextBuildingCallback
    {
        public void OnCallback(IServiceCollection collection, IConfiguration configuration,
            IReadOnlyList<Type> allTypes)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) collection.AddSingleton<IResourceManager, EditorBaseResourceManager>();
        }
    }
}