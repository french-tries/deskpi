using System.Collections.Generic;
using System.Collections.Immutable;

namespace immutableSsd
{
    public class SegmentsConverter
    {
        public static readonly ImmutableDictionary<char, byte> defaultValues = 
            new Dictionary<char, byte> {
                { 'a', 0b11101110 },
                { 'b', 0b00111110 },
                { 'c', 0b00011010 },
                { 'd', 0b01111010 },
                { 'e', 0b10011110 },
                { 'f', 0b10001110 },
                { 'g', 0b10111100 },
                { 'h', 0b00101110 },
                { 'i', 0b10100000 },
                { 'j', 0b01111000 },
                { 'k', 0b00101100 },
                { 'l', 0b00011100 },
                { 'm', 0b10101010 },
                { 'n', 0b00101010 },
                { 'o', 0b00111010 },
                { 'p', 0b11001110 },
                { 'q', 0b11010110 },
                { 'r', 0b00001010 },
                { 's', 0b10110110 },
                { 't', 0b10001100 },
                { 'u', 0b01111100 },
                { 'v', 0b00111000 },
                { 'w', 0b10111000 },
                { 'x', 0b01101110 },
                { 'y', 0b01110110 },
                { 'z', 0b10010010 },
                { '0', 0b11111100 },
                { '1', 0b01100000 },
                { '2', 0b11011010 },
                { '3', 0b11110010 },
                { '4', 0b01100110 },
                { '5', 0b10110110 },
                { '6', 0b00111110 },
                { '7', 0b11100000 },
                { '8', 0b11111110 },
                { '9', 0b11100110 },
                { '\'', 0b01000000 },
                { '-', 0b00000010 },
                { '\u02C2', 0b10000110 }, // left arrow
                { '\u02C3', 0b11000010 }, // right arrow
                { '\u02C4', 0b11000100 }, // top arrow
                { '\u02C5', 0b01000110 }, // bottom arrow
                { ' ', 0b00000000 },
                }.ToImmutableDictionary();

        public SegmentsConverter()
        {
            values = defaultValues;
        }

        public SegmentsConverter(ImmutableDictionary<char, byte> values)
        {
            this.values = values;
        }

        public byte GetSegments(Glyph glyph)
        {
            var value = char.ToLower(glyph.Character);
            if (values.ContainsKey(value))
            {
                byte dotMask = (byte)(glyph.Dot ? 0b00000001 : 0b00000000);
                return (byte)(values[value] | dotMask);
            }
            return 0;
        }

        private readonly ImmutableDictionary<char, byte> values;
    }
}
