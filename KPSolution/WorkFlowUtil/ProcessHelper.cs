using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BoysheO.Extensions;
using Microsoft.Extensions.Logging;

namespace WorkFlow
{
    public class ProcessHelper
    {
        /// <summary>
        /// withLog参数暂时不支持多线程
        /// </summary>
        /// <param name="mainCommand"></param>
        /// <param name="arg"></param>
        /// <param name="withLog"></param>
        /// <returns></returns>
        [Obsolete]
        public static async Task<(bool isSuccess, string log, int code)> InvokeAsync(string mainCommand, string arg,
            bool withLog = true)
        {
            var id = new Random().Next();
            Console.WriteLine($"id={id} thread={Thread.CurrentThread.ManagedThreadId} exec : {mainCommand} {arg} ");
            var psi = new ProcessStartInfo(mainCommand, arg)
            {
                RedirectStandardOutput = true
            };
            //启动
            var proc = Process.Start(psi);
            if (proc == null)
            {
                if (withLog) Console.WriteLine("Can not exec.");
                return (false, "", -1);
            }

            if (withLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("-------------Start read standard output(maybe missing some log)--------------");
                Console.ResetColor();
            }

            var sb = new StringBuilder();
            //开始读取
            using (var sr = proc.StandardOutput)
            {
                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    sb.AppendLine(line);
                    if (withLog) Console.WriteLine(line);
                }

                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }

            if (withLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("---------------Read end------------------");
                Console.ResetColor();
            }

            proc.WaitForExit();
            // Console.WriteLine($"Total execute time :{(proc.ExitTime-proc.StartTime).TotalMilliseconds} ms");//invalid in linux
            Console.WriteLine($"Exited Code ： {proc.ExitCode} id={id} threadId={Thread.CurrentThread.ManagedThreadId}");
            return (true, sb.ToString(), proc.ExitCode);
        }

        /// <summary>
        /// withLog参数暂时不支持多线程
        /// </summary>
        /// <param name="mainCommand"></param>
        /// <param name="arg"></param>
        /// <param name="withLog"></param>
        /// <returns></returns>
        [Obsolete]
        public static async Task<(bool isSuccess, string log, int code)> InvokeInMacAsync(string mainCommand,
            string arg,
            bool withLog = true)
        {
            var id = new Random().Next();
            var cmd = string.Format("-c \"sudo {0} {1}\"", mainCommand, arg);
            Console.WriteLine($" id={id} thread={Thread.CurrentThread.ManagedThreadId} exec :/bin/bash {cmd}");
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = cmd
            };

            //启动
            var proc = Process.Start(psi);
            if (proc == null)
            {
                if (withLog) Console.WriteLine("Can not exec.");
                return (false, "", -1);
            }

            if (withLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("-------------Start read standard output--------------");
                Console.ResetColor();
            }

            var sb = new StringBuilder();
            //开始读取
            using (var sr = proc.StandardOutput)
            {
                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    sb.AppendLine(line);
                    if (withLog) Console.WriteLine(line);
                }

                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }

            if (withLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("---------------Read end------------------");
                Console.ResetColor();
            }

            // Console.WriteLine($"Total execute time :{(proc.ExitTime-proc.StartTime).TotalMilliseconds} ms");//invalid in linux
            Console.WriteLine($"Exited Code ： {proc.ExitCode} id={id} threadId={Thread.CurrentThread.ManagedThreadId}");
            return (true, sb.ToString(), proc.ExitCode);
        }

        [Obsolete("无法判断需要执行chmod的情况")]
        public static async Task<(bool isSuccesss, string processlog, int code)> Invoke2Async(string cmd,
            bool printLog = true, CancellationToken token = default)
        {
            if (token.IsCancellationRequested) return (false, "", -1);
            var id = new Random().Next();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var head = $"[id:{id}][thread:{threadId}]";
            var (fileName, argument) = GetProcessCommand(cmd);
            Console.WriteLine($"{head}exec:{fileName} {argument}");
            //tips:如果目标命令是一个可执行程序，并且这个程序没有chmod777 那么会报错没有找到这个命令
            using var proc = System.Diagnostics.Process.Start(new ProcessStartInfo()
            {
                Arguments = argument,
                FileName = fileName,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false
            }) ?? throw new Exception("creat process fail");
            // ReSharper disable once AccessToDisposedClosure
            using var cancelToken = token.Register(() => proc.Kill());
            if (printLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{head}-------------Start read standard output--------------");
                Console.ResetColor();
            }

            using var tk2 = new CancellationTokenSource();
            using var tk3 = CancellationTokenSource.CreateLinkedTokenSource(tk2.Token, token);

            var sb = new StringBuilder();
            var t1 = Task.Run(async () =>
            {
                //开始读取
                using var sr = proc.StandardOutput;
                while (!sr.EndOfStream)
                {
                    tk3.Token.ThrowIfCancellationRequested();
                    var line = await sr.ReadLineAsync();
                    lock (sb)
                    {
                        sb.AppendLine(line);
                    }

                    if (printLog) Console.WriteLine("[Normal]" + head + line);
                }
            }, tk3.Token);

            var t2 = Task.Run(async () =>
            {
                using var sr = proc.StandardError;
                while (!sr.EndOfStream)
                {
                    tk3.Token.ThrowIfCancellationRequested();
                    var line = await sr.ReadLineAsync();
                    lock (sb)
                    {
                        sb.AppendLine(line);
                    }

                    if (printLog) Console.WriteLine("[Error]" + head + line);
                }
            }, tk3.Token);

            while (!proc.HasExited)
            {
                await Task.Yield();
            }

            tk2.Cancel();

            try
            {
                await Task.WhenAll(t1, t2);
            }
            catch (OperationCanceledException)
            {
                //ignore
            }

            if (printLog)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{head}---------------Read end------------------");
                Console.ResetColor();
            }

            // Console.WriteLine($"Total execute time :{(proc.ExitTime-proc.StartTime).TotalMilliseconds} ms");//invalid in linux
            Console.WriteLine($"{head}ExitCode={proc.ExitCode}");
            return (proc.ExitCode == 0, sb.ToString(), proc.ExitCode);
        }

        private static (string FileName, string Argument) GetProcessCommand(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ("CMD.exe", $"/c \"{command}\"");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return ("bash", $"-c \"sudo {command}\"");
            }

            throw new NotImplementedException("this platform haven't supported");
        }
    }
}