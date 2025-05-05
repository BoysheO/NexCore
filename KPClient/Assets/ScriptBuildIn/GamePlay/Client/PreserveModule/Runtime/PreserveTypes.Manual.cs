using System;
using System.Collections;
using System.Collections.Generic;
using BoysheO.Collection;
using BoysheO.Collection2;
using BoysheO.TinyStateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace PreserveTypesScripts
{
    partial class PreserveTypes
    {
        [Preserve]
        async UniTask Foo2()
        {
            //已添加，不再重复添加
            // _ = new Cinemachine.CinemachineVirtualCamera();
            // _ = new DG.Tweening.Core.DOGetter<float>(default (DG.Tweening.Core.DOGetter<float>));
            // _ = new DG.Tweening.Core.DOSetter<float>(default (DG.Tweening.Core.DOSetter<float>));
            // _ = new UnityEngine.Events.UnityAction<object, object>(default (UnityEngine.Events.UnityAction<object, object>));
            // _ = new UnityEngine.Grid();

            var b = StateMachineBuilder<SystemLanguage, SystemLanguage, object>.Creat();
            b.Build(SystemLanguage.Afrikaans, null!);
            
            _ = new System.Action<(bool, long)>(default (System.Action<(bool, long)>));
            _ = new System.Action<(long, int)>(default (System.Action<(long, int)>));
            _ = new System.Action<(long, int, object)>(default (System.Action<(long, int, object)>));
            _ = new System.Action<(long, object)>(default (System.Action<(long, object)>));
            _ = new System.Action<(long,float,float)>(default (System.Action<(long,float,float)>));
            _ = new System.Action<(long, int, int, int)>(default (System.Action<(long, int, int, int)>));
            _ = new System.Action<(int, int, int)>(default (System.Action<(int, int, int)>));
            _ = new System.Action<(int, int)>(default (System.Action<(int, int)>));
            _ = new System.Action<(int, long)>(default (System.Action<(int, long)>));
            _ = new System.Action<(int, long, int, long)>(default (System.Action<(int, long, int, long)>));
            _ = new System.Action<(int,long,object,object)>(default (System.Action<(int,long,object,object)>));
            _ = new System.Action<(int,long, object)>(default (System.Action<(int,long, object)>));
            _ = new System.Action<(int, object)>(default (System.Action<(int, object)>));
            _ = new System.Action<(int,object, long)>(default (System.Action<(int,object, long)>));
            _ = new System.Action<(int,object, object)>(default (System.Action<(int,object, object)>));
            _ = new System.Action<(long,bool, int)>(default (System.Action<(long,bool, int)>));
            _ = new System.Action<(long,int,object)>(default (System.Action<(long,int,object)>));
            _ = new System.Action<(long,long)>(default (System.Action<(long,long)>));
            _ = new System.Action<(long,object,object)>(default (System.Action<(long,object,object)>));
            _ = new System.Action<(bool,object)>(default (System.Action<(bool,object)>));
            _ = new System.Action<(object, long)>(default (System.Action<(object, long)>));
            _ = new System.Action<(object, float)>(default (System.Action<(object, float)>));
            _ = new System.Action<(object, object,long,long)>(default (System.Action<(object, object,long,long)>));
            _ = new System.Action<(object, long, ValueTuple<object>, int, long)>(default (System.Action<(object, long, ValueTuple<object>, int, long)>));
            
            _ = new System.Action<ValueTuple>(default (System.Action<ValueTuple>));
            
            _ = new System.Action<ValueTuple<long>>(default (System.Action<ValueTuple<long>>));
            _ = new System.Action<ValueTuple<int>>(default (System.Action<ValueTuple<int>>));
            _ = new System.Action<ValueTuple<bool>>(default (System.Action<ValueTuple<bool>>));
            _ = new System.Action<ValueTuple<object>>(default (System.Action<ValueTuple<object>>));

            IEnumerator enumerator = default;
            await enumerator;//
        }
    }
}
//link.xml