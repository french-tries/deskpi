using System.Collections.Generic;
using System.Collections.Immutable;
using immutableSsd.src;
using immutableSsd.test.stubs;
using NUnit.Framework;

namespace immutableSsd.test
{
    class TestGpio
    {
        public void Write(Step step)
        {
            if (step is WriteStep writeStep)
            {
                written = written.Enqueue(writeStep);
            }
            else
            {
                Assert.Fail();
            }
        }

        public void TestWritten(int expectedId, bool expectedActive)
        {
            var step = written.Peek();

            Assert.AreEqual(expectedId, step.Pin.Id);
            Assert.AreEqual(expectedActive, step.Value);

            written = written.Dequeue();
        }

        public void TestEmpty()
        {
            Assert.AreEqual(ImmutableQueue<WriteStep>.Empty, written);
        }

        public void Clear()
        {
            written = ImmutableQueue<WriteStep>.Empty;
        }

        private ImmutableQueue<WriteStep> written = ImmutableQueue<WriteStep>.Empty;
    }

    [TestFixture]
    public class DirectSsdWriterTests
    {
        [TestCase]
        public void Creation_Expects_WriteZerosToAllPins()
        {
            var gpio = new TestGpio();
            var interruptHandler = new InterruptHandlerStub();

            var writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio.Write, interruptHandler, 1);

            gpio.TestWritten(2, false);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestEmpty();
        }

        [TestCase]
        public void Creation_Expects_RequestInterrupt()
        {
            var gpio = new TestGpio();
            var interruptHandler = new InterruptHandlerStub();

            var writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio.Write, interruptHandler, 1);

            interruptHandler.TestReceived(writer, 1);
            interruptHandler.TestEmpty();
        }

        [TestCase]
        public void Write_Expects_Written()
        {
            var gpio = new TestGpio();
            var interruptHandler = new InterruptHandlerStub();

            ISsdWriter<ImmutableList<byte>> writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)).Add(new Pin(1, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio.Write, interruptHandler, 1);

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
            var interruptHandler = new InterruptHandlerStub();

            ISsdWriter<ImmutableList<byte>> writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)).Add(new Pin(1, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio.Write, interruptHandler, 5);

            writer = writer.Write(ImmutableList<byte>.Empty.Add(0b01000000).Add(0b10000000));

            gpio.Clear();

            writer = writer.ReceiveInterrupt(writer, 5);
            gpio.TestWritten(2, false);
            gpio.TestWritten(0, true);
            gpio.TestWritten(1, false);
            gpio.TestWritten(3, true);
            gpio.TestEmpty();

            writer = writer.ReceiveInterrupt(writer, 10);
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
            var interruptHandler = new InterruptHandlerStub();

            ISsdWriter<ImmutableList<byte>> writer = new DirectSsdWriter(
                ImmutableList<Pin>.Empty.Add(new Pin(0, true)).Add(new Pin(1, true)),
                ImmutableList<Pin>.Empty.Add(new Pin(2, true)).Add(new Pin(3, true)),
                gpio.Write, interruptHandler, 5);

            writer = writer.Write(ImmutableList<byte>.Empty.Add(0b01000000));

            gpio.Clear();

            writer = writer.ReceiveInterrupt(writer, 5);
            gpio.TestWritten(2, false);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, false);
            gpio.TestEmpty();

            writer = writer.ReceiveInterrupt(writer, 10);
            gpio.TestWritten(3, false);
            gpio.TestWritten(0, false);
            gpio.TestWritten(1, true);
            gpio.TestWritten(2, true);
            gpio.TestEmpty();
        }
    }
}
