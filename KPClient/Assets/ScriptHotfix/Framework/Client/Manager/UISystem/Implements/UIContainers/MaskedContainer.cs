using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BoysheO.Collection2;
using BoysheO.Collection2.Linq;
using BoysheO.Extensions.Unity3D.Extensions;
using DG.Tweening;
using UISystem.Abstractions;
using UnityEngine;
using UnityEngine.UI;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;

namespace UISystem.Implements.UIContainers
{
    /// <summary>
    /// 带Mask处理的
    /// *sealed标记很重要~防止调用到奇怪的虚拟成员
    /// </summary>
    public sealed class MaskedContainer : MonoBehaviour, IUIContainer
    {
        private const bool IsDEBUG = BuildInConstConfig.IsDebug;
        private MaskObj _maskObj;

        private void Start()
        {
            BuildMaskGo();
            _maskObj.name = "Mask";
        }

        private void BuildMaskGo()
        {
            var go = new GameObject();
            go.transform.SetParent(gameObject.transform);
            _maskObj = go.AddComponent<MaskObj>();
        }

        #region Container

        //表末为栈顶
        private readonly VList<IUIContainerItem> _uiStack = VList<IUIContainerItem>.Rent();
        private readonly VList<IUIContainerItem> _uiList = VList<IUIContainerItem>.Rent();

        //在操作父节点时，防止item在事件通知函数内再次调用PushPop
        private bool _lockStack;

        public IServiceProvider ServiceProvider { get; set; } = null!;

        public GameObject ParentForInstantiate => gameObject;

        public void Add(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.GameObject == null) throw new Exception("标准容器禁止nullGameObject");
            if (item.GameObject.activeSelf) throw new Exception("go的激活状态是由Container托管的，要求Add的时候是false");
            item.GameObject.transform.SetParent(gameObject.transform);
            _uiList.Add(item);
            item.UIContainer = this;
            item.OnAddedInUIContainer();
        }

        public void Remove(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            WarnIfNotParentOrNull(item);
            if (_uiStack.Contains(item)) PopUI(item);
            item.GameObject.transform.SetParent(null);
            _uiList.Remove(item);
            item.UIContainer = null;
            item.OnRemovedFromUIContainer();
        }

        public void GetUIStack(IList<IUIContainerItem> buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            buffer.Clear();
            foreach (var uiContainerItem in _uiStack.InternalBuffer.Span)
            {
                buffer.Add(uiContainerItem);
            }
        }

        public void GetUIList(IList<IUIContainerItem> buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            buffer.Clear();
            foreach (var uiContainerItem in _uiList.InternalBuffer.Span)
            {
                buffer.Add(uiContainerItem);
            }
        }

        public void PushUI(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            WarnIfNotParentOrNull(item);
            if (_lockStack) throw new Exception("不能在此时操作Stack");
            _lockStack = true;
            try
            {
                var nextIdx = _uiStack.Count;
                PublishStackChanging();
                item.OnStackChanging(nextIdx, _uiStack.InternalBuffer);
                item.OnUIPushing();
                var lastTop = _uiStack.Count > 0 ? _uiStack[^1] : null;

                _uiStack.Add(item);
                item.GameObject.transform.SetParent(gameObject.transform);
                _maskObj.transform.SetAsLastSibling();
                item.GameObject.transform.SetAsLastSibling();
                _maskObj.Show();
                item.GameObject.SetActive(true);

                item.OnUIPushed();
                PublishStackChanged();

                #region 发布focus事件

                lastTop?.OnUIUnFocused();
                item.OnUIFocused();

                #endregion
            }
            finally
            {
                _lockStack = false;
            }
        }

        private void PublishStackChanging()
        {
            for (var index = 0; index < _uiStack.Count; index++)
            {
                var uiContainerItem = _uiStack[index];
                uiContainerItem.OnStackChanging(index, _uiStack.InternalBuffer);
            }
        }

        private void PublishStackChanged()
        {
            for (var index = 0; index < _uiStack.Count; index++)
            {
                var uiContainerItem = _uiStack[index];
                uiContainerItem.OnStackChanging(index, _uiStack.InternalBuffer);
            }
        }

