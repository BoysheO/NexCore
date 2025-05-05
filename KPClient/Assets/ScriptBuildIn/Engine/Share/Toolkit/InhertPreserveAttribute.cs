using System;
using UnityEngine.Scripting;

namespace NexCore.Toolkit
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public sealed class InheritedPreserveAttribute : PreserveAttribute
    {
    }
}