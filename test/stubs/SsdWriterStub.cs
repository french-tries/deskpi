using System;
using System.Collections.Immutable;
using System.Linq;
using immutableSsd.src;
using NUnit.Framework;

namespace deskpi.test.stubs
{
    public class SsdWriterStub<T> : ISsdWriter<ImmutableList<T>>
    {
        public int AvailableDigits => 3;

        public void TestValues(ImmutableList<T> expected)
        {
            if (LastValues == null)
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(expected.SequenceEqual(LastValues));
            }
        }

        public void TestUnwritten()
        {
            Assert.IsNull(LastValues);
        }

        public void TestTime(int expected)
        {
            if (lastTime == null)
            {
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual(expected, lastTime);
            }
        }

        public void Reset()
        {
            LastValues = null;
            lastTime = null;
        }

        public ISsdWriter<ImmutableList<T>> Write(ImmutableList<T> values)
        {
            LastValues = values;
            return this;
        }

        public ISsdWriter<ImmutableList<T>> ReceiveInterrupt(object caller, uint currentTime)
        {
            lastTime = currentTime;
            return this;
        }

        public ImmutableList<T> LastValues { get; private set; }
        private uint? lastTime;
    }
}
