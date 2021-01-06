using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Optional;
using piCommon;

namespace immutableSsd
{
    public class DirectSsdWriter : ISsdWriter<ImmutableList<byte>>
    {
        public DirectSsdWriter(ImmutableList<int> segmentPins, ImmutableList<int> digitPins,
            Action<int, bool> writeAction, Func<ITicker> getTicker)
        {
            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.writeAction = writeAction;
            this.getTicker = getTicker;

            Write();
        }

        private DirectSsdWriter(DirectSsdWriter source, ImmutableList<int> segmentPins = null,
            ImmutableList<int> digitPins = null, Action<int, bool> writeAction = null,
            Func<ITicker> getTicker = null, ImmutableList<byte> values = null,
            Option<ITicker>? ticker = null, int? currentDigit = null)
        {
            Debug.Assert(segmentPins.Count == 8);

            this.segmentPins = segmentPins ?? source.segmentPins;
            this.digitPins = digitPins ?? source.digitPins;
            this.writeAction = writeAction ?? source.writeAction;
            this.getTicker = getTicker ?? source.getTicker;
            this.values = values ?? source.values;
            this.ticker = ticker ?? source.ticker;
            this.currentDigit = currentDigit ?? source.currentDigit;

            Write();
        }

        public ISsdWriter<ImmutableList<byte>> Write(ImmutableList<byte> newValues) =>
            new DirectSsdWriter(this, values : newValues, ticker: getTicker().SomeNotNull(),
                currentDigit: 0);

        public uint? NextTick(uint currentTime) => PiUtils.NextTick(ticker, currentTime);

        public ISsdWriter<ImmutableList<byte>> Tick(uint currentTime)
        {
            var result = this;
            ticker.MatchSome((tck) =>
            {
                if (tck.Ticked(currentTime))
                {
                    var nextDigit = currentDigit + 1;
                    if (nextDigit >= digitPins.Count) nextDigit = 0;

                    result = new DirectSsdWriter(this, ticker: getTicker().Some(),
                        currentDigit: nextDigit);
                }
            });
            return result;
        }

        public uint AvailableDigits => (uint)digitPins.Count;

        private void Clear()
        {
            for (int i = 0; i < digitPins.Count; ++i)
            {
                writeAction(digitPins[i], false);
            }
            for (int i = 0; i < segmentPins.Count; ++i)
            {
                writeAction(segmentPins[i], false);
            }
        }

        private void Write()
        {
            if (currentDigit < 0 || currentDigit >= digitPins.Count || currentDigit >= values.Count)
            {
                Clear();
                return;
            }
            writeAction(digitPins[currentDigit > 0 ? currentDigit - 1 : digitPins.Count - 1], false);

            for (int i = 0; i < segmentPins.Count; ++i)
            {
                writeAction(segmentPins[i], (values[currentDigit] & (1 << (7 - i))) != 0);
            }
            writeAction(digitPins[currentDigit], true);
        }

        private readonly ImmutableList<int> segmentPins;
        private readonly ImmutableList<int> digitPins;
        private readonly Action<int, bool> writeAction;
        private readonly Func<ITicker> getTicker;

        private readonly ImmutableList<byte> values = ImmutableList<byte>.Empty;
        private readonly Option<ITicker> ticker = Option.None<ITicker>();
        private readonly int currentDigit = 0;
    }
}
