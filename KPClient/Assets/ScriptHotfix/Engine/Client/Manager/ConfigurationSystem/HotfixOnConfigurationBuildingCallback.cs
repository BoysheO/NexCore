using Microsoft.Extensions.Configuration;
using NexCore.DI;
using ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem;
using UnityEngine.Scripting;

namespace Engine.Client.Manager.ConfigurationSystem
{
    [Preserve]
    [ConfigPriority(2)]
    public sealed class HotfixOnConfigurationBuildingCallback : IOnConfigurationBuildingCallback
    {
        public void OnCallback(IConfigurationBuilder builder)
        {
            builder.Add(YooAssetsConfigurationSource.Instance);
            builder.Add(MemoryConfigurationSource.Instance);
        }
    }
}