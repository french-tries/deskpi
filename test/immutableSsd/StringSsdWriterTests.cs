﻿using System.Collections.Immutable;
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
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character, 
                (arg1, arg2) => new SelectorStub<byte>(arg1));

            var str = "123";
            var tickable = stringWriter.Write(str);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
        }

        [TestCase]
        public void Write_MultipleStrings_Writes()
        {
            var testWriter = new SsdWriterStub<byte>();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => new SelectorStub<byte>(arg1));

            var strs = ImmutableList<(string, uint)>.Empty
                .Add(("1", 1)).Add(("2", 1)).Add(("3", 1));
            var tickable = stringWriter.Write(strs);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
        }

        [TestCase]
        public void Write_NotEnoughSpace_WritesFirsts()
        {
            var testWriter = new SsdWriterStub<byte>();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => new SelectorStub<byte>(arg1));

            var strs = ImmutableList<(string, uint)>.Empty
                .Add(("1", 1)).Add(("2", 1)).Add(("3", 1)).Add(("4", 1));
            var tickable = stringWriter.Write(strs);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
        }

        [TestCase]
        public void ReceiveInterrupt_OnSelector_Writes()
        {
            var testWriter = new SsdWriterStub<byte>();
            var testSelector = new SelectorStub<byte>();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => {testSelector.Text = arg1; return testSelector; });

            var str = "123";
            var tickable = stringWriter.Write(str);

            testWriter.Reset();
            testSelector.CreateNew = true;

            stringWriter = stringWriter.ReceiveInterrupt(testSelector);

            testWriter.TestValues(ImmutableList<byte>.Empty
                .Add((byte)'1').Add((byte)'2').Add((byte)'3'));
        }

        [TestCase]
        public void ReceiveInterrupt_onWriter_ReceivesIt()
        {
            var testWriter = new SsdWriterStub<byte>();
            var stringWriter = new StringSsdWriter(testWriter,
                (Glyph g) => (byte)g.Character,
                (arg1, arg2) => new SelectorStub<byte>(arg1));

            var str = "123";
            var tickable = stringWriter.Write(str);

            testWriter.Reset();

            stringWriter = stringWriter.ReceiveInterrupt(testWriter);

            testWriter.TestUnwritten();
            testWriter.TestCaller(testWriter);
        }
    }
}
