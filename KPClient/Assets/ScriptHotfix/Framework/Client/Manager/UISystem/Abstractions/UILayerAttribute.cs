using System;

namespace UISystem.Abstractions
{
    /// <summary>
    /// 标记一个UI在使用UIManager并且以无UILayer参数、无指定UIContainer形式加载时，应当取什么UILayer值
    /// * 只应附加在IView实现上
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class UILayerAttribute:System.Attribute
    {
        public UILayerAttribute(UILayer uiLayer)
        {
            UILayer = uiLayer;
        }

        public UILayer UILayer { get; }
    }
}