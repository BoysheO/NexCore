using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hotfix.ContentSystems.LocalDbSystem;

public interface ISqliteMapDbManager
{
    /// <summary>
    /// 获取最大全图范围
    /// </summary>
    (int xMin, int yMin, int xMax, int yMax) Area { get; }

    /// <summary>
    /// 更新范围最大最小值
    /// </summary>
    bool UpdateArea(int x, int y);

    /// <summary>
    /// 更新缓存的版本时间戳 现已更改为逐块请求，全图时间戳没有意义
    /// </summary>
    [Obsolete("",true)]
    bool UpdateTime(DateTimeOffset time);

    /// <summary>
    /// 打开对应trunk的sql连接。如果没有缓存，将创建新缓存
    /// </summary>
    IntPtr CreatOrOpen(ulong trunkId);

    /// <summary>
    /// 关闭对应trunk的sql连接
    /// </summary>
    void Close(IntPtr ptr);

    /// <summary>
    /// 这个trunk的缓存是否存在
    /// </summary>
    bool IsCacheDataExist(ulong trunkId);

    void GetData(int xMin, int yMin, int xMax, int yMax, ISqliteMapDataObserver onSqliteMapData, int maxCount);

    Task GetDataAsync(int xMin, int yMin, int xMax, int yMax, ISqliteMapDataObserver onSqliteMapData, int maxCount,
        CancellationToken token);

    /// <summary>
    /// 开始事务
    /// </summary>
    void StartTransaction(IntPtr conn);

    /// <summary>
    /// 提交事务
    /// </summary>
    void EndTransaction(IntPtr conn);

    /// <summary>
    /// 将xy转换为trunk信息(thread safe)
    /// </summary>
    (ulong trunkId, int trunkMinX, int trunkMinY) GetTrunkInfo(int x, int y);

    IEnumerable<ulong> GetTrunkIds(int xMin, int yMin, int xMax, int yMax);

    (int xMin, int yMin, int xMax, int yMax, DateTimeOffset? timeStamp) GetTrunkInfo(ulong trunkId);

    // /// <summary>
    // /// 这里传入的value只能是SQLite3.Result枚举
    // /// 其行为如下：
    // /// done=>do nothing,return true
    // /// error=>LogError about sqlite error; return false
    // /// constraint =>LogError but more info; return false
    // /// </summary>
    // bool HandleSQLite3Result(int value,IntPtr conn);

    /// <summary>
    /// 尝试保存像素点并更新area和time
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="timeStamp"></param>
    /// <param name="power"></param>
    /// <param name="color"></param>
    /// <returns>isSave?</returns>
    bool SaveSinglePixel(int x, int y, DateTimeOffset timeStamp, int color);

    bool IsConnectToTheZeroLocation(int x, int y);

    bool HandleSQLite3Result(int value, IntPtr conn);
}