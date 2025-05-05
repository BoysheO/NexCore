using BoysheO.Extensions.Configuration.Reloadable.Json;
using UnityEngine;

namespace ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem
{
    public class ResourceConfigurationSource : ReloadableJsonConfigurationSource
    {
        public ResourceConfigurationSource()
        {
            var resJson = Resources.Load<TextAsset>("Appsetting");
            if (resJson != null)
            {
                base.Reload(resJson.bytes);
            }
            else
            {
                Debug.LogWarning("missing Resources/Appsetting.json");
            }
        }
    }
}