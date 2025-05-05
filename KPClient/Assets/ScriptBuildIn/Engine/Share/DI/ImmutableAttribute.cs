using System;

namespace NexCore.DI
{
    /// <summary>
    /// 标记一个类为不可变、多线程安全或无状态 用于后端构建容器时，根据manager上是否有ImmutableAttribute来决定将服务生命周期设置为全局，还是设置为跟随用户上下文
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ImmutableAttribute:Attribute
    {
        
    }
}