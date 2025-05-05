using System;
using System.Collections.Generic;

namespace ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions
{
    /// <summary>
    /// thread safe
    /// 只管理文本类多语言。不管理本地化，例如时区、美术资源替换等
    /// 最佳实践：只在游戏业务中可能出现玩家打开配置界面变更语言的场合下，才监听语言管理器
    /// </summary>
    public interface ILanManager
    {
        /// <summary>
        /// 与对应的表名应一一对应，例如English对应EnglishTable
        /// </summary>
        string CurLanguage { get; set; }

        //note:使用接口而不是Delegate可以减少gc
        void Add(ILanObserver observer);
        void Remove(ILanObserver observer);
        //note：使用Delegate以方便使用
        void Add(Action<ILanManager> action);
        void Remove(Action<ILanManager> action);
        
        //迁移到单独的管理：时区
        //TimeSpan TimeOffset { get; set; }
        string GetText(int id);
        //迁移到时区管理
        //string FormatTime(DateTimeOffset time);

        IReadOnlyList<string> GetLanguageSupported();
    }
}