using System;
using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace HotScripts.Hotfix.GamePlay.FrameworkSystems.LanSystem
{
    [ExecuteAlways]
    // ReSharper disable once InconsistentNaming
    public sealed class LanText_TMP : MonoBehaviour, ILanTextComponent, ILanObserver
    {
        [NonSerialized] private bool _isDirty = false;
        [SerializeField] [HideInInspector] private int _lanKey;
        private ILanManager LanManager;

        [ShowInInspector]
        public int LanKey
        {
            get => _lanKey;
            set
            {
                SetDirty();
                _lanKey = value;
            }
        }

        public string TextShowing
        {
            get
            {
                var tmp = gameObject.GetRequireComponent<TextMeshProUGUI>();
                return tmp.text;
            }
        }

        private void Awake()
        {
            LanManager = DIContext.ServiceProvider.GetRequiredService<ILanManager>();
            LanManager.Add(this);
        }

        private void OnDestroy()
        {
            if (LanManager != null) LanManager.Remove(this);
        }

        private void OnEnable()
        {
            SetDirty();
        }

        public void SetDirty()
        {
            if (_isDirty) return;
            _isDirty = true;
            UpdateAsync().Forget();
        }

        private async UniTaskVoid UpdateAsync()
        {
            try
            {
                var token = this.GetCancellationTokenOnDestroy();
                await UniTask.Yield();
                token.ThrowIfCancellationRequested();
                string text = "";
                if (LanKey == 0) text = "";
                else
                {
                    var t = LanManager.GetText(LanKey);
#if UNITY_EDITOR && DEBUG
                    if (t == "")
                    {
                        Debug.LogWarning($"may be an error lanId={LanKey} in {gameObject.GetHierarchyPath()}");
                    }
#endif
                    text = t;
                }

                var tmp = GetComponent<TextMeshProUGUI>().ThrowIfNullOrFakeNull();
                tmp.text = text;
            }
            finally
            {
                _isDirty = false;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnDidReloadScripts()
        {
            var objets = GameObject.FindObjectsByType<LanText_TMP>(FindObjectsSortMode.None);
            foreach (var lanTextTMP in objets)
            {
                lanTextTMP.OnReloadScripts();
            }
        }

        private void OnReloadScripts()
        {
            Awake();
            SetDirty();
        }
#endif
        void ILanObserver.OnLanChanged()
        {
            SetDirty();
        }
    }
}