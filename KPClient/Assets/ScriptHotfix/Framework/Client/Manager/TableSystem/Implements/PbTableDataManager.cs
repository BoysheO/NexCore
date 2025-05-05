using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NexCore.DI;
using NexCore.UnityEnvironment;
using BoysheO.Extensions;
using BoysheO.Util;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Hotfix.ResourceMgr.Abstractions;
using HotScripts.Hotfix.SharedCode.HotfixCommon.ProtobufTableSystem;
using TableModels.Abstractions;
using TableSystem.Abstractions;
using UnityEngine;
using UnityEngine.Scripting;

namespace TableSystem.Implements
{
    [Service(typeof(ITableDataManager))]
    public class PbTableDataManager : ITableDataManager
    {
        private readonly IUnityEnvironment _unityEnvironment;
        private readonly Dictionary<string, ITableDataList> _tbName2tbLst = new(); //value: GenericTableDataList<T> 
        private readonly IResourceManager _resourceManager;
        private readonly IGameServiceProvider _gameServiceProvider;
        private bool _isInit;

        public PbTableDataManager(IUnityEnvironment unityEnvironment, IResourceManager resourceManager,
            IGameServiceProvider gameServiceProvider)
        {
            _unityEnvironment = unityEnvironment;
            _resourceManager = resourceManager;
            _gameServiceProvider = gameServiceProvider;
        }

        public class InitHandle
        {
            public int TotalJobs { get; private set; }
            public int JobsDone { get; private set; }
            private readonly Type[] _types;
            private readonly PbTableDataManager _man;
            private readonly IResourceManager _resourceManager;
            private readonly IUnityEnvironment _unityEnvironment;

            internal InitHandle(Type[] types, PbTableDataManager man, IResourceManager resourceManager,
                IUnityEnvironment unityEnvironment)
            {
                _types = types;
                _man = man;
                _resourceManager = resourceManager;
                _unityEnvironment = unityEnvironment;
                TotalJobs = _types.Length;
            }

            public async UniTask Init(bool isImmediately)
            {
                if (_man._isInit) return;
                _man._isInit = true;
                var buildListMethod =
                    typeof(PbTableDataManager).GetMethod(nameof(BuildList),
                        BindingFlags.NonPublic | BindingFlags.Instance)
                    ?? throw new Exception("missing method");
                var typeBuff = new Type[1];
                var argBuff = new object[1];

                foreach (var type in _types)
                {
                    var key = new ResourceKey(type.Name + ".bytes", typeof(TextAsset));
                    ResourceId tk;
                    try
                    {
                        tk = _resourceManager.BeginLoad(key, default);
                    }
                    catch(Exception ex)
                    {
                        Debug.Log($"key={key}.ex={ex}");
                        continue;
                    }

                    if (isImmediately)
                    {
                        _resourceManager.Wait(tk);
                    }
                    else
                    {
                        var itor = _resourceManager.WaitAsync(tk);
                        await itor;
                        _unityEnvironment.CancellationToken.ThrowIfCancellationRequested();
                    }

                    var res = _resourceManager.GetResource<TextAsset>(tk);
                    if (res == null)
                    {
                        Debug.Log($"[{nameof(PbTableDataManager)}]missing table:{type.Name}");
                    }
                    else
                    {
                        typeBuff[0] = type;
                        argBuff[0] = res.bytes;
                        var genericMethod = buildListMethod.MakeGenericMethod(typeBuff);
                        genericMethod.Invoke((PbTableDataManager)_man, argBuff); //参数的强制转换是防止以后重构_man改了类型这里不报错
                    }

                    _resourceManager.Release(tk);
                    JobsDone++;
                }
            }
        }

        public InitHandle Init()
        {
            if (_isInit) throw new Exception("you can not init twice");

            //todo 反射有点慢，研究下怎么优化
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(v => v.GetTypes())
                .Where(v => v.IsSealedAndImplement(typeof(ITableData)) && v.IsSealedAndImplement(typeof(IMessage)))
                .ToArray();
            return new InitHandle(types, this, _resourceManager, _unityEnvironment);
        }

        //通过反射被调用
        [Preserve]
        private void BuildList<T>(byte[] bytes) where T : class, ITableData
        {
            var ary = ResolveUtil.Resolve<T>(bytes);
            var mem = typeof(T).GetProperty("ServiceProvider", BindingFlags.NonPublic | BindingFlags.Instance) ??
                      throw new Exception("missing property");
            var provider = DIContext.ServiceProvider;
            //为所有实例注入服务容器
            foreach (var tableData in ary)
            {
                mem.SetValue(tableData, provider);
            }

            var lst = new GenericTableDataList<T>(ary);
            _tbName2tbLst[typeof(T).Name] = lst;
        }

        private void InitImmediatelyIfEditor()
        {
#if UNITY_EDITOR
            if (_isInit) return;
            var handle = Init();
            handle.Init(true).Forget();
#endif
        }

        public IReadOnlyList<T> Get<T>()
            where T : ITableData
        {
            InitImmediatelyIfEditor();
            var a = (GenericTableDataList<T>)_tbName2tbLst[typeof(T).Name];
            return a.AsReadOnlyListT;
        }

        public IReadOnlyList<ITableData> Get(string tableName)
        {
            InitImmediatelyIfEditor();
            return _tbName2tbLst[tableName].AsReadOnlyListTableData;
        }

        public IReadOnlyDictionary<int, T> GetDic<T>()
            where T : ITableData
        {
            InitImmediatelyIfEditor();
            var a = (GenericTableDataList<T>)_tbName2tbLst[typeof(T).Name];
            return a.AsReadOnlyDictionaryT;
        }

        public IReadOnlyDictionary<int, ITableData> GetDic(string tableName)
        {
            InitImmediatelyIfEditor();
            return _tbName2tbLst[tableName].AsReadOnlyDictionaryTableData;
        }

        public bool HasTable(string tableName)
        {
            InitImmediatelyIfEditor();
            return _tbName2tbLst.ContainsKey(tableName);
        }
    }
}