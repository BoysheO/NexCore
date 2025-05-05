// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

const string template = """
                        <Project>
                            <!--    auto generate,don't edit manually-->
                            <PropertyGroup>
                                <RootDir>{0}</RootDir>
                                <UnityEditorDlls>{1}</UnityEditorDlls>
                                <Unity3DClientDir>{2}</Unity3DClientDir>
                                <SolutionDir>{3}</SolutionDir>
                            </PropertyGroup>
                        </Project>
                        """;
var config = EnvironmentConfigHelper.ReadConfig();
var editorDllDir = config.UnityEditorDlls;
var rootDir = config.Root;
var u3dClientDir = rootDir + "/" + config.Client;
var solutionDir = rootDir + "/" + config.Solution;
var text = string.Format(template, rootDir, editorDllDir, u3dClientDir, solutionDir);
string file = solutionDir + "/Directory.Build.props";
File.WriteAllText(file, text);
Console.WriteLine($"write to {file} done");