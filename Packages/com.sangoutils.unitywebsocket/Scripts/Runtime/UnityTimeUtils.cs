using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SangoUtils.UnityWebsockets
{
    internal static class UnityTimeUtils
    {
        public static long GetCurrentTimestampInMillis()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTime = DateTime.UtcNow - epochStart;
            return (long)currentTime.TotalMilliseconds;
        }
    }
}