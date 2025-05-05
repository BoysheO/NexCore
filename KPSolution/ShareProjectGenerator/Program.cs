// See https://aka.ms/new-console-template for more information

using BoysheO.Extensions;
using BoysheO.Util;
using ShareProjectGenerator;
using SolutionConfigSpace;

Console.WriteLine("Hello, World!");
var files1 = Directory.GetFiles(SolutionConfig.Unity3DProjectAssetsDir, "*.moddef",
    SearchOption.AllDirectories);
var files2 = Directory.GetFiles(SolutionConfig.Unity3DPackageDir, "*.moddef", SearchOption.AllDirectories);
var allModdef = files1.Concat(files2).ToArray();

var root = SolutionConfig.CUnity3DProjectDir;

var moduleNames = allModdef.Select(v => v.AsPath().GetFileNameWithoutExt()).ToArray();
var moduleDir = allModdef.Select(v => v.AsPath().GetDirectoryName().Value.Value.AsSpan().SkipCount(root).ToString().ReplaceBackslash().TrimStart('/')).ToArray();

var generator = new GeneratedTextTransformation();
generator.Session = new Dictionary<string, object>();
generator.Session["GenerateLocation"] = DebugUtil.GetCallerContext();
generator.Session["ModuleNames"] = moduleNames;
generator.Session["ModuleDirs"] = moduleDir;
var code = generator.TransformText();
var file = $"{SolutionConfig.SolutionDir}/SharedProject/SharedProject.projitems";
File.WriteAllText(file,code);
Console.WriteLine($"done.file gen at {file}");