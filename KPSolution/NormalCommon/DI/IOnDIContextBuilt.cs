using System;
using System.Collections.Generic;
using System.Text;

namespace BearMonoScript.DI
{
    /// <summary>
    /// call after DIContext build sucess
    /// Notify:It may be called in thread not main
    /// </summary>
    public interface IOnDIContextBuilt
    {
        void OnDIContextBuilt(IServiceProvider provider);
    }
}
