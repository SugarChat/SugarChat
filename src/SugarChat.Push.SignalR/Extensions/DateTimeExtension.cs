using System;

namespace SugarChat.Push.SignalR.Extensions
{
    public static class DateTimeExtension
    {
        public static TimeSpan GetRedisExpireTimeSpanFromMinutes(int minute = 5)
        {
            var multiple = minute / 5 == 0 ? 1 : minute / 5;
            var millisecond = CacheRandom.Next(1000, 10 * 1000 * multiple);
            //(0s-10s) * multiple
            return TimeSpan.FromMilliseconds(minute * 60 * 1000 + millisecond);
        }

        private static readonly Random CacheRandom = new Random(DateTime.Now.Second);
    }
}
