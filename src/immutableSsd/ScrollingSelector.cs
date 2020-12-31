using System;
using System.Collections.Immutable;
using System.Diagnostics;
using piCommon;

namespace immutableSsd
{
    public class ScrollingSelector<T> : ISelector<T>
    {
        public ScrollingSelector(Func<object, uint, Action> requestInterrupt, 
            uint delay, uint endsDelay, uint availableDigits) : 
            this(requestInterrupt, delay, endsDelay, availableDigits, ImmutableList<T>.Empty, 0)
        {
        }

        private ScrollingSelector(Func<object, uint, Action> requestInterrupt, uint delay,
            uint endsDelay, uint availableDigits, ImmutableList<T> values, int offset)
        {
            Debug.Assert(availableDigits > 0);

            this.requestInterrupt = requestInterrupt;
            this.delay = delay;
            this.endsDelay = endsDelay;
            this.availableDigits = availableDigits;
            this.values = values;
            this.offset = offset;

            if (values.Count > availableDigits)
            {
                var currentDelay = delay;
                if (offset == 0 || offset == values.Count - availableDigits)
                {
                    currentDelay = endsDelay;
                }
                cancelInterrupt = requestInterrupt(this, currentDelay);
            }
        }

        public ISelector<T> UpdateValues(ImmutableList<T> newValues)
        {
            cancelInterrupt?.Invoke();
            return new ScrollingSelector<T>(
                requestInterrupt, delay, endsDelay, availableDigits, newValues, 0);
        }

        public ImmutableList<T> GetSelected()
        {
            if (values == ImmutableList<T>.Empty)
                return values;
            return values.GetRange(
                offset,
                Math.Min((int)availableDigits, values.Count - offset));
        }

        public ISelector<T> ReceiveInterrupt(object caller)
        {
            if (caller == this && caller is ScrollingSelector<T> selector)
            {
                return new ScrollingSelector<T>(
                    requestInterrupt, delay, endsDelay, availableDigits, values,
                    offset >= values.Count - availableDigits ? 0 : offset + 1);
            }
            return this;
        }

        private readonly Func<object, uint, Action> requestInterrupt;
        private readonly uint delay;
        private readonly uint endsDelay;
        private readonly uint availableDigits;
        private readonly ImmutableList<T> values;
        private readonly int offset;

        private readonly Action cancelInterrupt;
    }
}
