using System;

public sealed class ProgramConstConfigModel
{
    /// <summary>
    /// 是否跳过Loader逻辑。这样的话AppMain就不显示logo，不进入Loader，直接进入游戏流程。这样开发快
    /// </summary>
    public bool IsSkipLoader { get; set; }
    
    /// <summary>
    /// Debug模式下，游戏拥有更多Debug信息和设计模型校验
    /// </summary>
    public bool IsDebug { get; set; }

    // /// <summary>
    // /// LoadMode=1时，loader从这个位置IO读取dll和pdb的bytes
    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // /// </summary>
    // public string? ASBDir { get; set; }

    // /// <summary>
    // /// LoadMode=1时，loader从这个位置IO读取PatchDll的bytes
    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // /// </summary>
    // public string? HybridCLRDevPatchDllDir { get; set; }

    /// <summary>
    /// 决定如何读取热更Dll
    /// 0：不读取DLL数据，运行（也就是非热更模式/Editor模式）
    /// 1：读app.json用IO从UnityEditor里面读取（仅windows本机）
    /// 2：使用资源管理系统读取
    /// </summary>
    public int LoadMode { get; set; } = 0;

    public int LOADMODE_INTERNAL => 0;
    public int LOADMODE_IO => 1;
    public int LOADMODE_ASSET => 2;

    /// <summary>
    /// 决定YooAssets的工作模式
    /// 0：编辑器模式
    /// 1：单机模式
    /// 2：联机模式
    /// 3:WebGL
    /// </summary>
    public int YooMode { get; set; } = 0;

    public int YOOMODE_EDITOR => 0;
    public int YOOMODE_OFFLINE => 1;
    public int YOOMODE_ONLINE => 2;

    public int YOOMODE_WEBGL => 3;

    // /// <summary>
    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // /// </summary>
    // public string YooUpdateURL { get; set; } = "http://127.0.0.1:80/AAAGame";
    //
    // /// <summary>
    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // /// </summary>
    // public string YooUpdateFallbackURL { get; set; } = "http://127.0.0.1:80/AAAGame";

    /// <summary>
    /// 读取表格数据的形式
    /// 0：从资源管理读取
    /// 1：使用使用app.json配置，从本地文件Excel通过IO读取
    /// </summary>
    public int TableDataLoadMode { get; set; }

    public int TABLEDATALOADMODE_Assets => 0;
    public int TABLEDATALOADMODE_URL => 1;

    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // public string TableDataLoadURL { get; set; } = "";

    // /// <summary>
    // /// 出包后的默认语言(en,chs,cht,jp,ru....)
    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // /// </summary>
    // public string DefaultLanguage { get; set; } = "en";

    // /// <summary>
    // /// 出包后的默认时区
    // /// *应该使用app.json文件进行配置，以免更改io位置要重新编译
    // /// </summary>
    // public int DefaultTimeOffset { get; set; }

    //好像没用
    // /// <summary>
    // /// 不是hclr打包的时候不用loadPath
    // /// </summary>
    // public bool IsSkipLoadPatch { get; set; }

    /// <summary>
    /// 固定层版本，一般就是固定v1.0 手动操作
    /// </summary>
    public string AppVer { get; set; } = "v1.0";
    
    /// <summary>
    /// 记录打包时的版本号。对git来说是Commit号，对svn来说是revision，由构建流程外部写入
    /// </summary>
    public string VCSVersion { get; set; }
    
    //应在热更程序集内定义
    // /// <summary>
    // /// 热更层版本
    // /// </summary>
    // public string HotVer { get; set; } = "240711Build_Debug";

    /// <summary>
    /// 是否跳过程序集加载
    /// 仅在LoadMode包含加载逻辑时有效 这个选项通常用于调试热更流程
    /// </summary>
    public int AssemblyLoad { get; set; }

    public int ASSEMBLYLOAD_NOSKIP => 0;
    public int ASSEMBLYLOAD_SKIP => 1;
}