using UnityEngine;
using UnityEngine.Scripting;

namespace NormalScripts.BearMonoScript
{
    [Preserve]
    [CreateAssetMenu(fileName = "Curve", menuName = "Math/Curve", order = 1)]
    public class AnimationCurveAsset : ScriptableObject
    {
        public AnimationCurve AnimationCurve;
    }
}