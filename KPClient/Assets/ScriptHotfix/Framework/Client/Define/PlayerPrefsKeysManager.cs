using HotScripts.Hotfix.GamePlay.FrameworkSystems.SoundManagerSystem;
using NexCore.DI;
using UnityEngine;

namespace SharedCode.Define
{
    [Service(typeof(PlayerPrefsKeysManager))]
    public sealed class PlayerPrefsKeysManager
    {
        public string GlobalVolumeKey {get;} = "GlobalVolume";

        public string GetSoundVolumeLayerKey(VolumeLayer volumeLayer) => $"SoundVolumeLayer{(int)volumeLayer}";
        
        /// <summary>
        /// 当前语言
        /// </summary>
        public string LanguageKey { get; } = "Language";
    }
}