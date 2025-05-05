#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TableModels.Abstractions;

namespace TableSystem.Abstractions
{
    /// <summary>
    /// 总管跟表格数据相关的数据
    /// *它不负责具体表格数据的逻辑，这需要Coder具体实现自己的Manager类（使用而不继承此接口）
    /// *immutable and thread safe
    /// </summary>
    public interface ITableDataManager
    {
        [return: NotNull]
        IReadOnlyList<T> Get<T>() where T : ITableData;

        [return: NotNull]
        IReadOnlyList<ITableData> Get(string tableName);
        
        [return: NotNull]
        IReadOnlyDictionary<int, T> GetDic<T>()
            where T : ITableData;

        [return: NotNull]
        IReadOnlyDictionary<int, ITableData> GetDic(string tableName);

        /// <summary>
        /// 多语言需要判断是否有这个表来决定是否提供这个语言选项
        /// </summary>
        bool HasTable(string tableName);
    }

    public static class TableDataManagerExtensions
    {
        [return: NotNull]
        public static T GetRequire<T>(this ITableDataManager manager, int key)
            where T : ITableData
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            return manager.GetDic<T>().TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"key={key} not found in {typeof(T).Name}");
        }

        public static T? GetOrNull<T>(this ITableDataManager manager, int key)
            where T : ITableData
        {
            return manager.GetDic<T>().GetValueOrDefault(key);
        }
    }
}