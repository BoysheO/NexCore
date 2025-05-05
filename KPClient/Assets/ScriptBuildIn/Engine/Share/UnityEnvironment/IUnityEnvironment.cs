using System;
using System.Threading;
using UnityEngine;

namespace NexCore.UnityEnvironment
{
    /// <summary>
    /// 提供Unity3D环境的基本抽象接口
    /// </summary>
    public interface IUnityEnvironment
    {
        /// <summary>
        ///     <see cref="Application.platform"/>，在初始化后支持多线程访问
        /// </summary>
        RuntimePlatform Platform { get; }

        /// <summary>
        /// 在UnityEditor中，这个值应为开发者按下播放按钮之后会true，松开播放按钮之后为false。对应的值应为EditorApplication.isPlayingOrWillChangePlaymode（但是还是有点小区别）
        /// 在后端和Runtime中，这个值应恒为true。
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// 根据IL2CPP运行环境的特点关闭一些不受支持的特性。
        /// </summary>
        [Obsolete("使用意义太小，不如用宏命令")]
        bool IsIL2CPP { get; }

        /// <summary>
        ///     等价或等义<see cref="Application.version"/>，在初始化后支持多线程访问
        /// </summary>
        string Version { get; }

        /// <summary>
        ///     等价<see cref="Application.streamingAssetsPath"/>，在初始化后支持多线程访问
        /// </summary>
        string StreamingAssetsPath { get; }

        /// <summary>
        ///     等价<see cref="Application.dataPath"/>，在初始化后支持多线程访问
        /// </summary>
        string DataPath { get; }

        /// <summary>
        /// 等价<see cref="Application.persistentDataPath"/>，在初始化后支持多线程访问
        /// </summary>
        string PersistentDataPath { get; }

        /// <summary>
        /// 等价<see cref="Application.temporaryCachePath"/>，在初始化后支持多线程访问
        /// </summary>
        string TemporaryCachePath { get; }

        /// <summary>
        /// 等价<see cref="Application.systemLanguage"/>，在初始化后支持多线程访问
        /// </summary>
        SystemLanguage SystemLanguage { get; }

        /// <summary>
        /// Unity3D上下文；在后端则是当前用户的上下文
        /// </summary>
        SynchronizationContext SynchronizationContext { get; }

        /// <summary>
        /// 当UnityEditor退出播放模式时，此Token会被设置为Cancel状态
        /// 在后端则是用户上下文释放时会被设置为Cancel状态
        /// Runtime中理论上不需要处理，区分Cancel状态没有意义，暂时不作定义
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Unity3D的线程引用；用户上下文的线程引用。虽然有，但是应避免使用此值。
        /// </summary>
        Thread Thread { get; }

        /// <summary>
        /// 等价<see cref="Application.productName"/>，在初始化后支持多线程访问
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// 等价<see cref="Application.quitting"/>
        /// </summary>
        event Action Quitting;

        /// <summary>
        /// 等价<see cref="Application.wantsToQuit"/>
        /// </summary>
        event Func<bool> WantsToQuit;

        /// <summary>
        /// 当前上下文是否在Unity/用户上下文内
        /// </summary>
        bool IsInUnityThread();

        /// <summary>
        /// 等价<see cref="MonoBehaviour"/>.Update
        /// </summary>
        event Action onUpdate;

        /// <summary>
        /// 等价<see cref="MonoBehaviour"/>.FixedUpdate
        /// </summary>
        event Action onFixedUpdate;

        /// <summary>
        /// 等价<see cref="MonoBehaviour"/>.LateUpdate
        /// </summary>
        event Action onLateUpdate;
    }
}