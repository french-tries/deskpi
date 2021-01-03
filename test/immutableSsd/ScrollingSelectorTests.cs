﻿using System;
using System.Collections.Immutable;
using System.Linq;
using immutableSsd;
using immutableSsd.test.stubs;
using NUnit.Framework;

namespace immutableSsd.test
{
    [TestFixture]
    public class ScrollingSelectorTests
    {
        [TestCase]
        public void GetSelected_Empty_ReturnsEmpty()
        {
            var selector = new ScrollingSelector<int>((arg1, arg2) => () => { },
                1, 2, 1, ImmutableList<int>.Empty);

            Assert.AreEqual(ImmutableList<int>.Empty, selector.GetSelected());
        }

        [TestCase]
        public void GetSelected_AtStart_ReturnsWithoutOffset()
        {
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b');

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingSelector<char>(
                (arg1, arg2) => () => { }, delay, endsDelay, 3, text);

            Assert.AreEqual(text, selector.GetSelected());
        }

        [TestCase]
        public void GetGlyph_PartialDisplay_ReturnsSubList()
        {
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b').Add('c');

            uint delay = 1;
            uint endsDelay = 2;

            var selector = new ScrollingSelector<char>(
                (arg1, arg2) => () => { }, delay, endsDelay, 2, text);

            Assert.True(text.GetRange(0,2).SequenceEqual(selector.GetSelected()));
        }

        [TestCase]
        public void GetGlyph_WithDelay_ReturnsWithOffset()
        {
            var text = ImmutableList<char>.Empty.Add('a')
                .Add('b').Add('c');

            uint delay = 1;
            uint endsDelay = 2;

            object receivedCaller = null;
            uint receivedDelay = 0;
            ISelector<char> selector = new ScrollingSelector<char>(
                (arg1, arg2) => { receivedCaller = arg1; receivedDelay = arg2; return() => { }; },
                delay, endsDelay, 1, text);

            Assert.AreEqual(selector, receivedCaller);
            Assert.AreEqual(2, receivedDelay);

            selector = selector.ReceiveInterrupt(selector);

            Assert.True(text.GetRange(1, 1).SequenceEqual(selector.GetSelected()));
            Assert.AreEqual(selector, receivedCaller);
            Assert.AreEqual(1, receivedDelay);

            selector = selector.ReceiveInterrupt(selector);

            Assert.True(text.GetRange(2, 1).SequenceEqual(selector.GetSelected()));
            Assert.AreEqual(selector, receivedCaller);
            Assert.AreEqual(2, receivedDelay);

            selector = selector.ReceiveInterrupt(selector);

            Assert.True(text.GetRange(0, 1).SequenceEqual(selector.GetSelected()));
            Assert.AreEqual(selector, receivedCaller);
            Assert.AreEqual(2, receivedDelay);
        }
    }
}
