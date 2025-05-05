using UnityEngine.Scripting;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize
{
    /// <summary>
    /// 写死在代码里的消息处理器。每个消息只有一个或没有消息处理器。
    /// 做出一个处理器限制的目的是防止多处监听造成先后顺序混乱。
    /// 每个handler都应该是无状态的，因此不要在Handler内保存状态。要保存状态的话应该将状态传递给具体的某个Manager，由它来保存
    /// 要求实现类使用Sealed关键字，并且使用<see cref="SubscribeMessageAttribute"/>标注要监听的事件
    /// 如果是定义在AOT中，还需要记得添加<see cref="UnityEngine.Scripting.PreserveAttribute"/>，否则可能会被裁剪掉
    /// 命名规范：约定实现类以SH_开头命名
    /// </summary>
    public interface ISyncHandler
    {
        [Preserve]
        void Process(object? msg);
    }
}