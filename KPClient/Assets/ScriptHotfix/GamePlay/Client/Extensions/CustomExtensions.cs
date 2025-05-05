using System.Threading;
using NexCore.DI;
using NexCore.UnityEnvironment;
using BoysheO.Extensions.Unity3D.Extensions;
using Cysharp.Threading.Tasks;
using Hotfix.ContentSystems;
using Hotfix.Helper;
using Hotfix.LanMgr;
using Microsoft.Extensions.DependencyInjection;
using Primitive;
using UniRx;

namespace Hotfix.Extensions
{
    public static class CustomExtensions
    {
        public static Unity.Linq.GameObjectExtensions.ChildrenEnumerable Hide(
            this Unity.Linq.GameObjectExtensions.ChildrenEnumerable childrenEnumerable)
        {
            foreach (var gameObject in childrenEnumerable)
            {
                gameObject.Hide();
            }

            return childrenEnumerable;
        }
    }
}