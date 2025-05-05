using System;

namespace Hotfix.ResourceMgr.Abstractions
{
    /// <summary>
    /// just a token。All logic is implemented by IResourceManager.
    /// contract:all token created by the mgr should not be same every time
    /// </summary>
    public readonly struct ResourceId:IDisposable
    {
        public readonly ulong Id;
        private readonly IResourceManager _resourceManager;

        public ResourceId(ulong id,IResourceManager resourceManager)
        {
            Id = id;
            _resourceManager = resourceManager;
        }

        public void Dispose()
        {
            //判空是防止invalid的tk
            _resourceManager?.Release(this);
        }
    }
}
