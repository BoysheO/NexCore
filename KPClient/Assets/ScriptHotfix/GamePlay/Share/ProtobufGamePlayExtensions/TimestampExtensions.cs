using System;
using Google.Protobuf.WellKnownTypes;

namespace CommonCode.ProtobufGamePlayExtensions
{
    public static class TimestampExtensions
    {
        public static void From(this Timestamp timestamp, DateTimeOffset dateTimeOffset)
        {
            if (timestamp == null) throw new ArgumentNullException(nameof(timestamp));
            var utc = dateTimeOffset.UtcDateTime;
            
            long num1 = utc.Ticks / 10000000L;
            int num2 = (int) (utc.Ticks % 10000000L) * 100;
            timestamp.Seconds = num1 - 62135596800L;
            timestamp.Nanos = num2;
        }
    }
}