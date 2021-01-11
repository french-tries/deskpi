using System.Collections.Immutable;
using System.Linq;
using immutableSsd.test.stubs;
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
            ISsd stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, 
                (arg1, arg2) => new SelectorStub<byte> { Text = arg1 });

            var str = "123";
            stringWriter = stringWriter.Write(str);

            Assert.IsTrue(testWriter.LastValues.SequenceEqual(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3')));
        }

        [TestCase]
        public void Write_MultipleStrings_Writes()
        {
            var testWriter = new SsdWriterStub<byte>();
            ISsd stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => new SelectorStub<byte> { Text = arg1 });

            var strs = ImmutableList<(string, uint)>.Empty
                .Add(("1", 1)).Add(("2", 1)).Add(("3", 1));
            stringWriter = stringWriter.Write(strs);

            Assert.IsTrue(testWriter.LastValues.SequenceEqual(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3')));
        }

        [TestCase]
        public void Write_NotEnoughSpace_WritesFirsts()
        {
            var testWriter = new SsdWriterStub<byte>();
            ISsd stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => new SelectorStub<byte> { Text = arg1 });

            var strs = ImmutableList<(string, uint)>.Empty
                .Add(("1", 1)).Add(("2", 1)).Add(("3", 1)).Add(("4", 1));
            stringWriter = stringWriter.Write(strs);

            Assert.IsTrue(testWriter.LastValues.SequenceEqual(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3')));
        }

        [TestCase]
        public void ReceiveInterrupt_OnSelector_Writes()
        {
            var testWriter = new SsdWriterStub<byte>();
            var testSelector = new SelectorStub<byte>();
            ISsd stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => {testSelector.Text = arg1; return testSelector; });

            var str = "123";
            stringWriter = stringWriter.Write(str);

            testSelector.NewText = ImmutableList<byte>.Empty.Add(100);

            stringWriter = stringWriter.Tick(1);

            Assert.IsTrue(testWriter.LastValues.SequenceEqual(testSelector.NewText));
        }

        [TestCase]
        public void ReceiveInterrupt_onWriter_ReceivesIt()
        {
            var testWriter = new SsdWriterStub<byte>();
            ISsd stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => new SelectorStub<byte> { Text = arg1 });

            var str = "123";
            stringWriter = stringWriter.Write(str);

            testWriter.NextTickTime = 0;

            stringWriter = stringWriter.Tick(1);

            Assert.AreEqual(1, testWriter.TickTime);
        }
    }
}
