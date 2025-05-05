using System;
using System.IO;
using System.Reflection;
using Tomlyn;

public static class EnvironmentConfigHelper
{
    const string tomlFile = "environmentconfig.toml";

    public static EnvironmentConfigModel ReadConfig()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream(tomlFile) ??
                     throw new Exception($"missing {tomlFile},please link it ,set it as EmbeddedResource and let it named {tomlFile}");
        //示例xml    
        // <ItemGroup>
        // <EmbeddedResource Include="..\..\environmentconfig.toml">
        // <LogicalName>environmentconfig.toml</LogicalName>
        // </EmbeddedResource>
        // </ItemGroup>
       
        string solutionConfigTomlString;
        using (stream)
        {
            var reader = new StreamReader(stream);
            using (reader)
            {
                solutionConfigTomlString = reader.ReadToEnd();
            }
        }

        var model = Toml.ToModel<EnvironmentConfigModel>(solutionConfigTomlString,options:new TomlModelOptions()
        {
            IgnoreMissingProperties = true
        });
        return model;
    }}