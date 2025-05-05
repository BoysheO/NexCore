using BoysheO.Util;
using HybridCLR.Editor.Settings;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ScriptBuildIn.Engine.Client.Configuration.Editor
{
    public class BuildChecker : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (BuildInConstConfig.LoadMode == BuildInConstConfig.LOADMODE_INTERNAL &&
                HybridCLRSettings.Instance.enable)
            {
                throw new BuildFailedException(
                    $"build with {nameof(BuildInConstConfig)}.{nameof(BuildInConstConfig.LoadMode)} " +
                    $"== {nameof(BuildInConstConfig)}.{nameof(BuildInConstConfig.LOADMODE_INTERNAL)} " +
                    $"is not allow on build\n{DebugUtil.GetCallerContext()}");
            }

            if (BuildInConstConfig.LoadMode == BuildInConstConfig.LOADMODE_IO)
            {
                BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
                if (activeBuildTarget == BuildTarget.StandaloneWindows ||
                    activeBuildTarget == BuildTarget.StandaloneWindows64 ||
                    activeBuildTarget == BuildTarget.StandaloneOSX)
                {
                    //do nothing
                }
                else
                {
                    //只有单机端才能用加载Dll的方式直接加载。
                    throw new BuildFailedException(
                        $"build with {nameof(BuildInConstConfig)}.{nameof(BuildInConstConfig.LoadMode)} == {nameof(BuildInConstConfig)}.{nameof(BuildInConstConfig.LOADMODE_IO)} is not allow while build target is not pc");
                }
            }

            if (BuildInConstConfig.YooMode == BuildInConstConfig.YOOMODE_EDITOR)
                throw new BuildFailedException(
                    $"build with {nameof(BuildInConstConfig)}.{nameof(BuildInConstConfig.YooMode)} == {nameof(BuildInConstConfig)}.{nameof(BuildInConstConfig.YOOMODE_EDITOR)} is not allow on build");
        }

        private bool IsDefineSymbolSet(string defineSymbol)
        {
            // 获取当前的宏定义
            string defines =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return defines.Contains(defineSymbol);
        }
    }
}