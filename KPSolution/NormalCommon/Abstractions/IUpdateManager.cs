using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NormalCommon.Abstractions;

public interface IUpdateManager
{
    public enum ErCode
    {
        NoUpdate,
        ConnectFail,
        NeedUpdate,
    }
    
    public float DownSize { get; }
    public float TotalSize { get; }
    public Task<ErCode> IsNeedUpdateAsync(CancellationToken cancellationToken);
    public Task UpdateAsync(CancellationToken cancellationToken);
}