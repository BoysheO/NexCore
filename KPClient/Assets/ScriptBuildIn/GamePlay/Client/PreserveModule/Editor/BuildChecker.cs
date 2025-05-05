using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace PreserveModule.Editor
{
    public class BuildChecker: IPreprocessBuildWithReport
    {
        public const string INCLUDE_PRESERVETYPE = IncludeMenu.INCLUDE_PRESERVETYPE;

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // 检查是否以IL2CPP方式构建
            if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP)
            {
                // 检查是否包含INCLUDE_PRESERVETYPE宏
                if (!IsDefineSymbolSet(INCLUDE_PRESERVETYPE))
                {
                    // 提示用户并终止构建
                    EditorUtility.DisplayDialog("Build Error",
                        $"The build requires the '{INCLUDE_PRESERVETYPE}' define symbol. Please add it to the Scripting Define Symbols and try again.",
                        "OK");
                    throw new BuildFailedException($"Missing required define symbol: {INCLUDE_PRESERVETYPE}");
                }
            }
        }

        private bool IsDefineSymbolSet(string defineSymbol)
        {
            // 获取当前的宏定义
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return defines.Contains(defineSymbol);
        }
    }
}