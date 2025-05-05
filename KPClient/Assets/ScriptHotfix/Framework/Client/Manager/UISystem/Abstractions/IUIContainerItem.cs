using System.Collections.Generic;
using UnityEngine;

namespace UISystem.Abstractions
{
    //UI的生命周期如下
    //被创建，并临时挂载到UIContainer指定节点下->被添加到UIContainer管理(并挂载到真正的节点上)->入栈，处于置顶状态（覆盖底层UI）
    //（被新的UI覆盖）->不再处于置顶状态
    //（UI需要销毁）->出栈（不再显示）->被从UIContainer管理中移除（UIContainer最后会Destroy这个UI的GameObject）
    public interface IUIContainerItem
    {
        /// <summary>
        /// 提供GameObject以使得UIContainer可以进行节点操作<br />
        /// *如果没有提供GameObject，则相应的节点操作会被跳过
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// 此Object被哪个UIContainer管理
        /// *由Container注入，请勿替换该引用
        /// </summary>
        IUIContainer UIContainer { get; set; }

        /// <summary>
        /// 即将修改<see cref="GameObject"/>的节点位置
        /// </summary>
        void OnStackChanging(int idx, IReadOnlyList<IUIContainerItem> stack)
        {
            //参数 containList 可用于判断本UIContainerItem在栈中的具体位置和与特定UI的前后关系
        }

        /// <summary>
        /// 即将修改<see cref="GameObject"/>的节点位置
        /// </summary>
        void OnStackChanged(int idx, IReadOnlyList<IUIContainerItem> stack)
        {
            //参数 containList 可用于判断本UIContainerItem在栈中的具体位置和与特定UI的前后关系 
        }

        /// <summary>
        /// 被添加到Container里 这里可以初始化UI层逻辑
        /// </summary>
        void OnAddedInUIContainer()
        {
        }

        //通常，在这个事件发生后就进行Destroy销毁 销毁是UI自己启动的，容器不处理销毁问题
        void OnRemovedFromUIContainer()
        {
        }

        /// <summary>
        /// UI已被置顶<br />
        /// note:在<see cref="OnStackChanged"/>后发生
        /// 置顶后的UI通常是可见的，可以在这里恢复动画等消耗性能的UI层的逻辑
        /// </summary>
        void OnUIFocused()
        {
        }

        /// <summary>
        /// UI已被取消置顶<br />
        /// note:在<see cref="OnStackChanged"/>后发生
        /// 取消置顶后的UI通常是不可见的，可以在这里暂停动画等消耗性能的UI层的逻辑
        /// </summary>
        void OnUIUnFocused()
        {
        }

        /// <summary>
        /// UI将入栈<br/>
        /// *此后<see cref="GameObject"/>被设置为activity
        /// </summary>
        void OnUIPushing()
        {
        }

        /// <summary>
        /// UI已入栈<br/>
        /// *此时<see cref="GameObject"/>已被设置activity
        /// </summary>
        void OnUIPushed()
        {
        }

        /// <summary>
        /// UI将出栈 <br />
        /// *此后<see cref="GameObject"/>被设置为inactivity
        /// </summary>
        void OnUIPopping()
        {
        }

        /// <summary>
        /// UI已出栈<br/>
        /// *此时<see cref="GameObject"/>已被设置为inactivity
        /// </summary>
        void OnUIPopped()
        {
        }
    }
}