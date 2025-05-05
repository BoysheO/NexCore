using System;
using NexCore.DI;
using BoysheO.Extensions;
using ScriptEngine.BuildIn.ShareCode.Manager.LanSystem.Abstractions;

namespace ScriptGamePlay.Hotfix.ClientCode.Manager.TimeSpanSystem
{
    [Service(typeof(TimeSpanManager))]
    public class TimeSpanManager
    {
        /*
         * 1550	天
         * 1551	时
         * 1552	分
         * 1553	秒
         */
        private const int Day = 1550;
        private const int Hour = 1551;
        private const int Min = 1552;
        private const int Sec = 1553;
        
        private ILanManager _lanManager;

        public TimeSpanManager(ILanManager lanManager)
        {
            _lanManager = lanManager;
        }

        /// <summary>
        /// 显示时间片的最大单位
        /// ex. 01:02:01
        /// </summary>
        public string GetString(TimeSpan span)
        {
            //todo 多语言GetText这一块应该可以优化下
            if (span.TotalDays > 1)
            {
                return span.TotalDays.FloorToInt() + _lanManager.GetText(Day);
            }

            if (span.TotalHours > 1)
            {
                return span.TotalHours.FloorToInt() + _lanManager.GetText(Hour);
            }

            if (span.TotalMinutes > 1)
            {
                return span.TotalMinutes.FloorToInt() + _lanManager.GetText(Min);
            }

            return span.TotalSeconds.FloorToInt() + _lanManager.GetText(Sec);
        }
    }
}