using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BoysheO.Extensions;
using BoysheO.Extensions.Unity3D.Extensions;
using BoysheO.Util;
using Unity.CodeEditor;
using Unity.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorScript.CommandMenu
{
    static class CopyChildrenTransformCodeEditorMenu
    {
        [MenuItem("GameObject/CopyChildrenTransformCode")]
        static void Invoke(MenuCommand menuCommand)
        {
            Debug.Log(DebugUtil.GetCallerContext());
            var sb = new StringBuilder();
            var root = (GameObject) menuCommand.context;
            foreach (var gameObject in root.Children())
            {
                var path = GetPath(gameObject, root);
                sb.AppendLine($"GameObject {gameObject.name} => transform.Find(\"{path}\").gameObject;");
            }

            var code = sb.ToString();
            EditorGUIUtility.systemCopyBuffer = code;
        }

        private static string GetPath(GameObject child, GameObject root)
        {
            if (child.Ancestors().All(v => v != root))
            {
                throw new Exception("not a child of root");
            }

            var lst = new List<string>();
            for (GameObject p = child; p != root && p != null; p = p.transform.parent.gameObject)
            {
                lst.Add(p.name);
            }

            lst.Reverse();
            return string.Join("/", lst);
        }
    }
}