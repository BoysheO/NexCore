#nullable enable
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace UpdateSystem.FixedLogic
{
    public static class DownloadHelper
    {
        public static async UniTask<byte[]> Download(string url, IProgress<float>? onProgress = null)
        {
            using var req = UnityWebRequest.Get(url);
            await req.SendWebRequest();
            while (!req.isDone)
            {
                if (onProgress != null)
                {
                    onProgress.Report(req.downloadProgress);
                }

                await UniTask.Yield();
            }

            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"[{req.result}][{url}]{req.error}");
            }

            return req.downloadHandler.data;
        }
    }
}