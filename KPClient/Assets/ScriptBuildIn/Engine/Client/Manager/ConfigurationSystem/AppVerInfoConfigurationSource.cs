using System.Collections.Generic;
using Microsoft.Extensions.Configuration.Memory;
using ScriptBuildIn.Engine.Client.Configuration;
using UnityEngine;

namespace ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem
{
    public class AppVerInfoConfigurationSource : MemoryConfigurationSource
    {
        public AppVerInfoConfigurationSource()
        {
            this.InitialData = new[]
            {
                new KeyValuePair<string, string?>("AppVer", BuildInConstConfig.FrameworkBuildInVer)
            };
        }
    }
}