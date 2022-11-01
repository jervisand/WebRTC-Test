using System;
using UnityEngine;

namespace Nagih
{
    public class GameTime
    {
        public DateTimeOffset ServerLoginTime { get; private set; }

        public GameTime(long serverTime)
        {
            ServerLoginTime = DateTimeOffset.FromUnixTimeMilliseconds(serverTime);
            ServerLoginTime.AddSeconds(-(long)Time.realtimeSinceStartup);
        }

        public int DayElapse(long currentTime, long pastTime)
        {
            var current = DateTimeOffset.FromUnixTimeMilliseconds(currentTime);
            var past = DateTimeOffset.FromUnixTimeMilliseconds(pastTime);
            return DayElapse(current, past);
        }

        public int DayElapse(DateTimeOffset currentTime, DateTimeOffset pastTime)
        {
            return (currentTime - pastTime).Days;
        }

        public int DayElapseWithoutDate(long currentTime, long pastTime)
        {
            var current = DateTimeOffset.FromUnixTimeMilliseconds(currentTime);
            var past = DateTimeOffset.FromUnixTimeMilliseconds(pastTime);
            return DayElapseWithoutDate(current, past);
        }

        public int DayElapseWithoutDate(DateTimeOffset currentTime, DateTimeOffset pastTime)
        {
            return (currentTime.Date - pastTime.Date).Days;
        }

        public int SecondElapse(DateTimeOffset currentTime, DateTimeOffset pastTime)
        {
            return (int)(currentTime - pastTime).TotalSeconds;
        }

        public int MinuteElapse(DateTimeOffset currentTime, DateTimeOffset pastTime)
        {
            return (int)(currentTime - pastTime).TotalMinutes;
        }

        public DateTimeOffset CurrentTime
        {
            get
            {
                return ServerLoginTime.AddSeconds((long)Time.realtimeSinceStartup);
            }
        }
    }
}
