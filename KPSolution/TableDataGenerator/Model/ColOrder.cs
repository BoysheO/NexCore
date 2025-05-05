using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDataGenerator.Model
{
    /// <summary>
    /// [0,+)，0为第一列
    /// Excel列字母A为第一列
    /// </summary>
    [DebuggerDisplay("{ToExcelColumLetters()}")]
    public readonly struct ColOrder
    {
        public readonly int Order;

        public ColOrder(int order)
        {
            if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));
            Order = order;
        }

        /// <summary>
        /// 转换成excel的列英文表示
        /// </summary>
        public string ToExcelColumLetters()
        {
            return GetColumnLetter(Order + 1);
        }

        private static string GetColumnLetter(int iColumnNumber, bool fixedCol = false)
        {
            if (iColumnNumber < 1)
            {
                //throw new Exception("Column number is out of range");
                return "#REF!";
            }

            string sCol = "";
            do
            {
                sCol = ((char)('A' + (iColumnNumber - 1) % 26)).ToString() + sCol;
                iColumnNumber = (iColumnNumber - (iColumnNumber - 1) % 26) / 26;
            } while (iColumnNumber > 0);

            return fixedCol ? "$" + sCol : sCol;
        }

        public static ColOrder FirstOrder { get; } = new(0);
    }

    public sealed class ColOrderComparer : IEqualityComparer<ColOrder>, IComparer<ColOrder>
    {
        public static readonly ColOrderComparer Instance = new ColOrderComparer();

        public int Compare(ColOrder x, ColOrder y)
        {
            return Comparer<int>.Default.Compare(x.Order, y.Order);
        }

        public bool Equals(ColOrder x, ColOrder y)
        {
            return EqualityComparer<int>.Default.Equals(x.Order, y.Order);
        }

        public int GetHashCode([DisallowNull] ColOrder obj)
        {
            return EqualityComparer<int>.Default.GetHashCode(obj.Order);
        }
    }
}