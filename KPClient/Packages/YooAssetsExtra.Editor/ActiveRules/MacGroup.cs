using YooAsset.Editor;

namespace YooAssetsExtra.Editor.ActiveRules
{
    [DisplayName("mac分组")]
    public class MacGroup: IActiveRule
    {
        public bool IsActiveGroup(GroupData data)
        {
#if UNITY_STANDALONE_OSX
            return true;
#else
            return false;
#endif
        }
    }
}