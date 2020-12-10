using System;
using System.Collections.Immutable;
using System.Linq;
using immutableSsd.src;
using immutableSsd.test.stubs;
using NUnit.Framework;

namespace immutableSsd.test
{
    [TestFixture]
    public class ScrollingGlyphSelectorTests
    {
        [TestCase]
        public void GetSelected_Empty_ReturnsEmpty()
        {
            var interruptHandler = new InterruptHandlerStub();
            var selector = new ScrollingSelector<int>(interruptHandler, 1, 2,1);

            Assert.AreEqual(ImmutableList<int>.Empty, selector.GetSelected());
        }

        [TestCase]
        public void GetSelected_AtStart_ReturnsWithoutOffset()
        {
            var interruptHandler = new InterruptHandlerStub();
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b');

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingSelector<char>(
                interruptHandler, delay, endsDelay, 3).UpdateValues(text);

            Assert.AreEqual(text, selector.GetSelected());
        }

        [TestCase]
        public void GetGlyph_PartialDisplay_ReturnsSubList()
        {
            var interruptHandler = new InterruptHandlerStub();
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b').Add('c');

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingSelector<char>(
                interruptHandler, delay, endsDelay, 2).UpdateValues(text);

            Assert.True(text.GetRange(0,2).SequenceEqual(selector.GetSelected()));
        }

        [TestCase]
        public void GetGlyph_WithDelay_ReturnsWithOffset()
        {
            var interruptHandler = new InterruptHandlerStub();
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b').Add('c');

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingSelector<char>(
                interruptHandler, delay, endsDelay, 1).UpdateValues(text);

            interruptHandler.TestReceived(selector, 2);
            interruptHandler.TestEmpty();

            selector = selector.ReceiveInterrupt(selector, 2);

            Assert.True(text.GetRange(1, 1).SequenceEqual(selector.GetSelected()));
            interruptHandler.TestReceived(selector, 1);
            interruptHandler.TestEmpty();

            selector = selector.ReceiveInterrupt(selector, 3);

            Assert.True(text.GetRange(2, 1).SequenceEqual(selector.GetSelected()));
            interruptHandler.TestReceived(selector, 2);
            interruptHandler.TestEmpty();

            selector = selector.ReceiveInterrupt(selector, 5);

            Assert.True(text.GetRange(0, 1).SequenceEqual(selector.GetSelected()));
            interruptHandler.TestReceived(selector, 2);
            interruptHandler.TestEmpty();
        }
    }
}
