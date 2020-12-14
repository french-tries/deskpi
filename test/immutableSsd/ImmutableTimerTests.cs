using System;
using immutableSsd;
using NUnit.Framework;

namespace immutableSsd.test
{
    [TestFixture]
    public class ImmutableTimerTests
    {
        [TestCase]
        public void Tick_AtStart_Triggers()
        {
            var timer = new ImmutableTimer(2);
            Assert.AreNotEqual(timer, timer.Tick(0));
        }

        [TestCase]
        public void Tick_SmallerThanInterval_ReturnsSameInstance()
        {
            var timer = new ImmutableTimer(2).Tick(0);
            Assert.AreEqual(timer, timer.Tick(1));
        }

        [TestCase]
        public void Tick_LargerThanInterval_ReturnsDifferentInstance()
        {
            var timer = new ImmutableTimer(2).Tick(0);
            Assert.AreNotEqual(timer, timer.Tick(2));
        }

        [TestCase]
        public void SetInterval_WhenTicked_UseNewInterval()
        {
            var timer = new ImmutableTimer(2).Tick(0).SetInterval(3);
            Assert.AreEqual(timer, timer.Tick(2));
        }

        [TestCase]
        public void Remaining_AtStart_ReturnsZero()
        {
            var timer = new ImmutableTimer(2);
            Assert.AreEqual(0, timer.Remaining(0));
        }

        [TestCase]
        public void Remaining_AfterTicked_ReturnsRemaining()
        {
            var timer = new ImmutableTimer(2).Tick(0);
            Assert.AreEqual(1, timer.Remaining(1));
        }

        [TestCase]
        public void Remaining_AfterExpectedEnd_ReturnsZero()
        {
            var timer = new ImmutableTimer(2).Tick(0);
            Assert.AreEqual(0, timer.Remaining(3));
        }

        [TestCase]
        public void Remaining_WhenWrappingInterval_ReturnsRemaining()
        {
            var timer = new ImmutableTimer(4).Tick(uint.MaxValue);
            Assert.AreEqual(4, timer.Remaining(uint.MaxValue));
        }

        [TestCase]
        public void Remaining_WhenWrappingNext_ReturnsRemaining()
        {
            var timer = new ImmutableTimer(4).Tick(uint.MaxValue);
            Assert.AreEqual(2, timer.Remaining(1));
        }

        [TestCase]
        public void Remaining_BeforeStart_ReturnsInterval()
        {
            var timer = new ImmutableTimer(2).Tick(2);
            Assert.AreEqual(3, timer.Remaining(1));
        }
    }
}
