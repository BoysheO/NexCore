using BoysheO.Util;
using ScriptEditor.Engine.Client.Manager.ScriptingDefineSymbolsSystem;
using UnityEditor;
using UnityEngine;

namespace PreserveModule.Editor
{
    //PreserveType包含的代码太多，容易拖慢开发中编译。所以开发中最好关闭PreserveType
    public class IncludeMenu
    {
        public const string INCLUDE_PRESERVETYPE = "INCLUDE_PRESERVETYPE";

        [MenuItem("Engine/PreserveModule/IncludePreserve", true)]
        static bool ValidateInclude()
        {
#if !INCLUDE_PRESERVETYPE
            return true;
#else
            return false;
#endif
        }

        [MenuItem("Engine/PreserveModule/ExcludePreserve", true)]
        static bool ValidateExclude()
        {
#if INCLUDE_PRESERVETYPE
            return true;
#else
            return false;
#endif
        }

        [MenuItem("Engine/PreserveModule/IncludePreserve")]
        public static void IncludePreserve()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            Debug.Log($"cur build targetGroup = {EditorUserBuildSettings.selectedBuildTargetGroup}");
            ScriptingDefinesManager.Instance.Ensure(new []{INCLUDE_PRESERVETYPE});
            AssetDatabase.Refresh();
        }

        [MenuItem("Engine/PreserveModule/ExcludePreserve")]
        public static void ExcludePreserve()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            ScriptingDefinesManager.Instance.Remove(new []{INCLUDE_PRESERVETYPE});
            AssetDatabase.Refresh();
        }
    }
}