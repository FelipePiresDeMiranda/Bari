using System;

namespace Bari.AWS.Infrastructure
{
    public static class Stamp
    {
        public static String GetTimestamp(this DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }
    }
}
