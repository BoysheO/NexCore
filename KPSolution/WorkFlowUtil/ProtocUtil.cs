using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoysheO.Extensions;
using BoysheO.ProcessSystem;

namespace WorkFlow
{
    public sealed class ProtoToolsInformation
    {
        public string WellKnownTypes { get; set; }
        public string Protobuf_ProtocFullPath { get; set; }
        public string gRPC_PluginFullPath { get; set; }
    }

    public enum GenOption
    {
        Both,
        ServerOnly,
        ClientOnly,
    }

    public struct Option
    {
        public string InputDir;
        public string[] IncludeDirs;
        public string OutputDir;
        public bool IsIncludeWellKnownTypes;
        public ProtoToolsInformation ProtoToolsInformation;
        public bool UsingGrpcPlugin;
        public GenOption GenOption;
    }

    public static class ProtocUtil
    {
        /// <summary>
        /// 不会清空目标文件夹，需要自己在外部处理。会创建文件夹.
        /// 对于自定义的protoc，需要:Mac中先在系统设置中放行protoc，
        /// 然后执行chmod 777来确保有运行权限
        /// </summary>
        /// <param name="inputDir">文件夹绝对路径,形如 C:\3865667989587855948 </param>
        /// <param name="extraIncludeDir">不需要添加WellKnownTypes和inputDir自身，工具已做好此工作</param>
        /// <param name="protoOutputDir"></param>
        /// <param name="isIncludeWellKnownTypes">是否在生成中引用WellKnownTypes</param>
        /// <param name="protoToolsInformation"></param>
        /// <param name="usingGrpc">是否生成grpc</param>
        public static async Task GenProto(string inputDir, string[] extraIncludeDir, string protoOutputDir,
            bool isIncludeWellKnownTypes, ProtoToolsInformation protoToolsInformation, bool usingGrpc = true)
        {
            if (protoToolsInformation == null) throw new ArgumentNullException(nameof(protoToolsInformation));
            var atb = protoToolsInformation;
            var cmd = new StringBuilder();
            var input = inputDir + Path.DirectorySeparatorChar + "*.proto";
            cmd.Append(" ");
            cmd.Append(input);
            foreach (var include in extraIncludeDir)
            {
                cmd.Append(" -I=");
                cmd.Append(include);
            }

            #region IncludeWellKnwonTypes

            if (isIncludeWellKnownTypes)
            {
                cmd.Append(" -I=");
                cmd.Append(atb.WellKnownTypes);
            }

            #endregion

            #region IncludeInputDir 将根目录指示出来

            if (!extraIncludeDir.ToList().Contains(inputDir))
            {
                cmd.Append(" -I=");
                cmd.Append(inputDir);
            }

            #endregion

            var outputDir = new DirectoryInfo(protoOutputDir);
            if (!outputDir.Exists) outputDir.Create();


            cmd.Append(" --csharp_out=");
            cmd.Append(protoOutputDir);
            cmd.Append(" ");

            if (usingGrpc)
            {
                cmd.Append("--grpc_out=");
                cmd.Append(protoOutputDir);
                cmd.Append(" ");
                cmd.Append("--plugin=protoc-gen-grpc=");
                string protocPluginPath = atb.gRPC_PluginFullPath;
                cmd.Append(protocPluginPath);
            }

            string protocPath = atb.Protobuf_ProtocFullPath;
            await ProcessHelper.Invoke2Async($"{protocPath} {cmd}", true);
        }

