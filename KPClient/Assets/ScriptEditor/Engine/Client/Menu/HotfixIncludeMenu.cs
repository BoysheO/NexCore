using BoysheO.Util;
using ScriptEditor.Engine.Client.Manager.ScriptingDefineSymbolsSystem;
using UnityEditor;
using UnityEngine;

namespace ScriptEditor.Engine.Client.Menu
{
    public static class HotfixIncludeMenu
    {
        public const string INCLUDE_HOTFIX = "INCLUDE_HOTFIX";

        [MenuItem("Engine/HotfixModule/IncludeHotfix", true)]
        static bool ValidateInclude()
        {
#if INCLUDE_HOTFIX
            return false;
#else
            return true;
#endif
        }

        [MenuItem("Engine/HotfixModule/IncludeHotfix")]
        public static void IncludeHotfix()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            ScriptingDefinesManager.Instance.Ensure(new[] { INCLUDE_HOTFIX });
            AssetDatabase.Refresh();
        }

        [MenuItem("Engine/HotfixModule/ExcludeHotfix", true)]
        static bool ValidateExclude()
        {
#if INCLUDE_HOTFIX
            return true;
#else
            return false;
#endif
        }

        [MenuItem("Engine/HotfixModule/ExcludeHotfix")]
        public static void ExcludeHotfix()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            ScriptingDefinesManager.Instance.Remove(new[] { INCLUDE_HOTFIX });
            AssetDatabase.Refresh();
        }
    }
}