using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SolutionConfigSpace
{
    public class SolutionConfig
    {
        public const string RootDir = EnvironmentConfig.Root;
        public const string EditorDllDir = EnvironmentConfig.UnityEditorDlls;

        //----------下面这些一般不用动----------

        #region 这些一般不用动

        public const string CUnity3DProjectDir = RootDir + "/" + CUnity3DProjectName;

        public const string SolutionConfigProjectFilePath =
            SolutionDir + "/SolutionConfig/SolutionConfig.csproj";

        public const string SolutionConfigDllInUnity = CUnity3DProjectDir + "/Assets/Editor";

        public const string SolutionDir = RootDir + "/" + EnvironmentConfig.Solution;
        const string CUnity3DProjectName = EnvironmentConfig.Client;

        public const string SolutionSlnFile = SolutionDir + "/" + EnvironmentConfig.Solution + ".sln";

        public const string ScriptAssembliesDir = CUnity3DProjectDir + "/Library/ScriptAssemblies";

        [Obsolete("use CUnity3DProjectDir instead")]
        public DirectoryInfo Unity3DProjectDir { get; } = new DirectoryInfo(CUnity3DProjectDir);

        [Obsolete("use CUnity3DProjectName instead")]
        public string Unity3DProjectName { get; } = CUnity3DProjectName;

        /// <summary>
        /// 使用workFlow编译时，需要读取project所在SolutionFolder来决定dll和pdb的存放位置
        /// </summary>
        [Obsolete("use CSlnFile instead")] public readonly FileInfo SlnFile = new FileInfo(SolutionSlnFile);

        /// <summary>
        ///  使用workFlow编译时，HotfixMono下的工程编译后的dll和pdb放置到此文件夹(编辑器)
        /// </summary>
        [Obsolete("use CHotfixDllDirInUnityEditor instead")]
        public readonly DirectoryInfo HotfixDllDirInUnityEditor =
            new DirectoryInfo(CHotfixDllDirInUnityEditor);

        public const string CHotfixDllDirInUnityEditor = CUnity3DProjectDir + "/Assets/Scripts/Hotfix";

        public const string CNormalMonoProjectDir = CUnity3DProjectDir + "/Assets/Scripts/NormalMono";

        [Obsolete("use CNormalMonoProjectDir instead")]
        public readonly DirectoryInfo NormalMonoProjectDir = new DirectoryInfo(CNormalMonoProjectDir);

        /// <summary>
        ///  使用workFlow编译时，HotfixMono下的工程编译后的(dll)bytes和pdb(pdb.bytes)放置到此文件夹(打测试包时)
        /// </summary>
        [Obsolete("use CHotfixDllBytesDir instead")]
        public readonly DirectoryInfo HotfixDllBytesDir =
            new DirectoryInfo(CHotfixDllBytesDir);

        public const string CHotfixDllBytesDir = CUnity3DProjectDir + "/Assets/AddressableResources/ASB";

        /// <summary>
        ///  使用workFlow编译时，HotfixMono下的工程编译后的dll和pdb放置到此文件夹(打测试包时)
        /// *此举是为了HybridCLR进行AOT扫描
        /// </summary>
        public readonly DirectoryInfo HybridCLRHotfixProjectDirOnProductMode =
            new DirectoryInfo($@"{CUnity3DProjectDir}/temp/ASB");

        /// <summary>
        ///  使用workFlow编译时，HotfixEditor下的工程编译后的dll和pdb放置到此文件夹
        ///  *运行时会删除
        /// </summary>
        public readonly DirectoryInfo HotfixEditorDir =
            new DirectoryInfo($@"{CUnity3DProjectDir}/Assets/Editor/HotfixEditor");

        /// <summary>
        /// 指定WorkFlow的project以buildMenu
        /// </summary>
        public readonly FileInfo WorkFlowProjectFile =
            new FileInfo(@$"{SolutionDir}/WorkFlow/WorkFlow.csproj");

        /// <summary>
        /// 前端表数据
        /// </summary>
        public string[] ClientTableDirs = new string[]
        {
            SolutionDir + "/Resources/Tables/Both",
            SolutionDir + "/Resources/Tables/Language",
        };

        /// <summary>
        /// 后端表数据
        /// </summary>
        public string[] ServerTableDirs = new string[]
        {
            SolutionDir + "/Resources/Tables/Both",
            SolutionDir + "/Resources/Tables/Language",
            SolutionDir + "/Resources/Tables/ServerOnly",
        };

        public const string Unity3DProjectAssetsDir = CUnity3DProjectDir + "/Assets";

        public const string Unity3DPackageDir = CUnity3DProjectDir + "/Packages";

        /// <summary>
        /// BuildMenu时生成的cs文件
        /// </summary>
        [Obsolete] public readonly FileInfo WorkFlowMenuGenerateCsFile =
            new FileInfo(
                $@"{CUnity3DProjectDir}/Assets/Editor/CustomEditorScript/WorkFlowMenu/WorkFlowMenu.Generate.cs");

        public readonly string UIScriptDir = $"{Unity3DProjectAssetsDir}/HotScripts/Hotfix/GamePlay/UIScripts";

        /// <summary>
        /// ExcelModel生成时存放的位置
        /// 生成时删除文件夹内所有文件
        /// *特指前端
        /// </summary>
        // @formatter:off
        public const string ClientTableModelCodeSaveToDir = Unity3DProjectAssetsDir + "/ScriptHotfix/GamePlay/Client/Generate/Table";
        // @formatter:on

        /// <summary>
        /// ExcelModel生成时存放的位置
        /// 生成时删除文件夹内所有文件
        /// *特指后端
        /// </summary>
        // @formatter:off
        public const string ServerTableModelCodeSaveToDir = SolutionDir + "/ServicesCommon/TableModel";
        // @formatter:on

        /// <summary>
        /// 生成表Model构建程序集时用到
        /// </summary>
        // @formatter:off
        [Obsolete("修改逻辑，不用了",true)]
        public const string SirenixOdinInspectorAttributesDLL = CUnity3DProjectDir + "/Assets/Plugins/Sirenix/Assemblies/Sirenix.OdinInspector.Attributes.dll";
        // @formatter:on

        /// <summary>
        /// Excel序列化数据生成时存放的位置
        /// 生成时删除文件夹内所有文件
        /// *特指客户端的表数据路径
        /// </summary>
        public readonly string[] ClientTableBytesSaveToDirs =
        {
            $"{CUnity3DProjectDir}/Assets/Resources_Hotfix/Bytes/Table",
        };

        /// <summary>
        /// Excel序列化数据生成时存放的位置
        /// 生成时删除文件夹内所有文件
        /// *特指后端的表数据路径
        /// </summary>
        public readonly string[] ServerTableBytesSaveToDirs =
        {
            $"{SolutionDir}/Resources/TablesBytes",
        };

        /// <summary>
        /// 后端表格数据生成后，需要同时生成Manifest文件以供CDNTableManager使用
        /// </summary>
        public const string TableBytesManifestFileName = "TableBytesManifest.json";

        /// <summary>
        /// input dir to output dir
        /// 服务器自动拥有ProtobufFile文件夹下所有协议代码，所以不要生成到SharedCode里，以免前后端同时存在同一份代码
        /// </summary>
        public IReadOnlyDictionary<string, string> ProtoDirs { get; } = new Dictionary<string, string>()
        {
            // @formatter:off
            [SolutionDir + "/Resources/ProtobufFile/BuildIn/Engine/ClientCode"] = Unity3DProjectAssetsDir + "/ScriptBuildIn/Engine/Client/Generate/Protobuf",
            [SolutionDir + "/Resources/ProtobufFile/BuildIn/Framework/ClientCode"] = Unity3DProjectAssetsDir + "/ScriptBuildIn/Framework/Client/Generate/Protobuf",
            [SolutionDir + "/Resources/ProtobufFile/BuildIn/GamePlay/ClientCode"] = Unity3DProjectAssetsDir + "/ScriptBuildIn/GamePlay/Client/Generate/Protobuf",
            
            [SolutionDir + "/Resources/ProtobufFile/Hotfix/Engine/ClientCode"] = Unity3DProjectAssetsDir + "/ScriptHotfix/Engine/Client/Generate/Protobuf",
            [SolutionDir + "/Resources/ProtobufFile/Hotfix/Framework/ClientCode"] = Unity3DProjectAssetsDir + "/ScriptHotfix/Framework/Client/Generate/Protobuf",
            [SolutionDir + "/Resources/ProtobufFile/Hotfix/GamePlay/ClientCode"] = Unity3DProjectAssetsDir + "/ScriptHotfix/GamePlay/Client/Generate/Protobuf",
            
            // @formatter:on
        };

        public readonly string[] ProtoInclude = new[]
        {
            $"{SolutionDir}/Resources/ProtobufFile", //要把根目录指示出来否则报错
        };

        private const string _ProgramConstConfigCSPath = SolutionDir + @"/ProgramConstConfig/ProgramConstConfig.cs";

        public string ProgramConstConfigCSPath => _ProgramConstConfigCSPath;

        public readonly string ProgramConstConfigJsonPath =
            $@"{SolutionDir}/ProgramConstConfigGenerator/Resources/ProgramConstConfig.json";

        /// <summary>
        /// 自己加上平台
        /// </summary>
        public readonly string HotfixPatchDllRootDir = $@"{CUnity3DProjectDir}/HybridCLRData/AssembliesPostIl2CppStrip";

        public string AddressableSettingOptionJson
        {
            get
            {
                var assembly = typeof(SolutionConfig).Assembly;
                using var stream =
                    assembly.GetManifestResourceStream("SolutionConfig.Resources.AddressableSettingOptionJson.json");
                using var reader = new StreamReader(stream);
                var text = reader.ReadToEnd();
                return text;
            }
        }

        private const string _ProgramConstConfigProjectFile =
            SolutionDir + @"/ProgramConstConfig/ProgramConstConfig.csproj";

        /// <summary>
        /// ProgramConstConfig工程的位置。每次ConstConfig变更都要重新编译到Unity。
        /// </summary>
        public string ProgramConstConfigProjectFile => _ProgramConstConfigProjectFile;

        /// <summary>
        /// HCLR的Path文件放置到什么地方
        /// </summary>
        public const string PatchBytesSaveDir = CUnity3DProjectDir + "/Assets/AddressableResources/Patch";

        /// <summary>
        /// 执行Tools/GenLoaderData代码生成时，覆盖这个文件
        /// </summary>
        public const string LoaderDataCsFile = CUnity3DProjectDir + @"/Assets/HotScripts/Loader/Data.cs";

        #endregion
    }
}