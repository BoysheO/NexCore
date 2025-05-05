using System;
using Cysharp.Threading.Tasks;

namespace Engine.Client.Extensions
{
    public static class UniTaskExtensions
    {
        public static UniTask.Awaiter GetAwaiter(this float f)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(f), DelayType.DeltaTime).GetAwaiter();
        }
    }
}