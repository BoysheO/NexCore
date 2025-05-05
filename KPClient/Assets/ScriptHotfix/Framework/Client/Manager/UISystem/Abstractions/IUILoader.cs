#nullable enable
using System;
using System.Threading;
using NexCore.DI;
using NexCore.Toolkit;
using Cysharp.Threading.Tasks;
using NexCore.UnityEnvironment;

namespace UISystem.Abstractions
{
    /// <summary>
    /// 它的实现必须是class、new()。 全局单例
    /// 大部分UI加载使用无Loader的标准加载即可。
    /// 遇到需要在UI加载前进行某些动作（例如需要长时间网络加载等待），则应当实现自定义Loader
    /// </summary>
    [InheritedPreserve]
    public interface IUILoader
    {
        // /// <summary>
        // /// 由管理类负责注入的DI容器(弃用。使用DIContext直接获取容器是最佳实践，可以保证UnityEditor下能直接调试UI）
        // /// </summary>
        // [Inject]
        // IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// gameObject加载到场景中时，默认挂载到哪个Layer节点下（对于Editor编辑时，如果场景中已经给出了UI，则不遵循Layer挂载规则，而是按GameObject原样提供）
        /// Layer是一种预置的UI容器，如果从UIManager加载UI，则使用此Layer所确定的预置容器
        /// 加载时可以无视此值指定Container，此时此值无意义（除非有特殊逻辑）
        /// </summary>
        UILayer Layer { get; }
        
       
        /// <summary>
        /// 1.如果UI需要接受长驻或需要记忆参数打开的，例如记忆了上次关闭背包在“武器标签”分页，再次开启背包也要从“武器标签”分页开始，则应将“武器标签”这个信息先设置到Loader上，UI里去读保存在Loader的信息。不要额外增加Manager类
        /// 2.加载UI设置UI层次这一套流程，不负责参数传递。要对UI脚本传入临时参数，应对返回的IView进行强制类型转换，例如IView转换成UIMainView，再调用它上面的成员来达成传参目的。
        /// 3.如果container为null，则LoadUIAsync实现应根据Loader的Layer信息选择容器
        /// 4.如果token为default，则使用默认使用<see cref="IUnityEnvironment.CancellationToken"/>
        /// * 默认加载流程可调用<see cref="UIManager.UILoaderLoadUIAsUsualAsync{T}"/>
        /// LoadUI的任务是请求网络，加载UI资源，挂载UI脚本，唤醒，实例化到Container里，以隐藏、容器置底状态返回
        /// 如果token返回取消信号，界面应按取消流程取消加载（包含网络资源），并抛出异常；同理，如果网络IO出现问题导致不能获取用户信息等，也应抛出异常。调用方负责处理这些异常
        /// </summary>
        UniTask<IView> LoadUIAsync(IUIContainer? containerOverrider = null, CancellationToken token = default);
    }
}