using System;

namespace NexCore.DI
{
    /// <summary>
    /// 优先权小的排前面，这意味着优先权大的覆盖优先权小的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConfigPriorityAttribute:System.Attribute
    {
        /// <summary>
        /// 配置序号
        /// 在整个前端范围内（UnityEditor、Runtime）的所有配置的这个序号须是唯一的
        /// </summary>
        public readonly int Priority;

        public ConfigPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}