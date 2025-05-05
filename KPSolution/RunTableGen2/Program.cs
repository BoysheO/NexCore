// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text.Json;
using BoysheO.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProtocToolScripts;
using ServicesCommon.Manager;
using SolutionConfigSpace;
using TableDataGenerator;
using WorkFlow;
using WorkFlowUtil;

//原来的设计是先生成表格代码，然后将这些表格代码编译成dll，然后加载这个dll，然后用这个dll去生成序列化数据
//实践问题是生成的代码表格可以是高度自定义的，要将这些代码编译成dll会引入不必要的复杂性。
//最终方案是以官方代码生成为标准，生成表格数据
Console.WriteLine("RunTableGen!");
var solutionConfig = new SolutionConfig();
var fac = LoggerFactory.Create(v => v.AddConsole());
var logger = fac.CreateLogger("");

#region 生成客户端的Table代码

{
    var dirs = solutionConfig.ClientTableDirs.Select(v => new DirectoryInfo(v)).ToArray();
    //读入Excel数据
    var excelTables = await ExcelReader.BuildAsync(logger, dirs);

    /*
     * locate protoc  定位protoc的位置 这里使用的是增加Odin支持和池化的protoc 要改为使用原始的protoc，
     * 下面传参的时候使用ProtoToolsInformationAttribute.GetProtoToolsInformationAttribute()
     * 返回的值即可。参见表格数据生成与后端代码生成
     */

    #region

    var sys = CommonPlatformDetection.GetOSKind();
    var cpu = CommonPlatformDetection.GetProcessArchitecture();
    var protoc = $"UnityProtocWithOdin/{sys}_{cpu}/protoc".ToLowerInvariant();
    if (sys == CommonPlatformDetection.OSKind.Windows)
    {
        protoc += ".exe";
    }

    var curDir = Directory.GetCurrentDirectory();
    protoc = Path.Combine(curDir, protoc);
    protoc = protoc.Replace("\\", "/");

    #endregion

    //取得电脑内protoc的位置信息
    var info = ProtoToolsInformationAttribute.GetProtoToolsInformationAttribute();
    //生成客户端的TableModel
    await PbModelGenerator.Gen(new[]
        {
            //生成到前端的代码目录
            SolutionConfig.ClientTableModelCodeSaveToDir,
        },
        excelTables,
        CancellationToken.None,
        new ProtoToolsInformation()
        {
            WellKnownTypes = info.WellKnownTypes,
            Protobuf_ProtocFullPath = protoc,
            gRPC_PluginFullPath = info.gRPC_PluginFullPath
        },
        "c");

    //所有代码已经生成到SolutionConfig.ClientTableModelCodeSaveToDir目录
    Console.WriteLine("Client model gen done");
}

#endregion

#region 导出客户端的Excel表数据

{
    var dirs = solutionConfig.ClientTableDirs.Select(v => new DirectoryInfo(v)).ToArray();
    //读入Excel数据
    var excelTables = await ExcelReader.BuildAsync(logger, dirs);

    //取得电脑内protoc的位置信息
    var info = ProtoToolsInformationAttribute.GetProtoToolsInformationAttribute();
    //生成随机路径，作为构建tableModel的Dll的代码放置目录
    var tempPath = Path.GetTempPath() + Random.Shared.Next();
    await PbModelGenerator.Gen(
        new[] { tempPath, },
        excelTables,
        CancellationToken.None,
        new ProtoToolsInformation()
        {
            WellKnownTypes = info.WellKnownTypes,
            Protobuf_ProtocFullPath = info.Protobuf_ProtocFullPath,
            gRPC_PluginFullPath = info.gRPC_PluginFullPath
        }, "c");
    //生成客户端的TableModel 补充partial代码
    await ExternGenerator.GenerateInterfaceCodeAsync(excelTables, new string[]
        {
            SolutionConfig.ClientTableModelCodeSaveToDir,
        },
        CancellationToken.None,
        "c");
    //所有代码已经生成到SolutionConfig.ClientTableModelCodeSaveToDir目录

    string ProjectFileContent = $"""
                                 <Project Sdk="Microsoft.NET.Sdk">
                                 
                                     <PropertyGroup>
                                         <TargetFramework>netstandard2.0</TargetFramework>
                                     </PropertyGroup>
                                 
                                     <ItemGroup>
                                       <PackageReference Include="Google.Protobuf" Version="3.25.1" />
                                     </ItemGroup>

                                 </Project>

                                 """;

    //使用tempPath目录中的代码进行TableBytes生成
    await PbDataGenerator.Gen(
        solutionConfig.ClientTableBytesSaveToDirs,
        excelTables,
        tempPath,
        CancellationToken.None,
        ProjectFileContent,
        "c");
}

