#nullable enable
using System;
using System.Buffers;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoysheO.Extensions.Unity3D.Extensions
{
    public static class TransformExtensions
    {
        public static RectTransform? GetParentAsRectTransform(this Transform transform)
        {
            return transform.parent as RectTransform;
        }

        public static RectTransform? AsRectTransform(this Transform transform)
        {
            return transform as RectTransform;
        }

        /// <summary>
        /// del all children and low gc
        /// 只执行一次删除，不保证删除后父节点childCount=0
        /// note：如果子节点在OnDestroy回调中给父节点添加了新的子节点，这个子节点不会被删除
        /// 删除之后，transform.childCount与传入参数有关
        /// </summary>
        public static void DestroyChildrenOnce(this Transform transform, bool immediately = false,
            bool detachChildren = true)
        {
            var count = transform.childCount;
            var buff = ArrayPool<Transform>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                buff[i] = transform.GetChild(i);
            }

            if (detachChildren)
            {
                transform.DetachChildren();
            }

            if (immediately)
            {
                foreach (var transform1 in buff.AsSpan(0, count))
                {
                    if (transform1) Object.DestroyImmediate(transform1.gameObject);
                }
            }
            else
            {
                foreach (var transform1 in buff.AsSpan(0, count))
                {
                    if (transform1) Object.Destroy(transform1.gameObject);
                }
            }

            ArrayPool<Transform>.Shared.Return(buff,true);
        }

        /// <summary>
        /// del all children and low gc
        /// 同<see cref="DestroyChildrenOnce(UnityEngine.Transform,bool,bool)"/>
        /// </summary>
        public static void DestroyChildrenOnce(this GameObject gameObject, bool immediately = false,
            bool detachChildren = true)
        {
            DestroyChildrenOnce(gameObject.transform, immediately, detachChildren);
        }

        /// <summary>
        /// del all children and low gc
        /// 循环删除直到父节点childCount为0（保证删除完成后为0)
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="timeout">only work in DEBUG mode</param>
        /// <param name="immediately"></param>
        /// <exception cref="TimeoutException">存在某种逻辑反复给父节点写入新的object导致一直删不干净，仅Debug有效</exception>
        public static void DestroyChildrenUntilEmpty(this Transform transform, TimeSpan timeout,
            bool immediately = false)
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            while (transform.childCount != 0)
            {
#if DEBUG
                if (sw.Elapsed > timeout) throw new TimeoutException();
#endif
                var child = transform.GetChild(0);
                if (immediately) Object.DestroyImmediate(child.gameObject);
                else
                {
                    child.SetParent(null);
                    Object.Destroy(child.gameObject);
                }
            }
#if DEBUG
            sw.Stop();
#endif
        }

        public static Transform[] ToPoolArray(this Transform transform, out int count)
        {
            count = transform.childCount;
            var buf = ArrayPool<Transform>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                buf[i] = transform.GetChild(i);
            }

            return buf;
        }
    }
}