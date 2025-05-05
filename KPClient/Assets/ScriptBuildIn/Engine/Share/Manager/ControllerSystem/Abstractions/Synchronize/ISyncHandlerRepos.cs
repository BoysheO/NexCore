using System;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize
{
    public interface ISyncHandlerRepos
    {
        bool TryGetHandler(string msgType, out object handle);
    }
}