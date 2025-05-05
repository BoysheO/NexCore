#nullable disable
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NexCore.DI;
using NexCore.UnityEnvironment;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NexCore.Customer
{
    internal sealed class UnityEnvironment : IUnityEnvironment, IOnDIContextBuildingCallback
    {
        public UnityEnvironment()
        {
            Platform = Application.platform;
            Version = Application.version;
            StreamingAssetsPath = Application.streamingAssetsPath;
            DataPath = Application.dataPath;
            PersistentDataPath = Application.persistentDataPath;
            TemporaryCachePath = Application.persistentDataPath;
            SystemLanguage = Application.systemLanguage;
            SynchronizationContext = SynchronizationContext.Current;
            Thread = Thread.CurrentThread;
            ProductName = Application.productName;
        }

        private static UnityEnvironment Instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            if (Instance != null) return;
            Instance = new UnityEnvironment();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoad()
        {
            var dispatcher = Object.FindObjectOfType<UnityLifeTimeDispatcher>();
            if (!dispatcher)
            {
                var go = new GameObject(nameof(UnityLifeTimeDispatcher));
                dispatcher = go.AddComponent<UnityLifeTimeDispatcher>();
                UnityEngine.Object.DontDestroyOnLoad(go);
                go.hideFlags = HideFlags.HideAndDontSave;
            }

            dispatcher.unityEnvironment = Instance;
        }
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnLoad()
        {
            if (Application.isPlaying)
            {
                OnBeforeSceneLoad();
                OnAfterSceneLoad();
            }
        }
#endif

        private class UnityLifeTimeDispatcher : MonoBehaviour
        {
            internal UnityEnvironment unityEnvironment;

            void Update()
            {
                unityEnvironment?.onUpdate?.Invoke();
            }

            void FixedUpdate()
            {
                unityEnvironment?.onFixedUpdate?.Invoke();
            }

            void LateUpdate()
            {
                unityEnvironment?.onLateUpdate?.Invoke();
            }

            private void OnDestroy()
            {
                if (unityEnvironment != null)
                {
                    unityEnvironment._cancellationTokenSource.Cancel();
                    unityEnvironment._cancellationTokenSource.Dispose();
                }
            }
        }

        public RuntimePlatform Platform { get; private set; }

        public bool IsPlaying => throw new NotImplementedException(); //todo:要用Editor上的API来实现

        public bool IsIL2CPP => throw new NotImplementedException(); //弃用了

        public string Version { get; private set; }

        public string StreamingAssetsPath { get; private set; }

        public string DataPath { get; private set; }

        public string PersistentDataPath { get; private set; }

        public string TemporaryCachePath { get; private set; }

        public SystemLanguage SystemLanguage { get; private set; }

        public SynchronizationContext SynchronizationContext { get; private set; }
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public Thread Thread { get; private set; }

        public string ProductName { get; private set; }

        public event Action Quitting
        {
            add { Application.quitting += value; }
            remove { Application.quitting -= value; }
        }

        public event Func<bool> WantsToQuit
        {
            add { Application.wantsToQuit += value; }
            remove { Application.wantsToQuit -= value; }
        }

        public event Action onUpdate;
        public event Action onFixedUpdate;
        public event Action onLateUpdate;

        public bool IsInUnityThread()
        {
            return Thread.CurrentThread == Thread;
        }

        public void OnCallback(IServiceCollection collection, IConfiguration configuration, IReadOnlyList<Type> _)
        {
            Instance ??= new UnityEnvironment();
            collection.AddSingleton<IUnityEnvironment>(Instance);
        }
    }
}