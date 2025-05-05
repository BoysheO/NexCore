using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// ReSharper disable once IdentifierTypo
namespace Cysharp.Threading.Tasks
{
    //不打算热更的函数
    public static class UniTaskHelper
    {
        // public static async UniTask WaitAll(AsyncOperationHandle[] handle, int offset, int count)
        // {
        //     for (int i = offset; i < offset + count; i++)
        //     {
        //         var cur = handle[i];
        //         if (!cur.IsValid() || cur.IsDone) continue;
        //         await cur;
        //     }
        // }
        //
        // public static async UniTask WaitAll<T>(AsyncOperationHandle<T>[] handle, int offset, int count)
        // {
        //     for (int i = offset; i < offset + count; i++)
        //     {
        //         var cur = handle[i];
        //         if (!cur.IsValid() || cur.IsDone) continue;
        //         await cur;
        //     }
        // }

        // public static async UniTask WaitAll<T>(IReadOnlyList<AsyncOperationHandle<T>> handle)
        // {
        //     for (var index = 0; index < handle.Count; index++)
        //     {
        //         var asyncOperationHandle = handle[index];
        //         var cur = asyncOperationHandle;
        //         if (!cur.IsValid() || cur.IsDone) continue;
        //         await cur;
        //     }
        // }

        private static readonly Func<bool> _waitForAnyInput = () => Input.touchCount > 1 || Input.anyKey;

        public static UniTask WaitForAnyInput()
        {
            return UniTask.WaitUntil(_waitForAnyInput);
        }

        //防止重构意外带入泛型UniTask.WhenAll
        public static async UniTask WhenAll(UniTask task1, UniTask task2, UniTask task3)
        {
            await UniTask.WhenAll(task1, task2, task3);
        }

        //防止重构意外带入泛型UniTask.WhenAll
        public static async UniTask WhenAll(UniTask task1, UniTask task2)
        {
            await UniTask.WhenAll(task1, task2);
        }

        //防止重构意外带入泛型UniTask.WhenAll
        public static async UniTask WhenAll(UniTask[] tasks, int taskCount)
        {
            for (int i = 0; i < taskCount; i++)
            {
                var task = tasks[i];
                switch (task.Status)
                {
                    case UniTaskStatus.Faulted:
                    case UniTaskStatus.Canceled:
                    case UniTaskStatus.Pending:
                        await task;
                        break;
                    case UniTaskStatus.Succeeded:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}