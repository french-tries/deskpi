using System;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;

namespace immutableSsd.test.stubs
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

        public void TestCaller(object expected)
        {
            if (lastCaller == null)
            {
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual(expected, lastCaller);
            }
        }

        public void Reset()
        {
            LastValues = null;
            lastCaller = null;
        }

        public ISsdWriter<ImmutableList<T>> Write(ImmutableList<T> values)
        {
            LastValues = values;
            return this;
        }

        public ISsdWriter<ImmutableList<T>> ReceiveInterrupt(object caller)
        {
            lastCaller = caller;
            return this;
        }

        public ImmutableList<T> LastValues { get; private set; }
        private object lastCaller;
    }
}
