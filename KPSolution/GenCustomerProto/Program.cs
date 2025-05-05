// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using BoysheO.Extensions;
using ProtocToolScripts;
using WorkFlow;
using WorkFlowUtil;

Console.WriteLine("Hello, World!");

const string file = @"D:\Repository\FangZhou\FangZhouClient\Assets\ScriptGamePlay\Hotfix\ShareCode\Manager\MyQuestSystem\SerialModel\serialData.proto";

// var output = file.AsPath().GetDirectoryName()!.Value.Combine(file.AsPath().GetFileNameWithoutExt().MakeFirstCharUpperOrNot() + ".cs").Value;
// var output = file.AsPath().GetDirectoryName()!.Value.Value;
var output = @"D:\Repository\FangZhou\FangZhouClient\Assets\ScriptGamePlay\Hotfix\ShareCode\Manager\MyQuestSystem\SerialModel";
Console.WriteLine(output);
// #region locate protoc
//
// var sys = CommonPlatformDetection.GetOSKind();
// var cpu = CommonPlatformDetection.GetProcessArchitecture();
// var protoc = $"UnityProtocWithOdin/{sys}_{cpu}/protoc".ToLowerInvariant();
// if (sys == CommonPlatformDetection.OSKind.Windows)
// {
//     protoc += ".exe";
// }
//
// var curDir = Directory.GetCurrentDirectory();
// protoc = Path.Combine(curDir, protoc);
// protoc = protoc.Replace("\\", "/");
//
// #endregion

var info = ProtoToolsInformationAttribute.GetProtoToolsInformationAttribute();
await ProtocUtil.GenSingleProto(file, Array.Empty<string>(), output, true, new ProtoToolsInformation()
{
    WellKnownTypes = info.WellKnownTypes,
    Protobuf_ProtocFullPath = info.Protobuf_ProtocFullPath,
    gRPC_PluginFullPath = info.gRPC_PluginFullPath
});
