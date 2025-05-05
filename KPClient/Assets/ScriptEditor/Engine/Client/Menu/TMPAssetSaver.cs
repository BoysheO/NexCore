using TMPro;
using UnityEditor;
using UnityEngine;

//此脚本用于修复TMP某个版本不会自动保存FontAsset的问题。
//也是修复TMP动态字体不出字的代码
namespace ScriptEditor.Engine.Client.Menu
{
    public class TMPAssetSaver
    {
        // 添加右键菜单，只对 TMP_FontAsset 类型资源生效
        [MenuItem("Assets/Force Save TMP Asset", true)]
        private static bool ValidateForceSaveTMPAsset()
        {
            return Selection.activeObject is TMP_FontAsset;
        }

        [MenuItem("Assets/Force Save TMP Asset")]
        private static void ForceSaveTMPAsset()
        {
            TMP_FontAsset? tmpFont = Selection.activeObject as TMP_FontAsset;

            if (tmpFont != null)
            {
                string path = AssetDatabase.GetAssetPath(tmpFont);

                // 标记为已修改
                EditorUtility.SetDirty(tmpFont);
                // 强制刷新与保存
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Debug.Log($"[TMP Save] 强制保存 TMP_FontAsset: {tmpFont.name}");
            }
            else
            {
                Debug.LogWarning("[TMP Save] 选中的资源不是 TMP_FontAsset。");
            }
        }
    }
}