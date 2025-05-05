using System;

namespace UISystem.Abstractions
{
    /// <summary>
    /// 负责提供Loader实例。此Provider负责注入Loader上的待注入属性
    /// </summary>
    public interface IUILoaderProvider : IServiceProvider
    {
    }
}