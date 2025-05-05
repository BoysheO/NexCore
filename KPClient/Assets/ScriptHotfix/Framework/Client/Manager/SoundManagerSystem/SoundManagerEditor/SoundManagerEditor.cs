using System;
using Microsoft.Extensions.DependencyInjection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace HotScripts.Hotfix.GamePlay.FrameworkSystems.SoundManagerSystem.SoundManagerEditor
{
    public class SoundManagerEditor:OdinEditorWindow
    {
        [MenuItem("Debug/SoundManager")]
        private static void OpenSoundManager()
        {
            var win = GetWindow<SoundManagerEditor>();
            win.Show();
        }

        [ShowInInspector]
        public SoundManager soundManager;

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                soundManager = DIContext.ServiceProvider.GetRequiredService<SoundManager>();
            }
            else
            {
                soundManager = null;
            }
        }
    }
}