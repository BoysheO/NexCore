using System;
using ScriptBuildIn.Engine.Client.Configuration;

namespace Framework.Hotfix.ClientCode.Configuration
{
    public static class HotfixConstConfig
    {
#if UNITY_EDITOR
        private const bool EDITORTRUE = true;
#else
        private const bool EDITORTRUE = false;
#endif

        public const bool IsDebug = BuildInConstConfig.IsDebug;
        
        /// <summary>
        /// 一般的等待协议超时时间
        /// </summary>
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        public const string FallbackIcon = "FallbackIcon";
    }
}