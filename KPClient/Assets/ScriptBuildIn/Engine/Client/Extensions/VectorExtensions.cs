using System;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Extensions
{
    public static partial class VectorExtensions
    {
        private const float Epsilon = 0.0001f;

        public static float SqrDistance(this Vector2 a, Vector2 b)
        {
            return Vector2.SqrMagnitude(b - a);
        }

        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(b - a);
        }

        public static float ApplyAsLinerFuncArgs(this Vector2 v, float x)
        {
            return v.x + v.y * x;
        }

        public static float ApplyAsLinerFuncArgs(this Vector2Int v, float x)
        {
            return v.x + v.y * x;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector3Int v, out int x, out int y, out int z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector2 v, out float x, out float y)
        {
            x = v.x;
            y = v.y;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector2Int v, out int x, out int y)
        {
            x = v.x;
            y = v.y;
        }


        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector4 v, out float x, out float y, out float z, out float w)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Color v, out float r, out float g, out float b, out float a)
        {
            r = v.r;
            g = v.g;
            b = v.b;
            a = v.a;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Color32 v, out float r, out float g, out float b, out float a)
        {
            r = v.r;
            g = v.g;
            b = v.b;
            a = v.a;
        }

        public static float Dot(this Vector2 lhs, Vector2 rhs)
        {
            return Vector2.Dot(lhs, rhs);
        }

        public static float Dot(this Vector3 lhs, Vector3 rhs)
        {
            return Vector3.Dot(lhs, rhs);
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static Vector2 Dot(this Vector2 lhs, float factor)
        {
            return new Vector2(lhs.x * factor, lhs.y * factor);
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static Vector3 Dot(this Vector3 lhs, float factor)
        {
            return new Vector3(lhs.x * factor, lhs.y * factor, lhs.z * factor);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return vector3;
        }

        public static Vector2 ToVector2(this Vector3 vector3, string xyz)
        {
            if (xyz == null || xyz.Length != 2) throw new Exception($"arg={xyz} invalid.");
            var span = xyz.AsSpan();
            var result = new Vector2();
            for (int i = 0; i < 2; i++)
            {
                var c = span[i];
                switch (c)
                {
                    case 'x':
                        result[i] = vector3.x;
                        break;
                    case 'y':
                        result[i] = vector3.y;
                        break;
                    case 'z':
                        result[i] = vector3.z;
                        break;
                }
            }

            return result;
        }

        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return vector2;
        }

        public static bool IsNormalized(this Vector2 vec)
        {
            var d = vec.sqrMagnitude - 1;
            d = d.Abs();
            return d < 0.01f;
        }

        public static bool IsNormalized(this Vector3 vec)
        {
            var d = vec.sqrMagnitude - 1;
            d = d.Abs();
            return d < 0.01f;
        }

        public static bool IsNormalized(this Vector4 vec)
        {
            var d = vec.sqrMagnitude - 1;
            d = d.Abs();
            return d < 0.01f;
        }

        public static bool IsZeroLike(this Vector2 vec)
        {
            for (int i = 0; i < 2; i++)
            {
                if (vec[i].Abs() > Epsilon) return false;
            }

            return true;
        }

        public static bool IsZeroLike(this Vector3 vec)
        {
            for (int i = 0; i < 3; i++)
            {
                if (vec[i].Abs() > Epsilon) return false;
            }

            return true;
        }

        public static bool IsZeroLike(this Vector4 vec)
        {
            for (int i = 0; i < 4; i++)
            {
                if (vec[i].Abs() > Epsilon) return false;
            }

            return true;
        }

        /// <summary>
        /// 对nan值、无穷大、无穷小赋0值
        /// </summary>
        public static Vector3 AgainstInfinityNaN(this Vector3 vector3)
        {
            var vec = vector3;
            if (float.IsInfinity(vec.x) || float.IsNaN(vec.x)) vec.x = 0;
            if (float.IsInfinity(vec.y) || float.IsNaN(vec.y)) vec.y = 0;
            if (float.IsInfinity(vec.z) || float.IsNaN(vec.z)) vec.z = 0;
            return vec;
        }


        /// <summary>
        /// 对nan值、无穷大、无穷小赋0值
        /// </summary>
        public static Vector3 AgainstInfinityNaN(this Vector2 vector3)
        {
            var vec = vector3;
            if (float.IsInfinity(vec.x) || float.IsNaN(vec.x)) vec.x = 0;
            if (float.IsInfinity(vec.y) || float.IsNaN(vec.y)) vec.y = 0;
            return vec;
        }

        /// <summary>
        /// 对nan值、无穷大、无穷小赋0值
        /// </summary>
        public static Vector3 AgainstInfinityNaN(this Vector4 vector3)
        {
            var vec = vector3;
            if (float.IsInfinity(vec.x) || float.IsNaN(vec.x)) vec.x = 0;
            if (float.IsInfinity(vec.y) || float.IsNaN(vec.y)) vec.y = 0;
            if (float.IsInfinity(vec.z) || float.IsNaN(vec.z)) vec.z = 0;
            if (float.IsInfinity(vec.w) || float.IsNaN(vec.w)) vec.w = 0;
            return vec;
        }

        public static bool IsNormal(this Vector4 vector4)
        {
            //这个true &&是为了IDE自动格式化好看。没有实际意义
            return true && !float.IsNaN(vector4.x) && !float.IsInfinity(vector4.x)
                   && !float.IsNaN(vector4.y) && !float.IsInfinity(vector4.y)
                   && !float.IsNaN(vector4.z) && !float.IsInfinity(vector4.z)
                   && !float.IsNaN(vector4.w) && !float.IsInfinity(vector4.w);
        }

        public static bool IsNormal(this Vector3 vector4)
        {
            //这个true &&是为了IDE自动格式化好看。没有实际意义
            return true && !float.IsNaN(vector4.x) && !float.IsInfinity(vector4.x)
                   && !float.IsNaN(vector4.y) && !float.IsInfinity(vector4.y)
                   && !float.IsNaN(vector4.z) && !float.IsInfinity(vector4.z);
        }

        public static bool IsNormal(this Vector2 vector4)
        {
            //这个true &&是为了IDE自动格式化好看。没有实际意义
            return true && !float.IsNaN(vector4.x) && !float.IsInfinity(vector4.x)
                   && !float.IsNaN(vector4.y) && !float.IsInfinity(vector4.y);
        }
    }
}