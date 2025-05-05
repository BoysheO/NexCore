using System;

namespace NexCore.DI
{
    /// <summary>
    /// 如果要通过构造注入的方式获取ServiceProvider，使用此接口而不是<see cref="IServiceProvider"/>，以防止以后可能存在的冲突
    /// </summary>
    public interface IGameServiceProvider
    {
        IServiceProvider ServiceProvider { get; }
    }
}