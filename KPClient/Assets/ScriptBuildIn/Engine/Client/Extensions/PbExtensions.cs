using UnityEngine;

namespace Hotfix.Extensions{

public static class PbExtensions
{
    public static Vector2Int ToVector2Int(this ivec2 ivec2)
    {
        return new Vector2Int(ivec2.X, ivec2.Y);
    }

    public static Vector3Int ToVector3Int(this ivec3 ivec3)
    {
        return new Vector3Int(ivec3.X, ivec3.Y);
    }

    public static RectInt ToRectInt(this iRect rect)
    {
        return new RectInt(rect.Min.X, rect.Min.Y, rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y);
    }
}}