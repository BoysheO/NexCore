using System;

namespace ScriptGamePlay.Hotfix.ShareCode.Manager.ProfileSystem
{
    public interface IProfileManager
    {
        bool IsEnable { get; set; }

        /// <summary>
        /// 对标Unity的Profile.Sample，单线程支持
        /// </summary>
        /// <param name="method"></param>
        /// <param name="context">建议传入CallerContext</param>
        /// <typeparam name="T">当前类</typeparam>
        /// <returns>结束统计</returns>
        IDisposable BeginSampleProfile<T>(string method,string? context = null);
    }
}