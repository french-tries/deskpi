using System;
using System.Collections.Immutable;

namespace immutableSsd.src
{
    // TODO daisy chaining chips
    public class Max7219CommonAnodeWriter : ISsdWriter<ImmutableList<byte>>
    {
        public static ImmutableList<byte> Rotate(ImmutableList<byte> values)
        {
            var result = ImmutableList<byte>.Empty;
            return Rotate(values, result);
        }

        private static ImmutableList<byte> Rotate(ImmutableList<byte> values, ImmutableList<byte> buffer)
        {
            byte currentValue = 0;
            int index = buffer.Count;
            if (index >= 8) return buffer;

            int i = 0;
            foreach (var value in values)
            {
                currentValue |= (byte)((value & (1 << 7 - index)) != 0 ? (1 << 7 - i) : 0);
                if (++i >= 8) break;
            }

            return Rotate(values, buffer.Add(currentValue));
        }

        public static void Print(ImmutableList<byte> values)
        {
            byte i = 1;
            foreach (var value in values)
            {
                Console.WriteLine("{0:X}", value);
                if (++i >= 8) break;
            }
        }

        public Max7219CommonAnodeWriter(Action<byte[]> spiWriteAction)
        {
            this.spiWriteAction = spiWriteAction;

            // setup chip
            spiWriteAction(new byte[] { 0x09, 0x00 });
            spiWriteAction(new byte[] { 0x0a, 0x14 });
            spiWriteAction(new byte[] { 0x0b, 0x07 });
            spiWriteAction(new byte[] { 0x0c, 0x01 });
            spiWriteAction(new byte[] { 0x0f, 0x00 });
        }

        public void HelloWorld()
        {
            spiWriteAction(new byte[] { 0x01, 0b10110111 });
            spiWriteAction(new byte[] { 0x02, 0b11111001 });
            spiWriteAction(new byte[] { 0x03, 0b11011111 });
            spiWriteAction(new byte[] { 0x04, 0b10110110 });
            spiWriteAction(new byte[] { 0x05, 0b10100010 });
            spiWriteAction(new byte[] { 0x06, 0b10001110 });
            spiWriteAction(new byte[] { 0x07, 0b00111110 });
            spiWriteAction(new byte[] { 0x08, 0xff });
        }

        private void Display(ImmutableList<byte> values)
        {
            byte i = 1;
            foreach (var value in values)
            {
                spiWriteAction(new byte[] { i, value });
                if (++i > 8) break;
            }
        }

        public int AvailableDigits => 8;

        public ISsdWriter<ImmutableList<byte>> Write(ImmutableList<byte> values)
        {
            Display(Rotate(values));
            return this;
        }

        public ISsdWriter<ImmutableList<byte>> ReceiveInterrupt(object caller, uint currentTime)
            => this;

        private readonly Action<byte[]> spiWriteAction;
        /*
        public static void Main(string[] args)
        {
            Pi.Init<BootstrapWiringPi>();
            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;

            var writer = new Max7219CommonAnodeWriter((byte[] buffer) =>
                Pi.Spi.Channel0.SendReceive(buffer));

            var values = new List<byte> {
                0b11111100, 0b01100001, 0b11011011, 0b11110010,
                0b01100111, 0b10110110, 0b10111110
            }.ToImmutableList();

            writer.Write(values);

            while (true) { }
        }*/
    }
}
