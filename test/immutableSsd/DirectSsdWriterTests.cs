using System.Collections.Generic;
using System.Collections.Immutable;
using immutableSsd.test.stubs;
using NUnit.Framework;
using piCommon;

namespace immutableSsd.test
{
    class TestGpio
    {
        public void Write(int pin, bool value)
        {
            written = written.Enqueue((pin, value));
        }

        public void TestWritten(int expectedId, bool expectedActive)
        {
            (int pin, bool value) = written.Peek();

            Assert.AreEqual(expectedId, pin);
            Assert.AreEqual(expectedActive, value);

            written = written.Dequeue();
        }

        public void TestEmpty()
        {
            Assert.AreEqual(ImmutableQueue<(int, bool)>.Empty, written);
        }

        public void Clear()
        {
            written = ImmutableQueue<(int, bool)>.Empty;
        }

        private ImmutableQueue<(int, bool)> written = ImmutableQueue<(int, bool)>.Empty;
    }

    [TestFixture]
    public class DirectSsdWriterTests
    {
        [TestCase]
        public void Creation_Expects_WriteZerosToAllPins()
        {
            var gpio = new TestGpio();
            var writer = new DirectSsdWriter(
                ImmutableList<int>.Empty.Add(0),
                ImmutableList<int>.Empty.Add(2).Add(3),
                gpio.Write, (arg1, arg2) => () => { }, 1);

            gpio.TestWritten(2, false);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestEmpty();
        }

        [TestCase]
        public void Creation_Expects_RequestInterrupt()
        {
            var gpio = new TestGpio();

            object receivedCaller = null;
            uint receivedDelay = 0;

            var writer = new DirectSsdWriter(
                ImmutableList<int>.Empty.Add(0),
                ImmutableList<int>.Empty.Add(2).Add(3),
                gpio.Write,
                (arg1, arg2) => { receivedCaller = arg1; receivedDelay = arg2; return () => { }; },
                1);

            Assert.AreEqual(writer, receivedCaller);
            Assert.AreEqual(1, receivedDelay);
        }

        [TestCase]
        public void Write_Expects_Written()
        {
            var gpio = new TestGpio();

            ISsdWriter<ImmutableList<byte>> writer = new DirectSsdWriter(
                ImmutableList<int>.Empty.Add(0).Add(1),
                ImmutableList<int>.Empty.Add(2).Add(3),
                gpio.Write, (arg1, arg2) => () => { }, 1);

            gpio.Clear();

            writer = writer.Write(ImmutableList<byte>.Empty.Add(0b10000000));

            gpio.TestWritten(3, false);
            gpio.TestWritten(0, true);
            gpio.TestWritten(1, false);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();
        }

        [TestCase]
        public void CycleSteps_Expects_ToCycle()
        {
            var values = new List<byte>
            {
                0b01000000,
                0b10000000
            };
            var gpio = new TestGpio();

            ISsdWriter<ImmutableList<byte>> writer = new DirectSsdWriter(
                ImmutableList<int>.Empty.Add(0).Add(1),
                ImmutableList<int>.Empty.Add(2).Add(3),
                gpio.Write, (arg1, arg2) => () => { }, 5);

            writer = writer.Write(ImmutableList<byte>.Empty.Add(0b01000000).Add(0b10000000));

            gpio.Clear();

            writer = writer.ReceiveInterrupt(writer);
            gpio.TestWritten(2, false);
            gpio.TestWritten(0, true);
            gpio.TestWritten(1, false);
            gpio.TestWritten(3, true);
            gpio.TestEmpty();

            writer = writer.ReceiveInterrupt(writer);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, true);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();
        }

       [TestCase]
       public void WriteSteps_WhenSmallerValuesSize_ClearSteps()
       {
            var gpio = new TestGpio();

            ISsdWriter<ImmutableList<byte>> writer = new DirectSsdWriter(
                ImmutableList<int>.Empty.Add(0).Add(1),
                ImmutableList<int>.Empty.Add(2).Add(3),
                gpio.Write, (arg1, arg2) => () => { }, 5);

            writer = writer.Write(ImmutableList<byte>.Empty.Add(0b01000000));

            gpio.Clear();

            writer = writer.ReceiveInterrupt(writer);
            gpio.TestWritten(2, false);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, false);
            gpio.TestEmpty();

            writer = writer.ReceiveInterrupt(writer);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, true);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();
        }
    }
}
