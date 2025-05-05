using System;
using UnityEngine;

namespace ScriptBuildIn.Engine.Client.Configuration
{
    public static partial class BuildInConstConfig
    {
#if UNITY_EDITOR
        private const bool EDITORTRUE = true;
        [Obsolete] public const int YOOMODE_OFFLINE_BUT_EDITOR = YOOMODE_EDITOR;
        [Obsolete] public const int YOOMODE_ONLINE_BUT_EDITOR = YOOMODE_EDITOR;
        [Obsolete] public const int LOADMODE_ASSET_BUT_EDITOR = LOADMODE_INTERNAL;
        [Obsolete] public const int ASSEMBLYLOAD_NOSKIP_BUT_EDITOR = ASSEMBLYLOAD_SKIP;
#else
        private const bool EDITORTRUE = false;
        [Obsolete]public const int YOOMODE_OFFLINE_BUT_EDITOR = YOOMODE_OFFLINE;
        [Obsolete]public const int YOOMODE_ONLINE_BUT_EDITOR = YOOMODE_ONLINE;
        [Obsolete]public const int LOADMODE_ASSET_BUT_EDITOR = LOADMODE_ASSET;
        [Obsolete]public const int ASSEMBLYLOAD_NOSKIP_BUT_EDITOR = ASSEMBLYLOAD_NOSKIP;
#endif

        public const int LOADMODE_INTERNAL = 0; //热更代码加载模式-内置模式。不加载热更代码，直接使用内置代码。不使用热更和UnityEditor时，使用此模式。
        public const int LOADMODE_IO = 1; //热更代码加载模式-IO模式。加载热更代码时，通过配置的IO路径直接读取Dll加载。只有Win/Mac支持。这个模式是便于出Win/Mac包快速调试热更代码，不用走一遍打包流程将热更代码打包。理论上是可以直接脱离Unity开发的。但我实操中没这样做过，可能需要补全逻辑
        public const int LOADMODE_ASSET = 2;//热更代码加载模式-读包模式。从资源包里加载热更代码

        public const int YOOMODE_EDITOR = 0;//资源包加载模式-Editor模式。
        public const int YOOMODE_OFFLINE = 1;//资源包加载模式-纯本地模式。单机包使用。
        public const int YOOMODE_ONLINE = 2; //资源包加载模式-远程模式。此更新方式通过读取配置中的CDN地址，根据CDN上获取的DefaultPackage.version完成热更新，工作原理比较简单，适合demo和小型项目。
        public const int YOOMODE_WEBGL = 3; //资源包加载模式-WebGL模式。因为精力有限，WebGL模式在升级YooAsset后坏了，暂时不可使用，后续修复。
        public const int YOOMODE_ONLINE_WITH_REMOTE = 4; //资源包加载模式-远程模式2。此更新方式通过与逻辑服务器沟通来获取DefaultPackage.version而不是从CDN中读取版本信息，再去CDN取资源包完成热更（本示例工程未实现，可以自己补充）

        /// <summary>
        /// 即YooAsset的CustomPlayMode，未实现。做混合加载会用得上。
        /// </summary>
        public const int YOOMODE_BlendingMode = 5;

        /// <summary>
        /// 是否跳过Patch加载流程
        /// 只有在需要用hclr加载热更代码时才需要
        /// 如果是反射加载dll（Mono），也不需要加载Patch
        /// </summary>
        public const bool IsSkipLoadPatch = IsSkipCodeLauncher;

        /// <summary>
        /// 是否跳过热更代码加载流程
        /// Editor下，默认情况已经包含Hotfix代码，除非手动把Hotfix代码Exclude了，否则可以跳过代码加载流程，以免每次播放都看一遍加载界面
        /// 不使用热更功能时（出纯整包），也无需加载
        /// </summary>
        public const bool IsSkipCodeLauncher = EDITORTRUE;

        public const int TABLEDATALOADMODE_Assets = 0;
        public const int TABLEDATALOADMODE_Excel = 1; //未实现

        /// <summary>
        /// 这个选项决定了如果走完整个CodeLauncher（代码加载）流程，Assembly.Load是否实际执行
        /// 在UnityEditor下，默认已经包含Hotfix代码，如果误加载多一次AB包/IO里的Dll，会造成UnityEditor运行错误，不稳定，重启电脑才能恢复
        /// 因此，一般不要改动这个此值
        /// 但是，开发者有可能需要重新定制CodeLauncher流程的UI，要在UnityEditor下打开CodeLauncher和AssemblyLoad调试
        /// 如果打开AssemblyLoad，务必点击Engine/ExcludeHotfix将编辑器的Hotfix代码隔离出去。这样在UnityEditor就可以走完整的加载流程
        /// （当然了，这种方式是走Mono加载的，不代表真机IL2CPP加载）
        /// </summary>
        public const int AssemblyLoad = ASSEMBLYLOAD_NOSKIP_BUT_EDITOR;

        public const int ASSEMBLYLOAD_NOSKIP = 0;
        public const int ASSEMBLYLOAD_SKIP = 1;

        /// <summary>
        /// 是否跳过Logo加载环节
        /// </summary>
        public const bool IsSkipLogoStep = EDITORTRUE;
        // public const bool IsSkipLogoStep = false;

        /// <summary>
        /// 是否显示内置控制台
        /// </summary>
        public const bool IsShowLogConsole = true;
        
        /// <summary>
        /// 基础包版本号（这里既可以自定义，也可以用Configuration里的值，也可以用Unity的AppVersion。建议是用Unity的AppVersion，在CI/CD流程中修改它）
        /// *现实中发生过同事直接引用const导致hotfix代码取得的版本号是UnityEditor而不是App包的Version值，因此此值不设置为const以防止类似事件再次发生
        /// </summary>
        public static string FrameworkBuildInVer => Application.version;
        
        /// <summary>
        /// 指定表格管理器读取数据的方式。使用Excel方式读取就不用打表了。但是构建时不能用(直接读Excel功能未实现）
        /// </summary>
        public const int TableDataLoadMode = TABLEDATALOADMODE_Assets;

        /// <summary>
        /// Editor下，有个自动将当前git分支名字注入到Configuration的机制，这方便出包时出变体包包里的代码可以查到包的git分支信息
        /// 如果使用的是svn或没有安装git环境，请将此关闭
        /// </summary>
        public const bool IsUseGitBranchInfo = true;
    }
}