// See https://aka.ms/new-console-template for more information

using SolutionConfigSpace;

Console.WriteLine("Hello, World!");

var dir = SolutionConfig.Unity3DProjectAssetsDir!;
var files = Directory.GetFiles(dir,"*.asmdf");
Dictionary<string,string> tarDirs = new();
//1.如果asmdf文件的同目录下存在的ShareCode无后缀文件，则将asmdf的文件名（去掉asmdf后缀）作为key，asmdef所在目录的路径作为value存到tarDirs中

