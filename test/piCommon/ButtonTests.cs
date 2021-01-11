using System;
using NUnit.Framework;

namespace piCommon.test
{
    [TestFixture]
    public class ButtonTests
    {
        [TestCase]
        public void Tick_AtStart_ReadsPressed()
        {
            var ticker = new Ticker(1, 0);
            var read = true;
            bool? updated = null;
            IButton<bool> button = new Button<bool>(
                () => ticker, () => read, (u) => updated = u, true);

            Assert.IsTrue(button.Pressed);
        }

        [TestCase]
        public void Tick_OnPinValueChange_ResetTicker()
        {
            var ticker = new Ticker(1, 0);
            var read = true;
            bool? updated = null;
            IButton<bool> button = new Button<bool>(
                () => ticker, () => read, (u) => updated = u, true);

            ticker = new Ticker(2, 0);
            button = button.OnPinValueChange();

            Assert.AreEqual(2, button.NextTick(0));
        }

        [TestCase]
        public void Tick_SameValue_DoesNothing()
        {
            var ticker = new Ticker(1, 0);
            var read = true;
            bool? updated = null;
            IButton<bool> button = new Button<bool>(
                () => ticker, () => read, (u) => updated = u, true);

            Assert.AreEqual(button, button.Tick(1));
        }

        [TestCase]
        public void Tick_DiffValue_Updates()
        {
            var ticker = new Ticker(1, 0);
            var read = false;
            bool? updated = null;
            IButton<bool> button = new Button<bool>(
                () => ticker, () => read, (u) => updated = u, true).OnPinValueChange();

            read = true;
            IButton<bool> buttonN = button.Tick(1);

            Assert.AreNotEqual(button, buttonN);
            Assert.IsTrue(updated);
            Assert.IsTrue(buttonN.Pressed);
        }
    }
}
