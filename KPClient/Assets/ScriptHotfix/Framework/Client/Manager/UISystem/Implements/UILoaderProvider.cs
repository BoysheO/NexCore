using System;
using System.Collections.Generic;
using System.Linq;
using NexCore.DI;
using BoysheO.Collection2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UISystem.Abstractions;

namespace UISystem.Implements
{
    [Service(typeof(IUILoaderProvider))]
    public sealed class UILoaderProvider : IUILoaderProvider
    {
        private readonly IGameServiceProvider _gameServiceProvider;

        public UILoaderProvider(IGameServiceProvider gameServiceProvider)
        {
            _gameServiceProvider = gameServiceProvider;
        }

        public object GetService(Type serviceType)
        {
            var ins = (IUILoader)_gameServiceProvider.ServiceProvider.GetRequiredService(serviceType);
            return ins;
        }

        private sealed class UILoaderProviderBuilder : IOnDIContextBuildingCallback
        {
            public void OnCallback(IServiceCollection collection, IConfiguration configuration,
                IReadOnlyList<Type> allTypes)
            {
                //register to collection
                for (var index = 0; index < allTypes.Count; index++)
                {
                    var type = allTypes[index];
                    if (!type.IsSealed) continue;
                    if (!type.GetInterfaces().Contains(typeof(IUILoader))) continue;
                    collection.AddSingleton(type);
                }
            }
        }
    }
}