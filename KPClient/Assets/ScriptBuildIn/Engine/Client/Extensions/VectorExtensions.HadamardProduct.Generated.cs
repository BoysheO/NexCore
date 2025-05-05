/*CodeGen
*Don't edit manually
*/
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Extensions
{
    public static partial class VectorExtensions
    {
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector4 HadamardProduct(this Vector4 l,Vector4 r)
        {
            return new Vector4(
                l.x * r.x,
                l.y * r.y,
                l.z * r.z,
                l.w * r.w
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector4 HadamardProduct(this Vector4 l,
                float x,
                float y,
                float z,
                float w
                )
        {
            return new Vector4(
                l.x * x,
                l.y * y,
                l.z * z,
                l.w * w
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector3 HadamardProduct(this Vector3 l,Vector3 r)
        {
            return new Vector3(
                l.x * r.x,
                l.y * r.y,
                l.z * r.z
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector3 HadamardProduct(this Vector3 l,
                float x,
                float y,
                float z
                )
        {
            return new Vector3(
                l.x * x,
                l.y * y,
                l.z * z
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector2 HadamardProduct(this Vector2 l,Vector2 r)
        {
            return new Vector2(
                l.x * r.x,
                l.y * r.y
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector2 HadamardProduct(this Vector2 l,
                float x,
                float y
                )
        {
            return new Vector2(
                l.x * x,
                l.y * y
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector3Int HadamardProduct(this Vector3Int l,Vector3Int r)
        {
            return new Vector3Int(
                l.x * r.x,
                l.y * r.y,
                l.z * r.z
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector3Int HadamardProduct(this Vector3Int l,
                int x,
                int y,
                int z
                )
        {
            return new Vector3Int(
                l.x * x,
                l.y * y,
                l.z * z
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector2Int HadamardProduct(this Vector2Int l,Vector2Int r)
        {
            return new Vector2Int(
                l.x * r.x,
                l.y * r.y
            );
        }
        /// <summary>
        /// 哈达玛积
        /// </summary>
        public static Vector2Int HadamardProduct(this Vector2Int l,
                int x,
                int y
                )
        {
            return new Vector2Int(
                l.x * x,
                l.y * y
            );
        }
    }
}