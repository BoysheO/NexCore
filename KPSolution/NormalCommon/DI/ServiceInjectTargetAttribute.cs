using System;
using System.Runtime.CompilerServices;

namespace BearMonoScript.DI
{
    /// <summary>
    /// The property with this attribute will be inject value on DIInject called
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public sealed class ServiceInjectTargetAttribute : Attribute
    {
        //暂时性关闭以下代码，避免de识别添加括号        
        // #if UNITY_EDITOR
        //         public string FilePath;
        //         public ServiceInjectAttribute([CallerFilePath] string filePath = "")
        //         {
        //             FilePath = filePath;
        //         }
        // #endif
    }
}