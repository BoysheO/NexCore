using System.Diagnostics.CodeAnalysis;
using System.IO;
using Tomlyn;

namespace WorkspaceConfigReaderSystem
{

//only design for server
    public class WorkspaceConfigReader
    {
        public WorkspaceConfigModel IORead()
        {
            var tomlFile = EnvironmentConfig.ConfigFullFileName;
            var tomlConfig = File.ReadAllText(tomlFile);
            var model = Toml.ToModel<WorkspaceConfigModel>(tomlConfig, options: new TomlModelOptions()
            {
                IgnoreMissingProperties = true
            });
            return model;
        }
    }
}