#nullable enable
using Hotfix.ResourceMgr.Abstractions;
using NexCore.DI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using BoysheO.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace Hotfix.ResourceMgr.Implements
{
    public sealed class YooAssetsResourceManager2 : IResourceManager
    {
        private const string DefaultPackage = "DefaultPackage";
        private ulong _idVersion;

        private class AssetsInfo
        {
            public readonly YooAssetsResourceManager2 Manager;
            public readonly Action CancelTokenCallback;

            public ResourceId Id;
            public ResourceKey Key;
            public AssetHandle AssetHandle = null!;
            public CancellationTokenRegistration? CancelCancelTokenCallback;

            public AssetsInfo(YooAssetsResourceManager2 manager)
            {
                Manager = manager ?? throw new ArgumentNullException(nameof(manager));
                CancelTokenCallback = () => Manager.Release(Id);
            }
        }

        private readonly Stack<AssetsInfo> _infoPool = new();
        private readonly Dictionary<ResourceId, AssetsInfo> _resId2Info = new();
        private readonly List<SceneInfo> _sceneInfos = new();

        private class SceneInfo
        {
            public SceneId Id;
            public SceneHandle SceneHandle = null!;
        }

        private AssetsInfo GetAssetsInfo()
        {
            return _infoPool.TryPop(out var info) ? info : new AssetsInfo(this);
        }

        private void Return(AssetsInfo assetsInfo)
        {
            assetsInfo.Id = default;
            assetsInfo.Key = default;
            assetsInfo.AssetHandle = default!;
            assetsInfo.CancelCancelTokenCallback = null;
            _infoPool.Push(assetsInfo);
        }

        public ResourceId BeginLoad(ResourceKey key, CancellationToken token)
        {
            var info = GetAssetsInfo();
            var a = YooAssets.LoadAssetAsync(key.Value, key.Type);
            do _idVersion++;
            while (_idVersion == 0);

            info.Id = new ResourceId(_idVersion,this);
            info.Key = key;
            info.AssetHandle = a;
            if (token.IsCancellationRequested)
            {
                info.CancelCancelTokenCallback = token.Register(info.CancelTokenCallback);
            }

            _resId2Info.Add(info.Id, info);
            return info.Id;
        }

        public bool IsContains(ResourceKey key)
        {
            var info = YooAssets.GetAssetInfo(key.Value, key.Type);
            return info.Error != null;
        }

        public bool IsDisposed(ResourceId token)
        {
            return _resId2Info.ContainsKey(token);
        }

        public T GetResource<T>(ResourceId token)
        {
            if (!typeof(T).IsClassAndImplement(typeof(UnityEngine.Object)))
                throw new NotSupportedException("only UnityObject support");
            if (!_resId2Info.TryGetValue(token, out var info)) throw new ObjectDisposedException(nameof(token));
            return (T)(object)info.AssetHandle.AssetObject;
        }

        public float GetProgress(ResourceId token)
        {
            if (!_resId2Info.TryGetValue(token, out var info)) throw new ObjectDisposedException(nameof(token));
            return info.AssetHandle.Progress;
        }

        public ResourceKey GetKey(ResourceId token)
        {
            if (!_resId2Info.TryGetValue(token, out var info)) throw new ObjectDisposedException(nameof(token));
            return info.Key;
        }

        public IEnumerator WaitAsync(ResourceId token)
        {
            if (!_resId2Info.TryGetValue(token, out var info)) throw new ObjectDisposedException(nameof(token));
            return info.AssetHandle;
        }

        public void Wait(ResourceId token)
        {
            if (!_resId2Info.TryGetValue(token, out var info)) throw new ObjectDisposedException(nameof(token));
            info.AssetHandle.WaitForAsyncComplete();
        }

        public void Release(ResourceId token)
        {
            if (!_resId2Info.TryGetValue(token, out var info)) return;
            info.AssetHandle.Dispose();
            info.CancelCancelTokenCallback?.Dispose();
            _resId2Info.Remove(token);
            Return(info);
        }

        public void UnloadUnusedAssets()
        {
            const string pack = "DefaultPackage";
            var op = YooAssets.GetPackage(pack).UnloadUnusedAssetsAsync();
            op.WaitForAsyncComplete();
        }

        public SceneId LoadSceneAsync(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false)
        {
            var h = YooAssets.LoadSceneAsync(sceneName, sceneMode, LocalPhysicsMode.None, suspendLoad);
            do _idVersion++;
            while (_idVersion == 0);

            var info = new SceneInfo()
            {
                SceneHandle = h,
                Id = (SceneId)_idVersion
            };
            _sceneInfos.Add(info);
            return info.Id;
        }

        public float GetProgress(SceneId sceneId)
        {
            if (!TryGetSceneInfo(sceneId, out var info)) throw new ObjectDisposedException(nameof(sceneId));
            return info.SceneHandle.Progress;
        }

        private bool TryGetSceneInfo(SceneId sceneId, out SceneInfo info)
        {
            for (var index = 0; index < _sceneInfos.Count; index++)
            {
                var v = _sceneInfos[index];
                if (v.Id == sceneId)
                {
                    info = v;
                    return true;
                }
            }

            info = default!;
            return false;
        }

        public Scene GetSceneInstance(SceneId sceneId)
        {
            if (!TryGetSceneInfo(sceneId, out var info)) throw new ObjectDisposedException(nameof(sceneId));
            return info.SceneHandle.SceneObject;
        }

        public void ActivateScene(SceneId sceneId)
        {
            if (!TryGetSceneInfo(sceneId, out var info)) throw new ObjectDisposedException(nameof(sceneId));
            if (!info.SceneHandle.ActivateScene()) throw new Exception("ActivateScene fail");
        }

        public void Unsuspend(SceneId sceneId)
        {
            if (!TryGetSceneInfo(sceneId, out var info)) throw new ObjectDisposedException(nameof(sceneId));
            info.SceneHandle.UnSuspend();
        }

        public void Unload(SceneId sceneId, Action? onDone)
        {
            if (!TryGetSceneInfo(sceneId, out var info)) throw new ObjectDisposedException(nameof(sceneId));
            var a = info.SceneHandle.UnloadAsync();
            _sceneInfos.Remove(info);
            if (onDone != null) a.Completed += _ => onDone();
        }
    }
}