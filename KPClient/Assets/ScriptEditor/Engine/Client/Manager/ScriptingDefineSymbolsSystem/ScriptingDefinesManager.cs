using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BoysheO.Extensions;
using UnityEditor;

namespace ScriptEditor.Engine.Client.Manager.ScriptingDefineSymbolsSystem
{
    public class ScriptingDefinesManager
    {
        public static ScriptingDefinesManager Instance { get; } = new();
        private static readonly BuildTargetGroup[] InvalidBuildTargetGroup = new[]
        {
            BuildTargetGroup.Unknown,
        };

        private BuildTargetGroup[] GetValidGroups()
        {
            return Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Except(InvalidBuildTargetGroup)
                .Where(v =>
                {
                    return typeof(BuildTargetGroup).GetField(v.ToString())
                        .GetCustomAttribute<ObsoleteAttribute>() == null;
                })
                .ToArray();
        }

        public string[] GetDefines(BuildTargetGroup btg)
        {
            var groups = GetValidGroups();
            if (!groups.Contains(btg)) throw new Exception($"This build target={btg} is not supported");
            var group = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
            var sp = group.Split(";");
            return sp;
        }

        /// <summary>
        /// 如果没有这个宏则添加之（所有构建组）
        /// </summary>
        /// <returns>Any add</returns>
        public bool Ensure(IReadOnlyCollection<string> define)
        {
            define.ThrowIfNull();
            var group = GetValidGroups();
            bool isAdd = false;
            foreach (var buildTargetGroup in group)
            {
                isAdd = isAdd || Ensure(define, buildTargetGroup);
            }

            return isAdd;
        }

        /// <summary>
        /// 如果没有这个宏则添加之（指定构建组）
        /// </summary>
        /// <returns>Any add</returns>
        public bool Ensure(IReadOnlyCollection<string> define, BuildTargetGroup specifiedGroup)
        {
            define.ThrowIfNull();
            var defines = new SortedSet<string>(GetDefines(specifiedGroup));
            var count = defines.Count;
            foreach (var se in define)
            {
                defines.Add(se);
            }

            bool isAnyAdd = defines.Count > count;
            if (isAnyAdd)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(specifiedGroup, defines.ToArray());
            }

            return isAnyAdd;
        }

        public bool Remove(IReadOnlyCollection<string> defines, BuildTargetGroup specifiedGroup)
        {
            var old = GetDefines(specifiedGroup);
            var newAry = old.Except(defines).ToArray();
            if (old.Length != newAry.Length)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(specifiedGroup, newAry);
                return true;
            }

            return false;
        }

        public bool Remove(IReadOnlyCollection<string> defines)
        {
            var groups = GetValidGroups();
            bool isAnyRemoved = false;
            foreach (var buildTargetGroup in groups)
            {
                isAnyRemoved = isAnyRemoved || Remove(defines, buildTargetGroup);
            }

            return isAnyRemoved;
        }
    }
}