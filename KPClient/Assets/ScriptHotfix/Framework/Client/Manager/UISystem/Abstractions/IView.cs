#nullable enable
using System;
using NexCore.DI;
using Cysharp.Threading.Tasks;

namespace UISystem.Abstractions
{
    /// <summary>
    /// 它应由MonoBehaviour实现。并且使用Container设计模型
    /// 设计是这样的：UI作为Container中的成员出现，每个Container拥有自己的UI栈，UI充满整个Container，并且UI的显隐不影响容器外部显示状态。
    /// 加载生命周期如下：
    /// 加载预制体（无脚本）
    /// 实例化(克隆)，隐藏，挂载UI脚本
    /// 注入属性
    /// 显示（Awake,OnEnable)，隐藏(OnDisable)(注意，此过程中，Start函数不会被调用）
    /// 注入Container，并且添加到Container中(<see cref="IUIContainerItem.OnAddedInUIContainer"/>)
    /// 返回对象给UI请求者(此时，UI在容器中处于后台隐藏状态）
    /// </summary>
    public interface IView : IUIContainerItem
    {
        /// <summary>
        /// 方便取得View的Loader的信息,例如ui的层
        /// 当使用无loader流程加载UI时，Loader为null
        /// 由框架管理
        /// </summary>
        IUILoader? Loader { get; set; }

        /// <summary>
        /// 在加载完毕后,会先调用Awake(UI的Awake函数总是保证第一个触发且触发一次）
        /// 然后隐藏(Awake执行完就隐藏，不会触发Start；但在这个过程中，OnEnable，OnDisable会被触发）
        /// Awake时，Container为null，此时未置入容器中
        /// 然后调用此函数
        /// 限定由Loader调用
        /// 如业务流程要中断加载，请抛出异常<see cref="OperationCanceledException"/>。
        /// </summary>
        UniTask OnUILoaded();
        
        [Obsolete("仅兼容老ui使用")]
        public void ShowFront()
        {
            UIContainer.PushUI(this);
        }
    }
}