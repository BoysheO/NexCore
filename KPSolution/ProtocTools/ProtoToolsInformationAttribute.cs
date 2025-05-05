using System.Reflection;

namespace ProtocToolScripts
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ProtoToolsInformationAttribute(string wellKnownTypes, string protobufProtocFullPath, string gRPCPluginFullPath)
        : Attribute
    {
        public string WellKnownTypes { get; } = wellKnownTypes;
        public string Protobuf_ProtocFullPath { get; } = protobufProtocFullPath;
        public string gRPC_PluginFullPath { get; } = gRPCPluginFullPath;

        public static ProtoToolsInformationAttribute GetProtoToolsInformationAttribute()
        {
            var atb =
                typeof(ProtoToolsInformationAttribute).Assembly.GetCustomAttribute<ProtoToolsInformationAttribute>();
            if (string.IsNullOrWhiteSpace(atb.WellKnownTypes) ||
                string.IsNullOrWhiteSpace(atb.Protobuf_ProtocFullPath) ||
                string.IsNullOrWhiteSpace(atb.gRPC_PluginFullPath))
            {
                throw new Exception("只可以使用dotnet run命令行执行工程，否则无法注入相关路径");
            }

            return atb;
        }
    }
}