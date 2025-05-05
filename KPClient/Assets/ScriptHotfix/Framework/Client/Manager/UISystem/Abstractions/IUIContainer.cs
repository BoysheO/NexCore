using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NexCore.DI;
using UnityEngine;

namespace UISystem.Abstractions
{
    /// <summary>
    /// UI持有Handler以与容器进行交互
    /// 通常容器本身管理旗下UI实例化
    /// *原则上，容器内的UI不能访问容器内的另一个UI
    /// *容器会负责管理UI的GameObject的Activity状态，transform.parent状态
    /// *UI不应该访问其transform.parent的其他子节点，包括自身的idx位置。因为节点的顺序取决于容器实现且不统一
    /// *实现说明：既可以用MonoBehaviour的形式落地，也可以以抽象的方式使用
    /// </summary>
    public interface IUIContainer
    {
        [Inject] IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 提供临时节点，以挂载从资源管理Clone出来的预制体。这个临时节点根据具体实现，可以和正式节点是同一个，也可以是不同的一个，它的大小也是未定义的，所以不要做需要依赖这个属性的任何操作
        /// </summary>
        GameObject ParentForInstantiate { get; }

        /// <summary>
        /// 将UI添加到容器中，令uiObject的父容器设置为这个容器。
        /// 添加到容器后，此UI处于后台隐藏状态
        /// </summary>
        void Add(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 将UI从容器中移除。如果UI未出显示栈，则先出显示栈(如果是栈顶元素，会收到UnFocus通知)再移除
        /// 移除之后，此UI的父容器应为null，并且处于隐藏状态
        /// </summary>
        void Remove(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 获取当前UI容器内的在栈UI。其中，栈顶UI为列表最后一个UI
        /// </summary>
        void GetUIStack(IList<IUIContainerItem> buffer);

        /// <summary>
        /// 获取当前UI容器内的所有UI。UI顺序无意义
        /// </summary>
        void GetUIList(IList<IUIContainerItem> buffer);

        /// <summary>
        /// 将UI挂载到栈顶（不变更UI布局，须由UI自己在合适的时机自行处理）,并令GameObject->Activity
        /// 此后UI处于栈顶显示状态。上个未出栈的UI会收到UnFocus通知
        /// </summary>
        void PushUI(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 将UI出栈(即使不在栈顶），并令GameObject->Deactivate
        /// 此后UI处于后台隐藏状态。上个未出栈的UI会收到Focus通知
        /// </summary>
        void PopUI(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 这个ui是否在ui栈内并且置顶
        /// </summary>
        bool IsTop(IUIContainerItem item);

        /// <summary>
        /// UI栈置顶这个UI
        /// 此后这个UI会收到Focus通知，而之前置顶的UI（如果有）会收到UnFocus通知
        /// </summary>
        void Top(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 隐藏整个Container
        /// </summary>
        void Hide(
#if DEBUG
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 显示整个Container
        /// </summary>
        void Show(
#if DEBUG
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        );

        /// <summary>
        /// 清空Container内的所有UI(对所有UI进行Remove操作）
        /// </summary>
        void Clear();
    }
}