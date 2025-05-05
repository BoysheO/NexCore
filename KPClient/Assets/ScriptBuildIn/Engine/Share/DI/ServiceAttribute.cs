using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Scripting;

namespace NexCore.DI
{
    /// <summary>
    /// 使用这个属性标注的类会被自动注入到DIContext容器中。不支持泛型类的注入。如有此类需求，使用<see cref="IOnDIContextBuildingCallback"/>来注入
    /// 
    /// the type with this attribute will be injected to DIContext while DIContext building.
    /// If more than one type claim they implemented the interface has injected,exception will throw
    /// Not support generice type .
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceAttribute : PreserveAttribute
    {
        /// <summary>
        /// Tell the DI what type the manager implement
        /// If it's null,it means same to self
        /// 如果为null，则代表与服务本体一致
        /// </summary>
        public readonly Type? ServiceType;

        public ServiceAttribute(Type interfaceType)
        {
            this.ServiceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
        }

        public ServiceAttribute()
        {
            ServiceType = null;
        }
    }
}