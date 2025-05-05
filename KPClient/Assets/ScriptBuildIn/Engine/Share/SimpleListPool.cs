using System;
using System.Collections.Generic;

namespace BoysheO.Buffers
{
    /// <summary>
    /// 简单的列表池化实现。
    /// </summary>
    public sealed class SimpleListPool<T>
    {
        public static readonly SimpleListPool<T> Share = new SimpleListPool<T>();
        private readonly List<List<T>> _pool = new();
        private readonly HashSet<List<T>> _set = new HashSet<List<T>>();

        private readonly object gate = new object();

        public List<T> Rent()
        {
            lock (gate)
            {
                if (this._pool.Count > 0)
                {
                    var idx = this._pool.Count - 1;
                    var last = this._pool[idx];
                    _set.Remove(last);
                    this._pool.RemoveAt(idx);
                    return last;
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        public void Return(List<T> list)
        {
            lock (gate)
            {
                if (!_set.Add(list)) throw new Exception("the lst is already in pool");
                list.Clear();
                _pool.Add(list);
            }
        }
    }
}