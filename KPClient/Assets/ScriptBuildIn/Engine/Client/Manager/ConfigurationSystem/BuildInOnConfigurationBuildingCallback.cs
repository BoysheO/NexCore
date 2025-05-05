using Microsoft.Extensions.Configuration;
using NexCore.DI;
using ScriptBuildIn.Engine.Client.Configuration;
using UnityEngine.Scripting;

namespace ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem
{
    [Preserve]
    [ConfigPriority(1)]
    public sealed class BuildInOnConfigurationBuildingCallback : IOnConfigurationBuildingCallback
    {
        public void OnCallback(IConfigurationBuilder builder)
        {
#if UNITY_EDITOR
            const bool IsUnityEditor = true;
#else
            const bool IsUnityEditor = false;
#endif
            //UnityEditor会使用UnityEditor的ClientInfoConfigurationSource
            if (!IsUnityEditor && BuildInConstConfig.IsUseGitBranchInfo)
            {
                builder.Add(new BranchInfoConfigurationSource());
            }

            builder.Add(new AppVerInfoConfigurationSource());
            builder.Add(new ResourceConfigurationSource());
            builder.Add(StreamingAssetsConfigurationSource.Instance);
        }
    }
}