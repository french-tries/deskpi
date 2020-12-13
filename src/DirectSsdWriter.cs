﻿using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace immutableSsd.src
{
    public class DirectSsdWriter : ISsdWriter<ImmutableList<byte>>
    {
        public DirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            Action<Pin, bool> writeAction, IInterruptHandler interruptHandler, uint interval) :
            this(segmentPins, digitPins, writeAction, interruptHandler, interval, 
                ImmutableList<byte>.Empty)
        {
        }

        private DirectSsdWriter(ImmutableList<Pin> segmentPins, ImmutableList<Pin> digitPins,
            Action<Pin, bool> writeAction, IInterruptHandler interruptHandler, uint interval,
            ImmutableList<byte> values, int currentDigit = 0)
        {
            Debug.Assert(segmentPins.Count == 8);

            this.segmentPins = segmentPins;
            this.digitPins = digitPins;
            this.writeAction = writeAction;
            this.interruptHandler = interruptHandler;
            this.interval = interval;
            this.values = values;
            this.currentDigit = currentDigit;

            Write();
            interruptHandler.RequestInterrupt(this, interval);
        }

        public ISsdWriter<ImmutableList<byte>> Write(ImmutableList<byte> newValues) =>
            new DirectSsdWriter(segmentPins, digitPins, writeAction, interruptHandler, interval, newValues);

        public ISsdWriter<ImmutableList<byte>> ReceiveInterrupt(object caller, uint currentTime)
        {
            if (caller == this && caller is DirectSsdWriter writer)
            {
                var nextDigit = currentDigit + 1;
                if (nextDigit >= digitPins.Count) nextDigit = 0;

                return new DirectSsdWriter(segmentPins, digitPins, writeAction,
                    interruptHandler, interval, values, nextDigit);
            }
            return this;
        }

        public int AvailableDigits => digitPins.Count;

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

        private readonly Action<Pin, bool> writeAction;
        private readonly ImmutableList<Pin> segmentPins;
        private readonly ImmutableList<Pin> digitPins;

        private readonly ImmutableList<byte> values;
        private readonly IInterruptHandler interruptHandler;
        private readonly int currentDigit;
        private readonly uint interval;
    }
}
