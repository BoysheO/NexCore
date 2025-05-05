using System;
using NexCore.DI;

namespace ScriptGamePlay.Hotfix.ClientCode.Manager.RandomSystem
{
    [Service(typeof(RandomManager))]
    public class RandomManager
    {
        public readonly Random Random = new();
    }
}