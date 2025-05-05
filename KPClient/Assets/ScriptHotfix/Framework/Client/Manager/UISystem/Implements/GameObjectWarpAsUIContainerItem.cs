using UISystem.Abstractions;
using UnityEngine;

namespace UISystem.Implements
{
    //将一个GameObject节点简单地包装成一个UIContainer
    public class GameObjectWarpAsUIContainerItem : IUIContainerItem
    {
        public GameObjectWarpAsUIContainerItem(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public GameObject GameObject { get; }

        public IUIContainer UIContainer => ((IUIContainerItem)this).UIContainer;
        IUIContainer IUIContainerItem.UIContainer { get; set; }
    }
}