using System;
using System.Collections.Generic;
using NexCore.DI;
using Hotfix.ResourceMgr.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hotfix.ResourceMgr.Implements
{
    public sealed class YooAssetsResourceManager2OnDIContextBuildingCallback : IOnDIContextBuildingCallback
    {
        public void OnCallback(IServiceCollection collection, IConfiguration configuration,
            IReadOnlyList<Type> allTypes)
        {
            collection.TryAddSingleton<IResourceManager, YooAssetsResourceManager2>();
        }
    }
}