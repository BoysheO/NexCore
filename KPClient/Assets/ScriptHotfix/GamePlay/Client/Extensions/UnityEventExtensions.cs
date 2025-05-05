using System;
using System.Dynamic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Hotfix.Extensions
{
    public static class UnityEventExtensions
    {
        public static void AddAsyncListener(this UnityEvent unityEvent, Func<UniTask> func)
        {
            void Call()
            {
                unityEvent.RemoveListener(Call);
                Invoke().Forget();
            }

            async UniTask Invoke()
            {
                try
                {
                    await func();
                }
                finally
                {
                    unityEvent.AddListener(Call);
                }
            }

            unityEvent.AddListener(Call);
        }

        public static void AddOnceListener(this UnityEvent unityEvent, Action action)
        {
            void Call()
            {
                unityEvent.RemoveListener(Call);
                action();
            }

            unityEvent.AddListener(Call);
        }

        public static void AddOnceAsyncListener(this UnityEvent unityEvent, Func<UniTaskVoid> func)
        {
            void Call()
            {
                unityEvent.RemoveListener(Call);
                func().Forget();
            }

            unityEvent.AddListener(Call);
        }

        public static UniTask FirstAsync(this UnityEvent unityEvent)
        {
            var src = new UniTaskCompletionSource();
            unityEvent.AddListener(() => src.TrySetResult());
            return src.Task;
        }
    }
}