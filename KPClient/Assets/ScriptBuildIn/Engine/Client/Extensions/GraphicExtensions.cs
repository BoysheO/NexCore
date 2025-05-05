using UnityEngine.UI;

namespace NormalScripts.BearMonoScript.Extensions
{
    public static class GraphicExtensions
    {
        public static float GetAlpha(this UnityEngine.UI.Graphic graph)
        {
            return graph.color.a;
        }

        public static T SetAlpha<T>(this T graphic, float v) where T : UnityEngine.UI.Graphic
        {
            var c = graphic.color;
            c.a = v;
            graphic.color = c;
            return graphic;
        }
    }
}