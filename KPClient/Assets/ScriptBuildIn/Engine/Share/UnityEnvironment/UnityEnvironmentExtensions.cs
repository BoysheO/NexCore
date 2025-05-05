using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace NexCore.UnityEnvironment
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnityEnvironmentExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unityEnvironment"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowIfNotMainThread(this IUnityEnvironment unityEnvironment)
        {
            if (!unityEnvironment.IsInUnityThread()) throw new Exception("not in main thread");
        }

        //todo:这个函数的实现目前有问题，未能满足IUnityEnvironment.IsPlaying的语义。但现下没空改，以后再说吧
        [Conditional("UNITY_EDITOR")]
        public static void ThrowIfUnityNotPlay(this IUnityEnvironment unityEnvironment)
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying == false) throw new OperationCanceledException();
#endif
            unityEnvironment.CancellationToken.ThrowIfCancellationRequested();
        }
    }
}