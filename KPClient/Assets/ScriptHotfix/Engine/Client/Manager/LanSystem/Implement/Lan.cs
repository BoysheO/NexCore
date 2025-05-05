using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions;
using Sirenix.OdinInspector;
using TableSystem.Abstractions;
using UnityEngine;

namespace Hotfix.LanMgr
{
    /// <summary>
    /// 建议：自己实现一个新的Lan继承ILanManager，然后在内部实现语言设置自动储存。使用组合的方式将这个基础Lan包含进去自己的LanMgr
    /// </summary>
    public sealed class Lan : ILanManager
    {
        public const string RawLan = "RawLan";
        private const bool IsDebug = false;
        private readonly ITableDataManager _tbMgr;
        private readonly Dictionary<string, string> _lanKey2TableName = new();
        private readonly HashSet<object> _onLanChanged = new();

        private string _curLanguage = RawLan;

        [ShowInInspector]
        public string CurLanguage
        {
            get => _curLanguage;
            set
            {
                _curLanguage = value;
                NotifyLanChanged();
            }
        }

        public void Add(ILanObserver observer)
        {
            _onLanChanged.Add(observer);
        }

        public void Remove(ILanObserver observer)
        {
            _onLanChanged.Remove(observer);
        }

        public void Add(Action<ILanManager> action)
        {
            _onLanChanged.Add(action);
        }

        public void Remove(Action<ILanManager> action)
        {
            _onLanChanged.Remove(action);
        }

        private void NotifyLanChanged()
        {
            var count = _onLanChanged.Count;
            var buffer = ArrayPool<object>.Shared.Rent(count);
            //note 可以遇见切换语言的频率不会太频繁。
            _onLanChanged.CopyTo(buffer);
            foreach (var lanObserver in buffer.AsSpan(0, count))
            {
                switch (lanObserver)
                {
                    case Action<ILanManager> action:
                        action(this);
                        break;
                    case ILanObserver observer:
                        observer.OnLanChanged();
                        break;
                }
            }

            Array.Clear(buffer, 0, count);
            ArrayPool<object>.Shared.Return(buffer);
        }

        [return: NotNull]
        public string GetText(int id)
        {
            if (CurLanguage == RawLan) return $"[lan{id}]";

            var tbMgr = _tbMgr;
            if (!_lanKey2TableName.ContainsKey(CurLanguage))
            {
                _lanKey2TableName[CurLanguage] = CurLanguage + "Table";
            }

            var tableName = _lanKey2TableName[CurLanguage];
            if (!tbMgr.HasTable(tableName))
            {
                if (IsDebug) Debug.LogError($"no such lan support:{CurLanguage}");
                return "";
            }

            var tables = tbMgr.GetDic(tableName);
            if (tables.TryGetValue(id, out var table)) return table.GetString("Text");
            if (IsDebug) Debug.LogError($"missing lanId={id} in {tableName}");
            return "";
        }

        public IReadOnlyList<string> GetLanguageSupported()
        {
            return new[] { RawLan };
        }

        public Lan(ITableDataManager tbMgr)
        {
            _tbMgr = tbMgr;
        }
    }
}