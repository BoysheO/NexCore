using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.Json;
using BoysheO.Extensions;
using BoysheO.Util;
using Hotfix.ResourceMgr.Implements.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using YooAsset;

namespace Hotfix.ResourceMgr.Implements.EditorBaseExporter
{
    internal class EditorBaseExporter
    {
        private const string DefaultPackage = "DefaultPackage";

        //根据YooAsset里的TableBytes标记，将这些表格索引添加到编辑器资源管理类的索引中
        [MenuItem("YooAsset/GenerateEditorBaseAssetsIndex")]
        static void GenEditorBase()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            EditorPrefs.SetInt(Key, (int)State.Export);
            EditorApplication.isPlaying = true;
        }

        private enum State
        {
            Stop,
            Export,
        }

        //一个随机值，避免和其它状态混淆
        private const string Key = "E92BD120-EC82-422D-9A0D-864E98920AA2";

        [InitializeOnLoadMethod]
        static void StateMachineRun()
        {
            var sample = CustomSampler.Create(nameof(EditorBaseExporter));
            sample.Begin();
            var state = (State)EditorPrefs.GetInt(Key, (int)State.Stop);
            switch (state)
            {
                case State.Stop:
                    break;
                case State.Export:
                {
                    Debug.Log(DebugUtil.GetCallerContext());
                    //防止后续逻辑出现异常，立即销毁键值
                    EditorPrefs.DeleteKey(Key);
                    //进入play状态后，场景还没切到播放状态，不能直接new出GameObject，否则切换场景后GameObject就销毁了
                    EditorApplication.delayCall += OnDelayCall;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            sample.End();
        }

        private static void OnDelayCall()
        {
            //交给协程处理
            var go = new GameObject();
            var c = go.AddComponent<MyClass>();
            EditorApplication.delayCall -= OnDelayCall;
        }

        private class MyClass : MonoBehaviour
        {
            private IEnumerator Start()
            {
                Debug.Log(DebugUtil.GetCallerContext());
                yield return new WaitForSeconds(0.5f);
                YooAssets.Initialize();
                var package = YooAssets.CreatePackage(DefaultPackage);
                YooAssets.SetDefaultPackage(package);
                var initParameters = new EditorSimulateModeParameters();
                var simulateManifestFilePath =
                    EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters =
                    FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateManifestFilePath.PackageRootDirectory);
                yield return package.InitializeAsync(initParameters);
                Debug.Log("init done");
                var assetInfos = YooAssets.GetAssetInfos("TableBytes");
                var dic = assetInfos.ToDictionary(v => v.Address.IsNullOrEmpty() ? v.AssetPath : v.Address,
                    v => v.AssetPath);
                var ins = new AssetsIndexModel();
                ins.Key2Path = dic;
                var json = JsonSerializer.Serialize(ins);
                var path = EditorBaseResourceManager.GetAssetsIndexFilePath();
                var dir = path.AsPath().GetDirectoryName().Value.Value;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(path, json);
                Destroy(gameObject);
                Debug.Log("all done");
                EditorApplication.isPlaying = false;
            }
        }
    }
}