using NUnit.Framework;

namespace piCommon.test
{
    [TestFixture]
    public class ImmutableTimerTests
    {
        [TestCase]
        public void Tick_AtStart_NotTicked()
        {
            var timer = new Ticker(2, 0);
            Assert.False(timer.Ticked(0));
        }

        [TestCase]
        public void Tick_SmallerThanInterval_NotTicked()
        {
            var timer = new Ticker(2, 0);
            Assert.False(timer.Ticked(1));
        }

        [TestCase]
        public void Tick_LargerThanInterval_Ticked()
        {
            var timer = new Ticker(2, 0);
            Assert.True(timer.Ticked(2));
        }

        [TestCase]
        public void Remaining_AtStart_ReturnsInterval()
        {
            var timer = new Ticker(2, 0);
            Assert.AreEqual(2, timer.Remaining(0));
        }

        [TestCase]
        public void Remaining_SmallerThanInterval_ReturnsRemaining()
        {
            var timer = new Ticker(2, 0);
            Assert.AreEqual(1, timer.Remaining(1));
        }

        [TestCase]
        public void Remaining_AtExpectedEnd_ReturnsZero()
        {
            var timer = new Ticker(2, 0);
            Assert.AreEqual(0, timer.Remaining(2));
        }

        [TestCase]
        public void Remaining_AfterExpectedEnd_ReturnsZero()
        {
            var timer = new Ticker(2, 0);
            Assert.AreEqual(0, timer.Remaining(3));
        }

        [TestCase]
        public void Remaining_WhenWrappingInterval_ReturnsRemaining()
        {
            var timer = new Ticker(4, uint.MaxValue);
            Assert.AreEqual(4, timer.Remaining(uint.MaxValue));
        }

        [TestCase]
        public void Remaining_WhenWrappingNext_ReturnsRemaining()
        {
            var timer = new Ticker(4, uint.MaxValue);
            Assert.AreEqual(2, timer.Remaining(1));
        }

        [TestCase]
        public void Remaining_BeforeStart_ReturnsInterval()
        {
            var timer = new Ticker(2, 2);
            Assert.AreEqual(3, timer.Remaining(1));
        }
    }
}
