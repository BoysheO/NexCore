
using System;

namespace ScriptEngine.BuildIn.ShareCode.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime);
        }
        
        public static DateTimeOffset? ToDateTimeOffset(this DateTime? dateTime)
        {
            if (dateTime == null) return null;
            return new DateTimeOffset(dateTime.Value);
        }
    }
}