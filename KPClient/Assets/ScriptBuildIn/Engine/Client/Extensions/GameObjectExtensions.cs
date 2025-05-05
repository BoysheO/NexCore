#nullable enable
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Unity.Linq;
using UnityEngine;
using BoysheO.Util;

namespace BoysheO.Extensions.Unity3D.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null) throw new ArgumentNullException(nameof(go));
            var t = go.GetComponent<T>();
            if (t == null) t = go.AddComponent<T>();

            return t;
        }

        /// <summary>
        /// 如果没有指定组件，则抛异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequireComponent<T>(this GameObject o)
        {
            var com = o.GetComponent<T>();
            if (com is null)
            {
                Debug.LogError($"{o.name} is missing component {typeof(T).Name}", o);
                throw new Exception($"{o.name} is missing component {typeof(T).Name}");
            }

            return com;
        }

        /// <summary>
        /// 如果没有指定组件，则抛异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequireComponentInChildren<T>(this GameObject o)
            where T : Component
        {
            return o.GetComponentInChildren<T>() ??
                   throw new Exception($"{o.name} missing child component {typeof(T).Name}");
        }

        public static GameObject RequireChild(this GameObject o, string name)
        {
            if (o == null) throw new ArgumentNullException(nameof(o));
            return o.Child(name) ?? throw new Exception($"GameObject:{o.GetHierarchyPath()} has no child {name}");
        }

        /// <summary>
        /// 获得go在Hierarchy的路径<br />
        /// no gc
        /// </summary>
        public static string GetHierarchyPath(this GameObject go)
        {
            if (go == null) throw new ArgumentNullException(nameof(go));
            var buff = ArrayPool<string>.Shared.Rent(1);
            int count = 0;
            foreach (var gameObject in go.AncestorsAndSelf())
            {
                RefArrayPoolUtil.Add(ref buff,ref count,gameObject.name);
            }
            
            buff.AsSpan(0,count).Reverse();
            var res = string.Join("/", buff,0,count);
            ArrayPool<string>.Shared.Return(buff,true);
            return res;
        }

        public static GameObject[] SelectChildrenToPoolArray(this GameObject gameObject, out int count)
        {
            var buf = gameObject.transform.ToPoolArray(out count);
            var buff = ArrayPool<GameObject>.Shared.Rent(count);
            for (var index = count - 1; index >= 0; index--)
            {
                var transform = buf[index];
                buff[index] = transform.gameObject;
            }

            ArrayPool<Transform>.Shared.Return(buf);
            return buff;
        }

        public static GameObject[] SelectChildrenAliveToPooledArray(this GameObject gameObject, out int count)
        {
            var transforms = gameObject.transform.ToPoolArray(out count);
            var gameObjects = ArrayPool<GameObject>.Shared.Rent(count);
            var gameObjectsCount = 0;
            for (var index = 0; index < count; index++)
            {
                var transform = transforms[index];
                if (transform == null) continue;
                gameObjects[gameObjectsCount] = transform.gameObject;
                gameObjectsCount++;
            }

            ArrayPool<Transform>.Shared.Return(transforms);
            return gameObjects;
        }

        public static GameObject GetChild(this GameObject root, int i)
        {
            return root.transform.GetChild(i).gameObject;
        }

        public static RectTransform GetRectTransform(this GameObject go)
        {
            return go.transform as RectTransform ?? throw new Exception($"{go.name} is not RectTransform");
        }

        public static float GetWidth(this GameObject gameObject)
        {
            return gameObject.GetRectTransform().rect.width;
        }

        public static float GetHeight(this GameObject gameObject)
        {
            return gameObject.GetRectTransform().rect.height;
        }
        
        public static void ThrowOperationCanceledExceptionIfNull(this GameObject go)
        {
            if (go == null) throw new OperationCanceledException("gameObject is null");
        }
        
        /// <summary>
        /// 等价于Destroy(go)，使用频率高故独立成函数
        /// </summary>
        public static void MarkDestroy(this GameObject go)
        {
            UnityEngine.Object.Destroy(go);
        }
        
        public static GameObject Hide(this GameObject go)
        {
            go.SetActive(false);
            return go;
        }
        
        public static GameObject Show(this GameObject go)
        {
            go.SetActive(true);
            return go;
        }
    }
}