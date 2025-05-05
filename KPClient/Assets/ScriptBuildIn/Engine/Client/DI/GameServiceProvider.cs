using System;

namespace NexCore.DI
{
    public class GameServiceProvider:IGameServiceProvider
    {
        public IServiceProvider ServiceProvider { get; }

        public GameServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}