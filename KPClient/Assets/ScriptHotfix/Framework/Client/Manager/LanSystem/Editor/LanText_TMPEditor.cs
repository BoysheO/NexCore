using System.Linq;
using BoysheO.Extensions;
using Hotfix.LanMgr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScriptBuildIn.Engine.Client.Configuration;
using ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace HotScripts.Hotfix.GamePlay.FrameworkSystems.LanSystem.Editor
{
    [CustomEditor(typeof(LanText_TMP))]
    public class LanText_TMPEditor : OdinEditor
    {
        private string inputText = ""; // 用户输入的文本

        private string[] tips; // 提示数组

        private string[] filteredTips; // 筛选后的提示
        private Vector2 scrollPos; // 滚动条位置，用于显示多个提示时
        private const string TextFieldControlName = "E680C25F-9481-48C9-9A9A-72B924D3C2CF"; // 文本框控件的唯一名称
        private ILanManager _lanManager;
        private IConfiguration _configuration;

        private void Awake()
        {
            if (_lanManager == null)
            {
                _lanManager = DIContext.ServiceProvider.GetRequiredService<ILanManager>();
            }

            if (_configuration == null)
            {
                _configuration = DIContext.ServiceProvider.GetRequiredService<IConfiguration>();
            }
            
            // var app = DownloadHelper.LoadAppSetting();
            var app = _configuration.Get<AppSettingModel>();
            var lanSupport = app.LanguageSupport;
            tips = lanSupport
                .Where(v=>!v.IsNullOrWhiteSpace())
                .Append(Lan.RawLan)
                .ToArray();
            inputText = _lanManager.CurLanguage;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Lan");
            GUI.enabled = false;
            EditorGUILayout.TextField(_lanManager.CurLanguage);
            GUI.enabled = true;
            
            GUI.SetNextControlName(TextFieldControlName); // 设置控件名称
            // 绘制一个文本框
            string newText = EditorGUILayout.TextField(inputText);
            var isTextFieldFocused = GUI.GetNameOfFocusedControl() == TextFieldControlName;
            // 如果文本改变，更新提示
            if (newText != inputText)
            {
                inputText = newText;
                UpdateFilteredTips(); // 筛选符合条件的提示
            }

            // 绘制确认按钮
            if (GUILayout.Button("Change", GUILayout.Width(80)))
            {
                _lanManager.CurLanguage = inputText;
                Debug.Log($"LanManager.CurLanguage changed to {inputText}"); // 打印日志
            }

            EditorGUILayout.EndHorizontal();
            // 如果有提示，绘制提示列表
            if (isTextFieldFocused && filteredTips != null && filteredTips.Length > 0)
            {
                EditorGUILayout.LabelField("Suggestions:", EditorStyles.boldLabel);

                // 滚动区域，用于显示长提示列表
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));
                foreach (var tip in filteredTips)
                {
                    if (GUILayout.Button(tip)) // 提示项作为按钮
                    {
                        inputText = tip; // 点击提示，将其填入文本框
                        GUI.FocusControl(null); // 取消文本框焦点
                        UpdateFilteredTips(); // 更新提示
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            base.OnInspectorGUI();
        }

        private void UpdateFilteredTips()
        {
            if (string.IsNullOrEmpty(inputText))
            {
                filteredTips = tips; // 显示全部提示
            }
            else
            {
                filteredTips = System.Array.FindAll(tips,
                    tip => tip.StartsWith(inputText, System.StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}