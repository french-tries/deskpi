using System.Collections.Immutable;
using deskpi.test.stubs;
using immutableSsd.src;
using NUnit.Framework;

namespace immutableSsd.test
{
    [TestFixture]
    public class StringSsdWriterTests
    {
        [TestCase]
        public void WriteString_Writes()
        {
            var testWriter = new SsdWriterStub<byte>();
            var testSelector = new SelectorStub<byte>();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, testSelector);

            var str = "123";
            testSelector.CreateNew = true;
            var tickable = stringWriter.Write(str);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
        }

        [TestCase]
        public void ReceiveInterrupt_OnSelector_Writes()
        {
            var testWriter = new SsdWriterStub<byte>();
            var testSelector = new SelectorStub<byte>();
            ISsdWriter<string> stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, testSelector);

            var str = "123";
            var tickable = stringWriter.Write(str);

            testWriter.Reset();
            testSelector.CreateNew = true;

            stringWriter = stringWriter.ReceiveInterrupt(testSelector, 9);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
        }

        [TestCase]
        public void ReceiveInterrupt_onWriter_ReceivesIt()
        {
            var testWriter = new SsdWriterStub<byte>();
            var testSelector = new SelectorStub<byte>();
            ISsdWriter<string> stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, testSelector);

            var str = "123";
            var tickable = stringWriter.Write(str);

            testWriter.Reset();

            stringWriter = stringWriter.ReceiveInterrupt(testWriter, 9);

            testWriter.TestUnwritten();
            testWriter.TestTime(9);
        }
    }
}
