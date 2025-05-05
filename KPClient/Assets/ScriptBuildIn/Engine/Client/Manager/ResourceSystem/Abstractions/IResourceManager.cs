#nullable enable
using System;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections;


namespace Hotfix.ResourceMgr.Abstractions
{
    /// <summary>
    /// todo：原则上应该支持对Scene资源的Wait操作，实际上由于没时间去落地，所以目前不能wait场景资源，否则直接卡死
    /// 以下是关于资源管理通用接口的一些思考：
    /// 从自己撸到addressable切换到yooAsset，每次做资源架构都会有一些不一样的设计思路，有一些独特的机制
    /// 所以一个项目如果到了要换资源管理组件的时候，就算抽象出通用接口终归也是要重构的。
    /// 所以通用接口的意义其实并没有那么的大。建议开发者在业务中多多考虑直接使用YooAssets、Addressable这一层的API，而不是使用IResourceManager
    /// 理论上如果一个资源需要前后端都要加载，那么这个情况才是适合使用IResourceManager的情况。但是实践中这这种情况就不应该发生
    /// 后端用到的加载逻辑无非就是策划配置数据（表），自定义的json文件（例如技能数据）等。这种需求更应该将资源加载逻辑定义到具体管理类的抽象接口，而不是在IResourceManager上抽象。
    /// 参考我对ITableManager在前后端的实现，可见后端的ITableManager都不走IResourceManager的。因为表数据可以从cdn、数据库、本地IO等各种丰富的
    /// 途径方式获取，它还可以有在线更表的需求，这些都是不适合抽象成通用资源管理的。
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// load resource async.
        /// 创建加载资源的请求.这个API应该叫做CreatRequest或BeginLoad比较合适。偷懒不重名了
        /// 加载场景指定使用<see cref="UnityEngine.SceneManagement.Scene"/>类型
        /// </summary>
        ResourceId BeginLoad(ResourceKey key, CancellationToken token);

        /// <summary>
        /// 判断是否有这个资源
        /// </summary>
        bool IsContains(ResourceKey key);

        /// <summary>
        /// *对于未被管理的Token，也当成Disposed处理
        /// </summary>
        bool IsDisposed(ResourceId token);

        /// <summary>
        /// get the requesting resource if possible or throw exception
        /// 在资源请求完成后，可调用此获得资源
        /// </summary>
        T GetResource<T>(ResourceId token);

        /// <summary>
        /// get the requesting progress percent or throw <see cref="System.OperationCanceledException"/>.
        /// 获取加载进度
        /// *加载完成或加载失败发生异常（除<see cref="System.OperationCanceledException"/>），都返回进度1
        /// *加载场景的情况，会以子场景的形式完全加载好之后，返回进度1
        /// *加载时发生<see cref="System.OperationCanceledException"/>异常，会抛出此异常
        /// </summary>
        float GetProgress(ResourceId token);

        /// <summary>
        /// get the key witch the requestId created by
        /// </summary>
        ResourceKey GetKey(ResourceId token);

        IEnumerator WaitAsync(ResourceId token);
        
        /// <summary>
        /// block the thread and wait for request done
        /// *warning* this will block the thread,suggest check progress and wait in async way
        /// </summary>
        void Wait(ResourceId token);

        /// <summary>
        /// dispose request
        /// 在一个包失去所有引用之后，调用UnloadUnusedAssets
        /// </summary>
        void Release(ResourceId token);

        /// <summary>
        /// Unity资源管理中Unload这步少不了，请在合理时机调用
        /// </summary>
        void UnloadUnusedAssets();

        /// <summary>
        /// 加载场景
        /// </summary>
        SceneId LoadSceneAsync(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false);

        /// <summary>
        /// 获取场景加载进度(加载进度与suspendLoad挂钩，通常在90%停止）
        /// </summary>
        float GetProgress(SceneId sceneId);

        /// <summary>
        /// 获取场景的Instance
        /// </summary>
        Scene GetSceneInstance(SceneId sceneId);

        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="sceneId"></param>
        void ActivateScene(SceneId sceneId);

        /// <summary>
        /// 解除场景挂起状态
        /// </summary>
        /// <param name="sceneId"></param>
        void Unsuspend(SceneId sceneId);

        /// <summary>
        /// 如果是多个场景则可以卸载掉某个
        /// </summary>
        void Unload(SceneId sceneId, Action? onDone);
    }
}