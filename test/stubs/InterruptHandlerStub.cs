using System;
using System.Collections.Immutable;
using immutableSsd.src;
using NUnit.Framework;

namespace immutableSsd.test.stubs
{
    public class InterruptHandlerStub : IInterruptHandler
    {
        public void RequestInterrupt(object caller, uint delay)
        {
            written = written.Enqueue((caller, delay));
        }

        public void TestReceived(object expectedCaller, uint expectedDelay)
        {
            (object caller, uint delay) = written.Peek();

            Assert.AreEqual(expectedCaller, caller);
            Assert.AreEqual(expectedDelay, delay);

            written = written.Dequeue();
        }

        public void TestEmpty()
        {
            Assert.AreEqual(ImmutableQueue<(object, uint)>.Empty, written);
        }

        private ImmutableQueue<(object, uint)> written = ImmutableQueue<(object, uint)>.Empty;
    }
}
