using System;
using System.Buffers;
using JetBrains.Annotations;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Extensions
{
    /// <summary>
    ///     所有API均不考虑缩放比不为1的情况
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// 锚定四角充满Rect空间<br />
        /// *不考虑缩放比不为1的情况<br />
        /// </summary>
        public static void AnchorCornersAndFull([NotNull] this RectTransform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localScale = Vector3.one;
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.sizeDelta = Vector2.zero;
            var size = transform.rect.size;
            var pivotToCenter = transform.pivot - new Vector2(0.5f, 0.5f);
            var x = size.x * pivotToCenter.x;
            var y = size.y * pivotToCenter.y;
            var z = transform.localPosition.z;
            transform.localPosition = new Vector3(x, y, z);
        }

        /// <summary>
        /// 不改变当前位置，将锚定方式变更为百分比锚定
        /// </summary>
        public static void SetupAnchorsBaseOnRect([NotNull] this RectTransform rectTransform)
        {
            if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
            var parent = rectTransform.parent.AsRectTransform();
            if (parent == null) return; //ignore no parent
            var lbUVInParent = rectTransform.offsetMin / parent.rect.size + rectTransform.anchorMin;
            var rtUVInParent = lbUVInParent + rectTransform.rect.size / parent.rect.size;
            rectTransform.anchorMin = lbUVInParent;
            rectTransform.anchorMax = rtUVInParent;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// 已考虑缩放、pivot情况。获得的值为localPosition。不考虑旋转的情况
        /// *已验证可靠性
        /// </summary>
        public static Rect GetRectBaseCanvasCoordinate(this RectTransform transform)
        {
            var loc = transform.localPosition.ToVector2();
            var pivot = transform.pivot;
            var scale = transform.localScale.ToVector2();
            var size = transform.rect.size * scale;
            var min = loc - size * pivot;
            return new Rect(min, size);
        }

        /// <summary>
        /// 已考虑缩放、pivot情况。获得的值为worldPosition
        /// *以后再研究把
        /// </summary>
        [Obsolete]
        public static Rect GetRectBaseWorld(this RectTransform transform)
        {
            var localRect = transform.GetRectBaseCanvasCoordinate();
            //要先算出它pivot为0.5的时候的local点，否则转换成世界点是错误的
            var pivotW = transform.position; //pivot点世界坐标
            //在没有缩放的前提下，min的转换
            throw new NotImplementedException();


            // var max = transform.localToWorldMatrix.MultiplyPoint(localRect.max);
            // return new Rect(min, max-min);
        }

        /// <summary>
        /// 对RectTransform赋值(pivot、anchor、position、rotation，特别注意不含scale）
        /// </summary>
        public static RectTransform Set(this RectTransform transform, RectTransform another)
        {
            if (transform.parent != another.parent) throw new Exception("应当先保证父节点一致，否则可能不是预想中的结果");
            transform.pivot = another.pivot;
            transform.anchorMin = another.anchorMin;
            transform.anchorMax = another.anchorMax;
            transform.offsetMin = another.offsetMin;
            transform.offsetMax = another.offsetMax;
            transform.position = another.position;
            transform.rotation = another.rotation;
            return transform;
        }

        /// <summary>
        /// 求出RectTransform的中心点位置
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static Vector2 GetCenterPositionLocalSpace(this RectTransform rectTransform)
        {
            var buff = ArrayPool<Vector3>.Shared.Rent(4);
            rectTransform.GetLocalCorners(buff);
            var lt = buff[0];
            var rb = buff[2];
            ArrayPool<Vector3>.Shared.Return(buff);
            return rb - lt;
            // var offset = GetCenterOffset(rectTransform);
            // var localPosXY = rectTransform.localPosition.ToVector2() + offset;
            // return localPosXY;
        }
        
        /// <summary>
        /// 求出RectTransform的中心点位置
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static Vector2 GetCenterPositionWorldSpace(this RectTransform rectTransform)
        {
            var buff = ArrayPool<Vector3>.Shared.Rent(4);
            rectTransform.GetWorldCorners(buff);
            var lt = buff[0];
            var rb = buff[2];
            ArrayPool<Vector3>.Shared.Return(buff);
            return rb - lt;
            // var offset = GetCenterOffset(rectTransform);
            // var localPosXY = rectTransform.position.ToVector2() + offset;
            // return localPosXY;
        }

        /// <summary>
        /// 求出RectTransform的中心点位置
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        private static Vector2 GetCenterOffset(this RectTransform rectTransform)
        {
            var actuallySize = rectTransform.rect.size * rectTransform.localScale;
            var o = rectTransform.pivot - new Vector2(0.5f, 0.5f);
            return -o * actuallySize;
        }

        public static void GetWorldCorners(this RectTransform rectTransform, out Vector3 lb, out Vector3 lt, out Vector3 rt, out Vector3 rb)
        {
            var buff = ArrayPool<Vector3>.Shared.Rent(4);
            rectTransform.GetWorldCorners(buff);
            lb = buff[0];
            lt = buff[1];
            rt = buff[2];
            rb = buff[3];
            ArrayPool<Vector3>.Shared.Return(buff);
        } 
        public static void GetLocalCorners(this RectTransform rectTransform, out Vector3 lb, out Vector3 lt, out Vector3 rt, out Vector3 rb)
        {
            var buff = ArrayPool<Vector3>.Shared.Rent(4);
            rectTransform.GetLocalCorners(buff);
            lb = buff[0];
            lt = buff[1];
            rt = buff[2];
            rb = buff[3];
            ArrayPool<Vector3>.Shared.Return(buff);
        } 
        
        // /// <summary>
        // /// 求出RectTransform的端点位置
        // /// </summary>
        // /// <param name="rectTransform"></param>
        // /// <returns></returns>
        // [Obsolete("算法有误，请使用rect.GetWorldCorn",true)]
        // public static (Vector2 lb, Vector2 rt) GetLeftBottomRightTopPosWorldSpace(this RectTransform rectTransform)
        // {
        //     var actuallySize = rectTransform.rect.size * rectTransform.localScale;
        //     var wpos = rectTransform.position.ToVector2();
        //     var pivot = rectTransform.pivot;
        //     var lb = wpos - actuallySize * pivot;
        //     var rt = wpos + actuallySize * (Vector2.one - pivot);
        //     return (lb, rt);
        // }
        //
        // /// <summary>
        // /// 求出RectTransform的端点位置
        // /// </summary>
        // /// <param name="rectTransform"></param>
        // /// <returns></returns>
        // [Obsolete("算法有误,，请使用rect.GetLocalCorn",true)]
        // public static (Vector2 lb, Vector2 rt) GetLeftBottomRightTopPosLocalSpace(this RectTransform rectTransform)
        // {
        //     var actuallySize = rectTransform.rect.size * rectTransform.localScale;
        //     var wpos = rectTransform.localPosition.ToVector2();
        //     var pivot = rectTransform.pivot;
        //     var lb = wpos - actuallySize * pivot;
        //     var rt = wpos + actuallySize * (Vector2.one - pivot);
        //     return (lb, rt);
        // }
    }
}