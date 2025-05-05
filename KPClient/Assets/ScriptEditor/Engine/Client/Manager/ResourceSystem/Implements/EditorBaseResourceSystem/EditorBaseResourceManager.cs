using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using BoysheO.Extensions;
using Hotfix.ResourceMgr.Abstractions;
using NexCore.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hotfix.ResourceMgr.Implements.Editor
{
    public class EditorBaseResourceManager : IResourceManager
    {
        private readonly AssetsIndexModel _assetsIndexModel = new();
        private Dictionary<ResourceId, ResourceKey> _tk2key = new();
        private Dictionary<ResourceKey, int> _key2count = new();
        private ulong _id;

        public EditorBaseResourceManager()
        {
            var file = GetAssetsIndexFilePath();
            if (!File.Exists(file))
            {
                Debug.LogWarning($"missing assets index file at {file}.\nExample will be creat at {GetTemplatePath()}");
                CreatExample();
                return;
            }

            var t = File.ReadAllText(file);
            var model = JsonSerializer.Deserialize<AssetsIndexModel>(t);
            _assetsIndexModel = model;
        }

        private void CreatExample()
        {
            var ins = new AssetsIndexModel()
            {
                Key2Path = new Dictionary<string, string>()
                {
                    { "ExampleA", "Assets/Prefabs/Player.prefab" },
                    { "ExampleB", "Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader" }
                }
            };
            var json = System.Text.Json.JsonSerializer.Serialize(ins);
            var file = GetTemplatePath();
            var dir = file.AsPath().GetDirectoryName();
            if (!Directory.Exists(dir.Value.Value)) Directory.CreateDirectory(dir.Value.Value);
            File.WriteAllText(file, json);
        }

        public static string GetAssetsIndexFilePath()
        {
            var file = Application.dataPath.AsSpan().SkipTailCount("/Assets").ToString()
                       + "/ProjectSettings/EditorBaseResourceSystem/AssetsIndex.json";
            return file;
        }

        private string GetTemplatePath()
        {
            return GetAssetsIndexFilePath() + ".template";
        }

        public ResourceId BeginLoad(ResourceKey key, CancellationToken token)
        {
            if (!_assetsIndexModel.Key2Path.ContainsKey(key.Value))
            {
                Debug.LogError($"No such resource={key.Value},please write it to {GetAssetsIndexFilePath()}");
            }
            var newId = new ResourceId(_id++, this);
            _tk2key[newId] = key;
            if (!_key2count.TryAdd(key, 1))
            {
                _key2count[key]++;
            }

            return newId;
        }

        public bool IsContains(ResourceKey key)
        {
            if (!_assetsIndexModel.Key2Path.TryGetValue(key.Value, out var v)) return false;
            var ins = AssetDatabase.LoadAssetAtPath(v, key.Type);
            return ins != null;
        }

        public bool IsDisposed(ResourceId token)
        {
            return _tk2key.ContainsKey(token);
        }

        public T GetResource<T>(ResourceId token)
        {
            if (!typeof(T).IsClassAndImplement(typeof(UnityEngine.Object)))
                throw new NotSupportedException("only UnityObject support");
            var key = EnsureKey(token);
            if (!_assetsIndexModel.Key2Path.TryGetValue(key.Value, out var path))
            {
                return default;
            } 
            var ass = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            try
            {
                return (T)(System.Object)ass;
            }
            catch
            {
                return default;
            }
        }

        private ResourceKey EnsureKey(ResourceId token)
        {
            if (!_tk2key.TryGetValue(token, out var key)) throw new ObjectDisposedException(nameof(token));
            return key;
        }

        public float GetProgress(ResourceId token)
        {
            EnsureKey(token);
            return 1;
        }

        public ResourceKey GetKey(ResourceId token)
        {
            return EnsureKey(token);
        }

        public IEnumerator WaitAsync(ResourceId token)
        {
            yield break;
        }

        public void Wait(ResourceId token)
        {
            return;
        }

        public void Release(ResourceId token)
        {
            if (!_tk2key.Remove(token, out var key)) return;
            _key2count[key]--;
        }

        public void UnloadUnusedAssets()
        {
            //do nothing
        }

        public SceneId LoadSceneAsync(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false)
        {
            throw new NotSupportedException();
        }

        public float GetProgress(SceneId sceneId)
        {
            throw new NotSupportedException();
        }

        public Scene GetSceneInstance(SceneId sceneId)
        {
            throw new NotSupportedException();
        }

        public void ActivateScene(SceneId sceneId)
        {
            throw new NotSupportedException();
        }

        public void Unsuspend(SceneId sceneId)
        {
            throw new NotSupportedException();
        }

        public void Unload(SceneId sceneId, Action? onDone)
        {
            throw new NotSupportedException();
        }
    }
}