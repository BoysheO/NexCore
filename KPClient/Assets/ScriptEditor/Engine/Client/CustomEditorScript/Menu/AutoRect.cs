using BoysheO.Extensions.Unity3D.Extensions;
using BoysheO.Util;
using UnityEditor;
using UnityEngine;
#pragma warning disable CS8601 // Possible null reference assignment.

namespace EditorScript.CommandMenu
{
    //按下Ctr+g之后，将当前选中UI锚定到当前位置
    public class AutoRectClass
    {
        [MenuItem("Tools/UI/" + nameof(SetupAnchorsBaseOnRect) + " %g")]
        public static void SetupAnchorsBaseOnRect(MenuCommand menuCommand)
        {
            Debug.Log(DebugUtil.GetCallerContext()); //打个log好找
            if (!IsOk(out var self, out var parent)) return;
            Undo.RecordObject(self, "SetupAnchorsBaseOnRect");
            self.SetupAnchorsBaseOnRect();
        }

        private static bool IsOk(out RectTransform self, out RectTransform parent)
        {
            self = parent = null;
            var objs = Selection.activeObject as GameObject;
            if (objs == null) return false;
            self = objs.transform as RectTransform;
            parent = self.TrimFakeNull()?.parent as RectTransform;
            if (!parent || !self) return false;
            return true;
        }
    }
}