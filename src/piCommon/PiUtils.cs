using System;
using System.Collections.Immutable;
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

        public static ImmutableList<T> Fill<T>(this ImmutableList<T> source, uint size, T fillValue)
        {
            var diff = size - source.Count;
            if (diff <= 0) return source;

            var result = source;
            for (int i = 0; i < diff; i++)
            {
                result = result.Add(fillValue);
            }
            return result;
        }
    }
}
