#nullable enable
using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UISystem.Abstractions;
using UnityEngine;

namespace Hotfix.UIScripts.Common
{
    //示例UIBase。根据自己需要编写。
    public abstract class UIBase : MonoBehaviour, IView
    {
        public GameObject GameObject => gameObject;

        public IUIContainer UIContainer => ((IUIContainerItem)this).UIContainer;

        //由框架管理
        IUIContainer IUIContainerItem.UIContainer { get; set; }

        public IUILoader? Loader => ((IView)this).Loader;

        //由框架管理
        IUILoader? IView.Loader { get; set; }

        private Tween? _cgTween;

        UniTask IView.OnUILoaded()
        {
            return OnUILoaded();
        }

        protected virtual UniTask OnUILoaded()
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnUIFocused()
        {
            _cgTween.PlayForward();
        }

        public virtual void OnAddedInUIContainer()
        {
            InitTween();
        }

        public virtual void OnRemovedFromUIContainer()
        {
            Destroy(gameObject);
        }

        private void InitTween()
        {
            if (_cgTween != null) return;
            var cg = gameObject.GetOrAddComponent<CanvasGroup>();
            cg.alpha = 0;
            _cgTween = cg.DOFade(1, 0.2f);
            _cgTween.SetAutoKill(false);
        }

        //便利型函数
        public void Show()
        {
            _cgTween.PlayBackwards();
            UIContainer.PushUI(this);
        }

        //便利型函数
        public void Hide()
        {
            UIContainer.PopUI(this);
        }

        //便利型函数
        public void DestroyUI()
        {
            //如果在Awake的时候调用，则UIContainer会为null。UIContainer是在Awake之后，Start之前由UIContainer自己注入的
            //如果已经Remove过了，这里也会为null
            UIContainer.Remove(this);
        }
    }
}