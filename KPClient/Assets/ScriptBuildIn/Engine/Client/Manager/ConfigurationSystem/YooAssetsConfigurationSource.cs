using BoysheO.Extensions;
using BoysheO.Extensions.Configuration.Reloadable.Json;
using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem
{
    /// <summary>
    /// 在调用LoadAsync之后才会真正加载
    /// </summary>
    public class YooAssetsConfigurationSource : ReloadableJsonConfigurationSource
    {
        public static YooAssetsConfigurationSource Instance { get; } = new();

        private YooAssetsConfigurationSource()
        {
        }

        public async UniTask LoadAsync()
        {
            var info = YooAssets.GetAssetInfo("Appsetting.json");
            if (info.IsInvalid)
            {
                Debug.Log($"[{nameof(YooAssetsConfigurationSource)}]YooAssets has no Appsetting.json");
                return;
            }
            using var ta = YooAssets.LoadAssetAsync<TextAsset>("Appsetting.json");
            await ta;
            var bytes = ta.GetAssetObject<TextAsset>().TrimFakeNull()?.bytes.ThrowIfNull()!;
            Reload(bytes);
        }
    }
}