        /// <summary>
        /// 对于自定义的protoc，需要自己执行chmod 777来确保能够运行起来
        /// </summary>
        /// <param name="singleProtoFile">文件绝对路径,形如 C:\3865667989587855948\a.proto </param>
        /// <param name="extraIncludeDir">额外引用的文件夹，不需要添加WellKnownTypes和singleProtoFile自身，工具已做好此工作。可以传Empty以表达不用include，不能传null</param>
        /// <param name="protoOutputDir"></param>
        /// <param name="isIncludeWellKnownTypes">是否在生成中引用WellKnownTypes</param>
        /// <param name="protoToolsInformation"></param>
        public static async Task GenSingleProto(string singleProtoFile, string[] extraIncludeDir, string protoOutputDir,
            bool isIncludeWellKnownTypes, ProtoToolsInformation protoToolsInformation)
        {
            if (extraIncludeDir == null) throw new ArgumentNullException(nameof(extraIncludeDir));
            if (protoToolsInformation == null) throw new ArgumentNullException(nameof(protoToolsInformation));
            var atb = protoToolsInformation;
            var cmd = new StringBuilder();
            var input = singleProtoFile;
            cmd.Append(" ");
            cmd.Append(input);
            foreach (var include in extraIncludeDir)
            {
                cmd.Append(" -I=");
                cmd.Append(include);
            }

            #region IncludeWellKnwonTypes

            if (isIncludeWellKnownTypes)
            {
                cmd.Append(" -I=");
                cmd.Append(atb.WellKnownTypes);
            }

            #endregion

            #region IncludeInputDir 将根目录指示出来

            if (!extraIncludeDir.ToList().Contains(singleProtoFile.AsPath().GetDirectoryName()!.Value.Value))
            {
                cmd.Append(" -I=");
                cmd.Append(singleProtoFile.AsPath().GetDirectoryName()!.Value.Value);
            }

            #endregion

            var outputDir = new DirectoryInfo(protoOutputDir);
            if (!outputDir.Exists) outputDir.Create();


            cmd.Append(" --csharp_out=");
            cmd.Append(protoOutputDir);
            cmd.Append(" ");

            cmd.Append("--grpc_out=");
            cmd.Append(protoOutputDir);
            cmd.Append(" ");

            cmd.Append("--plugin=protoc-gen-grpc=");

            string protocPluginPath = atb.gRPC_PluginFullPath;
            string protocPath = atb.Protobuf_ProtocFullPath;
            cmd.Append(protocPluginPath);

            await ProcessHelper.Invoke2Async($"{protocPath} {cmd}", true);
        }


        /// <summary>
        /// 不会清空目标文件夹，需要自己在外部处理。会创建文件夹.
        /// 对于自定义的protoc，需要z自己执行chmod 777来确保能够运行起来
        /// </summary>
        public static async Task GenProto(Option option)
        {
            var atb = option.ProtoToolsInformation;
            var cmd = new StringBuilder();
            var input = option.InputDir + Path.DirectorySeparatorChar + "*.proto";
            cmd.Append(" ");
            cmd.Append(input);
            foreach (var include in option.IncludeDirs)
            {
                cmd.Append(" -I=");
                cmd.Append(include);
            }

            #region IncludeWellKnwonTypes

            if (option.IsIncludeWellKnownTypes)
            {
                cmd.Append(" -I=");
                cmd.Append(atb.WellKnownTypes);
            }

            #endregion

            #region IncludeInputDir 将根目录指示出来

            if (!option.IncludeDirs.ToList().Contains(option.InputDir))
            {
                cmd.Append(" -I=");
                cmd.Append(option.InputDir);
            }

            #endregion

            var outputDir = new DirectoryInfo(option.OutputDir);
            if (!outputDir.Exists) outputDir.Create();

            cmd.Append(" --csharp_out=");
            cmd.Append(option.OutputDir);
            cmd.Append(" ");

            if (option.UsingGrpcPlugin)
            {
                cmd.Append("--grpc_out=");
                cmd.Append(option.OutputDir);
                cmd.Append(" ");
                cmd.Append("--plugin=protoc-gen-grpc=");
                string protocPluginPath = atb.gRPC_PluginFullPath;
                cmd.Append(protocPluginPath);
            }

            string protocPath = atb.Protobuf_ProtocFullPath;
            await ProcessHelper.Invoke2Async($"{protocPath} {cmd}", true);
        }
    }
}