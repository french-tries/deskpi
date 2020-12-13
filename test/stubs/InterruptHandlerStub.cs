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
            requested = requested.Enqueue((caller, delay));
        }

        public void TestRequested(object expectedCaller, uint expectedDelay)
        {
            (object caller, uint delay) = requested.Peek();

            Assert.AreEqual(expectedCaller, caller);
            Assert.AreEqual(expectedDelay, delay);

            requested = requested.Dequeue();
        }

        public void TestRequestedEmpty()
        {
            Assert.AreEqual(ImmutableQueue<(object, uint)>.Empty, requested);
        }

        public void UnrequestInterrupt(object caller)
        {
            unrequested = unrequested.Enqueue(caller);
        }

        public void TestUnrequested(object expectedCaller)
        {
            object caller = unrequested.Peek();

            Assert.AreEqual(expectedCaller, caller);

            unrequested = unrequested.Dequeue();
        }

        public void TestUnrequestedEmpty()
        {
            Assert.AreEqual(ImmutableQueue<object>.Empty, unrequested);
        }

        private ImmutableQueue<(object, uint)> requested = ImmutableQueue<(object, uint)>.Empty;
        private ImmutableQueue<object> unrequested = ImmutableQueue<object>.Empty;
    }
}
