using System;
using Optional;

namespace piCommon
{
    public static class PiUtils
    {
        public static uint? Min(uint? first, uint? second)
        {
            if (!first.HasValue) return second;
            if (!second.HasValue) return first;

            return first < second ? first : second;
        }

        public static uint? NextTick(Option<ITicker> ticker, uint currentTime)
        {
            uint? result = null;
            ticker.MatchSome((tck) => result = tck.Remaining(currentTime));
            return result;
        }
    }
}
