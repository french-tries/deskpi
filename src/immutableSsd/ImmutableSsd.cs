using System;
using piCommon;

namespace immutableSsd
{
    public static class ImmutableSsd
    {
        public static StringSsdWriter CreateMax7219BackedDisplay(
            GpioHandler gpioHandler, Func<object, uint, Action> requestInterrupt,
            uint numDigits = 8, uint scrollDelay = 1000, uint endsScrollDelay = 2000)
        {
            var directWriter = new Max7219CommonAnodeWriter((obj) => gpioHandler.SpiWrite(obj));

            var converter = new SegmentsConverter();

            var selector = new ScrollingSelector<byte>(
                requestInterrupt, scrollDelay, endsScrollDelay, numDigits);

            return new StringSsdWriter(directWriter, converter.GetSegments, selector);
        }
    }
}
