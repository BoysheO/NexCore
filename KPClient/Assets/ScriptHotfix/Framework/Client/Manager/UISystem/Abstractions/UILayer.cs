namespace UISystem.Abstractions
{
    /// <summary>
    /// all ui must has the ui layer during the design and immutable
    /// UILayer is ordered by the order of the enum in design
    /// * no negative value support
    /// </summary>
    public enum UILayer
    {
        /// <summary>
        /// 通常是与世界物品挂钩的UI，例如血条，角色名牌
        /// </summary>
        Hud,

        /// <summary>
        /// 一般而言这层放置跟随场景切换而切换的UI
        /// Panel show or hide for scene.ex.HomePanel\BattlePanel
        /// </summary>
        ScenePanel,

        /// <summary>
        /// 一般覆盖整个屏幕的UI都是Panel
        /// 此层具有时自动隐藏距离镜头比Panel更远的容器和非栈顶UI的功能
        /// Panel is the base layer,all the panel should full the screen or let the user to look at the 3D world
        /// </summary>
        Panel,

        /// <summary>
        /// 不覆盖整个屏幕的UI
        /// 此层具有自动Mask背景的功能（阻止点击到后面的UI）
        ///  Window is the mid layer,all the window should be in the center of the screen
        /// </summary>
        Window,

        /// <summary>
        /// 通常是主界面置顶的资源栏
        /// </summary>
        TopBar,

        /// <summary>
        /// 鼠标停留在物品UI上时，弹出来的物品属性提示UI所在层次。
        /// 此UI容器具有根据鼠标位置自动容纳UI的功能（例如鼠标贴近屏幕右方时，UI会在鼠标左侧显示）
        /// 应置入具有确定大小的UI而不是自适应UI
        /// </summary>
        Tips,

        /// <summary>
        /// 当UI组件（例如Toggle）需要屏蔽全屏输入时，使用的输入遮蔽UI在这层发生
        /// 通常这一层不会显示任何物件，仅屏蔽
        /// </summary>
        UITogMask,

        /// <summary>
        /// 教程遮罩
        /// </summary>
        MaskPanel,

        GMPanel,
        
        News,

        /// <summary>
        /// 弹出式窗口，比如提示框，确认框等。
        /// 与Windows层类似，也具有显示Mask的功能。
        /// 当玩家点击Mask，容器会将UI出栈，UI需要注意处理相关事件
        /// </summary>
        Popup,

        /// <summary>
        /// 当UI跟随鼠标表达抓取、拖拽行为时，该UI显示在这一层
        /// 此容器尺寸为屏幕大小，中心点总是跟随鼠标移动
        /// </summary>
        Cursor,

        UILoading,

        /// <summary>
        /// 浮动文字专属
        /// </summary>
        FlowText,

        /// <summary>
        /// UIWaiting界面专属
        /// </summary>
        UIWaiting,
    }
}