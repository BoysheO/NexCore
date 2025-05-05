using Microsoft.Extensions.Configuration;

namespace NexCore.DI
{
    /// <summary>
    /// 当Configuration构建时触发此回调
    /// 必须同时使用<see cref="ConfigPriorityAttribute"/>标注实现类的优先级，并且优先级是唯一的
    /// 注意:
    /// 1.继承此接口需要保证有无参构造函数，并且注意此时触发时流程上的DI容器应视作未被初始化，不要意外地调用DI容器
    /// 2.只有声明为sealed类的类型才会被实例化并执行此构造
    /// 3.为了便于维护，要求继承此接口的类统一命名为XXXOnConfigurationBuildingCallback
    /// 4.运行时AOT中的回调类记得加上[Preserve]，否则就被裁剪了
    /// </summary>
    public interface IOnConfigurationBuildingCallback
    {
        void OnCallback(IConfigurationBuilder builder);
    }
}