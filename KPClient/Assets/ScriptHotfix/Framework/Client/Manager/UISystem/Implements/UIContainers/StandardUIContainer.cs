#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BoysheO.Collection2;
using BoysheO.Extensions.Unity3D.Extensions;
using UISystem.Abstractions;
using UnityEngine;
using BuildInConstConfig = ScriptBuildIn.Engine.Client.Configuration.BuildInConstConfig;

namespace UISystem.Implements.UIContainers
{
    //标准的UIContainer实现，在标准UIContainer中不允许Item的GameObject为空，并且假定在移出容器前一直存在
    //其他UIContainer的实现需要严格遵守此UIContainer的事件发布时机，以保证兼容性。非标容器可以自己决定要不要检查GameObject
    public class StandardUIContainer : MonoBehaviour, IUIContainer
    {
        private const bool IsDEBUG = BuildInConstConfig.IsDebug;
        private const bool IsTracking = false; //以后再实现追踪，现在先不搞
        private GameObject _root => gameObject;

        //表末为栈顶
        protected readonly VList<IUIContainerItem> _uiStack = VList<IUIContainerItem>.Rent();
        protected readonly VList<IUIContainerItem> _uiList = VList<IUIContainerItem>.Rent();

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
            if (item.GameObject == null) throw new Exception("标准容器禁止null GameObject");
            if (item.GameObject.activeSelf) throw new Exception("go的激活状态是由Container托管的，要求Add的时候是false");
            item.GameObject.transform.SetParent(_root.transform);
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

        public virtual void PushUI(IUIContainerItem item
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
                item.GameObject.transform.SetParent(_root.transform);
                item.GameObject.transform.SetAsLastSibling();
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
                uiContainerItem.OnStackChanged(index, _uiStack.InternalBuffer);
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
                if (item.GameObject == null)
                {
                    Debug.LogError($"item似乎在从容器移除前就被Destroy了，这是不允许的，应视作异常");
                }

                if (item.GameObject!.transform.parent != _root.transform)
                {
                    Debug.LogError("item的gameObject应由本容器管理的，但是它似乎被改变了");
                }

                if (!_uiList.Contains(item))
                {
                    Debug.LogError("不是本容器管理的Item");
                }
            }
        }

        public virtual void PopUI(IUIContainerItem item
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
                item.OnUIPopped();

                item.OnStackChanged(-1, _uiStack.InternalBuffer);
                PublishStackChanged();

                if (nextTopUI != null)
                {
                    nextTopUI.OnUIFocused();
                }
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
            _root.Hide();
        }

        public void Show(
#if DEBUG
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
#endif
        )
        {
            _root.Show();
        }

        public void Clear()
        {
            for (var index = _uiList.Count - 1; index >= 0; index--)
            {
                var uiContainerItem = _uiList[index];
                Remove(uiContainerItem);
            }
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
    }
}