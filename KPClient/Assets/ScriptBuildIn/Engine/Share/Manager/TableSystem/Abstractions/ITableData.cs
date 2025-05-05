using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TableModels.Abstractions
{
    /// <summary>
    /// excel表的某一行数据。
    /// </summary>
    public interface ITableData
    {
        int Key { get; }
        int GetInt(string columName);
        long GetLong(string columName);
        double GetDouble(string columName);
        bool GetBool(string columName);
        [return:NotNull]
        string GetString(string columName);
        IReadOnlyList<int> GetIntLst(string columName);
        IReadOnlyList<long> GetLongLst(string columName);
        IReadOnlyList<double> GetDoubleLst(string columName);
        IReadOnlyList<bool> GetBoolLst(string columName);
        IReadOnlyList<string> GetStringLst(string columName);
        /// <summary>
        /// 获取所有列名
        /// </summary>
        IReadOnlyList<string> GetKeys();
    }
}
