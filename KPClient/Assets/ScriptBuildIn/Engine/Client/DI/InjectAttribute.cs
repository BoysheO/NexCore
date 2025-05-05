using System;
using System.Reflection;
using UnityEngine;

namespace NexCore.DI
{
    /// <summary>
    /// 标记这个字段/属性由外部进行注入管理以提醒开发者不要手动设置这个值（但是注入逻辑落地是由各个System负责的，需要记得维护）
    /// 在有可能出现性能热点的地方不要使用注入方案
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class InjectAttribute : System.Attribute
    {
        //对被Inject标记的属性进行注入。注入要求该obj为sealed，以防止出现继承问题
        public static void Inject(IServiceProvider serviceProvider, object obj)
        {
            var type = obj.GetType();
            if (!type.IsSealed)
            {
                Debug.Log($"只有sealed类可以注入,obj={obj.GetType().Name}");
                return;
            }
            var properties = type.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.GetCustomAttribute(typeof(InjectAttribute)) != null)
                {
                    //在本游戏架构中，前端只存在单例注入，不存在其他生命周期的注入服务，因此不必考虑obj的生命周期问题
                    var ins = serviceProvider.GetService(propertyInfo.PropertyType);
                    propertyInfo.SetValue(obj, ins);
                }
            }
        }
    }
}