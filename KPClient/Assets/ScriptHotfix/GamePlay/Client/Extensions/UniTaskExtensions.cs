using System;
using Cysharp.Threading.Tasks;

namespace Hotfix.Extensions{

public static class UniTaskExtensions
{
    public static async UniTask Finally(this UniTask task, Action action)
    {
        try
        {
            await task;
        }
        finally
        {
            action();
        }
    }
}}