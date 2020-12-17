using System;
using System.Collections.Immutable;
using System.Diagnostics;
using piCommon;

namespace immutableSsd
{
    public class ScrollingSelector<T> : ISelector<T>
    {
        public ScrollingSelector(IInterruptHandler handler, 
            uint delay, uint endsDelay, uint availableDigits) : 
            this(handler, delay, endsDelay, availableDigits, ImmutableList<T>.Empty, 0)
        {
        }

        private ScrollingSelector(IInterruptHandler handler, uint delay,
            uint endsDelay, uint availableDigits, ImmutableList<T> values, int offset)
        {
            Debug.Assert(availableDigits > 0);

            this.handler = handler;
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
                handler.RequestInterrupt(this, currentDelay);
            }
        }

        public ISelector<T> UpdateValues(ImmutableList<T> newValues)
        {
            handler.UnrequestInterrupt(this);
            return new ScrollingSelector<T>(
                handler, delay, endsDelay, availableDigits, newValues, 0);
        }

        public ImmutableList<T> GetSelected()
        {
            if (values == ImmutableList<T>.Empty)
                return values;
            return values.GetRange(
                offset,
                Math.Min((int)availableDigits, values.Count - offset));
        }

        public ISelector<T> ReceiveInterrupt(object caller, uint currentTime)
        {
            if (caller == this && caller is ScrollingSelector<T> selector)
            {
                return new ScrollingSelector<T>(
                    handler, delay, endsDelay, availableDigits, values,
                    offset >= values.Count - availableDigits ? 0 : offset + 1);
            }
            return this;
        }

        private readonly IInterruptHandler handler;
        private readonly uint delay;
        private readonly uint endsDelay;
        private readonly uint availableDigits;
        private readonly ImmutableList<T> values;
        private readonly int offset;
    }
}
