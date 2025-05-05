using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hotfix.FrameworkSystems.CameraSystem{

public static class CameraHelper
{
    /// <summary>
    /// 设置内部射线发出相机 (有可能是因为没有<see cref="PhysicsRaycaster"/> 或 <see cref="Physics2DRaycaster"/>造成的？）
    /// 这个脚本是解决Unity从Camera发射射线的一个bug。多个相机存在时，以最后一个enable=true的相机为发送射线的相机
    /// </summary>
    public static void SetInternalRaySendFrom(Camera camera)
    {
        if (!camera.enabled) throw new Exception("没有激活的相机是没有办法设置为内部射线发出点的");
        camera.enabled = false;
        camera.enabled = true;
    }
}}