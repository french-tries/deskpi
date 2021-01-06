using System;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace immutableSsd.test.stubs
{
    public class SsdWriterStub<T> : ISsdWriter<ImmutableList<T>>
    {
        public uint AvailableDigits => 3;

        public ISsdWriter<ImmutableList<T>> Write(ImmutableList<T> values)
        {
            LastValues = values;
            return this;
        }

        public uint? NextTick(uint currentTime) => NextTickTime;
        public ISsdWriter<ImmutableList<T>> Tick(uint currentTime)
        {
            TickTime = currentTime;
            return NextTickTime == 0 ? new SsdWriterStub<T>(): this;
        }

        public uint NextTickTime { get; set; } = 1;
        public uint TickTime { get; set; } = 0;

        public ImmutableList<T> LastValues { get; private set; } = ImmutableList<T>.Empty;
    }
}
