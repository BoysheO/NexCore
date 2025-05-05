using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoysheO.Collection;
using TableModels.Abstractions;

namespace TableSystem.Implements
{
    internal interface ITableDataList
    {
        IReadOnlyList<ITableData> AsReadOnlyListTableData { get; }
        IReadOnlyDictionary<int, ITableData> AsReadOnlyDictionaryTableData { get; }
    }

    internal class GenericTableDataList<T> : ITableDataList
        where T : ITableData
    {
        private readonly PSortedList<int, T> _values;
        private readonly WarpList<T> _wap;
        private readonly WarpDic<T> _wap2;

        public IReadOnlyList<ITableData> AsReadOnlyListTableData => _wap;
        public IReadOnlyDictionary<int, ITableData> AsReadOnlyDictionaryTableData => _wap2;

        public IReadOnlyList<T> AsReadOnlyListT => _values.InternalValues;

        public IReadOnlyDictionary<int, T> AsReadOnlyDictionaryT => _values;

        public GenericTableDataList(T[] ary)
        {
            var lst = new PSortedList<int, T>();
            lst.Capacity = ary.Length;
            foreach (var tableData in ary)
            {
                lst.Add(tableData.Key, tableData);
            }

            lst.TrimExcess();
            _values = lst;
            _wap = new WarpList<T>(_values);
            _wap2 = new WarpDic<T>(_values);
        }

        public GenericTableDataList(PSortedList<int, T> values)
        {
            _values = values;
        }

        public int Count => _values.Count;

        public bool ContainsKey(int key)
        {
            return _values.ContainsKey(key);
        }

        public bool TryGetValue(int key, out T value)
        {
            return _values.TryGetValue(key, out value!);
        }

        public IEnumerable<int> Keys => _values.Keys;

        public IEnumerable<T> Values => _values.Values;
    }

    internal class CommonTableDataList<T> : IReadOnlyList<ITableData>, IReadOnlyDictionary<int, ITableData>
        where T : ITableData
    {
        private readonly SortedList<int, T> _values;

        public CommonTableDataList(SortedList<int, T> ary)
        {
            _values = ary;
        }

        IEnumerator<KeyValuePair<int, ITableData>> IEnumerable<KeyValuePair<int, ITableData>>.GetEnumerator()
        {
            foreach (var tableData in _values)
            {
                yield return new KeyValuePair<int, ITableData>(tableData.Key, tableData.Value);
            }
        }

        IEnumerator<ITableData> IEnumerable<ITableData>.GetEnumerator()
        {
            foreach (var tableData in _values)
            {
                yield return tableData.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var tableData in _values)
            {
                yield return tableData.Value;
            }
        }

        public int Count => _values.Count;

        public bool TryGetValue(int key, out ITableData value)
        {
            var r = _values.TryGetValue(key, out var v);
            value = v;
            return r;
        }

        ITableData IReadOnlyDictionary<int, ITableData>.this[int key] => _values[key];

        public bool TryGetValue(int key, out T value)
        {
            return _values.TryGetValue(key, out value);
        }

        bool IReadOnlyDictionary<int, ITableData>.ContainsKey(int key)
        {
            return _values.ContainsKey(key);
        }

        public IEnumerable<int> Keys => _values.Keys;

        IEnumerable<ITableData> IReadOnlyDictionary<int, ITableData>.Values => _values.Values.Cast<ITableData>();

        ITableData IReadOnlyList<ITableData>.this[int index] => _values.Values[index];
    }

    internal sealed class WarpList<T> : IReadOnlyList<ITableData> where T :  ITableData
    {
        private readonly PSortedList<int, T> _src;

        public WarpList(PSortedList<int, T> src)
        {
            _src = src;
        }

        public IEnumerator<ITableData> GetEnumerator()
        {
            foreach (var keyValuePair in _src)
            {
                yield return keyValuePair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _src.Count;

        public ITableData this[int index] => _src.InternalValues[index];
    }

    internal sealed class WarpDic<T> : IReadOnlyDictionary<int, ITableData> where T :  ITableData
    {
        private readonly PSortedList<int, T> _src;

        public WarpDic(PSortedList<int, T> src)
        {
            _src = src;
        }

        public IEnumerator<KeyValuePair<int, ITableData>> GetEnumerator()
        {
            foreach (var keyValuePair in _src)
            {
                yield return new KeyValuePair<int, ITableData>(keyValuePair.Key, keyValuePair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _src.Count;

        public bool ContainsKey(int key)
        {
            return _src.ContainsKey(key);
        }

        public bool TryGetValue(int key, out ITableData value)
        {
            var r = _src.TryGetValue(key, out var v);
            value = (r ? v : default)!;
            return r;
        }

        public ITableData this[int key] => _src[key];

        public IEnumerable<int> Keys => _src.Keys;
        public IEnumerable<ITableData> Values => _src.InternalValues.Cast<ITableData>();
    }
}