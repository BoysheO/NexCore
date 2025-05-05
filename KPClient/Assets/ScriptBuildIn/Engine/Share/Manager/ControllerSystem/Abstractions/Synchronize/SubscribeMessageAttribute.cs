using System;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SubscribeMessageAttribute:System.Attribute
    {
        public readonly string Message;

        public SubscribeMessageAttribute(string message)
        {
            Message = message;
        }
    }
}