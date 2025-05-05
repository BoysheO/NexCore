using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using GameFramework;
using UnityEngine;
public class FPSCounter : MonoBehaviour
{
    //显示在屏幕上的数字实时显示.如果快到看不清，请使用手机拍照拍一帧下来看
    private float fps;
    private IDisposable _disposable;
    private GUIStyle _style;
    private void Start()
    {
        //_disposable = zstring.Block();
        _style = new GUIStyle()
        {
            fontSize = 40,
        };
    }
    private void Update()
    {
        fps = 1f / Time.unscaledDeltaTime;
    }
    private void OnDestroy()
    {
        _disposable?.Dispose();
    }
    private void OnGUI()
    {
        using var _ = zstring.Block();
        var i = (zstring)(int) fps;
        var color = GUI.color;
        GUI.color = Color.white;//防止颜色相乘后，失去style的颜色
        var contentColor = GUI.contentColor;
        GUI.contentColor = Color.white;//防止颜色相乘后，失去style的颜色
        var col = (int)Time.realtimeSinceStartup % 2 == 0 ? Color.black : Color.white;
        _style.normal.textColor = col;
        GUILayout.Label(i,_style);
        GUI.color = color;
        GUI.contentColor = contentColor;
    }
}