using System;
using System.Collections.Generic;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace immutableSsd
{
    public class GpioHandler
    {
        public GpioHandler()
        {
            Pi.Init<BootstrapWiringPi>();
            pins = new Dictionary<int, GpioPinDriveMode>();

            Pi.Spi.Channel0Frequency = SpiChannel.MinFrequency;
        }

        // TODO EdgeDetection?
        public void RegisterInterruptCallback(int pinId, Action callback,
            EdgeDetection mode = EdgeDetection.FallingAndRisingEdge)
        {
            var pin = Pi.Gpio[pinId];
            if (!pins.ContainsKey(pinId) || pins[pinId] != GpioPinDriveMode.Input)
            {
                pins[pinId] = GpioPinDriveMode.Input;
                pin.PinMode = GpioPinDriveMode.Input;
                pin.InputPullMode = GpioPinResistorPullMode.PullUp;
            }
            pin.RegisterInterruptCallback(mode, callback);
        }

        public void Write(int pinId, bool value, bool activeHigh = true)
        {
            var pin = Pi.Gpio[pinId];
            if (!pins.ContainsKey(pinId) || pins[pinId] != GpioPinDriveMode.Output)
            {
                pins[pinId] = GpioPinDriveMode.Output;
                pin.PinMode = GpioPinDriveMode.Output;
            }
            pin.Write(value == activeHigh);
        }

        public bool Read(int pinId)
        {
            var pin = Pi.Gpio[pinId];
            if (!pins.ContainsKey(pinId) || pins[pinId] != GpioPinDriveMode.Input)
            {
                pins[pinId] = GpioPinDriveMode.Input;
                pin.PinMode = GpioPinDriveMode.Input;
            }
            return pin.Read();
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

        private readonly Dictionary<int, GpioPinDriveMode> pins;
    }
}
