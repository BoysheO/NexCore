using BoysheO.Extensions;
using ProtocToolScripts;
using SolutionConfigSpace;
using WorkFlow;
using WorkFlowUtil;

Console.WriteLine("GenClientProtoBuf!");

#region locate protoc

var sys = CommonPlatformDetection.GetOSKind();
var cpu = CommonPlatformDetection.GetProcessArchitecture();
var protoc = $"UnityProtoC/{sys}_{cpu}/protoc".ToLowerInvariant();
if (sys == CommonPlatformDetection.OSKind.Windows)
{
    protoc += ".exe";
}

if (sys != CommonPlatformDetection.OSKind.Windows)
{
    await ProcessHelper.Invoke2Async($"chmod 777 {protoc}");
}

var curDir = Directory.GetCurrentDirectory();
protoc = Path.Combine(curDir, protoc);
protoc = protoc.Replace("\\", "/");

#endregion

var config = new SolutionConfig();
foreach (var (configProtoDir, protoOutputDir) in config.ProtoDirs)
{
    //跳过没有实际创建的输入文件夹和空输入文件夹
    if(!Directory.Exists(configProtoDir))continue;
    var files = Directory.GetFiles(configProtoDir).Where(v=>v.AsPath().GetExtension() == ".proto").ToArray();
    if (files.Length == 0) continue;
    //输出文件夹如果不存在就创建
    if (!Directory.Exists(protoOutputDir)) Directory.CreateDirectory(protoOutputDir);
    
    //清空输出文件夹的cs文件
    foreach (var file in Directory.EnumerateFiles(protoOutputDir)
                 .Where(v => v.AsPath().GetExtension() == ".cs"))
    {
        File.Delete(file);
    }

    var info = ProtoToolsInformationAttribute.GetProtoToolsInformationAttribute();
    await ProtocUtil.GenProto(configProtoDir, config.ProtoInclude, protoOutputDir, true, new ProtoToolsInformation()
    {
        WellKnownTypes = info.WellKnownTypes,
        Protobuf_ProtocFullPath = protoc,
        gRPC_PluginFullPath = info.gRPC_PluginFullPath
    },true);
}

Console.WriteLine($"done.");
////PS C:\Program Files (x86)\grpc\bin> .\protoc.exe --csharp_out=. --grpc_out=. --plugin=protoc-gen-grpc=.\grpc_csharp_plugin.exe .\DemConDataTransfer.proto