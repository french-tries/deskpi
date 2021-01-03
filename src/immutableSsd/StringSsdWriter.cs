using System;
using System.Collections.Immutable;
using System.Linq;

namespace immutableSsd
{
    public class StringSsdWriter : ISsdWriter<string>
    {
        public delegate byte GlyphToSegments(Glyph glyph);

        public StringSsdWriter(ISsdWriter<ImmutableList<byte>> writer, 
            GlyphToSegments converter,
            Func<ImmutableList<byte>, uint, ISelector<byte>> createSelector)
            : this(writer, converter, createSelector, 
                ImmutableList<ISelector<byte>>.Empty
                    .Add(createSelector(ImmutableList<byte>.Empty, writer.AvailableDigits)))
        {
        }

        private StringSsdWriter(ISsdWriter<ImmutableList<byte>> writer, GlyphToSegments converter,
            Func<ImmutableList<byte>, uint, ISelector<byte>> createSelector, ImmutableList<ISelector<byte>> selectors)
        {
            this.writer = writer;
            this.converter = converter;
            this.createSelector = createSelector;
            this.selectors = selectors;
        }

        private ImmutableList<byte> ConcatSelected(ImmutableList<ISelector<byte>> sels) =>
            sels.Aggregate(ImmutableList<byte>.Empty, 
                (result, selector) => result.Concat(selector.GetSelected()).ToImmutableList()
        );

        public ISsdWriter<string> Write(string text)
        {
            var newValues = Glyph.FromString(text).ConvertAll((g) => converter(g));

            var newSelectors = ImmutableList<ISelector<byte>>.Empty
                .Add(createSelector(newValues, writer.AvailableDigits));

            var newWriter = writer.Write(ConcatSelected(newSelectors));

            return new StringSsdWriter(newWriter, converter, createSelector, newSelectors);
        }

        public ISsdWriter<string> Write(ImmutableList<(string, uint)> texts)
        {
            var newSelectors = ImmutableList<ISelector<byte>>.Empty;
            var availableDigits = writer.AvailableDigits;

            foreach ((string text, uint digits) in texts)
            {
                if (digits == 0) continue;

                var newValues = Glyph.FromString(text).ConvertAll((g) => converter(g));
                var minAvailable = Math.Min(availableDigits, digits);

                newSelectors = newSelectors.Add(createSelector(newValues, minAvailable));

                availableDigits -= minAvailable;

                if (availableDigits <= 0) break;
            }

            var newWriter = writer.Write(ConcatSelected(newSelectors));

            return new StringSsdWriter(newWriter, converter, createSelector, newSelectors);
        }

        public ISsdWriter<string> ReceiveInterrupt(object caller)
        {
            // todo non O(N^2) way

            var newSelectors = selectors;
            foreach (ISelector<byte> selector in selectors)
            {
                var newSelector = selector.ReceiveInterrupt(caller);
                if (selector != newSelector)
                {
                    newSelectors = newSelectors.Replace(selector, newSelector);
                }
            }

            var newWriter = (selectors != newSelectors) ?
                writer.Write(ConcatSelected(newSelectors)) :
                writer.ReceiveInterrupt(caller);

            if (selectors != newSelectors || newWriter != writer)
            {
                return new StringSsdWriter(newWriter, converter, createSelector, newSelectors);
            }
            return this;
        }

        public uint AvailableDigits => writer.AvailableDigits;

        private readonly ISsdWriter<ImmutableList<byte>> writer;
        private readonly GlyphToSegments converter;
        private readonly Func<ImmutableList<byte>, uint, ISelector<byte>> createSelector;

        private readonly ImmutableList<ISelector<byte>> selectors;
    }
}
