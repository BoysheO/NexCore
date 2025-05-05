#if !ODIN_INSPECTOR
namespace Sirenix.OdinInspector
{
    //由于OdinInspector不是免费的，这里单独做Attribute令编译顺利通过
    public sealed class ShowInInspectorAttribute : System.Attribute
    {
    }

    public sealed class ButtonAttribute : System.Attribute
    {
    }

    public sealed class PropertyRangeAttribute : System.Attribute
    {
        public PropertyRangeAttribute(float a, float b)
        {
        }
    }

    public sealed class ReadOnlyAttribute : System.Attribute
    {
    }

    public sealed class ShowIfAttribute : System.Attribute
    {
        public ShowIfAttribute(string str)
        {
        }
    }
}

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
    public class OdinEditorWindow : UnityEditor.EditorWindow
    {
    }

    public class OdinEditor : UnityEditor.Editor
    {
    }
}
#endif
#endif