        /// <summary>
        /// 对父节点被改变的item做个警告
        /// </summary>
        private void WarnIfNotParentOrNull(IUIContainerItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (IsDEBUG)
            {
                if (item.GameObject.transform.parent != gameObject.transform)
                {
                    Debug.LogError("item的gameObject应由本容器管理的，但是它似乎被改变了");
                }

                if (!_uiList.Contains(item))
                {
                    Debug.LogError("不是本容器管理的Item");
                }
            }
        }

        public void PopUI(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            WarnIfNotParentOrNull(item);
            if (!_uiStack.Contains(item)) return;
            if (_lockStack) throw new Exception("不能在此时操作Stack");
            _lockStack = true;
            try
            {
                PublishStackChanging();
                bool isTopUI = IsTop(item);
                var nextTopUI = _uiStack.Count > 1 ? _uiStack[^2] : null;
                //在popping事件前发布focus事件
                if (isTopUI) item.OnUIUnFocused();

                item.OnUIPopping();

                _uiStack.Remove(item);
                item.GameObject.SetActive(false);
                if (nextTopUI != null)
                {
                    _maskObj.transform.SetAsLastSibling();
                    _maskObj.Show();
                    nextTopUI.GameObject.transform.SetAsLastSibling();
                }
                else
                {
                    _maskObj.Hide();
                }

                item.OnUIPopped();

                item.OnStackChanged(-1, _uiStack.InternalBuffer);
                PublishStackChanged();

                nextTopUI?.OnUIFocused();
            }
            finally
            {
                _lockStack = false;
            }
        }

        public bool IsTop(IUIContainerItem item)
        {
            WarnIfNotParentOrNull(item);
            if (_uiStack.Count == 0) return false;
            var last = _uiStack.Last();
            return last == item;
        }

        public void Top(IUIContainerItem item
#if DEBUG
            , [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            WarnIfNotParentOrNull(item);
            var idx = _uiStack.IndexOf(item);
            if (idx < 0)
            {
                Debug.LogError("该ui不在显示栈内，请先Push");
                return;
            }

            if (idx == _uiStack.Count - 1) return; //已经是栈顶元素了
            PublishStackChanging();
            var curTop = _uiStack.Last();
            _uiStack.InternalBuffer.Add(item);
            _uiStack.InternalBuffer.RemoveAt(idx);
            item.GameObject.SetActive(false);
            PublishStackChanged();

            #region 发布focus事件

            curTop.OnUIUnFocused();
            item.OnUIFocused();

            #endregion
        }

        public void Hide(
#if DEBUG
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            gameObject.Hide();
        }

        public void Show(
#if DEBUG
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            gameObject.Show();
        }

        public void Clear()
        {
            for (var index = _uiList.Count - 1; index >= 0; index--)
            {
                var uiContainerItem = _uiList[index];
                Remove(uiContainerItem);
            }
        }

        public VList<IUIContainerItem> GetUIList()
        {
            return _uiList.ToVList();
        }

        private void OnDestroy()
        {
            for (var index = _uiList.Count - 1; index >= 0; index--)
            {
                var item = _uiList[index];
                Remove(item);
            }

            _uiList.Dispose();
            _uiStack.Dispose();
        }

        #endregion

        private class MaskObj : MonoBehaviour
        {
            private Image _img;
            private Tween _hideTween;

            private void Awake()
            {
                _hideTween.onComplete = () => gameObject.Hide();
            }

            private void Start()
            {
                gameObject.GetRectTransform().AnchorCornersAndFull();
                _img = gameObject.AddComponent<Image>();
                _img.color = new Color(0, 0, 0, 0.8f);
                _hideTween = _img.DOFade(0, 0.2f);
                _hideTween.SetAutoKill(false);
                _hideTween.onComplete = () =>
                {
                    if (_img) _img.gameObject.Hide();
                };
                _hideTween.Pause();
            }

            public void Hide()
            {
                _hideTween.PlayForward();
            }

            public void Show()
            {
                gameObject.Show();
                _hideTween.PlayBackwards();
            }
        }
    }
}