using System;
using Cysharp.Threading.Tasks;
using Hotfix.Extensions;

namespace Hotfix.Helper{

public static class DelegateHelper
{
    public static Action ToButtonAction(Func<UniTask> task)
    {
        bool isRunning = false;
        return () =>
        {
            if (isRunning) return;
            isRunning = true;
            task().Finally(() => isRunning = false).Forget();
        };
    }

    public static Action<T> ToButtonAction<T>(Func<T, UniTask> task)
    {
        bool isRunning = false;
        return t =>
        {
            if (isRunning) return;
            isRunning = true;
            task(t).Finally(() => isRunning = false).Forget();
        };
    }
}}