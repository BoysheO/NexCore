using System.Collections.Generic;
using System.Text.Json;
using BoysheO.Extensions;
using Microsoft.Extensions.Configuration.Memory;
using UnityEngine;

namespace ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem
{
    //Branch可以由CICD自动生成，因此和Appsetting分离开来
    public class BranchInfoConfigurationSource:MemoryConfigurationSource
    {
        private class BranchInfo
        {
            public string Branch { get; set; }
        }
        public  BranchInfoConfigurationSource()
        {
            var resJson = Resources.Load<TextAsset>("Branch");
            var mode = JsonSerializer.Deserialize<BranchInfo>(resJson.bytes);
            if (mode == null || mode.Branch.IsNullOrWhiteSpace())
            {
                Debug.LogError($"Missing Branch info");
                return;
            }
            InitialData = new[]
            {
                new KeyValuePair<string, string?>("Branch", mode.Branch)
            };
        }
    }
}