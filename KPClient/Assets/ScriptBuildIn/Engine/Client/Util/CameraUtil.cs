using System;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Util
{
    public static class CameraUtil
    {
        /// <summary>
        /// 将RectTransform(没有父节点的那种）放置到相机视锥体/视方体中的指定位置上
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="rectTransform"></param>
        /// <param name="distance"></param>
        public static void CenterAndFullCamera(Camera camera, RectTransform rectTransform, float distance)
        {
            var height = camera.orthographic
                ? camera.orthographicSize * 2
                : 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var width = height * camera.aspect;
            rectTransform.sizeDelta = new Vector2(width, height);
            var cameraTransform = camera.transform;
            var cameraTransformForward = cameraTransform.forward;
            rectTransform.forward = cameraTransformForward;
            rectTransform.position = cameraTransform.position + cameraTransformForward * distance;
        }
    }
}