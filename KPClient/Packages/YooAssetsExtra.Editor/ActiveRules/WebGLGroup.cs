using YooAsset.Editor;

namespace YooAssetsExtra.Editor.ActiveRules
{
    [DisplayName("webGL分组")]
    public class WebGLGroup: IActiveRule
    {
        public bool IsActiveGroup(GroupData data)
        {
#if UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }
    }
}