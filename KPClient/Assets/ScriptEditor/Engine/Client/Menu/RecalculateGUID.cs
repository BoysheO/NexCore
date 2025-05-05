using UnityEditor;
using UnityEngine;

namespace ScriptEditor.Engine.Client.Menu
{
    public class RecalculateGUID
    {
        [MenuItem("Assets/Recalculate GUIDs for Selected Assets")]
        private static void RecalculateSelectedGUIDs()
        {
            var selectedAssets = Selection.objects;
            foreach (var asset in selectedAssets)
            {
                string assetPath = AssetDatabase.GetAssetPath(asset);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    RecalculateGUIDForAsset(assetPath);
                }
                else
                {
                    Debug.LogWarning("Selected asset has an invalid path.");
                }
            }
        }

        private static void RecalculateGUIDForAsset(string assetPath)
        {
            // Generate a unique temporary path
            string tempPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            // Attempt to move the asset to the temporary path and back
            if (AssetDatabase.MoveAsset(assetPath, tempPath) == string.Empty &&
                AssetDatabase.MoveAsset(tempPath, assetPath) == string.Empty)
            {
                Debug.Log($"Successfully recalculated GUID for {assetPath}");
            }
            else
            {
                throw new System.Exception("Asset move operation failed.");
            }
        }
    }
}