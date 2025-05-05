using System;
using NexCore.DI;
using NexCore.UnityEnvironment;
using GameFramework;
using UniRx;
using UnityEngine.Profiling;

namespace ScriptGamePlay.Hotfix.ShareCode.Manager.ProfileSystem
{
    [Service(typeof(IProfileManager))]
    public class ProfileManager : IProfileManager
    {
        private readonly IUnityEnvironment _unityEnvironment;

        public ProfileManager(IUnityEnvironment unityEnvironment)
        {
            _unityEnvironment = unityEnvironment;
        }

        public bool IsEnable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="context">建议传入CallerContext</param>
        /// <typeparam name="T">当前类</typeparam>
        /// <returns></returns>
        public IDisposable BeginSampleProfile<T>(string method, string? context = null)
        {
            if (!IsEnable)
            {
                return Disposable.Empty;
            }

            if (method == null) throw new ArgumentNullException(nameof(method));
            _unityEnvironment.ThrowIfNotMainThread();
            var un = Disposable.Create(() => Profiler.EndSample());
            using var _ = zstring.Block();
            zstring s = "[" + nameof(T) + "]" + (zstring)method;
            if (context != null)
            {
                s += " - ";
                s += context;
            }

            Profiler.BeginSample(s);
            return un;
        }
    }
}