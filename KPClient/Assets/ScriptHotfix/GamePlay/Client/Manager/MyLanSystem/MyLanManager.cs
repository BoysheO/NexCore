using System;
using System.Collections.Generic;
using System.Linq;
using NexCore.DI;
using NexCore.UnityEnvironment;
using Hotfix.LanMgr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScriptBuildIn.Engine.Client.Configuration;
using ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions;
using SharedCode.Define;
using UnityEngine;

namespace ScriptGamePlay.Hotfix.ClientCode.Manager.MyLanSystem
{
    public class MyLanManager : ILanManager
    {
        private readonly IOptionsMonitor<AppSettingModel> _appSetting;
        private readonly Lan _lan;
        private bool _isLanInit = false;
        private readonly PlayerPrefsKeysManager _keysManager;

        public MyLanManager(IOptionsMonitor<AppSettingModel> appSetting, Lan lan, PlayerPrefsKeysManager keysManager)
        {
            _appSetting = appSetting;
            _lan = lan;
            _keysManager = keysManager;
        }

        /// <summary>
        /// 重新初始化语言：
        /// 游戏启动时的语言值取自AppSetting配置的值，进入游戏后，应当重新配置
        /// 如果玩家从未手动配置过语言选项，则取环境变量作为当前语言值首值（取环境变量可能会从SDK中获取，而SDK往往比lanMgr
        /// 初始化要晚，所以要在合适的时候执行Reinit）
        /// 如果玩家已经手动配置过语言选项，则按玩家配置来设置当前语言值
        /// </summary>
        public void Reinit()
        {
            _ = TrySetLanFromUserSetting() || TrySetLanFromSDK() || SetLanFromUnityEnv();
        }

        /// <summary>
        /// 标记当前语言值为玩家选择，下次启动游戏不再从环境变量中初始化语言而是从玩家配置初始化
        /// </summary>
        public void UserSelectLan(string lanKey)
        {
            EnsureLanSupported(lanKey);
            PlayerPrefs.SetString(_keysManager.LanguageKey, _lan.CurLanguage);
            this.CurLanguage = lanKey;
        }

        private void InitLanIfNeed()
        {
            if (_isLanInit) return;
            //初始化为默认语言
            _ = TrySetLanFromUserSetting() || SetLanFromAppSetting();
            _isLanInit = true;
        }

        private bool TrySetLanFromUserSetting()
        {
            if (PlayerPrefs.HasKey(_keysManager.LanguageKey))
            {
                var userLan = PlayerPrefs.GetString(_keysManager.LanguageKey);
                if (GetLanguageSupported().Contains(userLan))
                {
                    _lan.CurLanguage = userLan;
                    return true;
                }
            }

            return false;
        }

        private bool SetLanFromAppSetting()
        {
            _lan.CurLanguage = _appSetting.CurrentValue.DefaultLanguage;
            return true;
        }

        private bool SetLanFromUnityEnv()
        {
            var ue = DIContext.ServiceProvider.GetRequiredService<IUnityEnvironment>();
            var curLan = ConvertLan(ue.SystemLanguage.ToString());
            _lan.CurLanguage = curLan;
            return true;
        }

        private bool TrySetLanFromSDK()
        {
#if ENABLENABLE_STEAMSYSTEM
            var stlan = stMgr.GetLan();
            lanOp.Read(true, default);
            var curLan = MyLanManager.ConvertLan(stlan);
            _lan.CurLanguage = curLan;
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 将多语言别称转换为支持的多语言
        /// </summary>
        public string ConvertLan(string lan)
        {
            if (lan == null) throw new ArgumentNullException(nameof(lan));
            if (_appSetting.CurrentValue.LanguageSupport.Contains(lan)) return lan;
            return _appSetting.CurrentValue.LanguageConvert.TryGetValue(lan, out var v) ? v : "";
        }

        public string CurLanguage
        {
            get { return PlayerPrefs.GetString(_keysManager.LanguageKey, ""); }
            //注意，这里不应该设置PlayerPrefs。要保存玩家选择，需使用UserSelectLan(str)
            set
            {
                EnsureLanSupported(value);
                _lan.CurLanguage = value;
            }
        }

        private void EnsureLanSupported(string lan)
        {
            if (!GetLanguageSupported().Contains(lan))
            {
                throw new Exception($"lan={lan} is not supported");
            }
        }

        public void Add(ILanObserver observer)
        {
            InitLanIfNeed();
            _lan.Add(observer);
        }

        public void Remove(ILanObserver observer)
        {
            _lan.Remove(observer);
        }

        public void Add(Action<ILanManager> action)
        {
            InitLanIfNeed();
            _lan.Add(action);
        }

        public void Remove(Action<ILanManager> action)
        {
            _lan.Remove(action);
        }

        public string GetText(int id)
        {
            InitLanIfNeed();
            return _lan.GetText(id);
        }

        public IReadOnlyList<string> GetLanguageSupported()
        {
            return _appSetting.CurrentValue.LanguageSupport;
        }

        private sealed class Lan2OnBuildingCallback : IOnDIContextBuildingCallback
        {
            public void OnCallback(IServiceCollection collection, IConfiguration configuration,
                IReadOnlyList<Type> allTypes)
            {
                collection.AddSingleton<Lan>();
                collection.AddSingleton<MyLanManager>();
                collection.AddSingleton<ILanManager>(v => v.GetRequiredService<MyLanManager>());
            }
        }
    }
}