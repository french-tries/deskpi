using System;
using System.Collections.Immutable;
using System.Linq;
using piCommon;

namespace immutableSsd
{
    public class StringSsdWriter : ISsd
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

        public ISsd Write(string text)
        {
            var newValues = Glyph.FromString(text).ConvertAll((g) => converter(g));

            var newSelectors = ImmutableList<ISelector<byte>>.Empty
                .Add(createSelector(newValues, writer.AvailableDigits));

            var newWriter = writer.Write(ConcatSelected(newSelectors));

            return new StringSsdWriter(newWriter, converter, createSelector, newSelectors);
        }

        // todo would be more useful with fractions of total digits...
        public ISsd Write(ImmutableList<(string, uint)> texts)
        {
            var newSelectors = ImmutableList<ISelector<byte>>.Empty;
            var availableDigits = writer.AvailableDigits;

            foreach ((string text, uint digits) in texts)
            {
                var newValues = Glyph.FromString(text).ConvertAll((g) => converter(g));

                var minAvailable = Math.Min(availableDigits, digits == 0 ? (uint)newValues.Count : digits);

                newSelectors = newSelectors.Add(createSelector(newValues, minAvailable));

                availableDigits -= minAvailable;

                if (availableDigits <= 0) break;
            }

            var newWriter = writer.Write(ConcatSelected(newSelectors));

            return new StringSsdWriter(newWriter, converter, createSelector, newSelectors);
        }

        public uint? NextTick(uint currentTime) =>
            PiUtils.Min(writer.NextTick(currentTime),
                selectors.Min((button) => button.NextTick(currentTime)));

        public ISsd Tick(uint currentTime)
        {
            var selectorsN = (from entry in selectors 
                          let newVal = entry.Tick(currentTime)
                          select newVal).ToImmutableList();

            var selectorEquals = selectors.SequenceEqual(selectorsN);

            var writerN = selectorEquals ? writer.Tick(currentTime) :
                writer.Write(ConcatSelected(selectorsN));

            if (!selectorEquals || writerN != writer)
            {
                return new StringSsdWriter(writerN, converter, createSelector, selectorsN);
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
