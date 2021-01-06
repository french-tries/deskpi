using System;
using System.Collections.Immutable;
using System.Linq;
using immutableSsd;
using immutableSsd.test.stubs;
using NUnit.Framework;
using piCommon;

namespace immutableSsd.test
{
    [TestFixture]
    public class ScrollingSelectorTests
    {
        [TestCase]
        public void GetSelected_Empty_ReturnsEmpty()
        {
            var selector = new ScrollingSelector<int>((a) => null,
                1, 2, 1, ImmutableList<int>.Empty, 0);

            Assert.AreEqual(ImmutableList<int>.Empty.Add(0), selector.GetSelected());
        }

        [TestCase]
        public void GetSelected_AtStart_ReturnsWithoutOffset()
        {
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b');

            var selector = new ScrollingSelector<char>(
                (a) => null, 1, 2, 2, text, ' ');

            Assert.AreEqual(text, selector.GetSelected());
        }

        [TestCase]
        public void GetGlyph_PartialDisplay_ReturnsSubList()
        {
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b').Add('c');

            var selector = new ScrollingSelector<char>(
                (d) => new Ticker(d, 0), 1, 2, 2, text, ' ');

            Assert.True(text.GetRange(0,2).SequenceEqual(selector.GetSelected()));
        }

        [TestCase]
        public void GetGlyph_WithDelay_ReturnsWithOffset()
        {
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b').Add('c');

            uint delay = 1;
            uint endsDelay = 2;
            uint millis = 0;

            ISelector<char> selector = new ScrollingSelector<char>(
                (d) => new Ticker(d, millis), delay, endsDelay, 1, text, ' ');

            millis += endsDelay;
            selector = selector.Tick(millis);

            Assert.True(text.GetRange(1, 1).SequenceEqual(selector.GetSelected()));
            Assert.AreEqual(delay, selector.NextTick(millis));

            millis += delay;
            selector = selector.Tick(millis);
            Assert.True(text.GetRange(2, 1).SequenceEqual(selector.GetSelected()));
            Assert.AreEqual(endsDelay, selector.NextTick(millis));

            millis += endsDelay;
            selector = selector.Tick(millis);

            Assert.True(text.GetRange(0, 1).SequenceEqual(selector.GetSelected()));
            Assert.AreEqual(endsDelay, selector.NextTick(millis));
        }
    }
}
