using System.Collections.Immutable;

namespace immutableSsd.src
{
    public class StringSsdWriter : ISsdWriter<string>
    {
        public delegate byte GlyphToSegments(Glyph glyph);

        public StringSsdWriter(ISsdWriter<ImmutableList<byte>> writer, GlyphToSegments converter,
            ISelector<byte> selector)
        {
            this.writer = writer;
            this.converter = converter;
            this.selector = selector;
        }

        public ISsdWriter<string> Write(string text)
        {
            var newValues = Glyph.FromString(text).ConvertAll((g) => converter(g));

            var newSelector = selector.UpdateValues(newValues);

            var newWriter = writer.Write(newSelector.GetSelected());

            return new StringSsdWriter(newWriter, converter, newSelector);
        }

        public ISsdWriter<string> ReceiveInterrupt(object caller, uint currentTime)
        {
            var newSelector = selector.ReceiveInterrupt(caller, currentTime);

            var newWriter = (selector != newSelector) ?
                writer.Write(newSelector.GetSelected()):
                writer.ReceiveInterrupt(caller, currentTime);

            if (selector != newSelector || newWriter != writer)
            {
                return new StringSsdWriter(newWriter, converter, newSelector);
            }
            return this;
        }

        public int AvailableDigits => writer.AvailableDigits;

        private readonly ISsdWriter<ImmutableList<byte>> writer;
        private readonly GlyphToSegments converter;
        private readonly ISelector<byte> selector;
    }
}
