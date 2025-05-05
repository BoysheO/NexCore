using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexCore.DI;

namespace ScriptBuildIn.Engine.Client.Configuration
{
    /// <summary>
    /// AppSetting配置模型
    /// 约定：要将游戏内用到的所有配置都在这里罗列出来，即使没用到。这样方便查阅配置。罗列就好，不需要一定用到。
    /// note：不建议在读取配置时通过绑定AppSetting的方式读取具体配置。这样做的话，AppSetting就变成跨BuildIn、Hotfix多个层次都要用到，依赖管理会变得很麻烦，不符合本框架设计理念
    /// </summary>
    public sealed class AppSettingModel
    {
        //这项是不能重新加载的
        public string PatchDir { get; init; }

        //这项是不能重新加载的
        public string DllDir { get; init; }

        /// <summary>
        /// 默认语言表（玩家初始进入游戏，如果没有第三方SDK提供的默认语言就参考使用此语言）
        /// </summary>
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// 语言别称，key=>别称，例如简体中文可能会是‘简体中文’ ‘chs’ ‘ChineseSimplified'等。根据实际SDK/渠道平台上会出现的值来填写
        /// value=>本游戏支持语言表
        /// note：目前，别称来自<see cref="UnityEngine.SystemLanguage"/>和SteamSDK的语言名
        /// ps：犯了蠢，应该命名为LanguageAlias的😅
        /// </summary>
        public Dictionary<string, string> LanguageConvert { get; init; }


        /// <summary>
        /// 本游戏提供多少种语言选项支持(Raw是内置的，不需要在配置中出现；在LanManager的具体实现中包含）
        /// </summary>
        public List<string> LanguageSupport { get; init; }
        
        public string ResourceHost { get; init; }
        public string ResourceHostFallback { get; init; }
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<string> HotfixList { get; init; }
        public string HelloWorldService { get; init; }
        //约定：其值与BuildInConfig.FrameworkBuildInVer完全相同
        public string AppVer { get; init; }
        //Git分支名
        public string Branch { get; init; }
        
        public string WorkSpaceRoot { get; init; }

        //将配置源绑定到AppSetting模型上，并注入容器
        private sealed class AppSettingOnBuildingCallback : IOnDIContextBuildingCallback
        {
            public void OnCallback(IServiceCollection collection, IConfiguration configuration,
                IReadOnlyList<Type> allTypes)
            {
                collection.AddOptions();
                collection.Configure<AppSettingModel>(configuration);
            }
        }
    }
}