using NexCore.DI;
using NexCore.UnityEnvironment;
using BoysheO.Extensions.Unity3D.Extensions;
using Hotfix.FrameworkSystems.CameraSystem;
using Unity.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using YooAsset;

namespace Hotfix.FrameworkSystems.GameManagerSystem
{
    /// <summary>
    /// 提供场景中的各种单例Root
    /// </summary>
    [Service(typeof(GameManager))]
    public class GameManager
    {
        private readonly IGameServiceProvider _gameServiceProvider;
        private readonly IUnityEnvironment _unityEnvironment;

        public GameManager(IGameServiceProvider gameServiceProvider, IUnityEnvironment unityEnvironment)
        {
            _gameServiceProvider = gameServiceProvider;
            _unityEnvironment = unityEnvironment;
        }

        public GameObject Root
        {
            get
            {
                var go = GameObject.FindWithTag("GameController");
                if (go) return go;
                var tk = YooAssets.LoadAssetAsync<GameObject>("GameManager.prefab");
                tk.WaitForAsyncComplete();
                var prefab = tk.GetAssetObject<GameObject>();
                var ins = Object.Instantiate(prefab);
                tk.Dispose();
                Object.DontDestroyOnLoad(ins);
                CameraHelper.SetInternalRaySendFrom(Camera.main.ThrowIfNullOrFakeNull());
                //PreCamera是解决AppMain界面如果没Camera的话，安卓上会闪屏的问题（可能和fpsCounter有关，总之就是会有问题）
                var preCamera = GameObject.Find("PreCamera");
                if (preCamera != null) Object.Destroy(preCamera); //删除预设PreCameraa
                return ins;
            }
        }

        /// <summary>
        /// globalVolume应该有自己的Manager，但现在先就这么写
        /// </summary>
        public Volume GlobalVolume
        {
            get
            {
                var volume = Root.Child("Global Volume");
                var v = volume.GetRequireComponent<Volume>();
                return v;
            }
        }
    }
}