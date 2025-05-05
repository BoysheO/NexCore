using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BoysheO.Extensions;
using BoysheO.Util;
using UnityEditor;
using HybridCLR.Editor.Settings;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace SimEngineEditor.Manager.HotfixSystem
{
    /// <summary>
    /// 如果一个.asmdef同名目录定义了.hotfixdef文件，则添加到HCLR热更中
    /// </summary>
    internal class AutoDiscoverHotfixAssembly
    {
        [MenuItem("Debug/DiscoverHotfixAssembly")]
        static void DiscoverHotfixAssembly()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            DiscoverHotfixAssemblyOnLoad();
        }

        [InitializeOnLoadMethod]
        static void DiscoverHotfixAssemblyOnLoad()
        {
            var sample = CustomSampler.Create(nameof(AutoDiscoverHotfixAssembly));
            sample.Begin();
            try
            {
                var asm = FindHotfixAssemblies();
                //在单独的Profile线程中，这个值会是null，此时后面的逻辑已经不需要了
                if (HybridCLRSettings.Instance == null) return;
                var set = HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.ToHashSet();
                bool isAnyAdd = false;
                foreach (var assemblyDefinitionAsset in asm)
                {
                    isAnyAdd = isAnyAdd || set.Add(assemblyDefinitionAsset);
                }

                if (isAnyAdd)
                {
                    HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = set.ToArray();
                    HybridCLRSettings.Save();
                }
            }
            finally
            {
                sample.End();
            }
        }

        public static AssemblyDefinitionAsset[] FindHotfixAssemblies()
        {
            List<AssemblyDefinitionAsset> hotfixAssets = new List<AssemblyDefinitionAsset>();

            // 查找所有 asmdef 文件
            string[] asmdefGuids =
                AssetDatabase.FindAssets("t:AssemblyDefinitionAsset", new[] { "Assets", "Packages" });

            foreach (string guid in asmdefGuids)
            {
                string asmdefPath = AssetDatabase.GUIDToAssetPath(guid);
                string directory = Path.GetDirectoryName(asmdefPath);
                string moddefPath = Path.Combine(directory, ".hotfixdef");

                if (File.Exists(moddefPath))
                {
                    AssemblyDefinitionAsset asmdefAsset =
                        AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
                    if (asmdefAsset != null)
                    {
                        hotfixAssets.Add(asmdefAsset);
                    }
                }
            }

            return hotfixAssets.ToArray();
        }
    }
}