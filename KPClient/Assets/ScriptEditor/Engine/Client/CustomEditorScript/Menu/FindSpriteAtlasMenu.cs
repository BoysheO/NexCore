using System.Linq;
using BoysheO.Extensions;
using BoysheO.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace EditorScript.CommandMenu
{
    public static class FindSpriteAtlasMenu
    {
        //选中Sprite，查找包含它的图集
        [MenuItem("Assets/GoToSpriteAtlas")]
        static void EditScript()
        {
            Debug.Log(DebugUtil.GetCallerContext());
            if (!(Selection.activeObject is Sprite sprite))
            {
                if (Selection.activeObject is null)
                    Debug.Log("当前没有选中任何东西");
                else Debug.Log("当前选中的不是Sprite,而是:" + Selection.activeObject?.GetType().Name);
                return;
            }

            var locations = AssetDatabase.FindAssets("t:SpriteAtlas")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SpriteAtlas>)
                .Where(v => v != null)
                .Where(v=>v.CanBindTo(sprite))
                // .Where(v => v.GetPackables().Contains(sprite))
                .ToArray();
            if (locations.Length > 0)
            {
                if (locations.Length > 1)
                {
                    Debug.Log($"多个Atlas包含此Sprite:{locations.Select(v => v.name).JoinAsOneString()}");
                }

                Debug.Log("Atlas:" + locations[0].name);
                Selection.activeObject = locations[0];
            }
            else Debug.Log("没有Atlas包含此Sprite");
        }
    }
}