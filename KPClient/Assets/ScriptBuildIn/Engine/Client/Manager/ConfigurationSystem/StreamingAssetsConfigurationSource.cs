using System;
using BoysheO.Extensions.Configuration.Reloadable.Json;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ScriptBuildIn.Engine.Client.Manager.ConfigurationSystem
{
    /// <summary>
    /// 来自SteamingAssets的配置
    /// 在调用LoadAsync之后才会真正加载
    /// </summary>
    public class StreamingAssetsConfigurationSource : ReloadableJsonConfigurationSource
    {
        public static StreamingAssetsConfigurationSource Instance { get; } = new();

        private StreamingAssetsConfigurationSource()
        {
        }

        public async UniTask LoadAsync()
        {
            var bytes = await GetAppsetting();
            if (bytes == null) return;
            Reload(bytes);
        }

        private static async UniTask<byte[]?> GetAppsetting()
        {
            string fileName = "Appsetting.json";
            string url;
            //如果在编译器或者单机中
#if UNITY_EDITOR || UNITY_STANDALONE
            url = "file://" + Application.streamingAssetsPath + "/" + fileName;
            //否则如果在Iphone下
#elif UNITY_IPHONE
            url = "file://" + Application.dataPath + "/Raw/"+ fileName;
            //否则如果在android下
#elif UNITY_ANDROID
            url = "jar:file://" + Application.dataPath + "!/assets/"+ fileName;
#else
            #error NotImplement
#endif
            Debug.Log($"Get url={url}");
            var request = UnityWebRequest.Get(url);
            try
            {
                await request.SendWebRequest();
            }
            catch 
            {
                //ignore：这个异常是UniTask对request返回码的一个封装。下面流程会处理错误码，不用再判别异常
                //note：不少库对UnityWebRequest做了封装，它们对await request.SendWebRequest()的处理可能是不同的，例如ETTask里就不会跳异常
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                var res = request.downloadHandler.data;
                return res;
            }
            else if (request.responseCode == 404)
            {
                Debug.Log($"[{nameof(StreamingAssetsConfigurationSource)}]获取{fileName} - 404");
                return null;
            }
            else
            {
                Debug.Log($"[{nameof(StreamingAssetsConfigurationSource)}]获取{fileName}失败，结果：{request.result}");
                return null;
            }
        }
    }
}