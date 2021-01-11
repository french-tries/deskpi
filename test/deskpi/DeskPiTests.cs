using System;
using deskpi.test.stubs;
using immutableSsd.test.stubs;
using NUnit.Framework;

namespace deskpi.test
{
    [TestFixture]
    public class DeskPiTests
    {
        [TestCase]
        public void ReceiveEvent_PinValueChangeEvent_TransmitToAggregator()
        {
            var stringWriter = new SsdStub();
            var aggr = new ButtonAggregatorStub { ReceivedButton = ButtonId.Bottom };
            var selector = new DeskPiModeStub();

            var deskPi = new DeskPi(stringWriter, aggr, selector);

            var ev = new PinValueChangeEvent(ButtonId.Top);
            deskPi = deskPi.ReceiveEvent(ev);

            Assert.AreEqual(ev.Button, aggr.ReceivedButton);
        }

        [TestCase]
        public void NextTick_MinAggr_ReturnsMin()
        {
            var stringWriter = new SsdStub { NextTickVal = 3 };
            var aggr = new ButtonAggregatorStub { NextTickVal = 2 };
            var selector = new DeskPiModeStub();

            var deskPi = new DeskPi(stringWriter, aggr, selector);

            Assert.AreEqual(2, deskPi.NextTick(0));
        }

        [TestCase]
        public void NextTick_MinSsd_ReturnsMin()
        {
            var stringWriter = new SsdStub { NextTickVal = 2 };
            var aggr = new ButtonAggregatorStub { NextTickVal = 3 };
            var selector = new DeskPiModeStub();

            var deskPi = new DeskPi(stringWriter, aggr, selector);

            Assert.AreEqual(2, deskPi.NextTick(0));
        }

        [TestCase]
        public void NextTick_MinSel_ReturnsMin()
        {
            var stringWriter = new SsdStub { NextTickVal = 2 };
            var aggr = new ButtonAggregatorStub { NextTickVal = 3 };
            var selector = new DeskPiModeStub { NextTickVal = 1 };

            var deskPi = new DeskPi(stringWriter, aggr, selector);

            Assert.AreEqual(1, deskPi.NextTick(0));
        }

        [TestCase]
        public void Tick_Ticks()
        {
            var stringWriter = new SsdStub();
            var aggr = new ButtonAggregatorStub();
            var selector = new DeskPiModeStub();

            var deskPi = new DeskPi(stringWriter, aggr, selector);
            var newPi = deskPi.Tick(1);

            Assert.AreEqual(deskPi, newPi);
            Assert.AreEqual(1, stringWriter.ReceivedCurrentTime);
            Assert.AreEqual(1, aggr.ReceivedTick);
            Assert.AreEqual(1, selector.ReceivedTick);
        }

        [TestCase]
        public void Tick_NewKey_Transmit()
        {
            var stringWriter = new SsdStub();
            var aggr = new ButtonAggregatorStub { KeyStateVal = KeyId.A,
                Next = new ButtonAggregatorStub { KeyStateVal = KeyId.B } };
            var selector = new DeskPiModeStub();

            var deskPi = new DeskPi(stringWriter, aggr, selector);
            var newPi = deskPi.Tick(1);

            Assert.AreNotEqual(deskPi, newPi);
            Assert.AreEqual(KeyId.B, selector.ReceivedKey);
        }

        [TestCase]
        public void Tick_NewText_Writes()
        {
            var stringWriter = new SsdStub { ReceivedText = "" };
            var aggr = new ButtonAggregatorStub();
            var selector = new DeskPiModeStub { TextVal = "old",
                Next = new DeskPiModeStub { TextVal = "new"}};

            var deskPi = new DeskPi(stringWriter, aggr, selector);
            var newPi = deskPi.Tick(1);

            Assert.AreNotEqual(deskPi, newPi);
            Assert.AreEqual("new", stringWriter.ReceivedText);
        }
    }
}
