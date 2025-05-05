using System;
using BoysheO.Extensions.Unity3D.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Hotfix.Extensions{

public static class GameObjectExtensions
{
    public static void AppendChildUntilCount(this GameObject root, GameObject prefab, int count)
    {
        var childCount = root.transform.childCount;
        for (int i = childCount; i < count; i++)
        {
            Object.Instantiate(prefab, root.transform);
        }
    }

    public static void AppendPosNodeUntilCount(this GameObject root, int count)
    {
        var childCount = root.transform.childCount;
        for (int i = childCount; i < count; i++)
        {
            var go = new GameObject();
            go.transform.SetParent(root.transform);
            if (root.transform is RectTransform)
            {
                var rect = go.AddComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.rotation = Quaternion.identity;
        }
    }

    public static GameObject HideInHierarchy(this GameObject go)
    {
        go.hideFlags = HideFlags.HideInHierarchy;
        return go;
    }

    public static GameObject Center(this GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
        return go;
    }

    public static TextMeshProUGUI GetText(this GameObject go)
    {
        return go.GetRequireComponent<TextMeshProUGUI>();
    }

    [Obsolete("Use GetText instead")]
    public static TextMeshProUGUI GetTextP(this GameObject go)
    {
        return go.GetRequireComponent<TextMeshProUGUI>();
    }

    public static Image GetImage(this GameObject go)
    {
        return go.GetRequireComponent<Image>();
    }

    public static Button GetButton(this GameObject go)
    {
        return go.GetRequireComponent<Button>();
    }
}}