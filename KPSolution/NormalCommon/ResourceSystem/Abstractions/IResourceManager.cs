using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hotfix.ResourceMgr.Abstractions
{
    /// <summary>
    /// 对Scene资源处理的约定(使用类型SceneInstance)：
    /// Scene加载时，不中断现在的场景。调用SceneInstance.ActivityAsync之后，才显示场景。显示新场景不会卸载老场景
    /// 加载后，token不再有用，应当立即释放
    /// 卸载场景仍使用原生的ScreenManager卸载
    /// *原则上应该支持对Scene资源的Wait操作，实际上由于没时间去落地，所以目前不能wait场景资源，否则直接卡死
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// load resource async.
        /// 创建加载资源的请求.这个API应该叫做CreatRequest或BeginLoad比较合适。偷懒不重名了
        /// </summary>
        RequestToken LoadAsync(ResourceKey key,CancellationToken token = default);

        /// <summary>
        /// get the requesting resource if possible or throw exception
        /// 在资源请求完成后，可调用此获得资源
        /// </summary>
        T GetResource<T>(RequestToken token);

        /// <summary>
        /// get the requesting progress percent or throw <see cref="System.OperationCanceledException"/>.
        /// 如果加载过程中发生了任何错误，则返回进度1，但是GetResource的时候会抛异常。总之除了CancelException外，完成或者失败都返回1
        /// </summary>
        float GetProgress(RequestToken token);

        /// <summary>
        /// get the key witch the requestId created by
        /// </summary>
        ResourceKey GetKey(RequestToken token);

        /// <summary>
        /// block the thread and wait for request done
        /// *warning* this will block the thread,suggest check progress and wait in async way
        /// </summary>
        void Wait(RequestToken token);

        /// <summary>
        /// dispose request
        /// </summary>
        void Release(RequestToken token);
    }
}