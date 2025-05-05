using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NexCore.Toolkit;

namespace NexCore.DI
{
    /// <summary>
    /// 在<see cref="ServiceAttribute"/>注入流程后，开始执行所有IOnDIContextBuildingCallback以处理自定义的注入逻辑。
    /// note:为了便于维护，要求继承此接口的类统一命名为XXXOnDIContextBuildingCallback
    /// 
    /// Types implement this interface will be creat and invoke while DIContext building.
    /// This will call after <see cref="ServiceAttribute"/> logic done.
    /// </summary>
   [InheritedPreserve]
    public interface IOnDIContextBuildingCallback
    {
        void OnCallback(IServiceCollection collection, IConfiguration configuration, IReadOnlyList<Type> allTypes);
    }
}
