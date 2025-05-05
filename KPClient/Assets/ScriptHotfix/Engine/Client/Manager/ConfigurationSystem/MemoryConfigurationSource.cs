using BoysheO.Extensions.Configuration.Reloadable.Memory;

namespace Engine.Client.Manager.ConfigurationSystem
{
    /// <summary>
    /// 提供内存层面上的AppSetting配置源
    /// 要对内存中的AppSetting进行修改的话，应调用此Provider修改上的方法修改；修改完毕调用Reload函数推送Reload事件
    /// </summary>
    public sealed class MemoryConfigurationSource : ReloadableMemoryConfigurationSource
    {
        public static MemoryConfigurationSource Instance { get; } = new();
        private MemoryConfigurationSource(){}
    }
}