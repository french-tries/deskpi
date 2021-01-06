using System;
using System.Collections.Immutable;
using piCommon;

namespace immutableSsd
{
    public static class ImmutableSsd
    {
        public static StringSsdWriter CreateMax7219BackedDisplay(
            GpioHandler gpioHandler, uint scrollDelay = 1000, uint endsScrollDelay = 2000)
        {
            var directWriter = new Max7219CommonAnodeWriter((obj) => gpioHandler.SpiWrite(obj));

            var converter = new SegmentsConverter();

            ISelector<byte> createSelector(ImmutableList<byte> text, uint availableDigits) =>
                new ScrollingSelector<byte>(
                    (interval) => new Ticker(interval, gpioHandler.Millis), 
                    scrollDelay, endsScrollDelay, availableDigits, text);

            return new StringSsdWriter(directWriter, converter.GetSegments, createSelector);
        }
    }
}
