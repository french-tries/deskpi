using System;
using System.Collections.Immutable;
using System.Linq;
using Optional;

namespace piCommon
{
    public static class PiUtils
    {
        public static uint? Min(params uint?[] values)
        {
            var nonNulls = from value in values
            where value.HasValue
            select value.Value;

            if (!nonNulls.Any())
            {
                return null;
            }
            return nonNulls.Min();
        }

        public static uint? NextTick(Option<ITicker> ticker, uint currentTime  )
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
