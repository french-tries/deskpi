using System;
using System.Collections.Generic;
using NUnit.Framework;
using piCommon;
using piCommon.test.stubs;

namespace deskpi.test
{
    [TestFixture]
    public class ButtonAggregatorTests
    {
        [TestCase]
        public void ButtonAggregator_AtStart_NoKey()
        {
            var ticker = new Ticker(1, 0);
            var buttons = new List<IButton<ButtonId>> { new ButtonStub<ButtonId>() };
            var aggr = new ButtonAggregator(
                () => ticker, buttons);

            Assert.AreEqual(KeyId.None, aggr.KeyState);
        }

        [TestCase]
        public void ButtonAggregator_OnPinValueChange_TransmitToButton()
        {
            var ticker = new Ticker(1, 0);
            var button = new ButtonStub<ButtonId> { IdVal = ButtonId.Bottom };
            var buttons = new List<IButton<ButtonId>> { button };

            var aggr = new ButtonAggregator(
                () => ticker, buttons).OnPinValueChange(ButtonId.Bottom);

            Assert.IsTrue(button.OnPinValueChanged);
        }

        [TestCase]
        public void NextTick_AtStart_GetsFromButton()
        {
            var ticker = new Ticker(1, 0);
            var button = new ButtonStub<ButtonId> { IdVal = ButtonId.Bottom, NextTickVal = 1 };
            var buttons = new List<IButton<ButtonId>> { button };

            var aggr = new ButtonAggregator(() => ticker, buttons);

            Assert.AreEqual(1, aggr.NextTick(0));
        }

        [TestCase]
        public void Tick_UpdatedButton_StartsTicker()
        {
            var ticker = new Ticker(2, 0);
            var button = new ButtonStub<ButtonId> { IdVal = ButtonId.Bottom, NextTickVal = 1,
                Next = new ButtonStub<ButtonId> { IdVal = ButtonId.Bottom } };
            var buttons = new List<IButton<ButtonId>> { button };

            var aggr = new ButtonAggregator(() => ticker, buttons).Tick(0);

            Assert.AreEqual(2, aggr.NextTick(0));
        }

        [TestCase]
        public void Tick_TickerEnd_UpdatesKey()
        {
            var ticker = new Ticker(2, 0);
            var button = new ButtonStub<ButtonId>
            {
                IdVal = ButtonId.Bottom,
                NextTickVal = 1,
                Next = new ButtonStub<ButtonId> { IdVal = ButtonId.Bottom, PressedVal = true }
            };
            var buttons = new List<IButton<ButtonId>> { button };

            var aggr = new ButtonAggregator(() => ticker, buttons).Tick(0).Tick(2);

            Assert.AreEqual(KeyId.A, aggr.KeyState);
            Assert.AreEqual(null, aggr.NextTick(2));
        }
    }
}
