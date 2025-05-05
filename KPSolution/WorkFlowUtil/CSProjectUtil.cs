using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BoysheO.Extensions;

// using Microsoft.Build.Locator;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.MSBuild;

namespace WorkFlow
{
    public class CSProjectUtil
    {
        private static (string FileName, string Argument) GetProcessCommand(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ("CMD.exe", "/c " + command);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return ("bash", $"-c \"sudo {command}\"");
            }

            throw new NotImplementedException();
        }

        public static async Task BuildProjectByDotnetCommandAsync(FileInfo projectInfo, bool isDebugMode,
            string framework,
            bool printLog = true)
        {
            string command =
                $"dotnet build {projectInfo.FullName} -f {framework} -c {(isDebugMode ? "Debug" : "Release")}";
            Console.WriteLine(command);
            var (fileName, argument) = GetProcessCommand(command);
            using var process = System.Diagnostics.Process.Start(new ProcessStartInfo()
            {
                Arguments = argument,
                FileName = fileName,
                CreateNoWindow = true,
                RedirectStandardError = printLog,
                RedirectStandardOutput = printLog,
                StandardErrorEncoding = printLog ? Encoding.UTF8 : null,
                StandardOutputEncoding = printLog ? Encoding.UTF8 : null,
                UseShellExecute = false
            }) ?? throw new Exception("creat process fail");

            if (printLog)
            {
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null) Console.WriteLine(args.Data);
                };


                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(args.Data);
                        Console.ResetColor();
                    }
                };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            process.WaitForExit();
            // await process.WaitForExitAsync();//this net version not support
            //log all error
            if (process.ExitCode != 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Build failed.file:" + projectInfo.FullName);
                // Console.WriteLine(await process.StandardError.ReadToEndAsync());
                Console.ResetColor();
                throw new Exception("build failed");
            }
            else
            {
                Console.WriteLine("build success");
            }
        }

        public static async Task<ImmutableSortedSet<string>> GetPlatformSupported(FileInfo csProjectFile)
        {
            var xml = await csProjectFile.OpenText().ReadToEndAsync();
            var csProject = XDocument.Parse(xml);
            var aa = csProject.Root?
                .Element("PropertyGroup")?
                .Element("TargetFramework")?
                .Value;
            var bb = csProject.Root?
                .Element("PropertyGroup")?
                .Element("TargetFrameworks")?
                .Value
                .Split(';');
            if (aa == null && bb == null)
            {
                return ImmutableSortedSet<string>.Empty;
            }

            var builder = ImmutableSortedSet.CreateBuilder<string>();
            if (aa != null) builder.Add(aa);
            if (bb != null)
            {
                foreach (var b in bb)
                {
                    builder.Add(b);
                }
            }

            return builder.ToImmutable();
        }


        public static void EnableDefine(FileInfo projectFileInfo, string define, bool isEnable)
        {
            var xml = projectFileInfo.OpenText().ReadToEnd();
            var csProject = XDocument.Parse(xml);
            var defineElements = csProject.Root?.Element("PropertyGroup")?.Element("DefineConstants");
            throw new NotImplementedException();
        }
        
        public static async Task<BuildResult> BuildProjectByDotnetCommandAsync2(
            FileInfo projectInfo, bool isDebugMode,
            string framework,
            bool printLog = true)
        {
            await BuildProjectByDotnetCommandAsync(projectInfo, isDebugMode, framework, printLog);
            var dir = projectInfo.Directory!.FullName.AsPath();
            var name = projectInfo.FullName.AsPath().GetFileNameWithoutExt();

            var resultDll = dir.Combine($"bin/{(isDebugMode ? "Debug" : "Release")}/{framework}/{name}.dll");
            var resultPdb = dir.Combine($"bin/{(isDebugMode ? "Debug" : "Release")}/{framework}/{name}.pdb");
            var resultXml = dir.Combine($"bin/{(isDebugMode ? "Debug" : "Release")}/{framework}/{name}.xml");
            return new BuildResult
            {
                DllPath = resultDll.Value.Replace('\\','/'),
                PdbPath = resultPdb.Value.Replace('\\','/'),
                XmlPath = resultXml.Value.Replace('\\','/'),
            };
        }

        public class BuildResult
        {
            public void Deconstruct(out string dllPath, out string pdbPath, out string xmlPath)
            {
                dllPath = DllPath;
                pdbPath = PdbPath;
                xmlPath = XmlPath;
            }

            public string DllPath;
            public string PdbPath;
            public string XmlPath;
        }
    }
}