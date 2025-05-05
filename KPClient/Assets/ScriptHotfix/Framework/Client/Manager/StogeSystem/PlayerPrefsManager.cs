#nullable enable
using System;
using System.Collections.Generic;
using NexCore.DI;
using BoysheO.Collection2;
using BoysheO.Extensions;
using UnityEngine;

namespace HotScripts.Hotfix.GamePlay.FrameworkSystems.PlayerPrefsSystem
{
    /// <summary>
    /// 备注：PlayerPrefs在WebGL上最多储存1MB数据，见<see cref="https://docs.unity3d.com/ScriptReference/PlayerPrefs.html"/>
    /// </summary>
    [Service(typeof(PlayerPrefsManager))]
    public class PlayerPrefsManager
    {
        private void ThrowIfKeyIllegal(string key)
        {
            if (key.IsNullOrWhiteSpace() || key[^1].Is0to9())
                throw new ArgumentOutOfRangeException(nameof(key), "应为非空且不以数字结尾");
        }

        public VList<int> GetIntList(string key)
        {
            ThrowIfKeyIllegal(key);
            var count = PlayerPrefs.GetInt(key); //已储存的表长
            var lst = VList<int>.Rent();
            for (int i = 0; i < count; i++)
            {
                var curKey = key + i;
                var f = PlayerPrefs.GetInt(curKey);
                lst.Add(f);
            }

            return lst;
        }

        public void SetIntList(string key, IReadOnlyList<int> data)
        {
            ThrowIfKeyIllegal(key);
            var count = PlayerPrefs.GetInt(key).Max(data.Count); //已储存的表长
            for (int i = 0; i < count; i++)
            {
                var curKey = key + i;
                if (i < data.Count)
                {
                    PlayerPrefs.SetInt(curKey, data[i]);
                }
                else PlayerPrefs.DeleteKey(curKey);
            }

            PlayerPrefs.SetInt(key, data.Count);
        }


        public VList<float> GetFloatList(string key)
        {
            ThrowIfKeyIllegal(key);
            var count = PlayerPrefs.GetInt(key); //已储存的表长
            var lst = VList<float>.Rent();
            for (int i = 0; i < count; i++)
            {
                var curKey = key + i;
                var f = PlayerPrefs.GetFloat(curKey);
                lst.Add(f);
            }

            return lst;
        }


        public void SetFloatList(string key, IReadOnlyList<float> data)
        {
            ThrowIfKeyIllegal(key);
            var count = PlayerPrefs.GetInt(key).Max(data.Count); //已储存的表长
            for (int i = 0; i < count; i++)
            {
                var curKey = key + i;
                if (i < data.Count)
                {
                    PlayerPrefs.SetFloat(curKey, data[i]);
                }
                else PlayerPrefs.DeleteKey(curKey);
            }

            PlayerPrefs.SetInt(key, data.Count);
        }
    }
}