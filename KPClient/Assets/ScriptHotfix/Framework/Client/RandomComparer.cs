using System.Collections.Generic;
using UnityEngine;

namespace Hotfix.FrameworkSystems{
//thread safe
public class RandomComparer<T> : IComparer<T>
{
    public static RandomComparer<T> Instance { get; } = new RandomComparer<T>();
    private readonly System.Random Random = new System.Random();

    public int Compare(T x, T y)
    {
        return Random.Next(2) == 0 ? 1 : -1;
    }
}}