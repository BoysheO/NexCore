using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using BoysheO.Extensions;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Abstractions.Synchronize;
using UnityEditor;
using UnityEngine;

namespace ScriptEngine.BuildIn.ClientCode.Manager.ControllerSystem.Implements.Synchronize.Editor
{
    public class UnityControllerEditorWindow : EditorWindow
    {
        private SortedSet<string> _history = new();
        private const string TextFieldControlName = "UnityControllerEditorWindow/Input";
        private Vector2 scrollPos; // 滚动条位置，用于显示多个提示时
        private string input = "";
        private readonly string key = $"{nameof(UnityControllerEditorWindow)}/history";

        [MenuItem("Tools/UnityControllerEditorWindow")]
        private static void Open1()
        {
            var win = GetWindow<UnityControllerEditorWindow>();
            win.Show();
        }

        private void Awake()
        {
            //读取最近历史记录
            var history = EditorPrefs.GetString(key);
            try
            {
                var h = JsonSerializer.Deserialize<History>(history);
                foreach (var se in h.history)
                {
                    if (se.IsNotNullOrWhiteSpace()) _history.Add(se);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Reading history fail.History has been rest.ex={ex}");
                EditorPrefs.DeleteKey(key);
            }
        }

        private void OnGUI()
        {
            GUI.SetNextControlName(TextFieldControlName);
            var newInput = EditorGUILayout.TextArea(input);
            var isFocus = GUI.GetNameOfFocusedControl() == TextFieldControlName;
            if (isFocus)
            {
                var selected = _history.Where(v => v.StartsWith(this.input));
                EditorGUILayout.LabelField("History:", EditorStyles.boldLabel);
                // 滚动区域，用于显示长提示列表
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));
                foreach (var se in selected)
                {
                    if (GUILayout.Button(se))
                    {
                        newInput = se;
                        GUI.FocusControl(null);
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            input = newInput;
            if (GUILayout.Button("Send"))
            {
                if (EditorApplication.isPlaying && DIContext.IsServiceCreated)
                {
                    if (_history.Add(input))
                    {
                        var ins = new History();
                        ins.history.AddRange(_history);
                        var json = JsonSerializer.Serialize(ins);
                        EditorPrefs.SetString(key, json);
                    }

                    var ctr = DIContext.ServiceProvider.GetRequiredService<IController>();
                    ctr.Public(input, null);
                }
                else
                {
                    Debug.Log("Do nothing due to Services not ready");
                }
            }
        }

        private class History
        {
            public List<string> history { get; set; } = new();
        }
    }
}