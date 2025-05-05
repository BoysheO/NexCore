using System.IO;
using System.Text.Json;
using BoysheO.Util;
using Hotfix.ResourceMgr.Implements.Editor;
using UnityEditor;
using UnityEngine;

namespace ScriptEngine.Editor.ClientCode.Manager.ResourceSystem.Implements.EditorBaseResourceSystem
{
    public class EditorMenu
    {
        [MenuItem("Tools/EditorBaseResourceSystem/OpenIndex")]
        public static void OpenEditorBaseAssetIndex()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            var file = EditorBaseResourceManager.GetAssetsIndexFilePath();
            EditorUtility.OpenWithDefaultApp(file);
        }
    }
}