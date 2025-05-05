using System;
using System.Reflection;
using NexCore.DI;
using NexCore.UnityEnvironment;
using BoysheO.Collection;
using BoysheO.Extensions;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
using UniRx;
using UnityEngine;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Implements.Synchronize
{
    [Service(typeof(IController))]
    public class RxController : IController
    {
        private readonly IUnityEnvironment _environment;
        private readonly ISyncHandlerRepos _repos;

        //key=Message Type value=Subject List
        // private readonly PDictionary<Type, object> _subjects = new();
        private readonly PDictionary<string, Subject<object?>> _subjects = new();

        public RxController(IUnityEnvironment environment, ISyncHandlerRepos repos)
        {
            _environment = environment;
            _repos = repos;
        }

        private object?[] publicBuffer = new object[1];

        public void Public(string eventName, object? args)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Debug.Log("Do nothing due to not playing");
                return;
            }
#endif
            _environment.ThrowIfNotMainThread();
            eventName.ThrowIfNullOrWhiteSpace();
            Debug.Log($"[public]{eventName}:{args}");
            if (_repos.TryGetHandler(eventName, out var handle))
            {
                Debug.Log($"Handle:{handle.GetType().Name} found");
                MethodInfo method = handle.GetType().GetMethod("Process").ThrowIfNull();
                publicBuffer[0] = args;
                method.Invoke(handle, publicBuffer);
                publicBuffer[0] = null;
            }

            if (_subjects.TryGetValue(eventName, out var s))
            {
                s.OnNext(args);
                if (!s.HasObservers)
                {
                    _subjects.Remove(eventName);
                    s.Dispose();
                }
            }
        }

        public IObservable<object> Observe(string eventName)
        {
            if (!_subjects.TryGetValue(eventName, out var subject))
            {
                subject = new();
                _subjects.Add(eventName, subject);
            }

            return subject!;
        }
    }
}