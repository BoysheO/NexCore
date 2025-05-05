// ReSharper disable RedundantNullableDirective
// ReSharper disable CheckNamespace

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NexCore.DI;
using BoysheO.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public static class DIContext
{
    private static IServiceProvider? _ServiceProvider;
    private static ConfigurationRoot? _Configuration;

    private static readonly object gate = new();

    public static IConfiguration Configuration
    {
        get
        {
            if (_Configuration == null)
            {
                lock (gate)
                {
                    if (_Configuration == null)
                    {
                        Profiler.BeginSample("building config");
                        var types = GetAllTypes();
                        _Configuration = BuildConfiguration(types);
                        Profiler.EndSample();
                    }
                }
            }

            return _Configuration;
        }
    }

    /// <summary>
    /// 此引用重载时更新
    /// </summary>
    public static IServiceProvider ServiceProvider
    {
        get
        {
            if (_ServiceProvider == null)
            {
                lock (gate)
                {
                    if (_ServiceProvider == null)
                    {
                        Profiler.BeginSample("building services collection");

#if ENABLE_IL2CPP
                        //禁用DI动态类型引擎
                        AppContext.SetSwitch("Microsoft.Extensions.DependencyInjection.DisableDynamicEngine",true);
#endif

                        var serviceCollection = new ServiceCollection();
                        // ReSharper disable once RedundantTypeArgumentsOfMethod
                        serviceCollection.AddSingleton<IConfiguration>(Configuration);

                        var types = GetAllTypes();

                        AppendServiceByAttribute(serviceCollection, types);
                        PublicOnDIContextBuildingEvent(serviceCollection, types);

                        serviceCollection.AddSingleton<IGameServiceProvider, GameServiceProvider>(v =>
                            new GameServiceProvider(v));
                        _ServiceProvider = serviceCollection.BuildServiceProvider();

                        Application.wantsToQuit += () =>
                        {
                            Dispose();
                            return true;
                        };
                        Profiler.EndSample();
                    }
                }
            }

            return _ServiceProvider;
        }
    }

    private static ConfigurationRoot BuildConfiguration(List<Type> allType)
    {
        var builder = new ConfigurationBuilder();

        var comparer = Comparer<(IOnConfigurationBuildingCallback ins, int priority)>.Create((a, b) =>
        {
            var order2 = a.priority - b.priority;
            if (order2 != 0) return order2; //优先权小的排在前面
            //不允许排序值完全相等
            throw new InvalidOperationException(
                $"Type={a.ins.GetType()} has same priority={a.priority} to type={b.ins.GetType()}");
        });

        Profiler.BeginSample("Discover IOnConfigurationBuildingCallback");
        var lst = new List<(IOnConfigurationBuildingCallback ins, int priority)>();
        foreach (var type in allType)
        {
            if (!type.IsSealedAndImplement(typeof(IOnConfigurationBuildingCallback)))
            {
                continue;
            }

            var priorityAtb = type.GetCustomAttribute<ConfigPriorityAttribute>();
            if (priorityAtb == null) continue;

            var ins = type.CreatInstance<IOnConfigurationBuildingCallback>();
            if (ins == null)
            {
                Debug.LogError($"Creat instance of {type} fail");
                continue;
            }

            lst.Add((ins, priorityAtb.Priority));
        }
        Profiler.EndSample();

        Profiler.BeginSample("Sort IOnConfigurationBuildingCallback");
        lst.Sort(comparer);
        //Debug.Log($"IOnConfigurationBuildingCallback list:{lst.Select(v=>v.ins.GetType().Name).JoinAsOneString()}");
        Profiler.EndSample();
        
        foreach (var onConfigurationBuildingCallback in lst)
        {
            Profiler.BeginSample($"Callback config:{onConfigurationBuildingCallback.GetType().Name}");
            onConfigurationBuildingCallback.ins.OnCallback(builder);
            Profiler.EndSample();
        }

        // Debug.Log($"Sources:{builder.Sources.Select(v=>v.ToString()).JoinAsOneString("\n")}");
        Profiler.BeginSample("build");
        var providers = new List<IConfigurationProvider>();
        foreach (IConfigurationSource source in builder.Sources)
        {
            // var st = new Stopwatch();
            // st.Start();
            Profiler.BeginSample($"{source.GetType().Name} building");
            IConfigurationProvider provider = source.Build(builder);
            Profiler.EndSample();
            // Debug.Log($"{source.GetType().Name} building cost:{st.Elapsed}");
            // st.Stop(); 
            providers.Add(provider);
        }
        var root = new ConfigurationRoot(providers);
        // var root = (ConfigurationRoot)builder.Build();
        Profiler.EndSample();
        // Debug.Log(root.GetDebugView());
        return root;
    }

    private static void AppendServiceByAttribute(IServiceCollection serviceDescriptors,
        IEnumerable<Type> types)
    {
        HashSet<Type> typesAdded = new HashSet<Type>();
        foreach (var type in types)
        {
            ServiceAttribute serviceAttribute = type.GetCustomAttribute<ServiceAttribute>();
            if (serviceAttribute == null)
            {
                continue;
            }

            var serviceType = serviceAttribute.ServiceType ?? type;
            var isNotInjected = typesAdded.Add(serviceType);
            if (!isNotInjected)
            {
                var implemented = serviceDescriptors.FirstOrDefault(v => v.ServiceType == type);
                if (implemented == null)
                {
                    var exist = typesAdded.FirstOrDefault(v => v.FullName == type.FullName);
                    throw new Exception(
                        $"There are conflict types.type={serviceType},imp={type},exist={exist}.It may occur while 2 asb in memory.");
                }
                else
                {
                    throw new Exception(
                        $"The serviceType is already in services.type={serviceAttribute.ServiceType},exist={implemented.ImplementationType!.Name},implement={type.Name}");
                }
            }

            serviceDescriptors.Add(new ServiceDescriptor(serviceType, type, ServiceLifetime.Singleton));
        }
    }

    private static void PublicOnDIContextBuildingEvent(IServiceCollection services, List<Type> types)
    {
        foreach (var type in types)
        {
            if (!type.IsClassAndImplement(typeof(IOnDIContextBuildingCallback)))
            {
                continue;
            }

            if (!type.IsSealed)
            {
#if UNITY_EDITOR
                if (!type.IsAbstract)
                {
                    // @formatter:off
                    Debug.LogWarning($"Type={type} implements {nameof(IOnDIContextBuildingCallback)} but not sealed,it will be ignored.");
                    // @formatter:on
                }
#endif
                continue;
            }

            var ins = (IOnDIContextBuildingCallback)Activator.CreateInstance(type);
            ins.OnCallback(services, Configuration, types);
        }
    }

    /// <summary>
    /// （在加载完Hotfix后）重新构建所有Configuration和ServiceProvider
    /// </summary>
    public static void Rebuild()
    {
        Dispose();
        _ = Configuration;
        _ = ServiceProvider;
    }

    private static void Dispose()
    {
        lock (gate)
        {
            if (_ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _Configuration?.Dispose();

            _ServiceProvider = null;
            _Configuration = null;
        }
    }

    private static List<Type> GetAllTypes()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .ToList();
        return types;
    }

#if UNITY_EDITOR
    /// <summary>
    /// UnityEditor会有一些尝试从容器中获取服务的操作，并且不能惊动容器构建，就要这个变量来判定一下容器是否被构建了
    /// </summary>
    public static bool IsServiceCreated
    {
        get
        {
            lock (gate)
            {
                return _ServiceProvider != null;
            }
        }
    }
#endif

    /// <summary>
    /// 输出所有配置。仅用于Debug
    /// </summary>
    public static string? GetConfigDebugView()
    {
        if (_Configuration == null) return null;
        var view = _Configuration.GetDebugView();
        return view;
        // var sb = new StringBuilder(view);
        //
        // sb.AppendLine("\n\n[File Configuration Sources]");
        // foreach (var provider in _Configuration.Providers.OfType<FileConfigurationProvider>())
        // {
        //     sb.AppendLine($"- {provider.Source.Path}");
        // }

        // return sb.ToString();
    }
}