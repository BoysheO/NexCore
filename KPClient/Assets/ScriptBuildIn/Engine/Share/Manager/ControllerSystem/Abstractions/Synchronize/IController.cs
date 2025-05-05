using System;
using System.Diagnostics.CodeAnalysis;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize
{
    public interface IController
    {
        void Public(string eventName, object? args);

        [return: NotNull]
        IObservable<object> Observe(string eventName);
    }

    public static class ControllerExtensions
    {
        public static void Public(this IController controller, string eventName)
        {
            controller.Public(eventName,null);
        }
    }
}