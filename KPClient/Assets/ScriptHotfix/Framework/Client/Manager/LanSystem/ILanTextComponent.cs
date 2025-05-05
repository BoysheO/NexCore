using System;
using System.Collections.Generic;

namespace HotScripts.Hotfix.GamePlay.FrameworkSystems.LanSystem
{
    public interface ILanTextComponent
    {
        /// <summary>
        /// 语言的Key
        /// </summary>
        public int LanKey { get; set; }
        
        //留下这个注释，避免以后又将参数化接口添加进来
        // /// <summary>
        // /// format参数。没有则不要动。不要保留此list引用，即用即弃
        // /// </summary>
        // [Obsolete("我认为当需要用到参数化时，已经不适合使用Component组件来定义文本显示了")]
        // public IList<object> Args { get; }
        
        /// <summary>
        /// 实际展示的文字(便于调试）
        /// </summary>
        public string TextShowing { get; }
    }
}