// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Nodes;
using BoysheO.Extensions;

//这个脚本是通过识别.verdaccio-db.json所在目录的文件夹，重建.verdaccio-db.json文件。
//需要有一个已经存在的.verdaccio-db.json文件，它有个secret字段，生成程序不了解生成机制，生成不了这个字段
Console.WriteLine("Hello, World!");

const string verdaccio_db_json = @"E:\verdaccio\storage\data\.verdaccio-db.json";
string[] exceptFolder = new[]
{
    "npm",
    "package"
};

var dirName = verdaccio_db_json.AsPath().GetDirectoryName() ?? throw new Exception("no dir");
var dir1 = new DirectoryInfo(dirName.Value);
var pckDirs = dir1.EnumerateDirectories().Where(v => !exceptFolder.Contains(v.Name));
var pcks = pckDirs.Select(v => v.Name).ToArray();
var content = File.ReadAllText(verdaccio_db_json);
var node = JsonSerializer.Deserialize<JsonNode>(content);
var listNode = node.Root["list"].AsArray();
var hashSet = new HashSet<string>();
foreach (var jsonNode in listNode)
{
    hashSet.Add(jsonNode.AsValue().ToString());
}

foreach (var pck in pcks)
{
    hashSet.Add(pck);
}

listNode.Clear();
foreach (var s in hashSet)
{
    listNode.Add(s);
}

File.WriteAllText(verdaccio_db_json, node.ToJsonString());