#endregion

//接下来是服务器的Table生成

#region 后端Table生成

{
    //读入Excel数据
    var excelTables = await ExcelReader.BuildAsync(
        logger,
        solutionConfig.ServerTableDirs
            .Select(v => new DirectoryInfo(v))
            .ToArray()
    );

//生成随机路径，作为构建tableModel的Dll的代码放置目录
    var tempPath = Path.GetTempPath() + Random.Shared.Next();
//取得电脑内protoc的位置信息
    var info = ProtoToolsInformationAttribute.GetProtoToolsInformationAttribute();
//生成后端的TableModel（使用官方默认protoc）
    await PbModelGenerator.Gen(
        new string[]
        {
            //生成到后端的代码目录
            SolutionConfig.ServerTableModelCodeSaveToDir,
            //生成到dll临时目录
            tempPath,
        },
        excelTables,
        CancellationToken.None,
        new ProtoToolsInformation()
        {
            WellKnownTypes = info.WellKnownTypes,
            Protobuf_ProtocFullPath = info.Protobuf_ProtocFullPath,
            gRPC_PluginFullPath = info.gRPC_PluginFullPath
        },
        "s");
//生成客户端的TableModel 补充partial代码 这些代码在tableByte生成中不会用得到也不会影响生成，所以不需要生成到tempPath目录去
    await ExternGenerator.GenerateInterfaceCodeAsync(
        excelTables,
        new string[]
        {
            SolutionConfig.ServerTableModelCodeSaveToDir,
        },
        CancellationToken.None,
        "s");
    Console.WriteLine("Serer model gen done");
//使用tempPath目录中的代码进行TableBytes生成
    await PbDataGenerator.Gen(
        solutionConfig.ServerTableBytesSaveToDirs,
        excelTables,
        tempPath,
        CancellationToken.None,
        null,
        "s");
    Console.WriteLine("Server bytes gen done.");

    #region 最后还要构建Manifest

    using var sha = SHA1.Create();
    foreach (var dir in solutionConfig.ServerTableBytesSaveToDirs)
    {
        var ins = new TableBytesManifestDto();
        //获取所有文件
        var tbFiles = Directory.GetFiles(dir).Where(v => v.AsPath().GetExtension() == ".bytes")
            .Select(v => v.Replace("\\", "/")).ToArray();
        foreach (var tbFile in tbFiles)
        {
            //将形如 C:/EnglishTable.bytes 这样的路径提取表名 English
            var tbName = tbFile.AsPath().GetFileNameWithoutExt().AsSpan().SkipTailCount("Table").ToString();
            var bytes = File.ReadAllBytes(tbFile);
            //计算hash
            var hash = sha.ComputeHash(bytes);
            var hashStr = WebEncoders.Base64UrlEncode(hash);
            var newName = $"{hashStr}.bytes";
            var newFileName = $"{tbFile.AsPath().GetDirectoryName()!.Value.Value}/{newName}";
            Console.WriteLine($"move {tbFile} to {newFileName}");
            //将文件名修改为{hash}.bytes
            if (File.Exists(newFileName))
            {
                throw new Exception($"hash冲突：这个表{tbName}和某个表数据可能完全一致导致冲突");
            }

            File.Move(tbFile, newFileName);
            //在Manifest里记录下新名字
            ins.FileNames.Add(new TableBytesManifestDto.Entry()
            {
                FileName = newName,
                TableName = tbName,
            });
        }

        //将manifest保存
        var json = JsonSerializer.Serialize(ins);
        File.WriteAllText($"{dir}/{SolutionConfig.TableBytesManifestFileName}", json);
    }

    #endregion
}

#endregion