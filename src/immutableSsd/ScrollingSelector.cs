using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Optional;
using piCommon;

namespace immutableSsd
{
    public class ScrollingSelector<T> : ISelector<T>
    {
        public ScrollingSelector(Func<uint, ITicker> getTicker,  uint delay,
            uint endsDelay, uint availableDigits, ImmutableList<T> values)
        {
            Debug.Assert(availableDigits > 0);

            this.getTicker = getTicker;
            this.delay = delay;
            this.endsDelay = endsDelay;
            this.availableDigits = availableDigits;
            this.values = values;
            this.offset = 0;
            this.ticker = values.Count <= availableDigits ? 
                Option.None<ITicker>() : getTicker(endsDelay).Some();
        }

        private ScrollingSelector(ScrollingSelector<T> source,
            Func<uint, ITicker> getTicker = null, uint? delay = null,
            uint? endsDelay = null, uint? availableDigits = null,
            ImmutableList<T> values = null, int? offset = null,
            Option<ITicker>? ticker = null)
        {
            this.getTicker = getTicker ?? source.getTicker;
            this.delay = delay ?? source.delay;
            this.endsDelay = endsDelay ?? source.endsDelay;
            this.availableDigits = availableDigits ?? source.availableDigits;
            this.values = values ?? source.values;
            this.offset = offset ?? source.offset;
            this.ticker = ticker ?? source.ticker;
        }

        public ImmutableList<T> GetSelected()
        {
            if (values == ImmutableList<T>.Empty)
                return values;
            return values.GetRange(
                offset,
                Math.Min((int)availableDigits, values.Count - offset));
        }

        public uint? NextTick(uint currentTime) => PiUtils.NextTick(ticker, currentTime);

        public ISelector<T> Tick(uint currentTime)
        {
            var result = this;
            ticker.MatchSome((tck) => {
                if (tck.Ticked(currentTime))
                {
                    var offsetN = offset >= values.Count - availableDigits ? 0 : offset + 1;
                    var currentDelay = offsetN == 0 || offsetN == values.Count - availableDigits ?
                        endsDelay : delay;
                    result = new ScrollingSelector<T>(this, offset: offsetN,
                        ticker: getTicker(currentDelay).SomeNotNull());
                }
            });
            return result;
        }

        private readonly Func<uint, ITicker> getTicker;
        private readonly uint delay;
        private readonly uint endsDelay;
        private readonly uint availableDigits;
        private readonly ImmutableList<T> values;

        private readonly int offset;
        private readonly Option<ITicker> ticker;
    }
}
