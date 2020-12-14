using System;
using System.Collections.Immutable;
using immutableSsd;
using NUnit.Framework;

namespace immutableSsd.test
{
    [TestFixture]
    public class GlyphTests
    {
        [TestCase]
        public void FromString_WithEmptyString_ReturnsEmptyList()
        {
            Assert.AreEqual(ImmutableList<Glyph>.Empty, Glyph.FromString(""));
        }

        [TestCase]
        public void FromString_WithDots_ReturnsSpacesWitDots()
        {
            var result = Glyph.FromString("...");

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(new Glyph(' ', true), result[0]);
            Assert.AreEqual(new Glyph(' ', true), result[1]);
            Assert.AreEqual(new Glyph(' ', true), result[2]);
        }

        [TestCase]
        public void FromString_TextWithoutDots_ReturnsGlyphsWithoutDots()
        {
            var result = Glyph.FromString("lol");

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(new Glyph('l', false), result[0]);
            Assert.AreEqual(new Glyph('o', false), result[1]);
            Assert.AreEqual(new Glyph('l', false), result[2]);
        }

        [TestCase]
        public void FromString_MixedText_ReturnsAppropriateGlyphs()
        {
            var result = Glyph.FromString(".hi.");

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(new Glyph(' ', true), result[0]);
            Assert.AreEqual(new Glyph('h', false), result[1]);
            Assert.AreEqual(new Glyph('i', true), result[2]);
        }
    }
}
