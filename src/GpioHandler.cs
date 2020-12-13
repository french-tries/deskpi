using System.Collections.Generic;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace immutableSsd.src
{
    public class GpioHandler
    {
        public GpioHandler()
        {
            Pi.Init<BootstrapWiringPi>();
            pins = new Dictionary<Pin, IGpioPin>();

            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
        }

        public void Write(Pin pin, bool value)
        {
            if (!pins.ContainsKey(pin))
            {
                var gpioPin = Pi.Gpio[pin.Id];
                gpioPin.PinMode = GpioPinDriveMode.Output;

                pins.Add(pin, gpioPin);
            }
            pins[pin].Write(value == pin.ActiveHigh);
        }

        public byte[] SpiWrite(byte[] buffer) =>
            Pi.Spi.Channel0.SendReceive(buffer);

        public uint Millis {  get { return Pi.Timing.Milliseconds; } }
        public uint Micros { get { return Pi.Timing.Microseconds; } }

        public void SleepMillis(uint millis)
        {
            Pi.Timing.SleepMilliseconds(millis);
        }

        public void SleepMicros(uint micros)
        {
            Pi.Timing.SleepMicroseconds(micros);
        }

        private readonly Dictionary<Pin, IGpioPin> pins;
    }
}
